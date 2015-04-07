namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IStorageItemProperties ) )]
    internal abstract class IStorageItemPropertiesContract : IStorageItemProperties
    {
        Task<IDictionary<string, object>> IStorageItemProperties.RetrievePropertiesAsync( IEnumerable<string> propertiesToRetrieve )
        {
            Contract.Requires<ArgumentNullException>( propertiesToRetrieve != null, "propertiesToRetrieve" );
            Contract.Ensures( Contract.Result<Task<IDictionary<string, object>>>() != null );
            return null;
        }

        Task IStorageItemProperties.SavePropertiesAsync()
        {
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task IStorageItemProperties.SavePropertiesAsync( IEnumerable<KeyValuePair<string, object>> propertiesToSave )
        {
            Contract.Requires<ArgumentNullException>( propertiesToSave != null, "propertiesToSave" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }
    }
}
