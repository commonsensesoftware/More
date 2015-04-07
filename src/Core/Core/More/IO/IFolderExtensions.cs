namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="IFolder"/> interface.
    /// </summary>
    public static class IFolderExtensions
    {
        /// <summary>
        /// Attempts to get a single file or sub-folder from the current folder by using the name of the item.
        /// </summary>
        /// <param name="folder">The extended <see cref="IFolder">folder</see>.</param>
        /// <param name="name">The name (or path relative to the current folder) of the file or sub-folder to try to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IStorageItem">storage item</see>
        /// or <c>null</c> if the operation fails.</returns>
        public static async Task<IStorageItem> TryGetItemAsync( this IFolder folder, string name )
        {
            Contract.Requires<ArgumentNullException>( folder != null, "folder" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Ensures( Contract.Result<Task<IStorageItem>>() != null );

            try
            {
                return await folder.GetItemAsync( name );
            }
            catch
            {
                return null;
            }
        }
    }
}
