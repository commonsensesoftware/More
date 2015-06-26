namespace More.Globalization
{
    using System;
    using System.Globalization;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="FourFourFiveCalendar"/>.
    /// </summary>
    public partial class FourFourFiveCalendarTest
    {
        [Fact( DisplayName = "get week of year should not allow date out of range" )]
        public void GetWeekOfYearShouldNotAllowDateOutOfRange()
        {
            var date = new DateTime( 2007, 1, 1 );
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetWeekOfYear( date ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "get week of year should return correct result" )]
        public void GetWeekOfYearShouldReturnExpectedValue()
        {
            var date = new DateTime( 2007, 7, 4 );
            var target = CreateCalendar();
            var actual = target.GetWeekOfYear( date );
            Assert.Equal( 1, actual );
        }

        [Fact( DisplayName = "get week of year with rule should not allow date out of range" )]
        public void GetWeekOfYearWithCustomRulesShouldNotAllowDateOutOfRange()
        {
            var date = new DateTime( 2007, 1, 1 );
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetWeekOfYear( date, CalendarWeekRule.FirstDay, target.MinSupportedDateTime.DayOfWeek ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "get week of year with rule should return correct result" )]
        public void GetWeekOfYearWithCustomRulesShouldReturnExpectedValue()
        {
            var date = new DateTime( 2007, 7, 7 );
            var target = CreateCalendar();
            var actual = target.GetWeekOfYear( date, CalendarWeekRule.FirstDay, target.MinSupportedDateTime.DayOfWeek );
            Assert.Equal( 2, actual );
        }

        [Fact( DisplayName = "epoch month should return expected value" )]
        public void EpochMonthPropertyShouldReturnExpectedValue()
        {
            var expected = 7; // test fiscal calendar starts in july
            ICalendarEpoch target = CreateCalendar();
            var actual = target.Month;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "eras should always return single era" )]
        public void ErasPropertyShouldAlwaysReturnOneEra()
        {
            var target = CreateCalendar();
            var actual = target.Eras;
            Assert.Equal( 1, actual.Length );
            Assert.Equal( 1, actual[0] );
        }

        [Fact( DisplayName = "add months should not allow date out of range" )]
        public void AddMonthsShouldNotAllowDateOutOfRange()
        {
            var date = new DateTime( 2007, 1, 1 );
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.AddMonths( date, 1 ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "add months with positive value should return correct result" )]
        public void AddMonthsWithPositiveValueShouldReturnExpectedResult()
        {
            var expected = new DateTime( 2007, 12, 29 );
            var date = new DateTime( 2007, 6, 30 );
            var target = CreateCalendar();
            var actual = target.AddMonths( date, 6 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "add months with negative value should return correct result" )]
        public void AddMonthsWithNegativeValueShouldReturnExpectedResult()
        {
            var expected = new DateTime( 2007, 6, 30 );
            var date = new DateTime( 2007, 12, 29 );
            var target = CreateCalendar();
            var actual = target.AddMonths( date, -6 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "add years should not allow date out of range" )]
        public void AddYearsShouldNotAllowDateOutOfRange()
        {
            var date = new DateTime( 2007, 1, 1 );
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.AddYears( date, 1 ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "add years with positive value should return correct result" )]
        public void AddYearsWithPositiveValueShouldReturnExpectedResult()
        {
            var expected = new DateTime( 2008, 6, 28 );
            var date = new DateTime( 2007, 6, 30 );
            var target = CreateCalendar();
            var actual = target.AddYears( date, 1 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "add years with negative value should return correct result" )]
        public void AddYearsWithNegativeValueShouldReturnExpectedResult()
        {
            var expected = new DateTime( 2007, 6, 30 );
            var date = new DateTime( 2008, 6, 28 );
            var target = CreateCalendar();
            var actual = target.AddYears( date, -1 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get day of month should not allow date out of range" )]
        public void GetDayOfMonthShouldNotAllowDateOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetDayOfMonth( new DateTime( 2006, 1, 1 ) ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "get day of month should return correct result" )]
        public void GetDayOfMonthShouldReturnExpectedValue()
        {
            var firstDay = new DateTime( 2007, 6, 30 ); // using 4-4-5, first day of month could be another month
            var target = CreateCalendar();
            var actual = target.GetDayOfMonth( firstDay );
            Assert.Equal( 1, actual );
        }

        [Fact( DisplayName = "get day of week should not allow date out of range" )]
        public void GetDayOfWeekShouldNotAllowDateOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetDayOfWeek( new DateTime( 2007, 1, 1 ) ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "get day of week should return correct result" )]
        public void GetDayOfWeekShouldReturnExpectedValue()
        {
            var expected = DayOfWeek.Saturday;
            var firstDay = new DateTime( 2007, 6, 30 );
            var target = CreateCalendar();
            var actual = target.GetDayOfWeek( firstDay );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get day of eyar should not allow date out of range" )]
        public void GetDayOfYearShouldNotAllowDateOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetDayOfYear( new DateTime( 2007, 1, 1 ) ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "get day of year should return correct result" )]
        public void GetDayOfYearShouldReturnExpectedValue()
        {
            var expected = 1;
            var firstDay = new DateTime( 2007, 6, 30 );
            var target = CreateCalendar();
            var actual = target.GetDayOfYear( firstDay );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get days in month should not allow year out of range" )]
        public void GetDaysInMonthShouldNotAllowYearOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetDaysInMonth( 2006, 6, 1 ) );
            Assert.Equal( "year", ex.ParamName );
        }

        [Fact( DisplayName = "get days in month should not allow month out of range" )]
        public void GetDaysInMonthShouldNotAllowMonthOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetDaysInMonth( 2007, 5, 1 ) );
            Assert.Equal( "month", ex.ParamName );
        }

        [Fact( DisplayName = "get days in month should return correct result" )]
        public void GetDaysInMonthShouldReturnExpectedValue()
        {
            var expected = 28;
            var target = CreateCalendar();
            var actual = target.GetDaysInMonth( 2008, 1, 1 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get days in year should not allow year out of range" )]
        public void GetDaysInYearShouldNotAllowYearOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetDaysInYear( 2006, 1 ) );
            Assert.Equal( "year", ex.ParamName );
        }

        [Fact( DisplayName = "get days in year should return correct result" )]
        public void GetDaysInYearShouldReturnExpectedValue()
        {
            var expected = 364;
            var target = CreateCalendar();
            var actual = target.GetDaysInYear( 2008, 1 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get era should not allow date out of range" )]
        public void GetEraShouldNotAllowDateOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetEra( new DateTime( 2007, 1, 1 ) ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "get era should return correct result" )]
        public void GetEraShouldReturnExpectedValue()
        {
            var expected = 1;
            var firstDay = new DateTime( 2007, 6, 30 );
            var target = CreateCalendar();
            var actual = target.GetEra( firstDay );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get month should not allow date out of range" )]
        public void GetMonthShouldNotAllowDateOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetMonth( new DateTime( 2007, 1, 1 ) ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "get month should return correct result" )]
        public void GetMonthShouldReturnExpectedValue()
        {
            var expected = 1;
            var firstDay = new DateTime( 2007, 6, 30 );
            var target = CreateCalendar();
            var actual = target.GetMonth( firstDay );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get months in year should not allow year out of range" )]
        public void GetMonthsInYearShouldNotAllowYearOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetMonthsInYear( 2006, 1 ) );
            Assert.Equal( "year", ex.ParamName );
        }

        [Fact( DisplayName = "get months in year should return correct result" )]
        public void GetMonthsInYearShouldReturnExpectedValue()
        {
            var expected = 12;
            var target = CreateCalendar();
            var actual = target.GetMonthsInYear( 2008, 1 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get year should not allow date out of range" )]
        public void GetYearShouldNotAllowDateOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.GetYear( new DateTime( 2007, 1, 1 ) ) );
            Assert.Equal( "time", ex.ParamName );
        }

        [Fact( DisplayName = "get year should return correct result" )]
        public void GetYearShouldReturnExpectedValue()
        {
            // when fiscal years span calendar years,
            // the calendar year from the last day in
            // the fiscal calendar is reported
            var expected = 2008;
            var firstDay = new DateTime( 2007, 6, 30 );
            var target = CreateCalendar();
            var actual = target.GetYear( firstDay );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "is leap day should not allow date out of range" )]
        public void IsLeapDayShouldNotAllowYearOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.IsLeapDay( 2006, 1, 1, 1 ) );
            Assert.Equal( "year", ex.ParamName );
        }

        [Fact( DisplayName = "is leap day should not allow month out of range" )]
        public void IsLeapDayShouldNotAllowMonthOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.IsLeapDay( 2007, 5, 1, 1 ) );
            Assert.Equal( "month", ex.ParamName );
        }

        [Fact( DisplayName = "is leap month should not allow year out of range" )]
        public void IsLeapMonthShouldNotAllowYearOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.IsLeapMonth( 2006, 1, 1 ) );
            Assert.Equal( "year", ex.ParamName );
        }

        [Fact( DisplayName = "is leap month should not allow month out of range" )]
        public void IsLeapMonthShouldNotAllowMonthOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.IsLeapMonth( 2007, 5, 1 ) );
            Assert.Equal( "month", ex.ParamName );
        }

        [Fact( DisplayName = "is leap year should not allow year out of range" )]
        public void IsLeapYearShouldNotAllowYearOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.IsLeapYear( 2006, 1 ) );
            Assert.Equal( "year", ex.ParamName );
        }

        [Fact( DisplayName = "to date time should not allow year out of range" )]
        public void ToDateTimeShouldNotAllowYearOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.ToDateTime( 2006, 1, 1, 0, 0, 0, 0 ) );
            Assert.Equal( "year", ex.ParamName );
        }

        [Fact( DisplayName = "to date time should not allow month out of range" )]
        public void ToDateTimeShouldNotAllowMonthOutOfRange()
        {
            var target = CreateCalendar();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.ToDateTime( 2007, 5, 1, 0, 0, 0, 0 ) );
            Assert.Equal( "month", ex.ParamName );
        }

        [Fact( DisplayName = "to date time should return correct result" )]
        public void ToDateTimeShouldReturnExpectedValue()
        {
            var expected = new DateTime( 2007, 6, 30 );
            var target = CreateCalendar();
            var actual = target.ToDateTime( 2008, 1, 1, 0, 0, 0, 0 );

            // first day on the fiscal calendar should be 6/30/07
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "4-4-5 calendar to string should return expected text" )]
        public void ToStringShouldReturnExpectedResult()
        {
            var target = CreateCalendar();
            var expected = string.Format( CultureInfo.CurrentCulture, "MinSupportedDateTime = {0:d}, MaxSupportedDateTime = {1:d}", target.MinSupportedDateTime, target.MaxSupportedDateTime );
            var actual = target.ToString();
            Assert.Equal( expected, actual );
        }
    }
}
