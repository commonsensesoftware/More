namespace System
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Provides extension methods for the <see cref="DateTime"/> strucuture.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the specified date and time can be represented by the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to evaluate.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <param name="year">The year to test.</param>
        /// <param name="month">The month to test.</param>
        /// <param name="day">The day to test.</param>
        /// <returns>True if the date and time can be represented by the calendar; otherwise, false.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static bool IsRepresentable( this DateTime date, Calendar calendar, int year, int month, int day )
        {
            Arg.NotNull( calendar, "calendar" );

            if ( date < calendar.MinSupportedDateTime || date > calendar.MaxSupportedDateTime )
                return false;

            try
            {
                calendar.ToDateTime( year, month, day, date.Hour, date.Minute, date.Second, date.Millisecond );
            }
            catch ( ArgumentOutOfRangeException )
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the first occurrence of the specified week day in the month using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the day from.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static DateTime FirstDayOfWeekInMonth( this DateTime date, DayOfWeek dayOfWeek, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );

            date = date.StartOfMonth( calendar );

            while ( date.DayOfWeek != dayOfWeek )
                date = date.AddDays( 1.0 );

            return date;
        }

        /// <summary>
        /// Returns the last occurrence of the specified week day in the month using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the day from.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static DateTime LastDayOfWeekInMonth( this DateTime date, DayOfWeek dayOfWeek, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );

            date = date.EndOfMonth( calendar );

            while ( date.DayOfWeek != dayOfWeek )
                date = date.AddDays( -1.0 );

            return date;
        }

        /// <summary>
        /// Returns the requested occurrence of the specified week day in the month using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the day from.</param>
        /// <param name="occurrence">The occurrence of the week day to return.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Validated by a code contract." )]
        public static DateTime DayOfWeekInMonth( this DateTime date, int occurrence, DayOfWeek dayOfWeek, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );
            Arg.GreaterThan( occurrence, 0, "occurrence" );

            var day = date.StartOfMonth( calendar );

            for ( var i = 0; i < occurrence; i++, day = day.AddDays( 1.0 ) )
            {
                while ( day.DayOfWeek != dayOfWeek )
                    day = day.AddDays( 1.0 );

                // it's not known how many weeks may be in the month, but once we've past the
                // original month we're out of range
                if ( day.Month != day.Month )
                    throw new ArgumentOutOfRangeException( "occurrence" );
            }

            // account for additional day added by loop increment
            day = day.AddDays( -1.0 );

            return day;
        }

        /// <summary>
        /// Returns the start of the month using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the month for.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static DateTime StartOfMonth( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( calendar.GetMonth( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetMonth( date ) ) );

            var year = calendar.GetYear( date );
            var month = calendar.GetMonth( date );
            return calendar.ToDateTime( year, month, 1, date.Hour, date.Minute, date.Second, date.Millisecond );
        }

        /// <summary>
        /// Returns the start of the quarter using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the quarter for.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static DateTime StartOfQuarter( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Quarter( calendar ) == Contract.OldValue( date.Quarter( calendar ) ) );

            var quarter = date.Quarter( calendar );
            return calendar.AddMonths( date.StartOfYear( calendar ), ( quarter - 1 ) * 3 );
        }

        /// <summary>
        /// Returns the start of the semester using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the semester for.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static DateTime StartOfSemester( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Semester( calendar ) == Contract.OldValue( date.Semester( calendar ) ) );

            var semester = date.Semester( calendar );
            return calendar.AddMonths( date.StartOfYear( calendar ), ( semester - 1 ) * 6 );
        }

        /// <summary>
        /// Returns the start of the year using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the year for.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static DateTime StartOfYear( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );

            return date.AddDays( (double) ( -calendar.GetDayOfYear( date ) + 1 ) );
        }

        /// <summary>
        /// Returns the end of the month using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the month for.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static DateTime EndOfMonth( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( calendar.GetMonth( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetMonth( date ) ) );

            return calendar.AddMonths( date.StartOfMonth( calendar ), 1 ).AddTicks( -1L );
        }

        /// <summary>
        /// Returns the end of the quarter using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the quarter for.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static DateTime EndOfQuarter( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Quarter( calendar ) == Contract.OldValue( date.Quarter( calendar ) ) );

            return calendar.AddMonths( date.StartOfQuarter( calendar ), 3 ).AddTicks( -1L );
        }

        /// <summary>
        /// Returns the end of the semester using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the semester for.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static DateTime EndOfSemester( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Semester( calendar ) == Contract.OldValue( date.Semester( calendar ) ) );

            return calendar.AddMonths( date.StartOfSemester( calendar ), 6 ).AddTicks( -1L );
        }

        /// <summary>
        /// Returns the end of the year using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the year for.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static DateTime EndOfYear( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );

            return calendar.AddYears( date.StartOfYear( calendar ), 1 ).AddTicks( -1L );
        }

        /// <summary>
        /// Returns the current week of the year using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the week of the year from.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the first day of the week for the calendar.</param>
        /// <returns>The week of the year.</returns>
        /// <remarks>The <see cref="CalendarWeekRule">calendar week rule</see> is based on the <see cref="P:CultureInfo.CurrentCulture">current culture</see>.</remarks>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static int Week( this DateTime date, Calendar calendar, DayOfWeek firstDayOfWeek )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<int>() > 0 );

            var rule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
            return date.Week( calendar, rule, firstDayOfWeek );
        }

        /// <summary>
        /// Returns the current week of the year using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the week of the year from.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <param name="rule">The <see cref="CalendarWeekRule">calendar week rule</see> used to calculate the day of the year.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the first day of the week for the calendar.</param>
        /// <returns>The week of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static int Week( this DateTime date, Calendar calendar, CalendarWeekRule rule, DayOfWeek firstDayOfWeek )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<int>() > 0 );

            return calendar.GetWeekOfYear( date, rule, firstDayOfWeek );
        }

        /// <summary>
        /// Returns the current month of the year using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the month of the year from.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>The month of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static int Month( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() <= calendar.GetMonthsInYear( date.Year ) );

            return calendar.GetMonth( date );
        }

        /// <summary>
        /// Returns the current quarter of the year using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the quarter from.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>The quarter of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static int Quarter( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 5 );

            var month = (double) calendar.GetMonth( date );
            return (int) Math.Ceiling( month / 3.0 );
        }

        /// <summary>
        /// Returns the current semester of the year using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the semester from.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>The semester of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static int Semester( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 3 );

            var month = (double) calendar.GetMonth( date );
            return (int) Math.Ceiling( month / 6.0 );
        }

        /// <summary>
        /// Returns the current year using the specified calendar.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the month of the year from.</param>
        /// <param name="calendar">The type of <see cref="Calendar"/> used.</param>
        /// <returns>The calendar year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static int Year( this DateTime date, Calendar calendar )
        {
            Arg.NotNull( calendar, "calendar" );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 10000 );

            return calendar.GetYear( date );
        }

        /// <summary>
        /// Returns the first occurrence of the specified week day in the month using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the day from.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime FirstDayOfWeekInMonth( this DateTime date, DayOfWeek dayOfWeek )
        {
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );

            return date.FirstDayOfWeekInMonth( dayOfWeek, CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the last occurrence of the specified week day in the month using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the day from.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime LastDayOfWeekInMonth( this DateTime date, DayOfWeek dayOfWeek )
        {
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );

            return date.LastDayOfWeekInMonth( dayOfWeek, CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the requested occurrence of the specified week day in the month using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the day from.</param>
        /// <param name="occurrence">The occurrence of the week day to return.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime DayOfWeekInMonth( this DateTime date, int occurrence, DayOfWeek dayOfWeek )
        {
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );
            Arg.GreaterThan( occurrence, 0, "occurrence" );

            return date.DayOfWeekInMonth( occurrence, dayOfWeek, CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the start of the day.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the day for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime StartOfDay( this DateTime date )
        {
            Contract.Ensures( Contract.Result<DateTime>().Day == Contract.OldValue( date.Day ) );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );

            return date.Date;
        }

        /// <summary>
        /// Returns the start of the week using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the week for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime StartOfWeek( this DateTime date )
        {
            Contract.Ensures( Contract.Result<DateTime>().Week() == Contract.OldValue( date.Week() ) );
            return date.StartOfWeek( CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek );
        }

        /// <summary>
        /// Returns the start of the week.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of week for.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the start of the week.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime StartOfWeek( this DateTime date, DayOfWeek firstDayOfWeek )
        {
            Contract.Ensures( Contract.Result<DateTime>().Week( CultureInfo.CurrentCulture.Calendar, firstDayOfWeek ) == Contract.OldValue( date.Week( CultureInfo.CurrentCulture.Calendar, firstDayOfWeek ) ) );
            
            if ( firstDayOfWeek == DayOfWeek.Sunday )
                return date.AddDays( -( (double) ( (int) date.DayOfWeek ) ) ).Date;

            var start = (int) firstDayOfWeek;

            while ( ( (int) date.DayOfWeek ) != start )
                date = date.AddDays( -1.0 );

            return date.Date;
        }

        /// <summary>
        /// Returns the start of the month using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the month.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime StartOfMonth( this DateTime date )
        {
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetYear( date ) ) );
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetMonth( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetMonth( date ) ) );

            return date.StartOfMonth( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the start of the quarter using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the quarter for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime StartOfQuarter( this DateTime date )
        {
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Quarter( CultureInfo.CurrentCulture.Calendar ) == Contract.OldValue( date.Quarter( CultureInfo.CurrentCulture.Calendar ) ) );

            return date.StartOfQuarter( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the start of the semester using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the semester for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime StartOfSemester( this DateTime date )
        {
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Semester( CultureInfo.CurrentCulture.Calendar ) == Contract.OldValue( date.Semester( CultureInfo.CurrentCulture.Calendar ) ) );

            return date.StartOfSemester( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the start of the year using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the year for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime StartOfYear( this DateTime date )
        {
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetYear( date ) ) );

            return date.StartOfYear( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the end of the day.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of day for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime EndOfDay( this DateTime date )
        {
            Contract.Ensures( Contract.Result<DateTime>().Day == Contract.OldValue( date.Day ) );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );

            return date.Date.AddDays( 1.0 ).AddTicks( -1L );
        }

        /// <summary>
        /// Returns the end of the week using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the week for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime EndOfWeek( this DateTime date )
        {
            Contract.Ensures( Contract.Result<DateTime>().Week() == Contract.OldValue( date.Week() ) );
            return date.EndOfWeek( CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek );
        }

        /// <summary>
        /// Returns the end of the week.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the start of the week.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime EndOfWeek( this DateTime date, DayOfWeek firstDayOfWeek )
        {
            Contract.Ensures( Contract.Result<DateTime>().Week( CultureInfo.CurrentCulture.Calendar, firstDayOfWeek ) == Contract.OldValue( date.Week( CultureInfo.CurrentCulture.Calendar, firstDayOfWeek ) ) );
            return date.StartOfWeek( firstDayOfWeek ).AddDays( 7.0 ).AddTicks( -1L );
        }

        /// <summary>
        /// Returns the end of the month using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of month for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime EndOfMonth( this DateTime date )
        {
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetYear( date ) ) );
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetMonth( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetMonth( date ) ) );

            return date.EndOfMonth( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the end of the quarter using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the quarter for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime EndOfQuarter( this DateTime date )
        {
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Quarter( CultureInfo.CurrentCulture.Calendar ) == Contract.OldValue( date.Quarter( CultureInfo.CurrentCulture.Calendar ) ) );

            return date.EndOfQuarter( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the end of the semester using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the semester for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime EndOfSemester( this DateTime date )
        {
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Semester( CultureInfo.CurrentCulture.Calendar ) == Contract.OldValue( date.Semester( CultureInfo.CurrentCulture.Calendar ) ) );

            return date.EndOfSemester( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the end of the year using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the end of year for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        public static DateTime EndOfYear( this DateTime date )
        {
            Contract.Ensures( CultureInfo.CurrentCulture.Calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( CultureInfo.CurrentCulture.Calendar.GetYear( date ) ) );

            return date.EndOfYear( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the current week of the year using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the week of the year from.</param>
        /// <returns>The week of the year.</returns>
        [Pure]
        public static int Week( this DateTime date )
        {
            Contract.Ensures( Contract.Result<int>() > 0 );

            var format = CultureInfo.CurrentCulture.DateTimeFormat;
            return date.Week( format.Calendar, format.FirstDayOfWeek );
        }

        /// <summary>
        /// Returns the current quarter of the year using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the quarter from.</param>
        /// <returns>The quarter of the year.</returns>
        [Pure]
        public static int Quarter( this DateTime date )
        {
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 5 );

            return date.Quarter( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the current semester of the year using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the semester from.</param>
        /// <returns>The semester of the year.</returns>
        [Pure]
        public static int Semester( this DateTime date )
        {
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 3 );

            return date.Semester( CultureInfo.CurrentCulture.Calendar );
        }

        /// <summary>
        /// Returns the current year using the current culture.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> to get the year from.</param>
        /// <returns>The current year.</returns>
        [Pure]
        public static int Year( this DateTime date )
        {
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 10000 );

            return date.Year( CultureInfo.CurrentCulture.Calendar );
        }
    }
}
