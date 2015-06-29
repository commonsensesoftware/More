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
