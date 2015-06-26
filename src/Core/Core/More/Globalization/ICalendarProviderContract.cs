namespace More.Globalization
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( ICalendarProvider ) )]
    internal abstract class ICalendarProviderContract : ICalendarProvider
    {
        Task<Calendar> ICalendarProvider.GetCurrentCalendarAsync()
        {
            Contract.Ensures( Contract.Result<Task<Calendar>>() != null );
            return null;
        }

        Task<Calendar> ICalendarProvider.GetCalendarAsync( int startYear, int endYear )
        {
            Contract.Requires<ArgumentOutOfRangeException>( startYear >= DateTime.MinValue.Year, "startYear" );
            Contract.Requires<ArgumentOutOfRangeException>( startYear <= endYear, "startYear" );
            Contract.Requires<ArgumentOutOfRangeException>( endYear <= DateTime.MaxValue.Year, "endYear" );
            Contract.Ensures( Contract.Result<Task<Calendar>>() != null );
            return null;
        }
    }
}
