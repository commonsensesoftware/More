namespace More
{
    using More.IO;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IResourceLocator ) )]
    abstract class IResourceLocatorContract : IResourceLocator
    {
        string IResourceLocator.GetString( string name )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), nameof( name ) );
            return null;
        }

        Task<IFile> IResourceLocator.GetFileAsync( string name )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), nameof( name ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return null;
        }
    }
}