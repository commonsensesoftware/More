namespace More.Windows.Data
{
    using global::Windows.ApplicationModel.DataTransfer;
    using global::Windows.Foundation;
    using global::Windows.Storage;
    using global::Windows.Storage.Streams;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    sealed class DataPackageViewAdapter : IDataPackageView
    {
        readonly DataPackageView adapted;
        readonly Lazy<IDataPackagePropertySetView> properties;

        internal DataPackageViewAdapter( DataPackageView dataPackageView )
        {
            Contract.Requires( dataPackageView != null );
            adapted = dataPackageView;
            properties = new Lazy<IDataPackagePropertySetView>( () => new DataPackagePropertySetViewAdapter( adapted.Properties ) );
        }

        public IReadOnlyList<string> AvailableFormats => adapted.AvailableFormats;

        public IDataPackagePropertySetView Properties => properties.Value;

        public DataPackageOperation RequestedOperation => adapted.RequestedOperation;

        public bool Contains( string formatId ) => adapted.Contains( formatId );

        public IAsyncOperation<Uri> GetApplicationLinkAsync() => adapted.GetApplicationLinkAsync();

        public IAsyncOperation<IRandomAccessStreamReference> GetBitmapAsync()
        {
            var cancellationToken = CancellationToken.None;
            var options = TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion;
            var scheduler = TaskScheduler.Current;
            return adapted.GetBitmapAsync().AsTask().ContinueWith( t => (IRandomAccessStreamReference) t.Result, cancellationToken, options, scheduler ).AsAsyncOperation();
        }

        public IAsyncOperation<object> GetDataAsync( string formatId ) => adapted.GetDataAsync( formatId );

        public IAsyncOperation<string> GetHtmlFormatAsync() => adapted.GetHtmlFormatAsync();

        public IAsyncOperation<IReadOnlyDictionary<string, IRandomAccessStreamReference>> GetResourceMapAsync()
        {
            var cancellationToken = CancellationToken.None;
            var options = TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion;
            var task = adapted.GetResourceMapAsync().AsTask();
            var scheduler = TaskScheduler.Current;
            var continuation = task.ContinueWith<IReadOnlyDictionary<string, IRandomAccessStreamReference>>( t => t.Result.ToDictionary( p => p.Key, p => (IRandomAccessStreamReference) p.Value ), cancellationToken, options, scheduler );
            return continuation.AsAsyncOperation();
        }

        public IAsyncOperation<string> GetRtfAsync() => adapted.GetRtfAsync();

        public IAsyncOperation<IReadOnlyList<IStorageItem>> GetStorageItemsAsync() => adapted.GetStorageItemsAsync();

        public IAsyncOperation<string> GetTextAsync() => adapted.GetTextAsync();

        public IAsyncOperation<string> GetTextAsync( string formatId ) => adapted.GetTextAsync( formatId );

        public IAsyncOperation<Uri> GetWebLinkAsync() => adapted.GetWebLinkAsync();

        public void ReportOperationCompleted( DataPackageOperation value ) => adapted.ReportOperationCompleted( value );
    }
}