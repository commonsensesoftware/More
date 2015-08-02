namespace More.VisualStudio.Views
{
    using Microsoft.CodeAnalysis.MSBuild;
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
    using ViewModels;

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
                    NameConvention = NameConvention,
                    FilterByConvention = !string.IsNullOrEmpty( NameConvention ),
                    LocalAssembly = localAssembly
                };

                model.RestrictedBaseTypeNames.AddRange( RestrictedBaseTypeNames ?? Enumerable.Empty<string>() );
                model.LocalAssemblyReferences.AddRange( localAssemblyReferences );

                using ( var view = new TypeBrowserDialog( model ) )
                {
                    var helper = new WindowInteropHelper( view )
                    {
                        Owner = owner
                    };

                    // change title from default value only if an alternate title is provided
                    if ( !string.IsNullOrEmpty( Title ) )
                        model.Title = Title;

                    var result = view.ShowDialog();

                    // if a type is selected, set it. wrap the type so that it is
                    // serialized across appdomain boundaries
                    if ( ( result ?? false ) )
                        SelectedType = new SimpleType( model.SelectedType );

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

            public async Task CreateLocalAssemblyAsync( AssemblyName localAssemblyName )
            {
                Contract.Requires( localAssemblyName != null );
                Contract.Ensures( Contract.Result<Task>() != null );

                try
                {
                    // try creating a dynamic assembly by compiling the source project with roslyn into memory
                    var result = await CreateDynamicAssemblyAsync( SourceProject, localAssemblyName.ContentType );

                    if ( ( localAssembly = result.Item1 ) != null )
                    {
                        localAssemblyReferences = result.Item2 ?? localAssembly.GetReferencedAssemblies();
                        return;
                    }
                }
                catch
                {
                    Debug.WriteLine( "Dynamic assembly generation failed." );
                }

                // if that fails, try loading the output assembly of the source project from the last build
                if ( ( localAssembly = LoadLocalAssemblyFromExistingBuild( SourceProject ) ) == null )
                    localAssemblyReferences = new AssemblyName[0];
                else
                    localAssemblyReferences = localAssembly.GetReferencedAssemblies();
            }

            private static async Task<Tuple<Assembly, IReadOnlyList<AssemblyName>>> CreateDynamicAssemblyAsync( ProjectInformation projectInfo, AssemblyContentType contentType )
            {
                Contract.Ensures( Contract.Result<Task<Tuple<Assembly, IReadOnlyList<AssemblyName>>>>() != null );

                if ( projectInfo == null )
                    return new Tuple<Assembly, IReadOnlyList<AssemblyName>>( null, new AssemblyName[0] );

                byte[] rawAssembly;
                IReadOnlyList<AssemblyName> referencedAssemblies;

                // create a msbuild workspace and get the compilation unit for the current project
                using ( var workspace = MSBuildWorkspace.Create() )
                {
                    var project = await workspace.OpenProjectAsync( projectInfo.ProjectPath ).ConfigureAwait( false );
                    var compilation = await project.GetCompilationAsync().ConfigureAwait( false );

                    using ( var stream = new MemoryStream() )
                    {
                        // compile into an in-memory assembly
                        var result = compilation.Emit( stream );

                        // handled failed compilation gracefully
                        if ( !result.Success )
                            return new Tuple<Assembly, IReadOnlyList<AssemblyName>>( null, new AssemblyName[0] );

                        rawAssembly = stream.ToArray();
                    }

                    // loading assemblies from binary will result in no location information being available. to ensure that referenced
                    // assemblies can be resolved, build a list of referenced assembly names from the metadata
                    referencedAssemblies = project.MetadataReferences.Select( mr => AssemblyName.GetAssemblyName( mr.Display ) ).ToArray();
                }

                // we only need the type name so we use a reflection-only load
                var assembly = Assembly.ReflectionOnlyLoad( rawAssembly );

                return Tuple.Create( assembly, referencedAssemblies );
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
