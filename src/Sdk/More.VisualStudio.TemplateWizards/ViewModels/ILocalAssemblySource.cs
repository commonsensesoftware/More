namespace More.VisualStudio.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    /// <summary>
    /// Defines the behavior of a local assembly source.
    /// </summary>
    public interface ILocalAssemblySource
    {
        /// <summary>
        /// Gets the local assembly.
        /// </summary>
        /// <value>The local <see cref="Assembly">assembly</see>.</value>
        Assembly LocalAssembly { get; }

        /// <summary>
        /// Gets the local assembly name used.
        /// </summary>
        /// <value>The local <see cref="AssemblyName">assembly name</see>.</value>
        AssemblyName LocalAssemblyName { get; }

        /// <summary>
        /// Gets a collection of references used by the local assembly.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="AssemblyName">assembly references</see>
        /// used by the <see cref="LocalAssembly">local assembly</see>.</value>
        IReadOnlyList<AssemblyName> LocalAssemblyReferences { get; }
    }
}