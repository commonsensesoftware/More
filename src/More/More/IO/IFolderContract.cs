namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IFolder ) )]
    abstract class IFolderContract : IFolder
    {
        Task<IFile> IFolder.CreateFileAsync( string desiredName )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredName ), nameof( desiredName ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return null;
        }

        Task<IFolder> IFolder.CreateFolderAsync( string desiredName )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredName ), nameof( desiredName ) );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );
            return null;
        }

        Task<IFile> IFolder.GetFileAsync( string name )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return null;
        }

        Task<IReadOnlyList<IFile>> IFolder.GetFilesAsync()
        {
            Contract.Ensures( Contract.Result<Task<IReadOnlyList<IFile>>>() != null );
            return null;
        }

        Task<IFolder> IFolder.GetFolderAsync( string name )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );
            return null;
        }

        Task<IReadOnlyList<IFolder>> IFolder.GetFoldersAsync()
        {
            Contract.Ensures( Contract.Result<Task<IReadOnlyList<IFolder>>>() != null );
            return null;
        }

        Task<IStorageItem> IFolder.GetItemAsync( string name )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IStorageItem>>() != null );
            return null;
        }

        Task<IReadOnlyList<IStorageItem>> IFolder.GetItemsAsync()
        {
            Contract.Ensures( Contract.Result<Task<IReadOnlyList<IStorageItem>>>() != null );
            return null;
        }

        DateTimeOffset IStorageItem.DateCreated => default( DateTimeOffset );

        string IStorageItem.Name => null;

        string IStorageItem.Path => null;

        Task IStorageItem.DeleteAsync() => null;

        Task<IBasicProperties> IStorageItem.GetBasicPropertiesAsync() => null;

        Task IStorageItem.RenameAsync( string desiredName ) => null;

        Task<IFolder> IStorageItem.GetParentAsync() => null;

        bool IEquatable<IStorageItem>.Equals( IStorageItem other ) => default( bool );
    }
}