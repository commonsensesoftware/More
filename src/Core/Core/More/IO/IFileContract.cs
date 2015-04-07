namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IFile ) )]
    internal abstract class IFileContract : IFile
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
            Contract.Requires<ArgumentNullException>( fileToReplace != null, "fileToReplace" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task<IFile> IFile.CopyAsync( IFolder destinationFolder, string desiredNewName )
        {
            Contract.Requires<ArgumentNullException>( destinationFolder != null, "destinationFolder" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredNewName ), "desiredNewName" );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return null;
        }

        Task IFile.MoveAndReplaceAsync( IFile fileToReplace )
        {
            Contract.Requires<ArgumentNullException>( fileToReplace != null, "fileToReplace" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task IFile.MoveAsync( IFolder destinationFolder, string desiredNewName )
        {
            Contract.Requires<ArgumentNullException>( destinationFolder != null, "destinationFolder" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredNewName ), "desiredNewName" );
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

        DateTimeOffset IStorageItem.DateCreated
        {
            get
            {
                return default( DateTimeOffset );
            }
        }

        string IStorageItem.Name
        {
            get
            {
                return null;
            }
        }

        string IStorageItem.Path
        {
            get
            {
                return null;
            }
        }

        Task IStorageItem.DeleteAsync()
        {
            return null;
        }

        Task<IBasicProperties> IStorageItem.GetBasicPropertiesAsync()
        {
            return null;
        }

        Task IStorageItem.RenameAsync( string desiredName )
        {
            return null;
        }

        Task<IFolder> IStorageItem.GetParentAsync()
        {
            return null;
        }

        bool IEquatable<IStorageItem>.Equals( IStorageItem other )
        {
            return default( bool );
        }
    }
}
