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
                return this.file;
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
                return this.file.Extension;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task CopyAndReplaceAsync( IFile fileToReplace )
        {
            var destinationFileName = fileToReplace.Path;
            IFile copy = new FileInfoAdapter( this.file.CopyTo( destinationFileName, true ) );
            return Task.FromResult( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task<IFile> CopyAsync( IFolder destinationFolder, string desiredNewName )
        {
            var destinationFileName = System.IO.Path.Combine( destinationFolder.Path, desiredNewName );
            IFile copy = new FileInfoAdapter( this.file.CopyTo( destinationFileName ) );
            return Task.FromResult( copy );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public async Task MoveAndReplaceAsync( IFile fileToReplace )
        {
            var destinationFileName = fileToReplace.Path;
            await fileToReplace.DeleteAsync();
            this.file.MoveTo( destinationFileName );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public async Task MoveAsync( IFolder destinationFolder, string desiredNewName )
        {
            var destinationFileName = System.IO.Path.Combine( destinationFolder.Path, desiredNewName );
            this.file.MoveTo( destinationFileName );
            await Task.Yield();
        }

        public Task<Stream> OpenReadAsync()
        {
            Stream stream = this.file.OpenRead();
            return Task.FromResult( stream );
        }

        public Task<Stream> OpenReadWriteAsync()
        {
            Stream stream = this.file.Open( FileMode.Open, FileAccess.ReadWrite );
            return Task.FromResult( stream );
        }

        public DateTimeOffset DateCreated
        {
            get
            {
                return this.file.CreationTime;
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
                return this.file.FullName;
            }
        }

        public async Task DeleteAsync()
        {
            this.file.Delete();
            await Task.Yield();
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync()
        {
            IBasicProperties properties = new FilePropertiesAdapter( this.file );
            return Task.FromResult( properties );
        }

        public async Task RenameAsync( string desiredName )
        {
            this.file.MoveTo( desiredName );
            await Task.Yield();
        }

        public Task<IFolder> GetParentAsync()
        {
            IFolder parent = new DirectoryInfoAdapter( this.file.Directory );
            return Task.FromResult( parent );
        }

        public override bool Equals( object obj )
        {
            return this.Equals( obj as IStorageItem );
        }

        public bool Equals( IStorageItem other )
        {
            if ( other is IFile )
                return this.file.FullName.Equals( other.Path, StringComparison.OrdinalIgnoreCase );

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode( this.file.FullName );
        }
    }
}
