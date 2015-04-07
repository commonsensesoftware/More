namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Manipulates folders and their contents, and provides information about them.
    /// </summary>
    [ContractClass( typeof( IFolderContract ) )]
    public interface IFolder : IStorageItem
    {
        /// <summary>
        /// Creates a new file in the current folder.
        /// </summary>
        /// <param name="desiredName">The desired name of the file to create.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the created <see cref="IFile">file</see>.</returns>
        Task<IFile> CreateFileAsync( string desiredName );
        
        /// <summary>
        /// Creates a new folder in the current folder.
        /// </summary>
        /// <param name="desiredName">The desired name of the folder to create.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the created <see cref="IFolder">folder</see>.</returns>
        Task<IFolder> CreateFolderAsync( string desiredName );
        
        /// <summary>
        /// Gets the specified file from the current folder.
        /// </summary>
        /// <param name="name">The name (or path relative to the current folder) of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFile">file</see>.</returns>
        Task<IFile> GetFileAsync( string name );
        
        /// <summary>
        /// Gets the files from the current folder.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IReadOnlyList{T}">read-only list</see>
        /// of <see cref="IFile">files</see>.</returns>
        Task<IReadOnlyList<IFile>> GetFilesAsync();

        /// <summary>
        /// Gets the specified folder from the current folder.
        /// </summary>
        /// <param name="name">The name of the child folder to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IFolder">folder</see>.</returns>
        Task<IFolder> GetFolderAsync( string name );

        /// <summary>
        /// Gets the folders in the current folder.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IReadOnlyList{T}">read-only list</see>
        /// of <see cref="IFolder">folders</see>.</returns>
        Task<IReadOnlyList<IFolder>> GetFoldersAsync();
        
        /// <summary>
        /// Gets the specified item from the current folder.
        /// </summary>
        /// <param name="name">The name of the item to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IStorageItem">storage item</see>.</returns>
        Task<IStorageItem> GetItemAsync( string name );

        /// <summary>
        /// Gets the items from the current folder.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IReadOnlyList{T}">read-only list</see>
        /// of <see cref="IStorageItem">storage items</see>.</returns>
        Task<IReadOnlyList<IStorageItem>> GetItemsAsync();
    }
}
