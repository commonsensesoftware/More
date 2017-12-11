namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="IFolder"/> interface.
    /// </summary>
    public static class IFolderExtensions
    {
        /// <summary>
        /// Attempts to get a single file from the current folder by using the specified name.
        /// </summary>
        /// <param name="folder">The extended <see cref="IFolder">folder</see>.</param>
        /// <param name="name">The name of the file to try to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFile">file</see>
        /// or <c>null</c> if the operation fails.</returns>
        public static async Task<IFile> TryGetFileAsync( this IFolder folder, string name )
        {
            Arg.NotNull( folder, nameof( folder ) );
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );

            try
            {
                return await folder.GetFileAsync( name ).ConfigureAwait( false );
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
        /// <param name="folder">The extended <see cref="IFolder">folder</see>.</param>
        /// <param name="name">The name of the folder to try to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFolder">folder</see>
        /// or <c>null</c> if the operation fails.</returns>
        public static async Task<IFolder> TryGetFolderAsync( this IFolder folder, string name )
        {
            Arg.NotNull( folder, nameof( folder ) );
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );

            try
            {
                return await folder.GetFolderAsync( name ).ConfigureAwait( false );
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
        /// Attempts to get a single file or sub-folder from the current folder by using the name of the item.
        /// </summary>
        /// <param name="folder">The extended <see cref="IFolder">folder</see>.</param>
        /// <param name="name">The name (or path relative to the current folder) of the file or sub-folder to try to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IStorageItem">storage item</see>
        /// or <c>null</c> if the operation fails.</returns>
        public static async Task<IStorageItem> TryGetItemAsync( this IFolder folder, string name )
        {
            Arg.NotNull( folder, nameof( folder ) );
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IStorageItem>>() != null );

            try
            {
                return await folder.GetItemAsync( name ).ConfigureAwait( false );
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
        /// Gets or creates a single folder from the current folder by using the specified name.
        /// </summary>
        /// <param name="folder">The extended <see cref="IFolder">folder</see>.</param>
        /// <param name="name">The name of the folder to get or create.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFolder">folder</see>.</returns>
        public static async Task<IFolder> GetOrCreateFolderAsync( this IFolder folder, string name )
        {
            Arg.NotNull( folder, nameof( folder ) );
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );

            try
            {
                return await folder.GetFolderAsync( name ).ConfigureAwait( false );
            }
            catch ( IOException )
            {
                return await folder.CreateFolderAsync( name ).ConfigureAwait( false );
            }
        }

        /// <summary>
        /// Gets or creates a single file from the current folder by using the specified name.
        /// </summary>
        /// <param name="folder">The extended <see cref="IFolder">folder</see>.</param>
        /// <param name="name">The name of the file to get or create.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFile">file</see>.</returns>
        public static async Task<IFile> GetOrCreateFileAsync( this IFolder folder, string name )
        {
            Arg.NotNull( folder, nameof( folder ) );
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );

            try
            {
                return await folder.GetFileAsync( name ).ConfigureAwait( false );
            }
            catch ( IOException )
            {
                return await folder.CreateFileAsync( name ).ConfigureAwait( false );
            }
        }
    }
}