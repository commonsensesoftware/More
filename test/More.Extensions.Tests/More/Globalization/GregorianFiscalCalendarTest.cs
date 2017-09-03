namespace More.Globalization
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xunit;
    using static System.Globalization.CalendarWeekRule;
    using static System.DayOfWeek;

    public class GregorianFiscalCalendarTest
    {
        [Theory]
        [InlineData( 0 )]
        [InlineData( 13 )]
        public void new_gregorian_fiscal_calendar_should_not_allow_invalid_epoch( int epochMonth )
        {
            // arrange

            // act
            Action @new = () => new GregorianFiscalCalendar( epochMonth );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( epochMonth ) );
        }

        [Fact]
        public void new_gregorian_fiscal_calendar_should_set_expected_epoch()
        {
            // arrange
            const int July = 7;

            // act
            var calendar = new GregorianFiscalCalendar( 7 );

            // assert
            calendar.EpochMonth.Should().Be( July );
        }

        [Theory]
        [InlineData( 2010, 7, 1, 1 )]
        [InlineData( 2010, 7, 23, 23 )]
        [InlineData( 2010, 8, 1, 32 )]
        [InlineData( 2010, 6, 30, 365 )]
        [InlineData( 2011, 6, 30, 365 )]
        public void get_day_of_year_should_return_correct_day( int year, int month, int day, int expected )
        {
            // arrange
            var date = new DateTime( year, month, day );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var dayOfYear = calendar.GetDayOfYear( date );

            // assert
            dayOfYear.Should().Be( expected );
        }

        [Theory]
        [InlineData( 2010, 7, 2011, 1 )]
        [InlineData( 2010, 8, 2011, 2 )]
        [InlineData( 2010, 6, 2010, 12 )]
        [InlineData( 2011, 6, 2011, 12 )]
        public void get_days_in_month_should_return_correct_result( int fiscalYear, int fiscalMonth, int calendarYear, int calendarMonth )
        {
            // arrange
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new GregorianCalendar().GetDaysInMonth( calendarYear, calendarMonth );

            // act
            var daysInMonth = calendar.GetDaysInMonth( fiscalYear, fiscalMonth );

            // assert
            daysInMonth.Should().Be( expected );
        }

        [Fact]
        public void get_days_in_year_should_return_correct_result()
        {
            // arrange
            var fiscalCalendar = new GregorianFiscalCalendar( 7 );
            var calendar = new GregorianCalendar();

            for ( var year = 1998; year <= 2030; year++ )
            {
                var expected = calendar.GetDaysInYear( year );

                // act
                var daysInYear = fiscalCalendar.GetDaysInYear( year );

                // assert
                daysInYear.Should().Be( expected );
            }
        }

        [Theory]
        [InlineData( 7, 1 )]
        [InlineData( 8, 2 )]
        [InlineData( 9, 3 )]
        [InlineData( 10, 4 )]
        [InlineData( 11, 5 )]
        [InlineData( 12, 6 )]
        [InlineData( 1, 7 )]
        [InlineData( 2, 8 )]
        [InlineData( 3, 9 )]
        [InlineData( 4, 10 )]
        [InlineData( 5, 11 )]
        [InlineData( 6, 12 )]
        public void get_month_should_return_correct_offset( int calendarMonth, int fiscalMonth )
        {
            // arrange
            var date = new DateTime( 2009, calendarMonth, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var month = calendar.GetMonth( date );

            // assert
            month.Should().Be( fiscalMonth );
        }

        [Theory]
        [InlineData( 2010, 7, 1, 2011 )]
        [InlineData( 2010, 7, 23, 2011 )]
        [InlineData( 2010, 8, 1, 2011 )]
        [InlineData( 2010, 6, 30, 2010 )]
        [InlineData( 2011, 6, 30, 2011 )]
        [InlineData( 2011, 7, 1, 2012 )]
        public void get_year_should_return_correct_result( int year, int month, int day, int expected )
        {
            // arrange
            var date = new DateTime( year, month, day );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var fiscalYear = calendar.GetYear( date );

            // assert
            fiscalYear.Should().Be( expected );
        }

        [Theory]
        [InlineData( 2005 )]
        [InlineData( 2006 )]
        [InlineData( 2007 )]
        [InlineData( 2008 )]
        [InlineData( 2009 )]
        [InlineData( 2010 )]
        [InlineData( 2011 )]
        [InlineData( 2012 )]
        [InlineData( 2013 )]
        [InlineData( 2014 )]
        [InlineData( 2015 )]
        public void is_leap_year_should_return_correct_result( int year )
        {
            // arrange
            var fiscalCalendar = new GregorianFiscalCalendar( 7 );
            var calendar = new GregorianCalendar();
            var expected = calendar.IsLeapYear( year - 1 ) | calendar.IsLeapYear( year );

            // act
            var leapYear = fiscalCalendar.IsLeapYear( year );

            // assert
            leapYear.Should().Be( expected );
        }

        [Theory]
        [InlineData( "2010-07-01", 2011, 1, 1 )]
        [InlineData( "2010-07-23", 2011, 1, 23 )]
        [InlineData( "2010-08-01", 2011, 2, 1 )]
        [InlineData( "2010-06-30", 2010, 12, 30 )]
        [InlineData( "2011-06-30", 2011, 12, 30 )]
        [InlineData( "2011-07-01", 2012, 1, 1 )]
        public void to_date_time_should_convert_from_fiscal_to_calendar_date_time( string dateValue, int year, int month, int day )
        {
            // arrange
            var expected = DateTime.Parse( dateValue );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var date = calendar.ToDateTime( year, month, day, 0, 0, 0, 0 );

            // assert
            date.Should().Be( expected );
        }

        [Theory]
        [InlineData( 2010, 7, 1, FirstDay, Sunday, 1 )]
        [InlineData( 2010, 7, 23, FirstDay, Sunday, 4 )]
        [InlineData( 2010, 7, 23, FirstFullWeek, Sunday, 3 )]
        [InlineData( 2010, 8, 1, FirstDay, Sunday, 6 )]
        [InlineData( 2010, 6, 30, FirstDay, Sunday, 53 )]
        [InlineData( 2011, 6, 30, FirstDay, Sunday, 53 )]
        public void get_week_of_year_should_return_correct_result( int year, int month, int day, CalendarWeekRule rule, DayOfWeek firstDayOfWeek, int expected )
        {
            // arrange
            var date = new DateTime( year, month, day );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var weekOfYear = calendar.GetWeekOfYear( date, rule, firstDayOfWeek );

            // assert
            weekOfYear.Should().Be( expected );
        }
    }
}