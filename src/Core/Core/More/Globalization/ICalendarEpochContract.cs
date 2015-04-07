namespace More.Globalization
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( ICalendarEpoch ) )]
    internal abstract class ICalendarEpochContract : ICalendarEpoch
    {
        int ICalendarEpoch.Month
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() > 0 );
                return default( int );
            }
        }
    }
}
