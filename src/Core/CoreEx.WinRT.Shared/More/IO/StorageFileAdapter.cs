namespace More.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using global::Windows.Storage;

    internal sealed partial class StorageFileAdapter : IFile, IPlatformStorageItem<StorageFile>
    {
        private readonly StorageFile file;

        internal StorageFileAdapter( StorageFile file )
        {
            Contract.Requires( file != null );
            this.file = file;
        }

        public StorageFile NativeStorageItem
        {
            get
            {
                return file;
            }
        }

        public string ContentType
        {
            get
            {
                return file.ContentType;
            }
        }

        public string FileType
        {
            get
            {
                return file.FileType;
            }
        }

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

        public Task<Stream> OpenReadAsync()
        {
            return file.OpenStreamForReadAsync();
        }

        public Task<Stream> OpenReadWriteAsync()
        {
            return file.OpenStreamForWriteAsync();
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return file.DateCreated;
            }
        }

        public string Name
        {
            get
            {
                return file.Name;
            }
        }

        public string Path
        {
            get
            {
                return file.Path;
            }
        }

        public Task DeleteAsync()
        {
            return file.DeleteAsync().AsTask();
        }

        public async Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            var properties = await file.GetBasicPropertiesAsync();
            return new BasicPropertiesAdapter( properties );
        }

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );
            return file.RenameAsync( desiredName ).AsTask();
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as IPlatformStorageItem<StorageFile> );
        }

        public bool Equals( IStorageItem other )
        {
            return Equals( other as IPlatformStorageItem<StorageFile> );
        }

        private bool Equals( IPlatformStorageItem<StorageFile> other )
        {
            return other == null ? false : file.Equals( other.NativeStorageItem );
        }

        public override int GetHashCode()
        {
            return file.GetHashCode();
        }
    }
}
