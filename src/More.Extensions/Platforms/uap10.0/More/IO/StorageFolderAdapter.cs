namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using global::Windows.Storage;

    sealed partial class StorageFolderAdapter : IFolder, IPlatformStorageItem<StorageFolder>
    {
        readonly StorageFolder folder;

        internal StorageFolderAdapter( StorageFolder folder )
        {
            Contract.Requires( folder != null );
            this.folder = folder;
        }

        public StorageFolder NativeStorageItem => folder;

        public async Task<IFile> CreateFileAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );
            var file = await folder.CreateFileAsync( desiredName );
            return new StorageFileAdapter( file );
        }

        public async Task<IFolder> CreateFolderAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );
            var folder = await this.folder.CreateFolderAsync( desiredName );
            return new StorageFolderAdapter( folder );
        }

        public async Task<IFile> GetFileAsync( string name )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );
            var file = await folder.GetFileAsync( name );
            return new StorageFileAdapter( file );
        }

        public async Task<IReadOnlyList<IFile>> GetFilesAsync()
        {
            var files = await folder.GetFilesAsync();
            return files.Select( f => new StorageFileAdapter( f ) ).ToArray();
        }

        public async Task<IFolder> GetFolderAsync( string name )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );
            var folder = await this.folder.GetFolderAsync( name );
            return new StorageFolderAdapter( folder );
        }

        public async Task<IReadOnlyList<IFolder>> GetFoldersAsync()
        {
            var folders = await folder.GetFoldersAsync();
            return folders.Select( f => new StorageFolderAdapter( f ) ).ToArray();
        }

        public async Task<IStorageItem> GetItemAsync( string name )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );

            var item = await this.folder.GetItemAsync( name );

            if ( item is StorageFile file )
            {
                return new StorageFileAdapter( file );
            }

            if ( item is StorageFolder folder )
            {
                return new StorageFolderAdapter( folder );
            }

            throw new NotSupportedException( ExceptionMessage.NativeStorageItemNotSupported.FormatDefault( item.GetType() ) );
        }

        public async Task<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            var nativeItems = await this.folder.GetItemsAsync();
            var items = new List<IStorageItem>();

            foreach ( var item in nativeItems )
            {
                if ( item is StorageFile file )
                {
                    items.Add( new StorageFileAdapter( file ) );
                }
                else if ( item is StorageFolder folder )
                {
                    items.Add( new StorageFolderAdapter( folder ) );
                }
            }

            return items.ToArray();
        }

        public DateTimeOffset DateCreated => folder.DateCreated;

        public string Name => folder.Name;

        public string Path => folder.Path;

        public Task DeleteAsync() => folder.DeleteAsync().AsTask();

        public async Task<IBasicProperties> GetBasicPropertiesAsync() => new BasicPropertiesAdapter( await folder.GetBasicPropertiesAsync() );

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );
            return folder.RenameAsync( desiredName ).AsTask();
        }

        public override bool Equals( object obj ) => Equals( obj as IPlatformStorageItem<StorageFolder> );

        public bool Equals( IStorageItem other ) => Equals( other as IPlatformStorageItem<StorageFolder> );

        bool Equals( IPlatformStorageItem<StorageFolder> other ) => other == null ? false : folder.Equals( other.NativeStorageItem );

        public override int GetHashCode() => folder.GetHashCode();
    }
}