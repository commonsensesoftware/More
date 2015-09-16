namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using static System.IO.Path;

    internal sealed class DirectoryInfoAdapter : IFolder, IPlatformStorageItem<DirectoryInfo>
    {
        private static readonly Task CompletedTask = Task.FromResult( false );
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

            var newFileName = Combine( folder.FullName, desiredName );
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

            if ( fileInfo == null )
                throw new FileNotFoundException( SR.PathNotFound.FormatDefault( Combine( folder.FullName, name ) ) );

            IFile file = new FileInfoAdapter( fileInfo );
            return Task.FromResult( file );
        }

        public Task<IReadOnlyList<IFile>> GetFilesAsync() => Task.FromResult<IReadOnlyList<IFile>>( folder.EnumerateFiles().Select( fi => new FileInfoAdapter( fi ) ).ToArray() );

        public Task<IFolder> GetFolderAsync( string name )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );

            var directoryInfo = folder.EnumerateDirectories().FirstOrDefault( d => d.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );

            if ( directoryInfo == null )
                throw new DirectoryNotFoundException( SR.PathNotFound.FormatDefault( Combine( folder.FullName, name ) ) );

            IFolder result = new DirectoryInfoAdapter( directoryInfo );
            return Task.FromResult( result );
        }

        public Task<IReadOnlyList<IFolder>> GetFoldersAsync() => Task.FromResult<IReadOnlyList<IFolder>>( folder.EnumerateDirectories().Select( di => new DirectoryInfoAdapter( di ) ).ToArray() );

        public async Task<IStorageItem> GetItemAsync( string name )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );

            try
            {
                // we don't know if it's a folder or file; try folder first
                return await GetFolderAsync( name ).ConfigureAwait( false );
            }
            catch ( DirectoryNotFoundException )
            {
                try
                {
                    // try a file
                    return await GetFileAsync( name ).ConfigureAwait( false );
                }
                catch ( FileNotFoundException )
                {
                    // use base IOException since we don't know if the intent was for a folder or file
                    throw new IOException( SR.PathNotFound.FormatDefault( Combine( folder.FullName, name ) ) );
                }
            }
        }

        public async Task<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            var foldersTask = GetFoldersAsync();
            var filesTask = GetFilesAsync();

            // get folders and files in parallel
            await Task.WhenAll( foldersTask, filesTask ).ConfigureAwait( false );

            // union the results
            var folders = foldersTask.Result;
            var files = filesTask.Result;
            var count = folders.Count + files.Count;
            var items = new IStorageItem[count];
            var i = 0;

            for ( var j = 0; j < folders.Count; j++ )
                items[i++] = folders[j];

            for ( var j = 0; j < filesTask.Result.Count; j++ )
                items[i++] = files[j];

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
            return CompletedTask;
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync() => Task.FromResult<IBasicProperties>( new BasicPropertiesAdapter<DirectoryInfo>( folder ) );

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );

            folder.MoveTo( desiredName );
            return CompletedTask;
        }

        public Task<IFolder> GetParentAsync() => Task.FromResult<IFolder>( folder.Parent == null ? null : new DirectoryInfoAdapter( folder.Parent ) );

        public override bool Equals( object obj ) => Equals( obj as IStorageItem );

        public bool Equals( IStorageItem other ) => ( other is IFolder ) && folder.FullName.Equals( other.Path, StringComparison.OrdinalIgnoreCase );

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode( folder.FullName );
    }
}
