namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
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
            this.data = new Lazy<IDataPackage>( this.CreateDataPackage );
        }

        private IDataPackage CreateDataPackage()
        {
            if ( this.request.Data == null )
                this.request.Data = new DataPackage();

            return new DataPackageAdapter( this.request.Data );
        }

        public IDataPackage Data
        {
            get
            {
                return this.data.Value;
            }
        }

        public DateTimeOffset Deadline
        {
            get
            {
                return this.request.Deadline;
            }
        }

        public void FailWithDisplayText( string value )
        {
            this.request.FailWithDisplayText( value );
        }

        public IDisposable GetDeferral()
        {
            return new DeferralAdapter( this.request.GetDeferral().Complete );
        }
    }
}
