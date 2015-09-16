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
                return folder;
            }
        }

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
                return folder.DateCreated;
            }
        }

        public string Name
        {
            get
            {
                return folder.Name;
            }
        }

        public string Path
        {
            get
            {
                return folder.Path;
            }
        }

        public Task DeleteAsync() => folder.DeleteAsync().AsTask();

        public async Task<IBasicProperties> GetBasicPropertiesAsync() => new BasicPropertiesAdapter( await folder.GetBasicPropertiesAsync() );

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );
            return folder.RenameAsync( desiredName ).AsTask();
        }

        public override bool Equals( object obj ) => Equals( obj as IPlatformStorageItem<StorageFolder> );

        public bool Equals( IStorageItem other ) => Equals( other as IPlatformStorageItem<StorageFolder> );

        private bool Equals( IPlatformStorageItem<StorageFolder> other ) => other == null ? false : folder.Equals( other.NativeStorageItem );

        public override int GetHashCode() => folder.GetHashCode();
    }
}
