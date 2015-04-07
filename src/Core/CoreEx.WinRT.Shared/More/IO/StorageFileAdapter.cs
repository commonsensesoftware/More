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
                return this.file;
            }
        }

        public string ContentType
        {
            get
            {
                return this.file.ContentType;
            }
        }

        public string FileType
        {
            get
            {
                return this.file.FileType;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task CopyAndReplaceAsync( IFile fileToReplace )
        {
            return this.file.CopyAndReplaceAsync( fileToReplace.AsFile() ).AsTask();
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public async Task<IFile> CopyAsync( IFolder destinationFolder, string desiredNewName )
        {
            var copy = await this.file.CopyAsync( destinationFolder.AsFolder(), desiredNewName );
            return new StorageFileAdapter( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task MoveAndReplaceAsync( IFile fileToReplace )
        {
            return this.file.MoveAndReplaceAsync( fileToReplace.AsFile() ).AsTask();
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task MoveAsync( IFolder destinationFolder, string desiredNewName )
        {
            return this.file.MoveAsync( destinationFolder.AsFolder(), desiredNewName ).AsTask();
        }

        public Task<Stream> OpenReadAsync()
        {
            return this.file.OpenStreamForReadAsync();
        }

        public Task<Stream> OpenReadWriteAsync()
        {
            return this.file.OpenStreamForWriteAsync();
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return this.file.DateCreated;
            }
        }

        public string Name
        {
            get
            {
                return this.file.Name;
            }
        }

        public string Path
        {
            get
            {
                return this.file.Path;
            }
        }

        public Task DeleteAsync()
        {
            return this.file.DeleteAsync().AsTask();
        }

        public async Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            var properties = await this.file.GetBasicPropertiesAsync();
            return new BasicPropertiesAdapter( properties );
        }

        public Task RenameAsync( string desiredName )
        {
            return this.file.RenameAsync( desiredName ).AsTask();
        }

        public override bool Equals( object obj )
        {
            return this.Equals( obj as IPlatformStorageItem<StorageFile> );
        }

        public bool Equals( IStorageItem other )
        {
            return this.Equals( other as IPlatformStorageItem<StorageFile> );
        }

        private bool Equals( IPlatformStorageItem<StorageFile> other )
        {
            return other == null ? false : this.file.Equals( other.NativeStorageItem );
        }

        public override int GetHashCode()
        {
            return this.file.GetHashCode();
        }
    }
}
