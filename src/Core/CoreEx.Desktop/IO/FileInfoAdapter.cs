namespace More.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using static System.IO.Path;

    internal sealed class FileInfoAdapter : IFile, IPlatformStorageItem<FileInfo>
    {
        private static readonly Task CompletedTask = Task.FromResult( false );
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

            var destinationFileName = Combine( destinationFolder.Path, desiredNewName );
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

            var destinationFileName = Combine( destinationFolder.Path, desiredNewName );
            file.MoveTo( destinationFileName );
            return CompletedTask;
        }

        public Task<Stream> OpenReadAsync() => Task.FromResult<Stream>( file.OpenRead() );

        public Task<Stream> OpenReadWriteAsync() => Task.FromResult<Stream>( file.Open( FileMode.Open, FileAccess.ReadWrite ) );

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
            return CompletedTask;
        }

        public Task<IBasicProperties> GetBasicPropertiesAsync() => Task.FromResult<IBasicProperties>( new FilePropertiesAdapter( file ) );

        public Task RenameAsync( string desiredName )
        {
            Arg.NotNullOrEmpty( desiredName, nameof( desiredName ) );

            file.MoveTo( desiredName );
            return CompletedTask;
        }

        public Task<IFolder> GetParentAsync() => Task.FromResult<IFolder>( new DirectoryInfoAdapter( file.Directory ) );

        public override bool Equals( object obj ) => Equals( obj as IStorageItem );

        public bool Equals( IStorageItem other ) => ( other is IFile ) && file.FullName.Equals( other.Path, StringComparison.OrdinalIgnoreCase );

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode( file.FullName );
    }
}
