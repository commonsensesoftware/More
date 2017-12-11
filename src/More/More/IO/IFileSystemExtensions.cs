namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="IFileSystem"/> interface.
    /// </summary>
    public static class IFileSystemExtensions
    {
        /// <summary>
        /// Attempts to get a single file from the file system by using the specified path.
        /// </summary>
        /// <param name="fileSystem">The extended <see cref="IFileSystem">file system</see>.</param>
        /// <param name="path">The path of the file to try to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFile">file</see>
        /// or <c>null</c> if the operation fails.</returns>
        public static async Task<IFile> TryGetFileAsync( this IFileSystem fileSystem, string path )
        {
            Arg.NotNull( fileSystem, nameof( fileSystem ) );
            Arg.NotNullOrEmpty( path, nameof( path ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );

            try
            {
                return await fileSystem.GetFileAsync( path ).ConfigureAwait( false );
            }
            catch ( IOException )
            {
            }
            catch ( UnauthorizedAccessException )
            {
            }
            catch ( ArgumentException )
            {
            }

            return null;
        }

        /// <summary>
        /// Attempts to get a single file from the file system by using the specified path.
        /// </summary>
        /// <param name="fileSystem">The extended <see cref="IFileSystem">file system</see>.</param>
        /// <param name="uri">The <see cref="Uri">URI</see> of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFile">file</see>
        /// or <c>null</c> if the operation fails.</returns>
        public static async Task<IFile> TryGetFileAsync( this IFileSystem fileSystem, Uri uri )
        {
            Arg.NotNull( fileSystem, nameof( fileSystem ) );
            Arg.NotNull( uri, nameof( uri ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );

            try
            {
                return await fileSystem.GetFileAsync( uri ).ConfigureAwait( false );
            }
            catch ( IOException )
            {
            }
            catch ( UnauthorizedAccessException )
            {
            }
            catch ( ArgumentException )
            {
            }

            return null;
        }

        /// <summary>
        /// Attempts to get a single folder from the current folder by using the specified name.
        /// </summary>
        /// <param name="fileSystem">The extended <see cref="IFileSystem">file system</see>.</param>
        /// <param name="path">The path of the folder to try to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFolder">folder</see>
        /// or <c>null</c> if the operation fails.</returns>
        public static async Task<IFolder> TryGetFolderAsync( this IFileSystem fileSystem, string path )
        {
            Arg.NotNull( fileSystem, nameof( fileSystem ) );
            Arg.NotNullOrEmpty( path, nameof( path ) );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );

            try
            {
                return await fileSystem.GetFolderAsync( path ).ConfigureAwait( false );
            }
            catch ( IOException )
            {
            }
            catch ( UnauthorizedAccessException )
            {
            }
            catch ( ArgumentException )
            {
            }

            return null;
        }
    }
}