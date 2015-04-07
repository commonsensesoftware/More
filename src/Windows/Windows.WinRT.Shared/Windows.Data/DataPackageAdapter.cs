namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel.DataTransfer;
    using global::Windows.Foundation;
    using global::Windows.Foundation.Metadata;
    using global::Windows.Storage;
    using global::Windows.Storage.Streams;
    using global::Windows.UI;

    internal sealed class DataPackageAdapter : IDataPackage
    {
        private readonly DataPackage adapted;
        private readonly Lazy<IDataPackagePropertySet> properties;
        private readonly Lazy<IDictionary<string, IRandomAccessStreamReference>> resourceMap;

        internal DataPackageAdapter( DataPackage dataPackage )
        {
            Contract.Requires( dataPackage != null );

            this.adapted = dataPackage;
            this.properties = new Lazy<IDataPackagePropertySet>( () => new DataPackagePropertySetAdapter( this.adapted.Properties ) );
            this.resourceMap = new Lazy<IDictionary<string, IRandomAccessStreamReference>>( () => new VariantDictionaryAdapter<string, RandomAccessStreamReference, IRandomAccessStreamReference>( this.adapted.ResourceMap ) );
            this.adapted.Destroyed += this.OnDestoryed;
            this.adapted.OperationCompleted += this.OnOperationCompleted;
        }

        public IDataPackagePropertySet Properties
        {
            get
            {
                return this.properties.Value;
            }
        }

        public DataPackageOperation RequestedOperation
        {
            get
            {
                return this.adapted.RequestedOperation;
            }
            set
            {
                this.adapted.RequestedOperation = value;
            }
        }

        public IDictionary<string, IRandomAccessStreamReference> ResourceMap
        {
            get
            {
                return this.resourceMap.Value;
            }
        }

        private void OnDestoryed( object sender, object e )
        {
            var handler = this.Destroyed;

            if ( handler != null )
                handler( this, e );
        }

        private void OnOperationCompleted( object sender, OperationCompletedEventArgs e )
        {
            var handler = this.OperationCompleted;

            if ( handler != null )
                handler( this, e );
        }

        public event TypedEventHandler<IDataPackage, object> Destroyed;

        public event TypedEventHandler<IDataPackage, OperationCompletedEventArgs> OperationCompleted;

        public IDataPackageView GetView()
        {
            return new DataPackageViewAdapter( this.adapted.GetView() );
        }

        public void SetApplicationLink( Uri value )
        {
            this.adapted.SetApplicationLink( value );
        }

        public void SetBitmap( IRandomAccessStreamReference value )
        {
            // note: this would seem to be a flaw in the design. this should have been an interface all along.
            this.adapted.SetBitmap( (RandomAccessStreamReference) value );
        }

        public void SetData( string formatId, object value )
        {
            this.adapted.SetData( formatId, value );
        }

        public void SetDataProvider( string formatId, DataProviderHandler delayRenderer )
        {
            this.adapted.SetDataProvider( formatId, delayRenderer );
        }

        public void SetHtmlFormat( string value )
        {
            this.adapted.SetHtmlFormat( value );
        }

        public void SetRtf( string value )
        {
            this.adapted.SetRtf( value );
        }

        public void SetStorageItems( IEnumerable<IStorageItem> value )
        {
            this.adapted.SetStorageItems( value );
        }

        public void SetStorageItems( IEnumerable<IStorageItem> value, bool readOnly )
        {
            this.adapted.SetStorageItems( value, readOnly );
        }

        public void SetText( string value )
        {
            this.adapted.SetText( value );
        }

        public void SetWebLink( Uri value )
        {
            this.adapted.SetWebLink( value );
        }
    }
}
