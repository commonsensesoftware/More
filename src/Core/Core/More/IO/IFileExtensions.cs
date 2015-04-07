namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="IFile"/> interface.
    /// </summary>
    public static class IFileExtensions
    {
        /// <summary>
        /// Creates a copy of the file in the specified folder.
        /// </summary>
        /// <param name="file">The extended <see cref="IFile">file</see>.</param>
        /// <param name="destinationFolder">The destination folder where the copy is created.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the  <see cref="IFile">file</see> that represents the copy.</returns>
        public static Task<IFile> CopyAsync( this IFile file, IFolder destinationFolder )
        {
            Contract.Requires<ArgumentNullException>( file != null, "file" );
            Contract.Requires<ArgumentNullException>( destinationFolder != null, "destinationFolder" );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return file.CopyAsync( destinationFolder, file.Name );
        }

        /// <summary>
        /// Moves the current file to the specified folder.
        /// </summary>
        /// <param name="file">The extended <see cref="IFile">file</see>.</param>
        /// <param name="destinationFolder">The destination folder where the file is moved. This destination folder must be a physical location.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        public static Task MoveAsync( this IFile file, IFolder destinationFolder )
        {
            Contract.Requires<ArgumentNullException>( file != null, "file" );
            Contract.Requires<ArgumentNullException>( destinationFolder != null, "destinationFolder" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return file.MoveAsync( destinationFolder, file.Name );
        }
    }
}
