namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a file system that provides access to folders and files.
    /// </summary>
    [ContractClass( typeof( IFileSystemContract ) )]
    public interface IFileSystem
    {
        /// <summary>
        /// Gets the folder that has the specified path in the file system.
        /// </summary>
        /// <param name="path">The path of the folder to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFolder">folder</see>.</returns>
        Task<IFolder> GetFolderAsync( string path );

        /// <summary>
        /// Gets the file that has the specified path in the file system.
        /// </summary>
        /// <param name="path">The path of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFile">file</see>.</returns>
        Task<IFile> GetFileAsync( string path );

        /// <summary>
        /// Gets the file that has the specified Uniform Resource Indicator (URI) in the file system.
        /// </summary>
        /// <param name="uri">The <see cref="Uri">URI</see> of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFile">file</see>.</returns>
        Task<IFile> GetFileAsync( Uri uri );
    }
}