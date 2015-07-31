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
            fileInfo = new Lazy<FileInfo>( () => new FileInfo( path ) );
        }

        public FileInfo NativeStorageItem
        {
            get
            {
                return fileInfo.Value;
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
                return NativeStorageItem.Extension;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract.")]
        public Task CopyAndReplaceAsync( IFile fileToReplace )
        {
            Arg.NotNull( fileToReplace, nameof( fileToReplace ) );

            var destinationFileName = fileToReplace.Path;
            var copy = NativeStorageItem.CopyTo( destinationFileName, true ).AsFile();
            return Task.FromResult( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task<IFile> CopyAsync( IFolder destinationFolder, string desiredNewName )
        {
            Arg.NotNull( destinationFolder, nameof( destinationFolder ) );
            Arg.NotNullOrEmpty( desiredNewName, nameof( desiredNewName ) );

            var destinationFileName = System.IO.Path.Combine( destinationFolder.Path, desiredNewName );
            var copy = NativeStorageItem.CopyTo( destinationFileName ).AsFile();
            return Task.FromResult( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public async Task MoveAndReplaceAsync( IFile fileToReplace )
        {
            Arg.NotNull( fileToReplace, nameof( fileToReplace ) );

            var destinationFileName = fileToReplace.Path;
            await fileToReplace.DeleteAsync();
            NativeStorageItem.MoveTo( destinationFileName );
            open = () => NativeStorageItem.Open( FileMode.Open, FileAccess.ReadWrite );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task MoveAsync( IFolder destinationFolder, string desiredNewName )
        {
            Arg.NotNull( destinationFolder, nameof( destinationFolder ) );
            Arg.NotNullOrEmpty( desiredNewName, nameof( desiredNewName ) );

            var destinationFileName = System.IO.Path.Combine( destinationFolder.Path, desiredNewName );
            NativeStorageItem.MoveTo( destinationFileName );
            open = () => NativeStorageItem.Open( FileMode.Open, FileAccess.ReadWrite );
            return Task.FromResult( 0 );
        }

        public Task<Stream> OpenReadAsync()
        {
            return Task.FromResult( open() );
        }

        public Task<Stream> OpenReadWriteAsync()
        {
            return Task.FromResult( open() );
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return NativeStorageItem.CreationTime;
            }
        }

        public string Name
        {
            get
            {
                return NativeStorageItem.Name;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
        }

        public Task DeleteAsync()
        {
            NativeStorageItem.Delete();
            return Task.FromResult( 0 );
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            return NativeStorageItem.AsFile().GetBasicPropertiesAsync();
        }

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );
            NativeStorageItem.MoveTo( desiredName );
            open = () => NativeStorageItem.Open( FileMode.Open, FileAccess.ReadWrite );
            return Task.FromResult( 0 );
        }

        public Task<IFolder> GetParentAsync()
        {
            return Task.FromResult( NativeStorageItem.Directory.AsFolder() );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as IStorageItem );
        }

        public bool Equals( IStorageItem other )
        {
            if ( other is IFile )
                return path.Equals( other.Path, StringComparison.OrdinalIgnoreCase );

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode( path );
        }
    }
}
