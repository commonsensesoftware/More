namespace More.Globalization
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Represents a fiscal calendar that is based on the Gregorian calendar <seealso cref="GregorianCalendar"/>.
    /// </summary>
    /// <remarks>NOTE: This class does not inherit from the <see cref="FiscalCalendar"/> base class because it is merely a variant
    /// of the standard <see cref="GregorianCalendar"/> class.</remarks>
    public partial class GregorianFiscalCalendar : GregorianCalendar, ICalendarEpoch
    {
        readonly int epochMonth = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="GregorianFiscalCalendar"/> class.
        /// </summary>
        /// <param name="epochMonth">The first month (1-12) in the calendar.</param>
        public GregorianFiscalCalendar( int epochMonth )
        {
            Arg.InRange( epochMonth, 1, 12, nameof( epochMonth ) );
            this.epochMonth = epochMonth;
        }

        /// <summary>
        /// Gets the epoch month for the calendar.
        /// </summary>
        /// <value>The epoch month (1-12) for the calendar.</value>
        public int EpochMonth
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 1 && Contract.Result<int>() <= 12 );
                return epochMonth;
            }
        }

        /// <summary>
        /// Returns the day of the year in the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to get the day of year for.</param>
        /// <returns>The one-based day of the year.</returns>
        public override int GetDayOfYear( DateTime time )
        {
            var year = time.Year - ( time.Month < epochMonth ? 1 : 0 );
            var start = new DateTime( year, epochMonth, 1 );
            return (int) Math.Ceiling( time.Subtract( start ).TotalDays ) + 1;
        }

        /// <summary>
        /// Returns the number of days in the specified month.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="month">An integer from 1 to 12 representing the month.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>The number of days in the month.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="P:MinSupportedDateTime"/><para>- or -</para>
        /// <para><paramref name="year"/> is greater than <see cref="P:MaxSupportedDateTime"/>.</para></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="month"/> is less than 1<para>- or -</para>
        /// <para><paramref name="month"/> is greater than 12.</para></exception>
        [SuppressMessage( "Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "year-1", Justification = "Validated by calendar year boundary" )]
        public override int GetDaysInMonth( int year, int month, int era )
        {
            if ( year < MinSupportedDateTime.Year || year > MaxSupportedDateTime.Year )
            {
                throw new ArgumentOutOfRangeException( nameof( year ) );
            }

            if ( month < 1 || month > 12 )
            {
                throw new ArgumentOutOfRangeException( nameof( month ) );
            }

            month = epochMonth + ( month - 1 );

            if ( month > 12 )
            {
                month -= 12;
            }
            else
            {
                year--;
            }

            return base.GetDaysInMonth( year, month, era );
        }

        /// <summary>
        /// Returns the number of days in the year.
        /// </summary>
        /// <param name="year">An integer representing the year.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>The number of days in the year.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="P:MinSupportedDateTime"/><para>- or -</para>
        /// <para><paramref name="year"/> is greater than <see cref="P:MaxSupportedDateTime"/>.</para></exception>
        [SuppressMessage( "Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "year-1", Justification = "Validated by calendar year boundary" )]
        public override int GetDaysInYear( int year, int era )
        {
            if ( epochMonth == 1 )
            {
                return base.GetDaysInYear( year, era );
            }

            if ( year < MinSupportedDateTime.Year || year > MaxSupportedDateTime.Year )
            {
                throw new ArgumentOutOfRangeException( nameof( year ) );
            }

            var days = 0;

            for ( var month = epochMonth; month <= 12; month++ )
            {
                days += base.GetDaysInMonth( year - 1, month, era );
            }

            for ( var month = 1; month < epochMonth; month++ )
            {
                days += base.GetDaysInMonth( year, month, era );
            }

            return days;
        }

        /// <summary>
        /// Returns the month for the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to get the month for.</param>
        /// <returns>An integer from 1 to 12 representing the month.</returns>
        public override int GetMonth( DateTime time )
        {
            if ( time.Month >= epochMonth )
            {
                return ( time.Month - epochMonth ) + 1;
            }

            return ( epochMonth + time.Month ) - 1;
        }

        /// <summary>
        /// Returns the year for the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to get the year for.</param>
        /// <returns>An integer representing the year.</returns>
        public override int GetYear( DateTime time )
        {
            var year = base.GetYear( time );

            if ( epochMonth == 1 || time.Month < epochMonth )
            {
                return year;
            }

            return year + 1;
        }

        /// <summary>
        /// Returns a value indicating whether the specified year is a leap year.
        /// </summary>
        /// <param name="year">An integer representing the year to evaluate.</param>
        /// <param name="era">An integer representing the era.</param>
        /// <returns>True if the specified year is a leap year; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "year-1", Justification = "Validated by calendar year boundary" )]
        public override bool IsLeapYear( int year, int era )
        {
            // the leap is always observed in Februrary on the Gregorian calendar; therefore,
            // if the fiscal calendar spans over two years, then the if the current year starts
            // before March and is a leap year or starts after February and the next year is a
            // leap year, then the fiscal year is a leap year.

            var leapYear = base.IsLeapYear( year, era );

            if ( epochMonth < 3 )
            {
                return leapYear;
            }

            return leapYear || ( epochMonth > 2 && year > MinSupportedDateTime.Year && base.IsLeapYear( year - 1, era ) );
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> that is set to the specified date and time in the specified era.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="month">A positive integer that represents the month.</param>
        /// <param name="day">A positive integer that represents the day.</param>
        /// <param name="hour">An integer from 0 to 23 that represents the hour.</param>
        /// <param name="minute">An integer from 0 to 59 that represents the minute.</param>
        /// <param name="second">An integer from 0 to 59 that represents the second.</param>
        /// <param name="millisecond">An integer from 0 to 999 that represents the millisecond.</param>
        /// <param name="era">An integer that represents the era.  This parameter is not used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="P:MinSupportedDateTime"/><para>- or -</para>
        /// <para><paramref name="year"/> is greater than <see cref="P:MaxSupportedDateTime"/>.</para></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="month"/> is less than 1<para>- or -</para>
        /// <para><paramref name="month"/> is greater than 12.</para></exception>
        [SuppressMessage( "Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "year-1", Justification = "Validated by calendar year boundary" )]
        public override DateTime ToDateTime( int year, int month, int day, int hour, int minute, int second, int millisecond, int era )
        {
            // to convert back to a normal date time, the fiscal year must be requested by the year for the last month
            // ex: Assume FY11 starts July 1, 2010 and ends on June 30, 2010; therefore ToDateTime( 2011, 1, 1 ) = July 1, 2010

            if ( month < 1 || month > 12 )
            {
                throw new ArgumentOutOfRangeException( nameof( month ) );
            }

            month = epochMonth + ( month - 1 );

            if ( month > 12 )
            {
                month -= 12;
            }
            else
            {
                year--;
            }

            if ( year < MinSupportedDateTime.Year || year > MaxSupportedDateTime.Year )
            {
                throw new ArgumentOutOfRangeException( nameof( year ) );
            }

            return base.ToDateTime( year, month, day, hour, minute, second, millisecond, era );
        }

        /// <summary>
        /// Gets the epoch month for the calendar.
        /// </summary>
        /// <value>The calendar epoch month.</value>
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is only meant to be used to support string formatting." )]
        int ICalendarEpoch.Month => epochMonth;
    }
}