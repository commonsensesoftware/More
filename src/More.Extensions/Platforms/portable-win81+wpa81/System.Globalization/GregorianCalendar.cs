namespace System.Globalization
{
    using More;
    using System;

    /// <summary>
    /// Represents the Gregorian calendar.
    /// </summary>
    public class GregorianCalendar : Calendar
    {
        static readonly int[] DaysToMonth365 = new[] { 0, 0x1f, 0x3b, 90, 120, 0x97, 0xb5, 0xd4, 0xf3, 0x111, 0x130, 0x14e, 0x16d };
        static readonly int[] DaysToMonth366 = new[] { 0, 0x1f, 60, 0x5b, 0x79, 0x98, 0xb6, 0xd5, 0xf4, 0x112, 0x131, 0x14f, 0x16e };
        int twoDigitYearMax = -1;

        static long DateToTicks( int year, int month, int day ) => GetAbsoluteDate( year, month, day ) * 0xc92a69c000L;

        static long GetAbsoluteDate( int year, int month, int day )
        {
            if ( ( ( year >= 1 ) && ( year <= 0x270f ) ) && ( ( month >= 1 ) && ( month <= 12 ) ) )
            {
                var numArray = ( ( ( year % 4 ) == 0 ) && ( ( ( year % 100 ) != 0 ) || ( ( year % 400 ) == 0 ) ) ) ? DaysToMonth366 : DaysToMonth365;

                if ( ( day >= 1 ) && ( day <= ( numArray[month] - numArray[month - 1] ) ) )
                {
                    var num = year - 1;
                    var num2 = ( ( ( ( ( ( num * 0x16d ) + ( num / 4 ) ) - ( num / 100 ) ) + ( num / 400 ) ) + numArray[month - 1] ) + day ) - 1;
                    return num2;
                }
            }

            throw new ArgumentOutOfRangeException( null, ExceptionMessage.ArgumentOutOfRangeBadYearMonthDay );
        }

        private static int GetDatePart( long ticks, int part )
        {
            var num = (int) ( ticks / 0xc92a69c000L );
            var num2 = num / 0x23ab1;

            num -= num2 * 0x23ab1;

            var num3 = num / 0x8eac;

            if ( num3 == 4 )
            {
                num3 = 3;
            }

            num -= num3 * 0x8eac;

            var num4 = num / 0x5b5;

            num -= num4 * 0x5b5;

            var num5 = num / 0x16d;

            if ( num5 == 4 )
            {
                num5 = 3;
            }

            if ( part == 0 )
            {
                return ( ( ( ( num2 * 400 ) + ( num3 * 100 ) ) + ( num4 * 4 ) ) + num5 ) + 1;
            }

            num -= num5 * 0x16d;

            if ( part == 1 )
            {
                return num + 1;
            }

            var numArray = ( ( num5 == 3 ) && ( ( num4 != 0x18 ) || ( num3 == 3 ) ) ) ? DaysToMonth366 : DaysToMonth365;
            var index = num >> 6;

            while ( num >= numArray[index] )
            {
                index++;
            }

            if ( part == 2 )
            {
                return index;
            }

            return ( num - numArray[index - 1] ) + 1;
        }

        /// <summary>
        /// Gets the list of eras in the <see cref="T:System.Globalization.GregorianCalendar" />.
        /// </summary>
        /// <value>An array of integers that represents the eras in the <see cref="T:System.Globalization.GregorianCalendar" />.</value>
        public override int[] Eras => new[] { 1 };

        /// <summary>
        /// Gets the latest date and time supported by the <see cref="T:System.Globalization.GregorianCalendar" /> type.
        /// </summary>
        /// <value>The latest date and time supported by the <see cref="T:System.Globalization.GregorianCalendar" /> type,
        /// which is the last moment of December 31, 9999 C.E. and is equivalent to <see cref="F:System.DateTime.MaxValue" />.</value>
        public override DateTime MaxSupportedDateTime => DateTime.MaxValue;

        /// <summary>
        /// Gets the earliest date and time supported by the <see cref="T:System.Globalization.GregorianCalendar" /> type.
        /// </summary>
        /// <value>The earliest date and time supported by the <see cref="T:System.Globalization.GregorianCalendar" /> type,
        /// which is the first moment of January 1, 0001 C.E. and is equivalent to <see cref="F:System.DateTime.MinValue" />.</value>
        public override DateTime MinSupportedDateTime => DateTime.MinValue;

        /// <summary>
        /// Gets or sets the last year of a 100-year range that can be represented by a 2-digit year.
        /// </summary>
        /// <value>The last year of a 100-year range that can be represented by a 2-digit year.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The value specified in a set operation is less than 99. -or- The value specified in a set operation is greater than MaxSupportedDateTime.Year.</exception>
        public override int TwoDigitYearMax
        {
            get
            {
                if ( twoDigitYearMax == -1 )
                {
                    twoDigitYearMax = 0x7ed;
                }

                return twoDigitYearMax;
            }
            set
            {
                if ( ( value < 0x63 ) || ( value > 0x270f ) )
                {
                    throw new ArgumentOutOfRangeException( nameof( value ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 0x63, 0x270f ) );
                }

                twoDigitYearMax = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.DateTime" /> that is the specified number of months away from the specified <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="time">The <see cref="T:System.DateTime" /> to which to add months.</param>
        /// <param name="months">The number of months to add.</param>
        /// <returns>The <see cref="T:System.DateTime" /> that results from adding the specified number of months to the specified <see cref="T:System.DateTime" />.</returns>
        /// <exception cref="T:System.ArgumentException">The resulting <see cref="T:System.DateTime" /> is outside the supported range.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="months" /> is less than -120000.-or- <paramref name="months" /> is greater than 120000.</exception>
        public override DateTime AddMonths( DateTime time, int months )
        {
            if ( ( months < -120000 ) || ( months > 0x1d4c0 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( months ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( -120000, 0x1d4c0 ) );
            }

            var datePart = GetDatePart( time.Ticks, 0 );
            var index = GetDatePart( time.Ticks, 2 );
            var day = GetDatePart( time.Ticks, 3 );
            var num4 = ( index - 1 ) + months;

            if ( num4 >= 0 )
            {
                index = ( num4 % 12 ) + 1;
                datePart += num4 / 12;
            }
            else
            {
                index = 12 + ( ( num4 + 1 ) % 12 );
                datePart += ( num4 - 11 ) / 12;
            }

            var numArray = ( ( ( datePart % 4 ) == 0 ) && ( ( ( datePart % 100 ) != 0 ) || ( ( datePart % 400 ) == 0 ) ) ) ? DaysToMonth366 : DaysToMonth365;
            var num5 = numArray[index] - numArray[index - 1];

            if ( day > num5 )
            {
                day = num5;
            }

            var ticks = DateToTicks( datePart, index, day ) + ( time.Ticks % 0xc92a69c000L );

            if ( ( ticks < MinSupportedDateTime.Ticks ) || ( ticks > MaxSupportedDateTime.Ticks ) )
            {
                throw new ArgumentException( ExceptionMessage.ArgumentResultCalendarRange.FormatDefault( MinSupportedDateTime, MaxSupportedDateTime ) );
            }

            return new DateTime( ticks );
        }

        /// <summary>
        /// Returns a <see cref="T:System.DateTime" /> that is the specified number of years away from the specified <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="time">The <see cref="T:System.DateTime" /> to which to add years.</param>
        /// <param name="years">The number of years to add.</param>
        /// <returns>The <see cref="T:System.DateTime" /> that results from adding the specified number of years to the specified <see cref="T:System.DateTime" />.</returns>
        /// <exception cref="T:System.ArgumentException">The resulting <see cref="T:System.DateTime" /> is outside the supported range.</exception>
        public override DateTime AddYears( DateTime time, int years ) => AddMonths( time, checked(years * 12) );

        /// <summary>
        /// Returns the day of the month in the specified <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
        /// <returns>An integer from 1 to 31 that represents the day of the month in <paramref name="time" />.</returns>
        public override int GetDayOfMonth( DateTime time ) => GetDatePart( time.Ticks, 3 );

        /// <summary>
        /// Returns the day of the week in the specified <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
        /// <returns>A <see cref="T:System.DayOfWeek" /> value that represents the day of the week in <paramref name="time" />.</returns>
        public override DayOfWeek GetDayOfWeek( DateTime time ) => (DayOfWeek) ( ( (int) ( ( time.Ticks / 0xc92a69c000L ) + 1L ) ) % 7 );

        /// <summary>
        /// Returns the day of the year in the specified <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
        /// <returns>An integer from 1 to 366 that represents the day of the year in <paramref name="time" />.</returns>
        public override int GetDayOfYear( DateTime time ) => GetDatePart( time.Ticks, 1 );

        /// <summary>
        /// Returns the number of days in the specified month in the specified year in the specified era.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="month">An integer from 1 to 12 that represents the month.</param>
        /// <param name="era">An integer that represents the era.</param>
        /// <returns>The number of days in the specified month in the specified year in the specified era.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="era" /> is outside the range supported by the calendar. -or- <paramref name="year" /> 
        /// is outside the range supported by the calendar.-or- <paramref name="month" /> is outside the range supported by the calendar.</exception>
        public override int GetDaysInMonth( int year, int month, int era )
        {
            if ( ( era != 0 ) && ( era != 1 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( era ), ExceptionMessage.ArgumentOutOfRangeInvalidEraValue );
            }

            if ( ( year < 1 ) || ( year > 0x270f ) )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 0x270f ) );
            }

            if ( ( month < 1 ) || ( month > 12 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( month ), ExceptionMessage.ArgumentOutOfRangeMonth );
            }

            var numArray = ( ( ( year % 4 ) == 0 ) && ( ( ( year % 100 ) != 0 ) || ( ( year % 400 ) == 0 ) ) ) ? DaysToMonth366 : DaysToMonth365;

            return numArray[month] - numArray[month - 1];
        }

        /// <summary>
        /// Returns the number of days in the specified year in the specified era.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="era">An integer that represents the era.</param>
        /// <returns>The number of days in the specified year in the specified era.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="era" /> is outside the range supported by the calendar.-or- <paramref name="year" />
        /// is outside the range supported by the calendar.</exception>
        public override int GetDaysInYear( int year, int era )
        {
            if ( ( era != 0 ) && ( era != 1 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( era ), ExceptionMessage.ArgumentOutOfRangeInvalidEraValue );
            }

            if ( ( year < 1 ) || ( year > 0x270f ) )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 0x270f ) );
            }

            if ( ( ( year % 4 ) != 0 ) || ( ( ( year % 100 ) == 0 ) && ( ( year % 400 ) != 0 ) ) )
            {
                return 0x16d;
            }

            return 0x16e;
        }

        /// <summary>
        /// Returns the era in the specified <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
        /// <returns>An integer that represents the era in <paramref name="time" />.</returns>
        public override int GetEra( DateTime time ) => 1;

        /// <summary>
        /// Calculates the leap month for a specified year and era.
        /// </summary>
        /// <param name="year">A year to evaluate.</param>
        /// <param name="era">An era to evaluate.</param>
        /// <returns>Always 0 because the Gregorian calendar does not recognize leap months.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="year" /> is less than the Gregorian calendar year 1 or greater than the
        /// Gregorian calendar year 9999.-or-<paramref name="era" /> is not GregorianCalendar.Eras[Calendar.CurrentEra].</exception>
        public override int GetLeapMonth( int year, int era )
        {
            if ( ( era != 0 ) && ( era != 1 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( era ), ExceptionMessage.ArgumentOutOfRangeInvalidEraValue );
            }

            if ( ( year < 1 ) || ( year > 0x270f ) )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 0x270f ) );
            }

            return 0;
        }

        /// <summary>
        /// Returns the month in the specified <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
        /// <returns>An integer from 1 to 12 that represents the month in <paramref name="time" />.</returns>
        public override int GetMonth( DateTime time ) => GetDatePart( time.Ticks, 2 );

        /// <summary>
        /// Returns the number of months in the specified year in the specified era.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="era">An integer that represents the era.</param>
        /// <returns>The number of months in the specified year in the specified era.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="era" /> is outside the range supported by the calendar.-or- <paramref name="year" />
        /// is outside the range supported by the calendar.</exception>
        public override int GetMonthsInYear( int year, int era )
        {
            if ( ( era != 0 ) && ( era != 1 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( era ), ExceptionMessage.ArgumentOutOfRangeInvalidEraValue );
            }

            if ( ( year < 1 ) || ( year > 0x270f ) )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 0x270f ) );
            }

            return 12;
        }

        /// <summary>
        /// Returns the year in the specified <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
        /// <returns>An integer that represents the year in <paramref name="time" />.</returns>
        public override int GetYear( DateTime time ) => GetDatePart( time.Ticks, 0 );

        /// <summary>
        /// Determines whether the specified date in the specified era is a leap day.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="month">An integer from 1 to 12 that represents the month.</param>
        /// <param name="day">An integer from 1 to 31 that represents the day.</param>
        /// <param name="era">An integer that represents the era.</param>
        /// <returns>true if the specified day is a leap day; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="era" /> is outside the range supported by the calendar. -or- <paramref name="year" />
        /// is outside the range supported by the calendar.-or- <paramref name="month" /> is outside the range supported by the calendar.-or- <paramref name="day" />
        /// is outside the range supported by the calendar.</exception>
        public override bool IsLeapDay( int year, int month, int day, int era )
        {
            if ( ( month < 1 ) || ( month > 12 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( month ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 12 ) );
            }

            if ( ( era != 0 ) && ( era != 1 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( era ), ExceptionMessage.ArgumentOutOfRangeInvalidEraValue );
            }

            if ( ( year < 1 ) || ( year > 0x270f ) )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 0x270f ) );
            }

            if ( ( day < 1 ) || ( day > GetDaysInMonth( year, month ) ) )
            {
                throw new ArgumentOutOfRangeException( nameof( day ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, GetDaysInMonth( year, month ) ) );
            }

            if ( !IsLeapYear( year ) )
            {
                return false;
            }

            return ( month == 2 ) && ( day == 0x1d );
        }

        /// <summary>
        /// Determines whether the specified month in the specified year in the specified era is a leap month.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="month">An integer from 1 to 12 that represents the month.</param>
        /// <param name="era">An integer that represents the era.</param>
        /// <returns>This method always returns false, unless overridden by a derived class.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="era" /> is outside the range supported by the calendar.-or- <paramref name="year" />
        /// is outside the range supported by the calendar.-or- <paramref name="month" /> is outside the range supported by the calendar.</exception>
        public override bool IsLeapMonth( int year, int month, int era )
        {
            if ( ( era != 0 ) && ( era != 1 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( era ), ExceptionMessage.ArgumentOutOfRangeInvalidEraValue );
            }

            if ( ( year < 1 ) || ( year > 0x270f ) )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 0x270f ) );
            }

            if ( ( month < 1 ) || ( month > 12 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( month ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 12 ) );
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified year in the specified era is a leap year.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="era">An integer that represents the era.</param>
        /// <returns>true if the specified year is a leap year; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="era" /> is outside the range supported by the calendar.-or- <paramref name="year" />
        /// is outside the range supported by the calendar.</exception>
        public override bool IsLeapYear( int year, int era )
        {
            if ( ( era != 0 ) && ( era != 1 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( era ), ExceptionMessage.ArgumentOutOfRangeInvalidEraValue );
            }

            if ( ( year < 1 ) || ( year > 0x270f ) )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 0x270f ) );
            }

            if ( ( year % 4 ) != 0 )
            {
                return false;
            }

            return ( ( year % 100 ) != 0 ) || ( ( year % 400 ) == 0 );
        }

        /// <summary>
        /// Returns a <see cref="T:System.DateTime" /> that is set to the specified date and time in the specified era.
        /// </summary>
        /// <param name="year">An integer that represents the year.</param>
        /// <param name="month">An integer from 1 to 12 that represents the month.</param>
        /// <param name="day">An integer from 1 to 31 that represents the day.</param>
        /// <param name="hour">An integer from 0 to 23 that represents the hour.</param>
        /// <param name="minute">An integer from 0 to 59 that represents the minute.</param>
        /// <param name="second">An integer from 0 to 59 that represents the second.</param>
        /// <param name="millisecond">An integer from 0 to 999 that represents the millisecond.</param>
        /// <param name="era">An integer that represents the era.</param>
        /// <returns>The <see cref="T:System.DateTime" /> that is set to the specified date and time in the current era.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="era" /> is outside the range supported by the calendar.-or- <paramref name="year" />
        /// is outside the range supported by the calendar.-or- <paramref name="month" /> is outside the range supported by the calendar.-or- <paramref name="day" />
        /// is outside the range supported by the calendar.-or- <paramref name="hour" /> is less than zero or greater than 23.-or- <paramref name="minute" />
        /// is less than zero or greater than 59.-or- <paramref name="second" /> is less than zero or greater than 59.-or- <paramref name="millisecond" />
        /// is less than zero or greater than 999.</exception>
        public override DateTime ToDateTime( int year, int month, int day, int hour, int minute, int second, int millisecond, int era )
        {
            if ( ( era != 0 ) && ( era != 1 ) )
            {
                throw new ArgumentOutOfRangeException( nameof( era ), ExceptionMessage.ArgumentOutOfRangeInvalidEraValue );
            }

            return new DateTime( year, month, day, hour, minute, second, millisecond );
        }

        /// <summary>
        /// Converts the specified year to a four-digit year by using the <see cref="P:System.Globalization.GregorianCalendar.TwoDigitYearMax" /> property to determine the appropriate century.
        /// </summary>
        /// <param name="year">A two-digit or four-digit integer that represents the year to convert.</param>
        /// <returns>An integer that contains the four-digit representation of <paramref name="year" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="year" /> is outside the range supported by the calendar.</exception>
        public override int ToFourDigitYear( int year )
        {
            if ( year < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeNeedNonNegNum );
            }

            if ( year > 0x270f )
            {
                throw new ArgumentOutOfRangeException( nameof( year ), ExceptionMessage.ArgumentOutOfRangeRange.FormatDefault( 1, 0x270f ) );
            }

            return base.ToFourDigitYear( year );
        }
    }
}