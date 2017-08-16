namespace System.ComponentModel.Design
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// Provides an interface to retrieve an assembly or type by name.
    /// </summary>
    public interface ITypeResolutionService
    {
        /// <summary>
        /// Gets the requested assembly.
        /// </summary>
        /// <param name="name">The name of the assembly to retrieve.</param>
        /// <returns>An instance of the requested <see cref="Assembly">assembly</see>, or null if no assembly can be located.</returns>
        Assembly GetAssembly( AssemblyName name );

        /// <summary>
        /// Gets the requested assembly.
        /// </summary>
        /// <param name="name">The name of the assembly to retrieve.</param>
        /// <param name="throwOnError">True if this method should throw an exception if the assembly cannot be located; otherwise,
        /// false, and this method returns null if the assembly cannot be located.</param>
        /// <returns>An instance of the requested <see cref="Assembly">assembly</see>, or null if no assembly can be located.</returns>
        Assembly GetAssembly( AssemblyName name, bool throwOnError );

        /// <summary>
        /// Gets the path to the file from which the assembly was loaded.
        /// </summary>
        /// <param name="name">The name of the assembly.</param>
        /// <returns>The path to the file from which the assembly was loaded.</returns>
        string GetPathOfAssembly( AssemblyName name );

        /// <summary>
        /// Loads a type with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Type">type</see>. If the type name is not a fully qualified name that
        /// indicates an assembly, this service will search its internal set of referenced assemblies.</param>
        /// <returns>An instance of <see cref="Type">type</see> that corresponds to the specified name, or null if no type can be found.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "GetType", Justification = "Ported from BCL." )]
        Type GetType( string name );

        /// <summary>
        /// Loads a type with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Type">type</see>. If the type name is not a fully qualified name that
        /// indicates an assembly, this service will search its internal set of referenced assemblies.</param>
        /// <param name="throwOnError">True if this method should throw an exception if the type cannot be located; otherwise,
        /// false, and this method returns null if the type cannot be located.</param>
        /// <returns>An instance of <see cref="Type">type</see> that corresponds to the specified name, or null if no type can be found.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "GetType", Justification = "Ported from BCL." )]
        Type GetType( string name, bool throwOnError );

        /// <summary>
        /// Loads a type with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Type">type</see>. If the type name is not a fully qualified name that
        /// indicates an assembly, this service will search its internal set of referenced assemblies.</param>
        /// <param name="throwOnError">True if this method should throw an exception if the type cannot be located; otherwise,
        /// false, and this method returns null if the type cannot be located.</param>
        /// <param name="ignoreCase">True to ignore casing when searching for types; otherwise, false.</param>
        /// <returns>An instance of <see cref="Type">type</see> that corresponds to the specified name, or null if no type can be found.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "GetType", Justification = "Ported from BCL." )]
        Type GetType( string name, bool throwOnError, bool ignoreCase );
        
        /// <summary>
        /// Adds a reference to the specified assembly.
        /// </summary>
        /// <param name="name">An <see cref="AssemblyName"/> that indicates the assembly to reference.</param>
        void ReferenceAssembly( AssemblyName name );
    }
}