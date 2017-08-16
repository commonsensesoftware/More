namespace More.Windows.Interactivity
{
    using More.IO;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    sealed class SavedFileAdapter : IFile, IPlatformStorageItem<FileInfo>
    {
        readonly Lazy<FileInfo> fileInfo;
        Func<Stream> open;

        internal SavedFileAdapter( string path, Func<Stream> open )
        {
            Contract.Requires( !string.IsNullOrEmpty( path ) );
            Contract.Requires( open != null );

            Path = path;
            this.open = open;
            fileInfo = new Lazy<FileInfo>( () => new FileInfo( path ) );
        }

        public FileInfo NativeStorageItem => fileInfo.Value;

        public string ContentType => System.Net.Mime.MediaTypeNames.Application.Octet;

        public string FileType => NativeStorageItem.Extension;

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

        public Task<Stream> OpenReadAsync() => Task.FromResult( open() );

        public Task<Stream> OpenReadWriteAsync() => Task.FromResult( open() );

        public DateTimeOffset DateCreated => NativeStorageItem.CreationTime;

        public string Name => NativeStorageItem.Name;

        public string Path { get; }

        public Task DeleteAsync()
        {
            NativeStorageItem.Delete();
            return CompletedTask.Value;
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync() => NativeStorageItem.AsFile().GetBasicPropertiesAsync();

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );
            NativeStorageItem.MoveTo( desiredName );
            open = () => NativeStorageItem.Open( FileMode.Open, FileAccess.ReadWrite );
            return CompletedTask.Value;
        }

        public Task<IFolder> GetParentAsync() => Task.FromResult( NativeStorageItem.Directory.AsFolder() );

        public override bool Equals( object obj ) => Equals( obj as IStorageItem );

        public bool Equals( IStorageItem other )
        {
            if ( other is IFile )
            {
                return Path.Equals( other.Path, StringComparison.OrdinalIgnoreCase );
            }

            return false;
        }

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode( Path );
    }
}