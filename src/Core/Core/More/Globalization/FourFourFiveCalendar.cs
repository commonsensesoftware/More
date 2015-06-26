namespace More.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents an accounting calendar using the 4-4-5 method.
    /// </summary>
    /// <remarks>Reference: http://en.wikipedia.org/wiki/4-4-5_Calendar .</remarks>
    [DebuggerDisplay( "MinSupportedDateTime = {MinSupportedDateTime}, MaxSupportedDateTime = {MaxSupportedDateTime}" )]
    public class FourFourFiveCalendar : FiscalCalendar
    {
        /// <summary>
        /// Gets the number of days in a standard fiscal year.
        /// </summary>
        /// <remarks>Calculation: ( ( ( 8 months * 4 weeks ) + ( 4 months * 5 weeks ) ) * 7 days/week ) = 364.</remarks>
        public const int DaysInStandardYear = 364;

        /// <summary>
        /// Gets the number of days in a leap fiscal year.
        /// </summary>
        /// <remarks>Calculation: ( ( ( 7 months * 4 weeks ) + ( 5 months * 5 weeks ) ) * 7 days/week ) = 371.</remarks>
        public const int DaysInLeapYear = 371;

        private readonly Dictionary<int, FiscalYear> calendarYears = new Dictionary<int, FiscalYear>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FourFourFiveCalendar"/> class.
        /// </summary>
        /// <param name="years">A sequence of <see cref="FiscalYear"/> items for the calendar.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public FourFourFiveCalendar( IEnumerable<FiscalYear> years )
            : base( years )
        {
            Arg.NotNull( years, "years" );

            foreach ( var year in years )
                this.calendarYears[year.LastDay.Year] = year;
        }

        private static bool IsValidDate( Calendar calendar, DateTime date )
        {
            Contract.Requires( calendar != null );
            return ( date >= calendar.MinSupportedDateTime ) && ( date <= calendar.MaxSupportedDateTime );
        }

        private static bool IsValidYear( Calendar calendar, int year )
        {
            Contract.Requires( calendar != null );
            return ( year >= calendar.MinSupportedDateTime.Year ) && ( year <= calendar.MaxSupportedDateTime.Year );
        }

        private static bool IsValidMonth( Calendar calendar, int year, int month, int era )
        {
            Contract.Requires( calendar != null );

            if ( month < 1 )
                return false;

            if ( year == calendar.MinSupportedDateTime.Year && month < calendar.MinSupportedDateTime.Month )
                return false;

            return month <= calendar.GetMonthsInYear( year, era );
        }

        private FiscalYear GetFiscalYear( DateTime date )
        {
            var year = this.GetYear( date );
            return this.calendarYears[year];
        }

        /// <summary>
        /// Returns the week of the year of the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to evaluate.</param>
        /// <returns>The fiscal week of the year.</returns>
        public virtual int GetWeekOfYear( DateTime time )
        {
            Contract.Ensures( Contract.Result<int>() >= 1 );
            Arg.InRange( time, this.MinSupportedDateTime, this.MaxSupportedDateTime, "time" );
            return this.GetWeekOfYear( time, CalendarWeekRule.FirstDay, this.MinSupportedDateTime.DayOfWeek );
        }

        /// <summary>
        /// Gets the epoch month for the calendar.
        /// </summary>
        /// <value>The calendar epoch month.</value>
        protected override int EpochMonth
        {
            get
            {
                // derive the epoch month from the first fiscal date under the 4-4-5 calendar, the epoch month might start
                // in a previous month (ex: fiscal July starts could start on 6/28).  a fiscal month would never start in
                // the middle of calendar month, so it's reasonable to add a month if we're over the half-way mark.
                var epoch = this.Years.First().FirstDay;
                var month = epoch.Day >= 15 ? epoch.Month + 1 : epoch.Month;
                return month;
            }
        }

        /// <summary>
        /// Gets the list of eras in the current calendar.
        /// </summary>
        /// <value>An array of era supported by the calendar.</value>
        public override int[] Eras
        {
            get
            {
                return new[] { 1 };
            }
        }

        /// <summary>
        /// Adds months to the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to add years to.</param>
        /// <param name="months">The number of months to add.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="Calendar.MinSupportedDateTime"/><p>- or -</p>
        /// <paramref name="time"/> is greater than <see cref="Calendar.MaxSupportedDateTime"/>.</exception>
        public override DateTime AddMonths( DateTime time, int months )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            if ( months == 0 )
                return time;

            var day = this.GetDayOfMonth( time );
            var fiscalYear = this.GetFiscalYear( time );
            var monthsInYear = this.GetMonthsInYear( fiscalYear.LastDay.Year );
            var years = (int) Math.Floor( ( (double) months ) / ( (double) monthsInYear ) );

            if ( months < 0 )
                years += 1;

            time = this.AddYears( time, years );
            months -= years * monthsInYear;

            if ( months == 0 )
                return time;

            var month = this.GetMonth( time ) + months;

            if ( month > monthsInYear )
            {
                var year = fiscalYear.LastDay.Year + 1;

                if ( !IsValidYear( this, year ) )
                    throw new ArgumentOutOfRangeException( "months" );

                month %= monthsInYear;
                fiscalYear = this.calendarYears[year];
            }
            else if ( month <= 0 )
            {
                var year = fiscalYear.LastDay.Year + (int) Math.Floor( month == 0 ? -1.0 : (double) month / (double) monthsInYear );

                if ( !IsValidYear( this, year ) )
                    throw new ArgumentOutOfRangeException( "months" );

                month = monthsInYear - ( Math.Abs( month ) % monthsInYear );
                fiscalYear = this.calendarYears[year];
            }

            var fiscalMonth = fiscalYear.Months[month];
            time = fiscalMonth.FirstDay.AddDays( Math.Min( day, fiscalMonth.DaysInMonth ) - 1 );

            return time;
        }

        /// <summary>
        /// Adds years to the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to add years to.</param>
        /// <param name="years">The number of years to add.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="Calendar.MinSupportedDateTime"/><p>- or -</p>
        /// <paramref name="time"/> is greater than <see cref="Calendar.MaxSupportedDateTime"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="years"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="years"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/>.</exception>
        public override DateTime AddYears( DateTime time, int years )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            if ( years == 0 )
                return time;

            var year = this.GetYear( time ) + years;

            if ( !IsValidYear( this, year ) )
                throw new ArgumentOutOfRangeException( "years" );

            var day = this.GetDayOfMonth( time );
            var month = this.GetMonth( time );
            var fiscalYear = this.calendarYears[year];
            var fiscalMonth = fiscalYear.Months[month];

            time = fiscalMonth.FirstDay.AddDays( Math.Min( day, fiscalMonth.DaysInMonth ) - 1 );

            return time;
        }

        /// <summary>
        /// Returns the fiscal day of the month for the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to get the fiscal day for.</param>
        /// <returns>The fiscal day of the month.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="Calendar.MinSupportedDateTime"/><p>- or -</p>
        /// <paramref name="time"/> is greater than <see cref="Calendar.MaxSupportedDateTime"/>.</exception>
        public override int GetDayOfMonth( DateTime time )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            var fiscalYear = this.GetFiscalYear( time );
            var month = this.GetMonth( time );
            var startDate = fiscalYear.Months[month].FirstDay;
            return (int) ( Math.Ceiling( time.Date.Subtract( startDate ).TotalDays ) + 1.0 );
        }

        /// <summary>
        /// Returns the day of the week for the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to evaluate.</param>
        /// <returns>One of the <see cref="DayOfWeek"/> values.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="Calendar.MinSupportedDateTime"/><p>- or -</p>
        /// <paramref name="time"/> is greater than <see cref="Calendar.MaxSupportedDateTime"/>.</exception>
        public override DayOfWeek GetDayOfWeek( DateTime time )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            return time.DayOfWeek;
        }

        /// <summary>
        /// Returns the fiscal day of the year for the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to get the fiscal day for.</param>
        /// <returns>The fiscal day of the year.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="Calendar.MinSupportedDateTime"/><p>- or -</p>
        /// <paramref name="time"/> is greater than <see cref="Calendar.MaxSupportedDateTime"/>.</exception>
        public override int GetDayOfYear( DateTime time )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            var startDate = this.GetFiscalYear( time ).FirstDay;
            return (int) ( Math.Ceiling( time.Date.Subtract( startDate ).TotalDays ) + 1.0 );
        }

        /// <summary>
        /// Returns the number of days in the specified fiscal month.
        /// </summary>
        /// <param name="year">The year to evaluate.</param>
        /// <param name="month">The month to evaluate.</param>
        /// <param name="era">The era to evaluate.  This parameter is not used.</param>
        /// <returns>The number of days in the fiscal month.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="year"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="month"/> is less than 1
        /// <p>- or -</p><paramref name="month"/> is greater than <see cref="GetMonthsInYear"/>.</exception>
        public override int GetDaysInMonth( int year, int month, int era )
        {
            if ( !IsValidYear( this, year ) )
                throw new ArgumentOutOfRangeException( "year" );

            if ( !IsValidMonth( this, year, month, era ) )
                throw new ArgumentOutOfRangeException( "month" );

            return this.calendarYears[year].Months[month].DaysInMonth;
        }

        /// <summary>
        /// Returns the number of days in the fiscal year.
        /// </summary>
        /// <param name="year">The year to evaluate.</param>
        /// <param name="era">The era to evaluate.  This parameter is not used.</param>
        /// <returns>The number of days in the fiscal year.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="year"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        public override int GetDaysInYear( int year, int era )
        {
            if ( !IsValidYear( this, year ) )
                throw new ArgumentOutOfRangeException( "year" );

            return this.calendarYears[year].DaysInYear;
        }

        /// <summary>
        /// Returns the era for the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to evaluate.</param>
        /// <returns>This method always returns zero.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="time"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        public override int GetEra( DateTime time )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            return 1;
        }

        /// <summary>
        /// Returns the fiscal month for the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to evaluate.</param>
        /// <returns>The fiscal month.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="time"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        public override int GetMonth( DateTime time )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            time = time.Date;

            var months = this.GetFiscalYear( time ).Months;
            var month = 0;

            while ( ++month <= months.Count )
            {
                if ( time <= months[month].LastDay )
                    break;
            }

            return month;
        }

        /// <summary>
        /// Returns the number of months in the specified year.
        /// </summary>
        /// <param name="year">The year to evaluate.</param>
        /// <param name="era">The era to evaluate.  This parameter is not used.</param>
        /// <returns>This method always returns twelve.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="year"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        public override int GetMonthsInYear( int year, int era )
        {
            if ( !IsValidYear( this, year ) )
                throw new ArgumentOutOfRangeException( "year" );

            return 12;
        }

        /// <summary>
        /// Returns the week of the year of the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to evaluate.</param>
        /// <param name="rule">One of the <see cref="CalendarWeekRule"/> values.  The parameter is not used.</param>
        /// <param name="firstDayOfWeek">One of the <see cref="DayOfWeek"/> values.  This parameter cannot be different for
        /// the day of week specified by <see cref="Calendar.MinSupportedDateTime"/>.</param>
        /// <returns>The fiscal week of the year.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="Calendar.MinSupportedDateTime"/><p>- or -</p>
        /// <paramref name="time"/> is greater than <see cref="Calendar.MaxSupportedDateTime"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="firstDayOfWeek"/> is different from the day of week specified
        /// by <see cref="Calendar.MinSupportedDateTime"/>.</exception>
        public override int GetWeekOfYear( DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            if ( this.MinSupportedDateTime.DayOfWeek != firstDayOfWeek )
                throw new ArgumentOutOfRangeException( "firstDayOfWeek", string.Format( null, ExceptionMessage.MustMatchEpochStartDay, firstDayOfWeek ) );

            time = time.Date; // truncate time

            var current = this.GetFiscalYear( time ).FirstDay;
            var week = 0;

            while ( current <= time )
            {
                week++;
                current = current.AddDays( 7.0 );
            }

            return week;
        }

        /// <summary>
        /// Returns the fiscal year for the specified date.
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> to evaluate.</param>
        /// <returns>The fiscal year.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="time"/> is less than <see cref="Calendar.MinSupportedDateTime"/><p>- or -</p>
        /// <paramref name="time"/> is greater than <see cref="Calendar.MaxSupportedDateTime"/>.</exception>
        public override int GetYear( DateTime time )
        {
            if ( !IsValidDate( this, time ) )
                throw new ArgumentOutOfRangeException( "time" );

            time = time.Date;

            foreach ( var year in this.calendarYears )
            {
                if ( ( time >= year.Value.FirstDay ) && ( time <= year.Value.LastDay ) )
                    return year.Value.LastDay.Year;
            }

            return time.Year;
        }

        /// <summary>
        /// Returns a value indicating whether the specified day is a leap day.
        /// </summary>
        /// <param name="year">The year to evaluate.</param>
        /// <param name="month">The month to evaluate.</param>
        /// <param name="day">The day to evaluate.</param>
        /// <param name="era">The era to evaluate.  This parameter is not used.</param>
        /// <returns>True if the specified day falls within the leap week of a leap year; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="year"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="month"/> is less than 1
        /// <p>- or -</p><paramref name="month"/> is greater than <see cref="GetMonthsInYear"/>.</exception>
        public override bool IsLeapDay( int year, int month, int day, int era )
        {
            if ( !IsValidYear( this, year ) )
                throw new ArgumentOutOfRangeException( "year" );

            if ( !IsValidMonth( this, year, month, era ) )
                throw new ArgumentOutOfRangeException( "month" );

            return ( day >= 28 && day <= 35 ) && this.IsLeapMonth( year, month, era );
        }

        /// <summary>
        /// Returns a value indicating whether the specified month is a leap month.
        /// </summary>
        /// <param name="year">The year to evaluate.</param>
        /// <param name="month">The month to evaluate.</param>
        /// <param name="era">The era to evaluate.  This parameter is not used.</param>
        /// <returns>True if the specified month is a leap month; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="year"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="month"/> is less than 1
        /// <p>- or -</p><paramref name="month"/> is greater than <see cref="GetMonthsInYear"/>.</exception>
        public override bool IsLeapMonth( int year, int month, int era )
        {
            if ( !IsValidYear( this, year ) )
                throw new ArgumentOutOfRangeException( "year" );

            if ( !IsValidMonth( this, year, month, era ) )
                throw new ArgumentOutOfRangeException( "month" );

            return ( this.GetMonthsInYear( year, era ) == month ) && this.IsLeapYear( year, era );
        }

        /// <summary>
        /// Returns a value indicating whether the specified year is a leap year.
        /// </summary>
        /// <param name="year">The year to evaluate.</param>
        /// <param name="era">The era to evaluate.  This parameter is not used.</param>
        /// <returns>True if the specified year is a leap year; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="year"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        public override bool IsLeapYear( int year, int era )
        {
            if ( !IsValidYear( this, year ) )
                throw new ArgumentOutOfRangeException( "year" );

            return this.calendarYears[year].DaysInYear == DaysInLeapYear;
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
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="year"/> is less than <see cref="DateTime.Year"/> for the <see cref="DateTime.MinValue"/> date
        /// <p>- or -</p><paramref name="year"/> is greater than <see cref="DateTime.Year"/> for the <see cref="DateTime.MaxValue"/> date.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="month"/> is less than 1
        /// <p>- or -</p><paramref name="month"/> is greater than <see cref="GetMonthsInYear"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="day"/> is less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="hour"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minute"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="second"/> is less than 0.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecond"/> is less than 0.</exception>
        public override DateTime ToDateTime( int year, int month, int day, int hour, int minute, int second, int millisecond, int era )
        {
            if ( !IsValidYear( this, year ) )
                throw new ArgumentOutOfRangeException( "year" );

            if ( !IsValidMonth( this, year, month, era ) )
                throw new ArgumentOutOfRangeException( "month" );

            if ( day < 1 )
                throw new ArgumentOutOfRangeException( "day", ExceptionMessage.ArgumentLessThanOne );

            if ( hour < 0 )
                throw new ArgumentOutOfRangeException( "hour", ExceptionMessage.ArgumentLessThanZero );

            if ( minute < 0 )
                throw new ArgumentOutOfRangeException( "minute", ExceptionMessage.ArgumentLessThanZero );

            if ( second < 0 )
                throw new ArgumentOutOfRangeException( "second", ExceptionMessage.ArgumentLessThanZero );

            if ( millisecond < 0 )
                throw new ArgumentOutOfRangeException( "millisecond", ExceptionMessage.ArgumentLessThanZero );

            var date = this.calendarYears[year].Months[month].FirstDay.AddDays( ( (double) day ) - 1.0 );
            return date.Add( new TimeSpan( 0, hour, minute, second, millisecond ) );
        }

        /// <summary>
        /// Returns the string equivalent of the current instance.
        /// </summary>
        /// <returns>A <see cref="String"/> object.</returns>
        public override string ToString()
        {
            return string.Format( CultureInfo.CurrentCulture, "MinSupportedDateTime = {0:d}, MaxSupportedDateTime = {1:d}", this.MinSupportedDateTime, this.MaxSupportedDateTime );
        }
    }
}
