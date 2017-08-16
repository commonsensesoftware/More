namespace System.Globalization
{
    using More.Globalization;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using static System.DateTime;
    using static System.Globalization.CultureInfo;

    /// <summary>
    /// Provides extension methods for the <see cref="Calendar"/> class.
    /// </summary>
    public static class CalendarExtensions
    {
        /// <summary>
        /// Returns the current first occurrence of the specified week day in the month from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the first day of the week in the month from.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentFirstDayOfWeekInMonth( this Calendar calendar, DayOfWeek dayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( Today.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( Today.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );

            return calendar.FirstDayOfWeekInMonth( Today, dayOfWeek );
        }

        /// <summary>
        /// Returns the first occurrence of the specified week day in the month using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the first day of the week in the month from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the first day of the week in the month for.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime FirstDayOfWeekInMonth( this Calendar calendar, DateTime date, DayOfWeek dayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );

            return date.FirstDayOfWeekInMonth( dayOfWeek, calendar );
        }

        /// <summary>
        /// Returns the current last occurrence of the specified week day in the month from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the last day of the week in the month from.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentLastDayOfWeekInMonth( this Calendar calendar, DayOfWeek dayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( Today.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( Today.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );

            return calendar.LastDayOfWeekInMonth( Today, dayOfWeek );
        }

        /// <summary>
        /// Returns the last occurrence of the specified week day in the month using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the last day of the week in the month from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the last day of the week in the month for.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime LastDayOfWeekInMonth( this Calendar calendar, DateTime date, DayOfWeek dayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );

            return date.LastDayOfWeekInMonth( dayOfWeek, calendar );
        }

        /// <summary>
        /// Returns the requested occurrence of the specified week day in the month using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the occurrence of day of the week in the month from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the occurrence from.</param>
        /// <param name="occurrence">The occurrence of the week day to return.</param>
        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to return.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime DayOfWeekInMonth( this Calendar calendar, DateTime date, int occurrence, DayOfWeek dayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<DateTime>().Month == Contract.OldValue( date.Month ) );
            Contract.Ensures( Contract.Result<DateTime>().Year == Contract.OldValue( date.Year ) );
            Contract.Ensures( Contract.Result<DateTime>().DayOfWeek == dayOfWeek );
            Arg.GreaterThan( occurrence, 0, nameof( occurrence ) );

            return date.DayOfWeekInMonth( occurrence, dayOfWeek, calendar );
        }

        /// <summary>
        /// Returns the current start of the week from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the week from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        /// <remarks>This method assumes that the minimum supported date and time of the calendar represents the first day of the week
        /// in the calendar.</remarks>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentStartOfWeek( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );

            return calendar.StartOfWeek( Today, CurrentCulture.DateTimeFormat.FirstDayOfWeek );
        }

        /// <summary>
        /// Returns the start of the week from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the week from.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the first day of the week for the calendar.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentStartOfWeek( this Calendar calendar, DayOfWeek firstDayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );

            return calendar.StartOfWeek( Today, firstDayOfWeek );
        }

        /// <summary>
        /// Returns the start of the week using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the week from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the week for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        /// <remarks>The first day of the week is based on the <see cref="P:CultureInfo.CurrentCulture">current culture</see>.</remarks>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime StartOfWeek( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );

            return date.StartOfWeek( CurrentCulture.DateTimeFormat.FirstDayOfWeek );
        }

        /// <summary>
        /// Returns the start of the week using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the week from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the week for.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the first day of the week for the calendar.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime StartOfWeek( this Calendar calendar, DateTime date, DayOfWeek firstDayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );

            return date.StartOfWeek( firstDayOfWeek );
        }

        /// <summary>
        /// Returns the current start of the month from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the month from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentStartOfMonth( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( Today ) ) );
            Contract.Ensures( calendar.GetMonth( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetMonth( Today ) ) );

            return calendar.StartOfMonth( Today );
        }

        /// <summary>
        /// Returns the start of the month using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the month from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the month for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime StartOfMonth( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( calendar.GetMonth( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetMonth( date ) ) );

            return date.StartOfMonth( calendar );
        }

        /// <summary>
        /// Returns the current start of the quarter from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the quarter from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentStartOfQuarter( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( Today ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Quarter( calendar ) == Contract.OldValue( Today.Quarter( calendar ) ) );

            return calendar.StartOfQuarter( Today );
        }

        /// <summary>
        /// Returns the start of the quarter using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the quarter from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the quarter for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime StartOfQuarter( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Quarter( calendar ) == Contract.OldValue( date.Quarter( calendar ) ) );

            return date.StartOfQuarter( calendar );
        }

        /// <summary>
        /// Returns the current start of the semester from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the semester from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentStartOfSemester( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( Today ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Semester( calendar ) == Contract.OldValue( Today.Semester( calendar ) ) );

            return calendar.StartOfSemester( Today );
        }

        /// <summary>
        /// Returns the start of the semester using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the semester from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the semester for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime StartOfSemester( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Semester( calendar ) == Contract.OldValue( date.Semester( calendar ) ) );

            return date.StartOfSemester( calendar );
        }

        /// <summary>
        /// Returns the current start of the year from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the year from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentStartOfYear( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( Today ) ) );

            return calendar.StartOfYear( Today );
        }

        /// <summary>
        /// Returns the start of the year using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the start of the year from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the start of the year for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime StartOfYear( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );

            return date.StartOfYear( calendar );
        }

        /// <summary>
        /// Returns the current end of the week from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the week from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        /// <remarks>This method assumes that the minimum supported date and time of the calendar represents the first day of the week
        /// in the calendar.</remarks>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentEndOfWeek( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );

            return calendar.EndOfWeek( Today );
        }

        /// <summary>
        /// Returns the end of the week from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the week from.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the first day of the week for the calendar.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentEndOfWeek( this Calendar calendar, DayOfWeek firstDayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );

            return calendar.EndOfWeek( Today, firstDayOfWeek );
        }

        /// <summary>
        /// Returns the end of the week using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the week from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the week for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        /// <remarks>The first day of the week is based on the <see cref="P:CultureInfo.CurrentCulture">current culture</see>.</remarks>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime EndOfWeek( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );

            return calendar.EndOfWeek( date, CurrentCulture.DateTimeFormat.FirstDayOfWeek );
        }

        /// <summary>
        /// Returns the end of the week using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the week from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the week for.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the first day of the week for the calendar.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime EndOfWeek( this Calendar calendar, DateTime date, DayOfWeek firstDayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );

            return date.EndOfWeek( firstDayOfWeek );
        }

        /// <summary>
        /// Returns the current end of the month from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the month from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentEndOfMonth( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( Today ) ) );
            Contract.Ensures( calendar.GetMonth( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetMonth( Today ) ) );

            return calendar.EndOfMonth( Today );
        }

        /// <summary>
        /// Returns the end of the month using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the month from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the month for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime EndOfMonth( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( calendar.GetMonth( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetMonth( date ) ) );

            return date.EndOfMonth( calendar );
        }

        /// <summary>
        /// Returns the current end of the quarter from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the quarter from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentEndOfQuarter( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( Today ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Quarter( calendar ) == Contract.OldValue( Today.Quarter( calendar ) ) );

            return calendar.EndOfQuarter( Today );
        }

        /// <summary>
        /// Returns the end of the quarter using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the quarter from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the quarter for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime EndOfQuarter( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Quarter( calendar ) == Contract.OldValue( date.Quarter( calendar ) ) );

            return date.EndOfQuarter( calendar );
        }

        /// <summary>
        /// Returns the current end of the semester from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the semester from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentEndOfSemester( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( Today ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Semester( calendar ) == Contract.OldValue( Today.Semester( calendar ) ) );

            return calendar.EndOfSemester( Today );
        }

        /// <summary>
        /// Returns the end of the semester using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the semester from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the semester for.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime EndOfSemester( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );
            Contract.Ensures( Contract.Result<DateTime>().Semester( calendar ) == Contract.OldValue( date.Semester( calendar ) ) );

            return date.EndOfSemester( calendar );
        }

        /// <summary>
        /// Returns the current end of the year from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the year from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime CurrentEndOfYear( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( Today ) ) );

            return calendar.EndOfYear( Today );
        }

        /// <summary>
        /// Returns the end of the year using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the end of the year from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the end of the year from.</param>
        /// <returns>A <see cref="DateTime"/> structure.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DateTime EndOfYear( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( calendar.GetYear( Contract.Result<DateTime>() ) == Contract.OldValue( calendar.GetYear( date ) ) );

            return date.EndOfYear( calendar );
        }

        /// <summary>
        /// Returns the current week of the year from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the week from.</param>
        /// <returns>The current week of the year.</returns>
        /// <remarks>The first day of the week is based on the <see cref="P:CultureInfo.CurrentCulture">current culture</see>.</remarks>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int CurrentWeek( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );

            return calendar.Week( Today, CurrentCulture.DateTimeFormat.FirstDayOfWeek );
        }

        /// <summary>
        /// Returns the week of the year using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the week from.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the first day of the week for the calendar.</param>
        /// <returns>The week of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int CurrentWeek( this Calendar calendar, DayOfWeek firstDayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );

            return calendar.Week( Today, firstDayOfWeek );
        }

        /// <summary>
        /// Returns the week of the year using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the week from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the week of the year from.</param>
        /// <returns>The week of the year.</returns>
        /// <remarks>The first day of the week is based on the <see cref="P:CultureInfo.CurrentCulture">current culture</see>.</remarks>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int Week( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );

            return calendar.Week( date, CurrentCulture.DateTimeFormat.FirstDayOfWeek );
        }

        /// <summary>
        /// Returns the week of the year using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the week from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the week of the year from.</param>
        /// <param name="firstDayOfWeek">The <see cref="DayOfWeek"/> representing the first day of the week for the calendar.</param>
        /// <returns>The week of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int Week( this Calendar calendar, DateTime date, DayOfWeek firstDayOfWeek )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );

            return date.Week( calendar, firstDayOfWeek );
        }

        /// <summary>
        /// Returns the current quarter of the year from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the quarter from.</param>
        /// <returns>The current quarter of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int CurrentQuarter( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 5 );

            return calendar.Quarter( Today );
        }

        /// <summary>
        /// Returns the quarter of the year using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the quarter from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the quarter for.</param>
        /// <returns>The quarter of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int Quarter( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 5 );

            return date.Quarter( calendar );
        }

        /// <summary>
        /// Returns the current semester of the year from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the semester from.</param>
        /// <returns>The current semester of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int CurrentSemester( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 3 );

            return calendar.Semester( Today );
        }

        /// <summary>
        /// Returns the semester of the year using the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the semester from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the semester for.</param>
        /// <returns>The semester of the year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int Semester( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 3 );

            return date.Semester( calendar );
        }

        /// <summary>
        /// Returns the current year from today.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the year from.</param>
        /// <returns>The current year.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int CurrentYear( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 10000 );

            return calendar.Year( Today );
        }

        /// <summary>
        /// Returns the year for the specified date.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the year from.</param>
        /// <param name="date">The <see cref="DateTime"/> to get the year for.</param>
        /// <returns>The year for the specified date.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int Year( this Calendar calendar, DateTime date )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );
            Contract.Ensures( Contract.Result<int>() < 10000 );

            return date.Year( calendar );
        }

        /// <summary>
        /// Returns the first month of the year.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> to get the month from.</param>
        /// <returns>The one-based month that the <see cref="Calendar">calendar</see> starts on.</returns>
        /// <remarks>Typically a <see cref="Calendar">calendar</see> starts on month one; however, some calendars have a
        /// custom <see cref="ICalendarEpoch">epoch month</see>.  This method can be used to retrieve the actual month the
        /// calendar starts on.</remarks>
        public static int FirstMonthOfYear( this Calendar calendar )
        {
            Arg.NotNull( calendar, nameof( calendar ) );
            Contract.Ensures( Contract.Result<int>() > 0 );

            var epoch = calendar as ICalendarEpoch;
            return epoch == null ? 1 : epoch.Month;
        }
    }
}