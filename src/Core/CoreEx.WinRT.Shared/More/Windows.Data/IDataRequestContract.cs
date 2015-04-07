namespace More.Windows.Data
{
    using global::System;
    using global::System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel.DataTransfer;

    [ContractClassFor( typeof( IDataRequest ) )]
    internal abstract class IDataRequestContract : IDataRequest
    {
        IDataPackage IDataRequest.Data
        {
            get
            {
                Contract.Ensures( Contract.Result<IDataPackage>() != null );
                return null;
            }
        }

        DateTimeOffset IDataRequest.Deadline
        {
            get
            {
                return default( DateTimeOffset );
            }
        }

        void IDataRequest.FailWithDisplayText( string value )
        {
            Contract.Requires<ArgumentNullException>( value != null, "value" );
        }

        IDisposable IDataRequest.GetDeferral()
        {
            Contract.Ensures( Contract.Result<IDisposable>() != null );
            return null;
        }
    }
}
