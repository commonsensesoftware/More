namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using global::Windows.Storage;

    internal sealed partial class StorageFolderAdapter : IFolder, IPlatformStorageItem<StorageFolder>
    {
        private readonly StorageFolder folder;

        internal StorageFolderAdapter( StorageFolder folder )
        {
            Contract.Requires( folder != null );
            this.folder = folder;
        }

        public StorageFolder NativeStorageItem
        {
            get
            {
                return this.folder;
            }
        }

        public async Task<IFile> CreateFileAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, "desiredName" );
            var file = await this.folder.CreateFileAsync( desiredName );
            return new StorageFileAdapter( file );
        }

        public async Task<IFolder> CreateFolderAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, "desiredName" );
            var folder = await this.folder.CreateFolderAsync( desiredName );
            return new StorageFolderAdapter( folder );
        }

        public async Task<IFile> GetFileAsync( string name )
        {
            Arg.NotNullOrEmpty( name, "name" );
            var file = await this.folder.GetFileAsync( name );
            return new StorageFileAdapter( file );
        }

        public async Task<IReadOnlyList<IFile>> GetFilesAsync()
        {
            var files = await this.folder.GetFilesAsync();
            return files.Select( f => new StorageFileAdapter( f ) ).ToArray();
        }

        public async Task<IFolder> GetFolderAsync( string name )
        {
            Arg.NotNullOrEmpty( name, "name" );
            var folder = await this.folder.GetFolderAsync( name );
            return new StorageFolderAdapter( folder );
        }

        public async Task<IReadOnlyList<IFolder>> GetFoldersAsync()
        {
            var folders = await this.folder.GetFoldersAsync();
            return folders.Select( f => new StorageFolderAdapter( f ) ).ToArray();
        }

        public async Task<IStorageItem> GetItemAsync( string name )
        {
            Arg.NotNullOrEmpty( name, "name" );

            var item = await this.folder.GetItemAsync( name );
            var file = item as StorageFile;

            if ( file != null )
                return new StorageFileAdapter( file );

            var folder = item as StorageFolder;

            if ( folder != null )
                return new StorageFolderAdapter( folder );

            return null;
        }

        public async Task<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            var nativeItems = await this.folder.GetItemsAsync();
            var items = new List<IStorageItem>();

            foreach ( var item in nativeItems )
            {
                var file = item as StorageFile;

                if ( file == null )
                {
                    var folder = item as StorageFolder;

                    if ( folder == null )
                        items.Add( new StorageFolderAdapter( folder ) );

                    continue;
                }

                items.Add( new StorageFileAdapter( file ) );
            }

            return items.ToArray();
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return this.folder.DateCreated;
            }
        }

        public string Name
        {
            get
            {
                return this.folder.Name;
            }
        }

        public string Path
        {
            get
            {
                return this.folder.Path;
            }
        }

        public Task DeleteAsync()
        {
            return this.folder.DeleteAsync().AsTask();
        }

        public async Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            var properties = await this.folder.GetBasicPropertiesAsync();
            return new BasicPropertiesAdapter( properties );
        }

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, "desiredName" );
            return this.folder.RenameAsync( desiredName ).AsTask();
        }

        public override bool Equals( object obj )
        {
            return this.Equals( obj as IPlatformStorageItem<StorageFolder> );
        }

        public bool Equals( IStorageItem other )
        {
            return this.Equals( other as IPlatformStorageItem<StorageFolder> );
        }

        private bool Equals( IPlatformStorageItem<StorageFolder> other )
        {
            return other == null ? false : this.folder.Equals( other.NativeStorageItem );
        }

        public override int GetHashCode()
        {
            return this.folder.GetHashCode();
        }
    }
}
