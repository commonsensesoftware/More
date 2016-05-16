namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using global::Windows.Storage.FileProperties;

    internal sealed class BasicPropertiesAdapter : IBasicProperties
    {
        private readonly BasicProperties properties;

        internal BasicPropertiesAdapter( BasicProperties properties )
        {
            Contract.Requires( properties != null );
            this.properties = properties;
        }

        public DateTimeOffset DateModified => properties.DateModified;

        public DateTimeOffset ItemDate => properties.ItemDate;

        public long Size => Convert.ToInt64( properties.Size );

        public Task<IDictionary<string, object>> RetrievePropertiesAsync( IEnumerable<string> propertiesToRetrieve )
        {
            Arg.NotNull( propertiesToRetrieve, nameof( propertiesToRetrieve ) );
            return properties.RetrievePropertiesAsync( propertiesToRetrieve ).AsTask();
        }

        public Task SavePropertiesAsync() => properties.SavePropertiesAsync().AsTask();

        public Task SavePropertiesAsync( IEnumerable<KeyValuePair<string, object>> propertiesToSave )
        {
            Arg.NotNull( propertiesToSave, nameof( propertiesToSave ) );
            return properties.SavePropertiesAsync( propertiesToSave ).AsTask();
        }
    }
}
