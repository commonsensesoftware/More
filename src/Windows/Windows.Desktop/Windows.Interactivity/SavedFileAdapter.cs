namespace More.Windows.Interactivity
{
    using More.IO;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    internal sealed class SavedFileAdapter : IFile, IPlatformStorageItem<FileInfo>
    {
        private readonly Lazy<FileInfo> fileInfo;
        private readonly string path;
        private Func<Stream> open;

        internal SavedFileAdapter( string path, Func<Stream> open )
        {
            Contract.Requires( !string.IsNullOrEmpty( path ) );
            Contract.Requires( open != null );

            this.path = path;
            this.open = open;
            this.fileInfo = new Lazy<FileInfo>( () => new FileInfo( path ) );
        }

        public FileInfo NativeStorageItem
        {
            get
            {
                return this.fileInfo.Value;
            }
        }

        public string ContentType
        {
            get
            {
                // not typically used by the desktop implementation
                return System.Net.Mime.MediaTypeNames.Application.Octet;
            }
        }

        public string FileType
        {
            get
            {
                return this.NativeStorageItem.Extension;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract.")]
        public Task CopyAndReplaceAsync( IFile fileToReplace )
        {
            var destinationFileName = fileToReplace.Path;
            var copy = this.NativeStorageItem.CopyTo( destinationFileName, true ).AsFile();
            return Task.FromResult( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task<IFile> CopyAsync( IFolder destinationFolder, string desiredNewName )
        {
            var destinationFileName = System.IO.Path.Combine( destinationFolder.Path, desiredNewName );
            var copy = this.NativeStorageItem.CopyTo( destinationFileName ).AsFile();
            return Task.FromResult( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public async Task MoveAndReplaceAsync( IFile fileToReplace )
        {
            var destinationFileName = fileToReplace.Path;
            await fileToReplace.DeleteAsync();
            this.NativeStorageItem.MoveTo( destinationFileName );
            this.open = () => this.NativeStorageItem.Open( FileMode.Open, FileAccess.ReadWrite );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task MoveAsync( IFolder destinationFolder, string desiredNewName )
        {
            var destinationFileName = System.IO.Path.Combine( destinationFolder.Path, desiredNewName );
            this.NativeStorageItem.MoveTo( destinationFileName );
            this.open = () => this.NativeStorageItem.Open( FileMode.Open, FileAccess.ReadWrite );
            return Task.FromResult( 0 );
        }

        public Task<Stream> OpenReadAsync()
        {
            return Task.FromResult( this.open() );
        }

        public Task<Stream> OpenReadWriteAsync()
        {
            return Task.FromResult( this.open() );
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return this.NativeStorageItem.CreationTime;
            }
        }

        public string Name
        {
            get
            {
                return this.NativeStorageItem.Name;
            }
        }

        public string Path
        {
            get
            {
                return this.path;
            }
        }

        public Task DeleteAsync()
        {
            this.NativeStorageItem.Delete();
            return Task.FromResult( 0 );
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            return this.NativeStorageItem.AsFile().GetBasicPropertiesAsync();
        }

        public Task RenameAsync( string desiredName )
        {
            this.NativeStorageItem.MoveTo( desiredName );
            this.open = () => this.NativeStorageItem.Open( FileMode.Open, FileAccess.ReadWrite );
            return Task.FromResult( 0 );
        }

        public Task<IFolder> GetParentAsync()
        {
            return Task.FromResult( this.NativeStorageItem.Directory.AsFolder() );
        }

        public override bool Equals( object obj )
        {
            return this.Equals( obj as IStorageItem );
        }

        public bool Equals( IStorageItem other )
        {
            if ( other is IFile )
                return this.path.Equals( other.Path, StringComparison.OrdinalIgnoreCase );

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode( this.path );
        }
    }
}
