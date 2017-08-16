namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IStorageItem ) )]
    abstract class IStorageItemContract : IStorageItem
    {
        DateTimeOffset IStorageItem.DateCreated => default( DateTimeOffset );

        string IStorageItem.Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return null;
            }
        }

        string IStorageItem.Path
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return null;
            }
        }

        Task IStorageItem.DeleteAsync()
        {
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task<IBasicProperties> IStorageItem.GetBasicPropertiesAsync()
        {
            Contract.Ensures( Contract.Result<Task<IBasicProperties>>() != null );
            return null;
        }

        Task IStorageItem.RenameAsync( string desiredName )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( desiredName ), nameof( desiredName ) );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task<IFolder> IStorageItem.GetParentAsync()
        {
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );
            return null;
        }

        bool IEquatable<IStorageItem>.Equals( IStorageItem other ) => default( bool );
    }
}