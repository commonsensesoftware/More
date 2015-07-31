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
                return folder;
            }
        }

        public Task<IFile> CreateFileAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );

            var newFileName = System.IO.Path.Combine( folder.FullName, desiredName );
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
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );

            IFolder newFolder = new DirectoryInfoAdapter( folder.CreateSubdirectory( desiredName ) );
            return Task.FromResult( newFolder );
        }

        public Task<IFile> GetFileAsync( string name )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );
            
            var fileInfo = folder.EnumerateFiles().FirstOrDefault( f => f.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
            IFile file = fileInfo == null ? null : new FileInfoAdapter( fileInfo );
            return Task.FromResult( file );
        }

        public Task<IReadOnlyList<IFile>> GetFilesAsync()
        {
            var query = from fileInfo in folder.EnumerateFiles()
                        let file = (IFile) new FileInfoAdapter( fileInfo )
                        select file;
            IReadOnlyList<IFile> files = query.ToArray();
            return Task.FromResult( files );
        }

        public Task<IFolder> GetFolderAsync( string name )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );

            var directoryInfo = folder.EnumerateDirectories().FirstOrDefault( d => d.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );
            IFolder result = directoryInfo == null ? null : new DirectoryInfoAdapter( directoryInfo );
            return Task.FromResult( result );
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
            Arg.NotNullOrEmpty( name, nameof( name ) );

            var folder = await GetFolderAsync( name );

            if ( folder == null )
                return await GetFileAsync( name );

            return folder;
        }

        public async Task<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            IReadOnlyList<IStorageItem> items = ( await GetFoldersAsync() ).Cast<IStorageItem>().Union( await GetFilesAsync() ).ToArray();
            return items;
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return folder.CreationTime;
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
                return folder.FullName;
            }
        }

        public Task DeleteAsync()
        {
            folder.Delete( true );
            return Task.FromResult( 0 );
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            IBasicProperties properties = new BasicPropertiesAdapter<DirectoryInfo>( folder );
            return Task.FromResult( properties );
        }

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );

            folder.MoveTo( desiredName );
            return Task.FromResult( 0 );
        }

        public Task<IFolder> GetParentAsync()
        {
            IFolder parent = folder.Parent == null ? null : new DirectoryInfoAdapter( folder.Parent );
            return Task.FromResult( parent );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as IStorageItem );
        }

        public bool Equals( IStorageItem other )
        {
            if ( other is IFolder )
                return folder.FullName.Equals( other.Path, StringComparison.OrdinalIgnoreCase );

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode( folder.FullName );
        }
    }
}
