namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IFolder ) )]
    internal abstract class IFolderContract : IFolder
    {
        Task<IFile> IFolder.CreateFileAsync( string desiredName )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredName ), "desiredName" );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return null;
        }

        Task<IFolder> IFolder.CreateFolderAsync( string desiredName )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredName ), "desiredName" );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );
            return null;
        }

        Task<IFile> IFolder.GetFileAsync( string name )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
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
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
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
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Ensures( Contract.Result<Task<IStorageItem>>() != null );
            return null;
        }

        Task<IReadOnlyList<IStorageItem>> IFolder.GetItemsAsync()
        {
            Contract.Ensures( Contract.Result<Task<IReadOnlyList<IStorageItem>>>() != null );
            return null;
        }

        DateTimeOffset IStorageItem.DateCreated
        {
            get
            {
                return default( DateTimeOffset );
            }
        }

        string IStorageItem.Name
        {
            get
            {
                return null;
            }
        }

        string IStorageItem.Path
        {
            get
            {
                return null;
            }
        }

        Task IStorageItem.DeleteAsync()
        {
            return null;
        }

        Task<IBasicProperties> IStorageItem.GetBasicPropertiesAsync()
        {
            return null;
        }

        Task IStorageItem.RenameAsync( string desiredName )
        {
            return null;
        }

        Task<IFolder> IStorageItem.GetParentAsync()
        {
            return null;
        }

        bool IEquatable<IStorageItem>.Equals( IStorageItem other )
        {
            return default( bool );
        }
    }
}
