namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    internal sealed class DirectoryInfoAdapter : IFolder, IPlatformStorageItem<DirectoryInfo>
    {
        private readonly DirectoryInfo folder;

        internal DirectoryInfoAdapter( DirectoryInfo folder )
        {
            Contract.Requires( folder != null );
            this.folder = folder;
        }
        public DirectoryInfo NativeStorageItem
        {
            get
            {
                return this.folder;
            }
        }

        public Task<IFile> CreateFileAsync( string desiredName )
        {
            var newFileName = System.IO.Path.Combine( this.folder.FullName, desiredName );
            var newFile = new FileInfo( newFileName );

            // close the stream immediately; creates a zero-byte file
            using ( newFile.Create() )
            {
            }

            IFile file = new FileInfoAdapter( newFile );
            return Task.FromResult( file );
        }

        public Task<IFolder> CreateFolderAsync( string desiredName )
        {
            IFolder newFolder = new DirectoryInfoAdapter( this.folder.CreateSubdirectory( desiredName ) );
            return Task.FromResult( newFolder );
        }

        public Task<IFile> GetFileAsync( string name )
        {
            var fileInfo = this.folder.EnumerateFiles().FirstOrDefault( f => f.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
            IFile file = fileInfo == null ? null : new FileInfoAdapter( fileInfo );
            return Task.FromResult( file );
        }

        public Task<IReadOnlyList<IFile>> GetFilesAsync()
        {
            var query = from fileInfo in this.folder.EnumerateFiles()
                        let file = (IFile) new FileInfoAdapter( fileInfo )
                        select file;
            IReadOnlyList<IFile> files = query.ToArray();
            return Task.FromResult( files );
        }

        public Task<IFolder> GetFolderAsync( string name )
        {
            var directoryInfo = this.folder.EnumerateDirectories().FirstOrDefault( d => d.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
            IFolder folder = directoryInfo == null ? null : new DirectoryInfoAdapter( directoryInfo );
            return Task.FromResult( folder );
        }

        public Task<IReadOnlyList<IFolder>> GetFoldersAsync()
        {
            var query = from directoryInfo in this.folder.EnumerateDirectories()
                        let folder = (IFolder) new DirectoryInfoAdapter( directoryInfo )
                        select folder;
            IReadOnlyList<IFolder> folders = query.ToArray();
            return Task.FromResult( folders );
        }

        public async Task<IStorageItem> GetItemAsync( string name )
        {
            var folder = await this.GetFolderAsync( name );

            if ( folder == null )
                return await this.GetFileAsync( name );

            return folder;
        }

        public async Task<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            IReadOnlyList<IStorageItem> items = ( await this.GetFoldersAsync() ).Cast<IStorageItem>().Union( await this.GetFilesAsync() ).ToArray();
            return items;
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return this.folder.CreationTime;
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
                return this.folder.FullName;
            }
        }

        public async Task DeleteAsync()
        {
            this.folder.Delete( true );
            await Task.Yield();
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            IBasicProperties properties = new BasicPropertiesAdapter<DirectoryInfo>( this.folder );
            return Task.FromResult( properties );
        }

        public async Task RenameAsync( string desiredName )
        {
            this.folder.MoveTo( desiredName );
            await Task.Yield();
        }

        public Task<IFolder> GetParentAsync()
        {
            IFolder parent = this.folder.Parent == null ? null : new DirectoryInfoAdapter( this.folder.Parent );
            return Task.FromResult( parent );
        }

        public override bool Equals( object obj )
        {
            return this.Equals( obj as IStorageItem );
        }

        public bool Equals( IStorageItem other )
        {
            if ( other is IFolder )
                return this.folder.FullName.Equals( other.Path, StringComparison.OrdinalIgnoreCase );

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode( this.folder.FullName );
        }
    }
}
