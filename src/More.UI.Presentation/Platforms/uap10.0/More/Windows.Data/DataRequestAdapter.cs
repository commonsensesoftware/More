namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel.DataTransfer;

    sealed class DataRequestAdapter : IDataRequest
    {
        readonly DataRequest request;
        readonly Lazy<IDataPackage> data;

        internal DataRequestAdapter( DataRequest request )
        {
            Contract.Requires( request != null );
            this.request = request;
            data = new Lazy<IDataPackage>( CreateDataPackage );
        }

        IDataPackage CreateDataPackage() => new DataPackageAdapter( request.Data ?? ( request.Data = new DataPackage() ) );

        public IDataPackage Data => data.Value;

        public DateTimeOffset Deadline => request.Deadline;

        public void FailWithDisplayText( string value )
        {
            Arg.NotNull( value, nameof( value ) );
            request.FailWithDisplayText( value );
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Must be disposed by the caller." )]
        public IDisposable GetDeferral() => new DeferralAdapter( request.GetDeferral().Complete );
    }
}