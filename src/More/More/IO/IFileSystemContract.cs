namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IFileSystem ) )]
    abstract class IFileSystemContract : IFileSystem
    {
        Task<IFolder> IFileSystem.GetFolderAsync( string path )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( path ), nameof( path ) );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );
            return null;
        }

        Task<IFile> IFileSystem.GetFileAsync( string path )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( path ), nameof( path ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return null;
        }

        Task<IFile> IFileSystem.GetFileAsync( Uri uri )
        {
            Contract.Requires<ArgumentNullException>( uri != null, nameof( uri ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return null;
        }
    }
}