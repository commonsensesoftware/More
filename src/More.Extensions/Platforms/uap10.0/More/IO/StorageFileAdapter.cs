namespace More.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using global::Windows.Storage;

    sealed class StorageFileAdapter : IFile, IPlatformStorageItem<StorageFile>
    {
        readonly StorageFile file;

        internal StorageFileAdapter( StorageFile file )
        {
            Contract.Requires( file != null );
            this.file = file;
        }

        public StorageFile NativeStorageItem => file;

        public string ContentType => file.ContentType;

        public string FileType => file.FileType;

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task CopyAndReplaceAsync( IFile fileToReplace )
        {
            Arg.NotNull( fileToReplace, nameof( fileToReplace ) );
            return file.CopyAndReplaceAsync( fileToReplace.AsFile() ).AsTask();
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public async Task<IFile> CopyAsync( IFolder destinationFolder, string desiredNewName )
        {
            Arg.NotNull( destinationFolder, nameof( destinationFolder ) );
            Arg.NotNullOrEmpty( desiredNewName, nameof( desiredNewName ) );
            var copy = await file.CopyAsync( destinationFolder.AsFolder(), desiredNewName );
            return new StorageFileAdapter( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task MoveAndReplaceAsync( IFile fileToReplace )
        {
            Arg.NotNull( fileToReplace, nameof( fileToReplace ) );
            return file.MoveAndReplaceAsync( fileToReplace.AsFile() ).AsTask();
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task MoveAsync( IFolder destinationFolder, string desiredNewName )
        {
            Arg.NotNull( destinationFolder, nameof( destinationFolder ) );
            Arg.NotNullOrEmpty( desiredNewName, nameof( desiredNewName ) );
            return file.MoveAsync( destinationFolder.AsFolder(), desiredNewName ).AsTask();
        }

        public Task<Stream> OpenReadAsync() => file.OpenStreamForReadAsync();

        public Task<Stream> OpenReadWriteAsync() => file.OpenStreamForWriteAsync();

        public DateTimeOffset DateCreated => file.DateCreated;

        public string Name => file.Name;

        public string Path => file.Path;

        public Task DeleteAsync() => file.DeleteAsync().AsTask();

        public async Task<IBasicProperties> GetBasicPropertiesAsync() => new BasicPropertiesAdapter( await file.GetBasicPropertiesAsync() );

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );
            return file.RenameAsync( desiredName ).AsTask();
        }

        public async Task<IFolder> GetParentAsync()
        {
            var parent = await file.GetParentAsync();
            return new StorageFolderAdapter( parent );
        }

        public override bool Equals( object obj ) => Equals( obj as IPlatformStorageItem<StorageFile> );

        public bool Equals( IStorageItem other ) => Equals( other as IPlatformStorageItem<StorageFile> );

        bool Equals( IPlatformStorageItem<StorageFile> other ) => other == null ? false : file.Equals( other.NativeStorageItem );

        public override int GetHashCode() => file.GetHashCode();
    }
}