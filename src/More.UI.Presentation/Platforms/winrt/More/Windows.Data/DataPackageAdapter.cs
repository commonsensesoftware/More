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

    sealed class DataPackageAdapter : IDataPackage
    {
        readonly DataPackage adapted;
        readonly Lazy<IDataPackagePropertySet> properties;
        readonly Lazy<IDictionary<string, IRandomAccessStreamReference>> resourceMap;

        internal DataPackageAdapter( DataPackage dataPackage )
        {
            Contract.Requires( dataPackage != null );

            adapted = dataPackage;
            properties = new Lazy<IDataPackagePropertySet>( () => new DataPackagePropertySetAdapter( adapted.Properties ) );
            resourceMap = new Lazy<IDictionary<string, IRandomAccessStreamReference>>( () => new VariantDictionaryAdapter<string, RandomAccessStreamReference, IRandomAccessStreamReference>( adapted.ResourceMap ) );
            adapted.Destroyed += OnDestoryed;
            adapted.OperationCompleted += OnOperationCompleted;
        }

        public IDataPackagePropertySet Properties => properties.Value;

        public DataPackageOperation RequestedOperation
        {
            get => adapted.RequestedOperation;
            set => adapted.RequestedOperation = value;
        }

        public IDictionary<string, IRandomAccessStreamReference> ResourceMap => resourceMap.Value;

        void OnDestoryed( object sender, object e ) => Destroyed?.Invoke( this, e );

        void OnOperationCompleted( object sender, OperationCompletedEventArgs e ) => OperationCompleted?.Invoke( this, e );

        public event TypedEventHandler<IDataPackage, object> Destroyed;

        public event TypedEventHandler<IDataPackage, OperationCompletedEventArgs> OperationCompleted;

        public IDataPackageView GetView() => new DataPackageViewAdapter( adapted.GetView() );

        public void SetApplicationLink( Uri value ) => adapted.SetApplicationLink( value );

        // note: this would seem to be a flaw in the design. this should have been an interface all along.
        public void SetBitmap( IRandomAccessStreamReference value ) => adapted.SetBitmap( (RandomAccessStreamReference) value );

        public void SetData( string formatId, object value ) => adapted.SetData( formatId, value );

        public void SetDataProvider( string formatId, DataProviderHandler delayRenderer ) => adapted.SetDataProvider( formatId, delayRenderer );

        public void SetHtmlFormat( string value ) => adapted.SetHtmlFormat( value );

        public void SetRtf( string value ) => adapted.SetRtf( value );

        public void SetStorageItems( IEnumerable<IStorageItem> value ) => adapted.SetStorageItems( value );

        public void SetStorageItems( IEnumerable<IStorageItem> value, bool readOnly ) => adapted.SetStorageItems( value, readOnly );

        public void SetText( string value ) => adapted.SetText( value );

        public void SetWebLink( Uri value ) => adapted.SetWebLink( value );
    }
}