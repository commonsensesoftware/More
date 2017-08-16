namespace More.Globalization
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( ICalendarEpoch ) )]
    abstract class ICalendarEpochContract : ICalendarEpoch
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