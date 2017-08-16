namespace System
{
    using FluentAssertions;
    using More.Globalization;
    using System;
    using System.Globalization;
    using Xunit;
    using static System.DayOfWeek;
    using static System.DateTime;
    using static System.Globalization.CultureInfo;
    using static System.TimeSpan;

    public class DateTimeExtensionsTest
    {
        [Fact]
        public void first_day_of_week_in_month_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 4 );

            // act
            var actual = date.FirstDayOfWeekInMonth( Monday );

            // assert
            actual.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void last_day_of_week_in_month_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 29 );

            // act
            var actual = date.LastDayOfWeekInMonth( Friday );

            // assert
            actual.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void day_of_week_in_month_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 11 );

            // act
            var actual = date.DayOfWeekInMonth( 2, Monday );

            // assert
            actual.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void start_of_day_should_return_expected_result()
        {
            // arrange
            var now = Now;
            var expected = now.Date;

            // act
            var startOfDay = now.StartOfDay();

            // assert
            startOfDay.Should().Be( expected );
        }

        [Fact]
        public void start_of_week_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 2, 24 );

            // act
            var startOfWeek = date.StartOfWeek();

            // assert
            startOfWeek.Date.Should().Be( expected.Date );
        }

        [Theory]
        [InlineData( Sunday, "02-24-2013" )]
        [InlineData( Monday, "02-25-2013" )]
        public void start_of_week_with_custom_day_should_return_expected_result( DayOfWeek dayOfWeek, string expectedDate )
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = DateTime.Parse( expectedDate );

            // act
            var startOfWeek = date.StartOfWeek( dayOfWeek );

            // assert
            startOfWeek.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void start_of_month_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 26 );
            var expected = new DateTime( 2013, 3, 1 );

            // act
            var startOfMonth = date.StartOfMonth();

            // assert
            startOfMonth.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void start_of_quarter_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 1, 1 );

            // act
            var startOfQuarter = date.StartOfQuarter();

            // assert
            startOfQuarter.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void start_of_semester_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 1, 1 );

            // act
            var startOfSemester = date.StartOfSemester();

            // assert
            startOfSemester.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void start_of_year_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 1, 1 );

            // act
            var startOfYear = date.StartOfYear();

            // assert
            startOfYear.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void end_of_day_should_return_expected_result()
        {
            // arrange
            var date = Now;
            var expected = Today.AddDays( 1d ).Subtract( FromTicks( 1L ) );

            // act
            var endOfDay = date.EndOfDay();

            // assert
            endOfDay.Should().Be( expected );
        }

        [Fact]
        public void end_of_week_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 2 );

            // act
            var endOfWeek = date.EndOfWeek();

            // assert
            endOfWeek.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void end_of_month_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 31 );

            // act
            var endOfMonth = date.EndOfMonth();

            // assert
            endOfMonth.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void end_of_quarter_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 3, 31 );

            // act
            var endOfQuarter = date.EndOfQuarter();

            // assert
            endOfQuarter.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void end_of_semester_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 6, 30 );

            // act
            var endOfSemester = date.EndOfSemester();

            // assert
            endOfSemester.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void end_of_year_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var expected = new DateTime( 2013, 12, 31 );

            // act
            var endOfYear = date.EndOfYear();

            // assert
            endOfYear.Date.Should().Be( expected.Date );
        }

        [Fact]
        public void week__should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );

            // act
            var week = date.Week();

            // assert
            week.Should().Be( 9 );
        }

        [Fact]
        public void quarter_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 4, 1 );

            // act
            var quarter = date.Quarter();

            // assert
            quarter.Should().Be( 2 );
        }

        [Fact]
        public void semester_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 4, 1 );

            // act
            var semester = date.Semester();

            // assert
            semester.Should().Be( 1 );
        }

        [Fact]
        public void year_should_return_expected_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );

            // act
            var year = date.Year();

            // assert
            year.Should().Be( 2013 );
        }

        [Theory]
        [InlineData( 2013, true )]
        [InlineData( 10000, false )]
        public void is_representable_should_return_expected_result_for_date( int year, bool expected )
        {
            // arrange
            var date = Today;
            var calendar = CurrentCulture.Calendar;

            // act
            var result = date.IsRepresentable( calendar, year, 1, 1 );

            // assert
            result.Should().Be( expected );
        }
    }
}