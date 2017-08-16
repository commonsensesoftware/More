namespace More.Globalization
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior to describe the epoch of a calendar.
    /// </summary>
    [ContractClass( typeof( ICalendarEpochContract ) )]
    public interface ICalendarEpoch
    {
        /// <summary>
        /// Gets the epoch month for the calendar.
        /// </summary>
        /// <value>The calendar epoch month.</value>
        int Month { get; }
    }
}