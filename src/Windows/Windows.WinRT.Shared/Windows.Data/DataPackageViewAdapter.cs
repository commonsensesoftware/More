namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Windows.ApplicationModel.DataTransfer;
    using global::Windows.Foundation;
    using global::Windows.Storage;
    using global::Windows.Storage.Streams;

    internal sealed class DataPackageViewAdapter : IDataPackageView
    {
        private readonly DataPackageView adapted;
        private readonly Lazy<IDataPackagePropertySetView> properties;

        internal DataPackageViewAdapter( DataPackageView dataPackageView )
        {
            Contract.Requires( dataPackageView != null );
            this.adapted = dataPackageView;
            this.properties = new Lazy<IDataPackagePropertySetView>( () => new DataPackagePropertySetViewAdapter( this.adapted.Properties ) );
        }


        public IReadOnlyList<string> AvailableFormats
        {
            get
            {
                return this.adapted.AvailableFormats;
            }
        }

        public IDataPackagePropertySetView Properties
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
        }

        public bool Contains( string formatId )
        {
            return this.adapted.Contains( formatId );
        }

        public IAsyncOperation<Uri> GetApplicationLinkAsync()
        {
            return this.adapted.GetApplicationLinkAsync();
        }

        public IAsyncOperation<IRandomAccessStreamReference> GetBitmapAsync()
        {
            var options = TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion;
            return this.adapted.GetBitmapAsync().AsTask().ContinueWith( t => (IRandomAccessStreamReference) t.Result, options ).AsAsyncOperation();
        }

        public IAsyncOperation<object> GetDataAsync( string formatId )
        {
            return this.adapted.GetDataAsync( formatId );
        }

        public IAsyncOperation<string> GetHtmlFormatAsync()
        {
            return this.adapted.GetHtmlFormatAsync();
        }

        public IAsyncOperation<IReadOnlyDictionary<string, IRandomAccessStreamReference>> GetResourceMapAsync()
        {
            var options = TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion;
            var task = this.adapted.GetResourceMapAsync().AsTask();
            var continuation = task.ContinueWith<IReadOnlyDictionary<string, IRandomAccessStreamReference>>( t => t.Result.ToDictionary( p => p.Key, p => (IRandomAccessStreamReference) p.Value ), options );
            return continuation.AsAsyncOperation();
        }

        public IAsyncOperation<string> GetRtfAsync()
        {
            return this.adapted.GetRtfAsync();
        }

        public IAsyncOperation<IReadOnlyList<IStorageItem>> GetStorageItemsAsync()
        {
            return this.adapted.GetStorageItemsAsync();
        }

        public IAsyncOperation<string> GetTextAsync()
        {
            return this.adapted.GetTextAsync();
        }

        public IAsyncOperation<string> GetTextAsync( string formatId )
        {
            return this.adapted.GetTextAsync( formatId );
        }

        public IAsyncOperation<Uri> GetWebLinkAsync()
        {
            return this.adapted.GetWebLinkAsync();
        }

        public void ReportOperationCompleted( DataPackageOperation value )
        {
            this.adapted.ReportOperationCompleted( value );
        }
    }
}
