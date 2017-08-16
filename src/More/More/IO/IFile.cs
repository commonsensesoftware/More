namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a file with information about its contents and ways to manipulate it.
    /// </summary>
    [ContractClass( typeof( IFileContract ) )]
    public interface IFile : IStorageItem
    {
        /// <summary>
        /// Gets the MIME type of the contents of the file.
        /// </summary>
        /// <value>The MIME type of the file contents.</value>
        string ContentType { get; }

        /// <summary>
        /// Gets the type (file name extension) of the file.
        /// </summary>
        /// <value>The file name extension of the file.</value>
        string FileType { get; }

        /// <summary>
        /// Replaces the specified file with a copy of the current file.
        /// </summary>
        /// <param name="fileToReplace">The file to replace.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task CopyAndReplaceAsync( IFile fileToReplace );

        /// <summary>
        /// Creates a copy of the file in the specified folder, using the desired name.
        /// </summary>
        /// <param name="destinationFolder">The destination folder where the copy is created.</param>
        /// <param name="desiredNewName">The desired name of the copy.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the  <see cref="IFile">file</see> that represents the copy.</returns>
        Task<IFile> CopyAsync( IFolder destinationFolder, string desiredNewName );

        /// <summary>
        /// Moves the current file to the location of the specified file and replaces the specified file in that location.
        /// </summary>
        /// <param name="fileToReplace">The file to replace.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task MoveAndReplaceAsync( IFile fileToReplace );

        /// <summary>
        /// Moves the current file to the specified folder and renames the file according to the desired name.
        /// </summary>
        /// <param name="destinationFolder">The destination folder where the file is moved. This destination folder must be a physical location.</param>
        /// <param name="desiredNewName">The desired name of the file after it is moved.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task MoveAsync( IFolder destinationFolder, string desiredNewName );

        /// <summary>
        /// Opens a stream for read access.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the opened <see cref="Stream">stream</see>.</returns>
        Task<Stream> OpenReadAsync();

        /// <summary>
        /// Opens a stream for read and write access.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the opened <see cref="Stream">stream</see>.</returns>
        Task<Stream> OpenReadWriteAsync();
    }
}