namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IFile ) )]
    abstract class IFileContract : IFile
    {
        string IFile.ContentType
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return null;
            }
        }

        string IFile.FileType
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return null;
            }
        }

        Task IFile.CopyAndReplaceAsync( IFile fileToReplace )
        {
            Contract.Requires<ArgumentNullException>( fileToReplace != null, nameof( fileToReplace ) );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task<IFile> IFile.CopyAsync( IFolder destinationFolder, string desiredNewName )
        {
            Contract.Requires<ArgumentNullException>( destinationFolder != null, nameof( destinationFolder ) );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredNewName ), nameof( desiredNewName ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return null;
        }

        Task IFile.MoveAndReplaceAsync( IFile fileToReplace )
        {
            Contract.Requires<ArgumentNullException>( fileToReplace != null, nameof( fileToReplace ) );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task IFile.MoveAsync( IFolder destinationFolder, string desiredNewName )
        {
            Contract.Requires<ArgumentNullException>( destinationFolder != null, nameof( destinationFolder ) );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredNewName ), nameof( desiredNewName ) );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task<Stream> IFile.OpenReadAsync()
        {
            Contract.Ensures( Contract.Result<Task<Stream>>() != null );
            return null;
        }

        Task<Stream> IFile.OpenReadWriteAsync()
        {
            Contract.Ensures( Contract.Result<Task<Stream>>() != null );
            return null;
        }

        DateTimeOffset IStorageItem.DateCreated => default( DateTimeOffset );

        string IStorageItem.Name => null;

        string IStorageItem.Path => null;

        Task IStorageItem.DeleteAsync() => null;

        Task<IBasicProperties> IStorageItem.GetBasicPropertiesAsync() => null;

        Task IStorageItem.RenameAsync( string desiredName ) => null;

        Task<IFolder> IStorageItem.GetParentAsync() => null;

        bool IEquatable<IStorageItem>.Equals( IStorageItem other ) => default( bool );
    }
}