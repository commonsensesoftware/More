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
        sealed class Proxy : MarshalByRefObject
        {
            Assembly localAssembly;
            IReadOnlyList<AssemblyName> localAssemblyReferences;

            [SuppressMessage( "Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "False positive. The field is initialized inline and a method cannot be called any other way." )]
            static Proxy() => SetDefaultTypeBrowserSize( new Size( 500, 500 ) );

            public bool? ShowDialog( IntPtr owner )
            {
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

                    if ( !string.IsNullOrEmpty( Title ) )
                    {
                        model.Title = Title;
                    }

                    var result = view.ShowDialog();

                    if ( ( result ?? false ) )
                    {
                        SelectedType = new SimpleType( model.SelectedType );
                    }

                    helper.Owner = IntPtr.Zero;

                    return result;
                }
            }

            static void SetDefaultTypeBrowserSize( Size size )
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

                if ( ( localAssembly = LoadLocalAssemblyFromExistingBuild( SourceProject ) ) == null )
                {
                    localAssemblyReferences = Array.Empty<AssemblyName>();
                }
                else
                {
                    localAssemblyReferences = localAssembly.GetReferencedAssemblies();
                }
            }

            static async Task<Tuple<Assembly, IReadOnlyList<AssemblyName>>> CreateDynamicAssemblyAsync( ProjectInformation projectInfo, AssemblyContentType contentType )
            {
                Contract.Ensures( Contract.Result<Task<Tuple<Assembly, IReadOnlyList<AssemblyName>>>>() != null );

                var rawAssembly = default( byte[] );
                var referencedAssemblies = default( IReadOnlyList<AssemblyName> );

                if ( projectInfo == null )
                {
                    referencedAssemblies = Array.Empty<AssemblyName>();
                    return Tuple.Create( default( Assembly ), referencedAssemblies );
                }

                using ( var workspace = MSBuildWorkspace.Create() )
                {
                    var project = await workspace.OpenProjectAsync( projectInfo.ProjectPath ).ConfigureAwait( false );
                    var compilation = await project.GetCompilationAsync().ConfigureAwait( false );

                    using ( var stream = new MemoryStream() )
                    {
                        var result = compilation.Emit( stream );

                        if ( !result.Success )
                        {
                            referencedAssemblies = Array.Empty<AssemblyName>();
                            return Tuple.Create( default( Assembly ), referencedAssemblies );
                        }

                        rawAssembly = stream.ToArray();
                    }

                    // loading assemblies from binary will result in no location information being available. to ensure that referenced
                    // assemblies can be resolved, build a list of referenced assembly names from the metadata
                    referencedAssemblies = project.MetadataReferences.Select( mr => AssemblyName.GetAssemblyName( mr.Display ) ).ToArray();
                }

                var assembly = Assembly.ReflectionOnlyLoad( rawAssembly );

                return Tuple.Create( assembly, referencedAssemblies );
            }

            static Assembly LoadLocalAssemblyFromExistingBuild( ProjectInformation projectInfo )
            {
                if ( projectInfo == null )
                {
                    return null;
                }

                var assemblyFile = projectInfo.TargetPath;

                if ( !File.Exists( assemblyFile ) )
                {
                    return null;
                }

                Debug.WriteLine( $"Attempting to load existing assembly from {assemblyFile}." );
                return Assembly.ReflectionOnlyLoadFrom( assemblyFile );
            }

            public string Title { get; set; }

            public Type SelectedType { get; set; }

            public string NameConvention { get; set; }

            public string[] RestrictedBaseTypeNames { get; set; }

            public ProjectInformation SourceProject { get; set; }
        }
    }
}