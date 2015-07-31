namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel.DataTransfer;

    internal sealed class DataRequestAdapter : IDataRequest
    {
        private readonly DataRequest request;
        private readonly Lazy<IDataPackage> data;

        internal DataRequestAdapter( DataRequest request )
        {
            Contract.Requires( request != null );
            this.request = request;
            data = new Lazy<IDataPackage>( CreateDataPackage );
        }

        private IDataPackage CreateDataPackage() => new DataPackageAdapter( request.Data ?? ( request.Data = new DataPackage() ) );

        public IDataPackage Data
        {
            get
            {
                return data.Value;
            }
        }

        public DateTimeOffset Deadline
        {
            get
            {
                return request.Deadline;
            }
        }

        public void FailWithDisplayText( string value )
        {
            Arg.NotNull( value, nameof( value ) );
            request.FailWithDisplayText( value );
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Must be disposed by the caller." )]
        public IDisposable GetDeferral() => new DeferralAdapter( request.GetDeferral().Complete );
    }
}
