namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.Contracts;

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

        DateTimeOffset IDataRequest.Deadline => default( DateTimeOffset );

        void IDataRequest.FailWithDisplayText( string value )
        {
            Contract.Requires<ArgumentNullException>( value != null, nameof( value ) );
        }

        IDisposable IDataRequest.GetDeferral()
        {
            Contract.Ensures( Contract.Result<IDisposable>() != null );
            return null;
        }
    }
}