namespace More.Globalization
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the behavior of a calendar provider.
    /// </summary>
    [ContractClass( typeof( ICalendarProviderContract ) )]
    public interface ICalendarProvider
    {
        /// <summary>
        /// Gets the current calendar asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task">task</see> that contains the <see cref="Calendar">calendar</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an asynchronous method and not suitable to be a property." )]
        Task<Calendar> GetCurrentCalendarAsync();

        /// <summary>
        /// Returns a calendar between the specified years asynchronously.
        /// </summary>
        /// <param name="startYear">The first year of the calendar.</param>
        /// <param name="endYear">The last year of the calendar.</param>
        /// <returns>A <see cref="Task">task</see> that contains the <see cref="Calendar">calendar</see>.</returns>
        Task<Calendar> GetCalendarAsync( int startYear, int endYear );
    }
}