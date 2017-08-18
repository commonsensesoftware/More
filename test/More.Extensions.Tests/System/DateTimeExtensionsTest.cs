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
        [Fact]
        public void first_day_of_week_in_month_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 4 );
            var actual = date.FirstDayOfWeekInMonth( DayOfWeek.Monday, calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void last_day_of_week_in_month_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 29 );
            var actual = date.LastDayOfWeekInMonth( DayOfWeek.Friday, calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void day_of_week_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 11 );
            var actual = date.DayOfWeekInMonth( 2, DayOfWeek.Monday, calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_month_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 26 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 1 );
            var actual = date.StartOfMonth( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_quarter_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = date.StartOfQuarter( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_semester_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = date.StartOfSemester( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_year_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2012, 7, 1 ); // because fiscal calendar starts in July
            var actual = date.StartOfYear( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_month_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = date.EndOfMonth( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_quarter_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = date.EndOfQuarter( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_semester_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 ); // because fiscal calendar starts in July
            var actual = date.EndOfSemester( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_year_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 ); // because fiscal calendar starts in July
            var actual = date.EndOfYear( calendar );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void week_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Week( calendar, DayOfWeek.Sunday );
            Assert.Equal( 35, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void month_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 6, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Month( calendar );
            Assert.Equal( 12, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void quarter_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Quarter( calendar );
            Assert.Equal( 3, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void semester_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Semester( calendar );
            Assert.Equal( 2, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void year_with_calendar_should_return_expected_result()
        {
            var date = new DateTime( 2013, 7, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = date.Year( calendar );
            Assert.Equal( 2014, actual ); // because fiscal calendar starts in July
        }
    }
}
