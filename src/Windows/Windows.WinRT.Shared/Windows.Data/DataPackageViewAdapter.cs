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
            adapted = dataPackageView;
            properties = new Lazy<IDataPackagePropertySetView>( () => new DataPackagePropertySetViewAdapter( adapted.Properties ) );
        }


        public IReadOnlyList<string> AvailableFormats
        {
            get
            {
                return adapted.AvailableFormats;
            }
        }

        public IDataPackagePropertySetView Properties
        {
            get
            {
                return properties.Value;
            }
        }

        public DataPackageOperation RequestedOperation
        {
            get
            {
                return adapted.RequestedOperation;
            }
        }

        public bool Contains( string formatId )
        {
            return adapted.Contains( formatId );
        }

        public IAsyncOperation<Uri> GetApplicationLinkAsync()
        {
            return adapted.GetApplicationLinkAsync();
        }

        public IAsyncOperation<IRandomAccessStreamReference> GetBitmapAsync()
        {
            var options = TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion;
            return adapted.GetBitmapAsync().AsTask().ContinueWith( t => (IRandomAccessStreamReference) t.Result, options ).AsAsyncOperation();
        }

        public IAsyncOperation<object> GetDataAsync( string formatId )
        {
            return adapted.GetDataAsync( formatId );
        }

        public IAsyncOperation<string> GetHtmlFormatAsync()
        {
            return adapted.GetHtmlFormatAsync();
        }

        public IAsyncOperation<IReadOnlyDictionary<string, IRandomAccessStreamReference>> GetResourceMapAsync()
        {
            var options = TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion;
            var task = adapted.GetResourceMapAsync().AsTask();
            var continuation = task.ContinueWith<IReadOnlyDictionary<string, IRandomAccessStreamReference>>( t => t.Result.ToDictionary( p => p.Key, p => (IRandomAccessStreamReference) p.Value ), options );
            return continuation.AsAsyncOperation();
        }

        public IAsyncOperation<string> GetRtfAsync()
        {
            return adapted.GetRtfAsync();
        }

        public IAsyncOperation<IReadOnlyList<IStorageItem>> GetStorageItemsAsync()
        {
            return adapted.GetStorageItemsAsync();
        }

        public IAsyncOperation<string> GetTextAsync()
        {
            return adapted.GetTextAsync();
        }

        public IAsyncOperation<string> GetTextAsync( string formatId )
        {
            return adapted.GetTextAsync( formatId );
        }

        public IAsyncOperation<Uri> GetWebLinkAsync()
        {
            return adapted.GetWebLinkAsync();
        }

        public void ReportOperationCompleted( DataPackageOperation value )
        {
            adapted.ReportOperationCompleted( value );
        }
    }
}
