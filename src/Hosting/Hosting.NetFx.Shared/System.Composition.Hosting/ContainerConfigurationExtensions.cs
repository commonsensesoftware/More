namespace More.Composition.Hosting
{
    using System;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="ContainerConfiguration"/> class.
    /// </summary>
    [CLSCompliant( false )]
    public static class ContainerConfigurationExtensions
    {
        /// <summary>
        /// Adds part types from assemblies in the current application domain defined in any relative probing paths.
        /// </summary>
        /// <param name="configuration">The extended <see cref="ContainerConfiguration">container configuration</see>.</param>
        /// <returns>The original <see cref="ContainerConfiguration">container configuration</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static ContainerConfiguration WithPrivateAssemblies( this ContainerConfiguration configuration )
        {
            Arg.NotNull( configuration, nameof( configuration ) );
            Contract.Ensures( Contract.Result<ContainerConfiguration>() != null );
            return configuration.WithPrivateAssemblies( (AttributedModelProvider) null );
        }

        /// <summary>
        /// Adds part types from assemblies in the current application domain defined in any relative probing paths using the specified conventions.
        /// </summary>
        /// <param name="configuration">The extended <see cref="ContainerConfiguration">container configuration</see>.</param>
        /// <param name="conventions">Conventions represented by a <see cref="AttributedModelProvider" /> or null.</param>
        /// <returns>The original <see cref="ContainerConfiguration">container configuration</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static ContainerConfiguration WithPrivateAssemblies( this ContainerConfiguration configuration, AttributedModelProvider conventions )
        {
            Arg.NotNull( configuration, nameof( configuration ) );
            Contract.Ensures( Contract.Result<ContainerConfiguration>() != null );

            var relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;

            // exit if no probing path for private assemblies is defined
            if ( string.IsNullOrEmpty( relativeSearchPath ) )
                return configuration;

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // find all libraries defined in probing paths
            var assemblyFiles = from relativePath in relativeSearchPath.Split( new[] { ';' }, StringSplitOptions.RemoveEmptyEntries )
                                let path = Path.Combine( baseDirectory, relativePath )
                                where Directory.Exists( path )
                                from dll in Directory.GetFiles( path, "*.dll" )
                                select dll;

            // get assemblies from paths
            var assemblies = from file in assemblyFiles
                             let name = AssemblyName.GetAssemblyName( file )
                             select Assembly.Load( name );

            // add all assemblies to the configuration
            configuration.WithAssemblies( assemblies, conventions );

            return configuration;
        }

        /// <summary>
        /// Adds part types from assemblies in the current application domain base directory as well as any defined, relative probing paths.
        /// </summary>
        /// <param name="configuration">The extended <see cref="ContainerConfiguration">container configuration</see>.</param>
        /// <returns>The original <see cref="ContainerConfiguration">container configuration</see>.</returns>
        /// <remarks>This method calls the <see cref="M:WithPrivateAssemblies"/> extension method to add assemblies in any
        /// defined relative probing paths.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static ContainerConfiguration WithAppDomain( this ContainerConfiguration configuration )
        {
            Arg.NotNull( configuration, nameof( configuration ) );
            Contract.Ensures( Contract.Result<ContainerConfiguration>() != null );
            return configuration.WithAppDomain( (AttributedModelProvider) null );
        }

        /// <summary>
        /// Adds part types from assemblies in the current application domain base directory as well as any defined, relative probing paths using the specified conventions.
        /// </summary>
        /// <param name="configuration">The extended <see cref="ContainerConfiguration">container configuration</see>.</param>
        /// <param name="conventions">Conventions represented by a <see cref="AttributedModelProvider" /> or null.</param>
        /// <returns>The original <see cref="ContainerConfiguration">container configuration</see>.</returns>
        /// <remarks>This method calls the <see cref="M:WithPrivateAssemblies"/> extension method to add assemblies in any
        /// defined relative probing paths.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static ContainerConfiguration WithAppDomain( this ContainerConfiguration configuration, AttributedModelProvider conventions )
        {
            Arg.NotNull( configuration, nameof( configuration ) );
            Contract.Ensures( Contract.Result<ContainerConfiguration>() != null );

            // find all the executables and libraries in the current base directory
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var assemblyFiles = Directory.GetFiles( baseDirectory, "*.exe" ).Union( Directory.GetFiles( baseDirectory, "*.dll" ) );
            var assemblies = from file in assemblyFiles
                             let name = AssemblyName.GetAssemblyName( file )
                             select Assembly.Load( name );

            // add all assemblies to the configuration
            configuration.WithAssemblies( assemblies, conventions );
            configuration.WithPrivateAssemblies( conventions );

            return configuration;
        }
    }
}
