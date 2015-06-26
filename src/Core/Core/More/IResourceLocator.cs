namespace More
{
    using More.IO;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the behavior of a resource locator. 
    /// </summary>
    [ContractClass( typeof( IResourceLocatorContract ))]
    public interface IResourceLocator
    {
        /// <summary>
        /// Returns the resource string with the specified name.
        /// </summary>
        /// <param name="name">The name of the string to retrieve.</param>
        /// <returns>The string with the specified name or <c>null</c>.</returns>
        string GetString( string name );

        /// <summary>
        /// Returns the resource file with the specified name asynchronously.
        /// </summary>
        /// <param name="name">The name of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the resource <see cref="IFile">file</see>.</returns>
        Task<IFile> GetFileAsync( string name );
    }
}
