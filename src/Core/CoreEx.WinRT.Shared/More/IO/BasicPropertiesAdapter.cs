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

        public DateTimeOffset DateModified
        {
            get
            {
                return this.properties.DateModified;
            }
        }

        public DateTimeOffset ItemDate
        {
            get
            {
                return this.properties.ItemDate;
            }
        }

        public long Size
        {
            get
            {
                return Convert.ToInt64( this.properties.Size );
            }
        }

        public Task<IDictionary<string, object>> RetrievePropertiesAsync( IEnumerable<string> propertiesToRetrieve )
        {
            return this.properties.RetrievePropertiesAsync( propertiesToRetrieve ).AsTask();
        }

        public Task SavePropertiesAsync()
        {
            return this.properties.SavePropertiesAsync().AsTask();
        }

        public Task SavePropertiesAsync( IEnumerable<KeyValuePair<string, object>> propertiesToSave )
        {
            return this.properties.SavePropertiesAsync( propertiesToSave ).AsTask();
        }
    }
}
