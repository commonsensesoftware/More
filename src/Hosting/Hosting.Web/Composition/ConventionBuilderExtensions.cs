namespace More.Composition
{
    using System;
    using System.Composition.Convention;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="ConventionBuilder"/> class.
    /// </summary>
    [CLSCompliant( false )]
    public static class ConventionBuilderExtensions
    {
        private static bool IsInFolder( Type type, string folderName )
        {
            return !type.IsAssignableFrom( typeof( Attribute ) ) && IsInNamespace( type, folderName );
        }

        private static bool IsInNamespace( Type type, string namespaceFragment )
        {
            if ( type.Namespace == null )
                return false;

            var fragment = "." + namespaceFragment;

            if ( type.Namespace.EndsWith( fragment, StringComparison.Ordinal ) )
                return true;

            fragment += ".";

            return type.Namespace.Contains( fragment );
        }

        /// <summary>
        /// Adds a convention to export all types in the specified namespace.
        /// </summary>
        /// <param name="builder">The extended <see cref="ConventionBuilder">convention builder</see>.</param>
        /// <param name="folder">The name of the namespace to export types from.</param>
        /// <returns>The original <see cref="ConventionBuilder">convention builder</see>.</returns>
        /// <remarks>Matched types and their implemented interfaces are exported.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static ConventionBuilder AddNamespace( this ConventionBuilder builder, string folder )
        {
            Contract.Requires<ArgumentNullException>( builder != null, "builder" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( folder ), "folder" );
            Contract.Ensures( Contract.Result<ConventionBuilder>() != null );

            var dir = folder;
            builder.ForTypesMatching( t => IsInFolder( t, dir ) ).Export().ExportInterfaces();

            return builder;
        }

        /// <summary>
        /// Adds a convention to export all types in namespaces that contain the name "Parts".
        /// </summary>
        /// <param name="builder">The extended <see cref="ConventionBuilder">convention builder</see>.</param>
        /// <returns>The original <see cref="ConventionBuilder">convention builder</see>.</returns>
        /// <remarks>Matched types and their implemented interfaces are exported.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static ConventionBuilder AddPartsNamespace( this ConventionBuilder builder )
        {
            Contract.Requires<ArgumentNullException>( builder != null, "builder" );
            Contract.Ensures( Contract.Result<ConventionBuilder>() != null );
            return builder.AddNamespace( "Parts" );
        }
    }
}
