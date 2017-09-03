namespace System
{
    using FluentAssertions;
    using More.Globalization;
    using System;
    using System.Globalization;
    using Xunit;
    using static System.DayOfWeek;

    public class DateTimeExtensionsTest
    {
        [Fact]
        public void first_day_of_week_in_month_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 4 );

            // act
            var firstDayOfWeekInMonth = date.FirstDayOfWeekInMonth( Monday, FiscalCalendar ).Date;

            // assert
            firstDayOfWeekInMonth.Should().Be( expected );
        }

        [Fact]
        public void last_day_of_week_in_month_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 29 );

            // act
            var lastDayOfWeekInMonth = date.LastDayOfWeekInMonth( Friday, FiscalCalendar ).Date;

            // assert
            lastDayOfWeekInMonth.Should().Be( expected );
        }

        [Fact]
        public void day_of_week_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 11 );

            // act
            var dayOfWeekInMonth = date.DayOfWeekInMonth( 2, Monday, FiscalCalendar ).Date;

            // assert
            dayOfWeekInMonth.Should().Be( expected );
        }

        [Fact]
        public void start_of_month_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 26 );
            var expected = new DateTime( 2013, 3, 1 );

            // act
            var startOfMonth = date.StartOfMonth( FiscalCalendar ).Date;

            // assert
            startOfMonth.Should().Be( expected );
        }

        [Fact]
        public void start_of_quarter_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 1, 1 );

            // act
            var startOfQuarter = date.StartOfQuarter( FiscalCalendar ).Date;

            // assert
            startOfQuarter.Should().Be( expected );
        }

        [Fact]
        public void start_of_semester_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 1, 1 );

            // act
            var startOfSemester = date.StartOfSemester( FiscalCalendar ).Date;

            // assert
            startOfSemester.Should().Be( expected );
        }

        [Fact]
        public void start_of_year_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2012, 7, 1 );

            // act
            var startOfYear = date.StartOfYear( FiscalCalendar ).Date;

            // assert
            startOfYear.Should().Be( expected );
        }

        [Fact]
        public void end_of_month_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 31 );

            // act
            var endOfMonth = date.EndOfMonth( FiscalCalendar ).Date;

            // assert
            endOfMonth.Should().Be( expected );
        }

        [Fact]
        public void end_of_quarter_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 31 );

            // act
            var endOfQuarter = date.EndOfQuarter( FiscalCalendar ).Date;

            // assert
            endOfQuarter.Should().Be( expected );
        }

        [Fact]
        public void end_of_semester_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 6, 30 );

            // act
            var endOfSemester = date.EndOfSemester( FiscalCalendar ).Date;

            // assert
            endOfSemester.Should().Be( expected );
        }

        [Fact]
        public void end_of_year_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 6, 30 );

            // act
            var endOfYear = date.EndOfYear( FiscalCalendar ).Date;

            // assert
            endOfYear.Should().Be( expected );
        }

        [Fact]
        public void week_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );

            // act
            var week = date.Week( FiscalCalendar, Sunday );

            // assert
            week.Should().Be( 35 );
        }

        [Fact]
        public void month_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 6, 1 );

            // act
            var month = date.Month( FiscalCalendar );

            // assert
            month.Should().Be( 12 );
        }

        [Fact]
        public void quarter_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );

            // act
            var quarter = date.Quarter( FiscalCalendar );

            // assert
            quarter.Should().Be( 3 );
        }

        [Fact]
        public void semester_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );

            // act
            var semester = date.Semester( FiscalCalendar );

            // assert
            semester.Should().Be( 2 );
        }

        [Fact]
        public void year_with_calendar_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 7, 1 );

            // act
            var year = date.Year( FiscalCalendar );

            // assert
            year.Should().Be( 2014 );
        }

        Calendar FiscalCalendar { get; } = new GregorianFiscalCalendar( 7 );
    }
}