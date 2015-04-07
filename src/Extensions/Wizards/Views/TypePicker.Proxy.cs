namespace More.VisualStudio.Views
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Emit;
    using Microsoft.CodeAnalysis.MSBuild;
    using More.VisualStudio.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;

    /// <content>
    /// Provides additional implementation for the <see cref="TypePicker"/> class.
    /// </content>
    public partial class TypePicker
    {
        /// <summary>
        /// Represents the proxy that can be used to marshal type selection via another AppDomain.
        /// </summary>
        private sealed class Proxy : MarshalByRefObject
        {
            private static readonly AssemblyName System_Runtime = new AssemblyName( "System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" );
            private Assembly localAssembly;
            private IReadOnlyList<AssemblyName> localAssemblyReferences;

            [SuppressMessage( "Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "False positive. The field is initialized inline and a method cannot be called any other way." )]
            static Proxy()
            {
                SetDefaultTypeBrowserSize( new Size( 500, 500 ) );
            }

            public bool? ShowDialog( IntPtr owner )
            {
                // create view model based on picker properties, then
                // create and show the view
                var model = new TypeBrowserViewModel()
                {
                    NameConvention = this.NameConvention,
                    FilterByConvention = !string.IsNullOrEmpty( this.NameConvention ),
                    LocalAssembly = this.localAssembly
                };

                model.RestrictedBaseTypeNames.AddRange( this.RestrictedBaseTypeNames ?? Enumerable.Empty<string>() );
                model.LocalAssemblyReferences.AddRange( AddSystemRuntimeIfNeeded( this.localAssemblyReferences ) );

                using ( var view = new TypeBrowserDialog( model ) )
                {
                    var helper = new WindowInteropHelper( view )
                    {
                        Owner = owner
                    };

                    // change title from default value only if an alternate title is provided
                    if ( !string.IsNullOrEmpty( this.Title ) )
                        model.Title = this.Title;

                    var result = view.ShowDialog();

                    // if a type is selected, set it. wrap the type so that it is
                    // serialized across appdomain boundaries
                    if ( ( result ?? false ) )
                        this.SelectedType = new SimpleType( model.SelectedType );

                    helper.Owner = IntPtr.Zero;

                    return result;
                }
            }

            private static void SetDefaultTypeBrowserSize( Size size )
            {
                // HACK: there seems to be no way in the api to set the default TypeBrowser size (which is an internal type)
                // using TypePresenter or Style was considered, but no other workable solution was found. the
                // TypeBrowser does remember it's size between calls, but in most scenarios this only occurs once
                // and the assembly is unloaded because it's in another AppDomain (which restarts the process)
                var assemblyName = typeof( System.Activities.Presentation.View.TypePresenter ).Assembly.FullName;
                var typeName = "System.Activities.Presentation.View.TypeBrowser, " + assemblyName;
                var type = Type.GetType( typeName, true );
                var field = type.GetField( "size", BindingFlags.Static | BindingFlags.NonPublic );

                field.SetValue( null, size );
            }

            private static IEnumerable<AssemblyName> AddSystemRuntimeIfNeeded( IReadOnlyList<AssemblyName> references )
            {
                Contract.Requires( references != null );
                Contract.Ensures( Contract.Result<IEnumerable<AssemblyName>>() != null );

                // HACK: references to System.Runtime may be indirect and not reported via Assembly.GetReferencedAssemblies or detected
                // via roslyn. this typically occurs with WinRT-based applications.  if System.Runtime 4.0 isn't loaded, all other
                // Reflection requests will fail (ex: for System.Runtime 4.0.10). if we provide an empty assembly reference list, things
                // will work as expected because System.Runtime 4.0 will be requested (and first); however, the type picker will display
                // a large number of extra assemblies that shouldn't be considered.  unless a better solution is found (if ever), this
                // is the most suitable workaround.
                var filteredReferences = references.Where( r => AssemblyName.ReferenceMatchesDefinition( r, System_Runtime ) ).ToArray();
                var reference = filteredReferences.FirstOrDefault( r => r.Version == System_Runtime.Version );

                if ( reference == null && filteredReferences.Length > 0 )
                {
                    // System.Runtime 4.0 was not found, but there are references to System.Runtime; inject the reference
                    var clone = references.ToList();
                    clone.Insert( 0, System_Runtime );
                    return clone;
                }

                return references;
            }

            public async Task CreateLocalAssemblyAsync( AssemblyName localAssemblyName )
            {
                Contract.Requires( localAssemblyName != null );
                Contract.Ensures( Contract.Result<Task>() != null );

                try
                {
                    // try creating a dynamic assembly by compiling the source project with roslyn into memory
                    var result = await CreateDynamicAssemblyAsync( this.SourceProject, localAssemblyName.ContentType );

                    if ( ( this.localAssembly = result.Item1 ) != null )
                    {
                        this.localAssemblyReferences = result.Item2 ?? this.localAssembly.GetReferencedAssemblies();
                        return;
                    }
                }
                catch
                {
                    Debug.WriteLine( "Dynamic assembly generation failed." );
                }

                // if that fails, try loading the output assembly of the source project from the last build
                if ( ( this.localAssembly = LoadLocalAssemblyFromExistingBuild( this.SourceProject ) ) == null )
                    this.localAssemblyReferences = new AssemblyName[0];
                else
                    this.localAssemblyReferences = this.localAssembly.GetReferencedAssemblies();
            }

            private static bool ReferencesFacadesIndirectly( Project project )
            {
                Contract.Requires( project != null );

                // we consider only level of depth from each reference (which should typically be as deep as we need to go anyway).
                // if we find at least one reference to 'System.Runtime', then we know we need to reference the facade assemblies.
                var references = from reference in project.MetadataReferences
                                 let assembly = Assembly.ReflectionOnlyLoadFrom( reference.Display )
                                 from indirectReference in assembly.GetReferencedAssemblies()
                                 where indirectReference.Name == "System.Runtime"
                                 select reference;

                return references.Any();
            }

            private static EmitResult CompileWithFacades( Project project, Compilation compilation, Stream stream, EmitResult previousResult )
            {
                Contract.Requires( project != null );
                Contract.Requires( compilation != null );
                Contract.Requires( stream != null );
                Contract.Requires( previousResult != null );
                Contract.Ensures( Contract.Result<EmitResult>() != null );

                var mscorlib = project.MetadataReferences.SingleOrDefault( r => r.Display.EndsWith( "mscorlib.dll", StringComparison.OrdinalIgnoreCase ) );

                if ( mscorlib == null )
                    return previousResult;

                var facadeDirectory = Path.Combine( Path.GetDirectoryName( mscorlib.Display ), "Facades" );

                if ( !Directory.Exists( facadeDirectory ) )
                    return previousResult;

                var references = Directory.GetFiles( facadeDirectory ).Select( file => MetadataReference.CreateFromFile( file ) );
                var recompilation = compilation.AddReferences( references );

                return recompilation.Emit( stream );
            }

            private static bool IsIntellisenseFile( string file, string ext )
            {
                // intellisense (and other vs features) use the *.g.i.<lang> convention for auto-generated files; excluded these
                var name = Path.GetFileName( file );
                return name.EndsWith( ext, StringComparison.OrdinalIgnoreCase );
            }

            private static async Task<Project> GetProjectIncludingGeneratedFilesAsync( Project project, ProjectInformation projectInfo, AssemblyContentType contentType )
            {
                Contract.Requires( project != null );
                Contract.Requires( projectInfo != null );
                Contract.Ensures( Contract.Result<Task<Project>>() != null );

                var obj = projectInfo.IntermediateDirectory;
                var searchPattern = "*" + projectInfo.FileExtension;
                var files = Directory.EnumerateFiles( obj, searchPattern, SearchOption.AllDirectories );

                switch ( contentType )
                {
                    case AssemblyContentType.WindowsRuntime:
                        // since roslyn doesn't support pre-build activities for ms build (that I can find),
                        // we need to include auto-generated files by visual studio that are used by
                        // intellisense and other features. without these files, roslyn will fail to
                        // build the project
                        break;
                    case AssemblyContentType.Default:
                        // for non-WinRT applications, we want to include auto-generated files (ex: *.g.cs),
                        // but not intellisense files (*.g.i.cs). doing so typically causes compilation
                        // errors do to duplicate source code.
                        var ext = ".g.i" + projectInfo.FileExtension;
                        files = files.Where( f => !IsIntellisenseFile( f, ext ) );
                        break;
                }

                foreach ( var file in files )
                {
                    using ( var stream = new FileStream( file, FileMode.Open ) )
                    {
                        var reader = new StreamReader( stream );
                        var text = await reader.ReadToEndAsync();
                        var name = Path.GetFileNameWithoutExtension( file );

                        project = project.AddDocument( name, text ).Project;
                    }
                }

                return project;
            }

            private static async Task<Tuple<Assembly, IReadOnlyList<AssemblyName>>> CreateDynamicAssemblyAsync( ProjectInformation projectInfo, AssemblyContentType contentType )
            {
                Contract.Ensures( Contract.Result<Task<Tuple<Assembly, IReadOnlyList<AssemblyName>>>>() != null );

                if ( projectInfo == null )
                    return new Tuple<Assembly, IReadOnlyList<AssemblyName>>( null, new AssemblyName[0] );

                byte[] rawAssembly;
                IReadOnlyList<AssemblyName> referencedAssemblies;

                using ( var workspace = MSBuildWorkspace.Create() )
                {
                    var project = await workspace.OpenProjectAsync( projectInfo.ProjectPath ).ConfigureAwait( false );
                    var completeProject = await GetProjectIncludingGeneratedFilesAsync( project, projectInfo, contentType ).ConfigureAwait( false );
                    var compilation = await completeProject.GetCompilationAsync().ConfigureAwait( false );

                    using ( var stream = new MemoryStream() )
                    {
                        // compile into an in-memory assembly
                        var result = compilation.Emit( stream );

                        if ( !result.Success )
                        {
                            // HACK: roslyn does not resolve the facade assemblies (e.g. System.Runtime) automatically.
                            // it's unknown at this point (1/19/2015) whether this is a bug or the expected behavior
                            // as MSBuild does this automatically. if this is the expected behavior, the correct setup
                            // is also unknown. this process is seemingly expensive and should be refactored at some point.
                            if ( ReferencesFacadesIndirectly( completeProject ) )
                                result = CompileWithFacades( completeProject, compilation, stream, result );

                            // if compilation fails again, the assembly cannot be generated
                            if ( !result.Success )
                                return new Tuple<Assembly, IReadOnlyList<AssemblyName>>( null, new AssemblyName[0] );
                        }

                        rawAssembly = stream.ToArray();
                    }

                    // loading assemblies from binary will result in no location information being available. to ensure that referenced
                    // assemblies can be resolved, but a list of referenced assembly names from the metadata
                    referencedAssemblies = completeProject.MetadataReferences.Select( mr => AssemblyName.GetAssemblyName( mr.Display ) ).ToArray();
                }

                var assembly = Assembly.ReflectionOnlyLoad( rawAssembly );

                return new Tuple<Assembly, IReadOnlyList<AssemblyName>>( assembly, referencedAssemblies );
            }

            private static Assembly LoadLocalAssemblyFromExistingBuild( ProjectInformation projectInfo )
            {
                if ( projectInfo == null )
                    return null;

                var assemblyFile = projectInfo.TargetPath;

                if ( !File.Exists( assemblyFile ) )
                    return null;

                Debug.WriteLine( "Attempting to load existing assembly from {0}.", new object[] { assemblyFile } );
                return Assembly.ReflectionOnlyLoadFrom( assemblyFile );
            }

            public string Title
            {
                get;
                set;
            }

            public Type SelectedType
            {
                get;
                private set;
            }

            public string NameConvention
            {
                get;
                set;
            }

            public string[] RestrictedBaseTypeNames
            {
                get;
                set;
            }

            public ProjectInformation SourceProject
            {
                get;
                set;
            }
        }
    }
}
