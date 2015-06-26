namespace System
{
    using More.Globalization;
    using System;
    using System.Globalization;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="DateTimeExtensions"/>.
    /// </summary>
    public class DateTimeExtensionsTest
    {
        [Fact( DisplayName = "first day of week in month should return expected result" )]
        public void FirstDayOfWeekInMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 4 );
            var actual = date.FirstDayOfWeekInMonth( DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "last day of week in month should return expected result" )]
        public void LastDayOfWeekInMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 29 );
            var actual = date.LastDayOfWeekInMonth( DayOfWeek.Friday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "day of week in month should return expected result" )]
        public void DayOfWeekInMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 11 );
            var actual = date.DayOfWeekInMonth( 2, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of day should return expected result" )]
        public void StartOfDayShouldReturnExpectedResult()
        {
            var now = DateTime.Now;
            var expected = now.Date;
            var actual = now.StartOfDay();
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "start of week should return expected result" )]
        public void StartOfWeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 2, 24 );
            var actual = date.StartOfWeek();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of week with custom day should return expected result" )]
        public void StartOfWeekWithCustomDayOfWeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 2, 24 );
            var actual = date.StartOfWeek( DayOfWeek.Sunday );
            Assert.Equal( expected.Date, actual.Date );

            expected = new DateTime( 2013, 2, 25 );
            actual = date.StartOfWeek( DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of month should return expected result" )]
        public void StartOfMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 26 );
            var expected = new DateTime( 2013, 3, 1 );
            var actual = date.StartOfMonth();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of quarter should return expected result" )]
        public void StartOfQuarterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = date.StartOfQuarter();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of semester should return expected result" )]
        public void StartOfSemesterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = date.StartOfSemester();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of year should return expected result" )]
        public void StartOfYearShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = date.StartOfYear();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of day should return expected result" )]
        public void EndOfDayShouldReturnExpectedResult()
        {
            var date = DateTime.Now;
            var expected = DateTime.Today.AddDays( 1d ).Subtract( TimeSpan.FromTicks( 1L ) );
            var actual = date.EndOfDay();
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "end of week should return expected result" )]
        public void EndOfWeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 2 );
            var actual = date.EndOfWeek();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of month should return expected result" )]
        public void EndOfMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = date.EndOfMonth();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of quarter should return expected result" )]
        public void EndOfQuarterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = date.EndOfQuarter();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of semester should return expected result" )]
        public void EndOfSemesterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 6, 30 );
            var actual = date.EndOfSemester();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of year should return expected result" )]
        public void EndOfYearShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 12, 31 );
            var actual = date.EndOfYear();
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "week  should return expected result" )]
        public void WeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var actual = date.Week();
            Assert.Equal( 9, actual );
        }

        [Fact( DisplayName = "quarter should return expected result" )]
        public void QuarterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 4, 1 );
            var actual = date.Quarter();
            Assert.Equal( 2, actual );
        }

        [Fact( DisplayName = "semester should return expected result" )]
        public void SemesterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 4, 1 );
            var actual = date.Semester();
            Assert.Equal( 1, actual );
        }

        [Fact( DisplayName = "year should return expected result" )]
        public void YearShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var actual = date.Year();
            Assert.Equal( 2013, actual );
        }

        [Fact( DisplayName = "is representable should return true for valid date" )]
        public void IsRepresentableShouldReturnTrueForValidDate()
        {
            var date = DateTime.Today;
            var calendar = CultureInfo.CurrentCulture.Calendar;
            Assert.True( date.IsRepresentable( calendar, 2013, 1, 1 ) );
        }

        [Fact( DisplayName = "is representable should return false for invalid date" )]
        public void IsRepresentableShouldReturnFalseForInvalidDate()
        {
            var date = DateTime.Today;
            var calendar = CultureInfo.CurrentCulture.Calendar;
            Assert.False( date.IsRepresentable( calendar, 10000, 1, 1 ) );
        }

        [Fact( DisplayName = "first day of week in month with calendar should return expected result" )]
        public void FirstDayOfWeekInMonthWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 4 );
            var actual = date.FirstDayOfWeekInMonth( DayOfWeek.Monday, calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "last day of week in month with calendar should return expected result" )]
        public void LastDayOfWeekInMonthWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 29 );
            var actual = date.LastDayOfWeekInMonth( DayOfWeek.Friday, calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "day of week with calendar should return expected result" )]
        public void DayOfWeekInMonthWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 11 );
            var actual = date.DayOfWeekInMonth( 2, DayOfWeek.Monday, calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of month with calendar should return expected result" )]
        public void StartOfMonthWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 26 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 1 );
            var actual = date.StartOfMonth( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of quarter with calendar should return expected result" )]
        public void StartOfQuarterWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = date.StartOfQuarter( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of semester with calendar should return expected result" )]
        public void StartOfSemesterWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = date.StartOfSemester( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of year with calendar should return expected result" )]
        public void StartOfYearWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2012, 7, 1 ); // because fiscal calendar starts in July
            var actual = date.StartOfYear( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of month with calendar should return expected result" )]
        public void EndOfMonthWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = date.EndOfMonth( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of quarter with calendar should return expected result" )]
        public void EndOfQuarterWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = date.EndOfQuarter( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of semester with calendar should return expected result" )]
        public void EndOfSemesterWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 ); // because fiscal calendar starts in July
            var actual = date.EndOfSemester( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of year with calendar should return expected result" )]
        public void EndOfYearWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 ); // because fiscal calendar starts in July
            var actual = date.EndOfYear( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "week with calendar should return expected result" )]
        public void WeekWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Week( calendar, DayOfWeek.Sunday );
            Assert.Equal( 35, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "month with calendar should return expected result" )]
        public void MonthWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 6, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Month( calendar );
            Assert.Equal( 12, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "quarter with calendar should return expected result" )]
        public void QuarterWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Quarter( calendar );
            Assert.Equal( 3, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "semester with calendar should return expected result" )]
        public void SemesterWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Semester( calendar );
            Assert.Equal( 2, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "year with calendar should return expected result" )]
        public void YearWithCalendarShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 7, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Year( calendar );
            Assert.Equal( 2014, actual ); // because fiscal calendar starts in July
        }
    }
}
