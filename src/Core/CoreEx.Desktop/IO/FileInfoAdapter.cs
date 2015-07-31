namespace More.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    internal sealed class FileInfoAdapter : IFile, IPlatformStorageItem<FileInfo>
    {
        private readonly FileInfo file;

        internal FileInfoAdapter( FileInfo file )
        {
            Contract.Requires( file != null );
            this.file = file;
        }

        public FileInfo NativeStorageItem
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
                // typically not used on the desktop; just use a binary stream
                return System.Net.Mime.MediaTypeNames.Application.Octet;
            }
        }

        public string FileType
        {
            get
            {
                return file.Extension;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task CopyAndReplaceAsync( IFile fileToReplace )
        {
            Arg.NotNull( fileToReplace, nameof( fileToReplace ) );

            var destinationFileName = fileToReplace.Path;
            IFile copy = new FileInfoAdapter( file.CopyTo( destinationFileName, true ) );
            return Task.FromResult( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task<IFile> CopyAsync( IFolder destinationFolder, string desiredNewName )
        {
            Arg.NotNull( destinationFolder, nameof( destinationFolder ) );
            Arg.NotNullOrEmpty( desiredNewName, nameof( desiredNewName ) );

            var destinationFileName = System.IO.Path.Combine( destinationFolder.Path, desiredNewName );
            IFile copy = new FileInfoAdapter( file.CopyTo( destinationFileName ) );
            return Task.FromResult( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public async Task MoveAndReplaceAsync( IFile fileToReplace )
        {
            Arg.NotNull( fileToReplace, nameof( fileToReplace ) );

            var destinationFileName = fileToReplace.Path;
            await fileToReplace.DeleteAsync();
            file.MoveTo( destinationFileName );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task MoveAsync( IFolder destinationFolder, string desiredNewName )
        {
            Arg.NotNull( destinationFolder, nameof( destinationFolder ) );
            Arg.NotNullOrEmpty( desiredNewName, nameof( desiredNewName ) );

            var destinationFileName = System.IO.Path.Combine( destinationFolder.Path, desiredNewName );
            file.MoveTo( destinationFileName );
            return Task.FromResult( 0 );
        }

        public Task<Stream> OpenReadAsync()
        {
            Stream stream = file.OpenRead();
            return Task.FromResult( stream );
        }

        public Task<Stream> OpenReadWriteAsync()
        {
            Stream stream = file.Open( FileMode.Open, FileAccess.ReadWrite );
            return Task.FromResult( stream );
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return file.CreationTime;
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
                return file.FullName;
            }
        }

        public Task DeleteAsync()
        {
            file.Delete();
            return Task.FromResult( 0 );
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            IBasicProperties properties = new FilePropertiesAdapter( file );
            return Task.FromResult( properties );
        }

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );

            file.MoveTo( desiredName );
            return Task.FromResult( 0 );
        }

        public Task<IFolder> GetParentAsync()
        {
            IFolder parent = new DirectoryInfoAdapter( file.Directory );
            return Task.FromResult( parent );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as IStorageItem );
        }

        public bool Equals( IStorageItem other )
        {
            if ( other is IFile )
                return file.FullName.Equals( other.Path, StringComparison.OrdinalIgnoreCase );

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode( file.FullName );
        }
    }
}
