namespace More.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="GregorianFiscalCalendar"/>.
    /// </summary>
    public class GregorianFiscalCalendarTest
    {
        [Fact]
        public void new_gregorian_fiscal_calendar_should_not_allow_invalid_epoch()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => new GregorianFiscalCalendar( 0 ) );
            Assert.Equal( "epochMonth", ex.ParamName );

            ex = Assert.Throws<ArgumentOutOfRangeException>( () => new GregorianFiscalCalendar( 13 ) );
            Assert.Equal( "epochMonth", ex.ParamName );
        }

        [Fact]
        public void new_gregorian_fiscal_calendar_should_set_expected_epoch()
        {
            Assert.Equal( 7, new GregorianFiscalCalendar( 7 ).EpochMonth );
        }

        [Fact]
        public void get_day_of_year_should_return_correct_day()
        {
            var target = new GregorianFiscalCalendar( 7 );

            Assert.Equal( 1, target.GetDayOfYear( new DateTime( 2010, 7, 1 ) ) );
            Assert.Equal( 23, target.GetDayOfYear( new DateTime( 2010, 7, 23 ) ) );
            Assert.Equal( 32, target.GetDayOfYear( new DateTime( 2010, 8, 1 ) ) );
            Assert.Equal( 365, target.GetDayOfYear( new DateTime( 2010, 6, 30 ) ) );
            Assert.Equal( 365, target.GetDayOfYear( new DateTime( 2011, 6, 30 ) ) );
        }

        [Fact]
        public void get_days_in_month_should_return_correct_result()
        {
            var target = new GregorianFiscalCalendar( 7 );
            var calendar = new GregorianCalendar();

            Assert.Equal( calendar.GetDaysInMonth( 2010, 7 ), target.GetDaysInMonth( 2011, 1 ) );
            Assert.Equal( calendar.GetDaysInMonth( 2010, 8 ), target.GetDaysInMonth( 2011, 2 ) );
            Assert.Equal( calendar.GetDaysInMonth( 2010, 6 ), target.GetDaysInMonth( 2010, 12 ) );
            Assert.Equal( calendar.GetDaysInMonth( 2011, 6 ), target.GetDaysInMonth( 2011, 12 ) );
        }

        [Fact]
        public void get_days_in_year_should_return_correct_result()
        {
            var target = new GregorianFiscalCalendar( 7 );
            var calendar = new GregorianCalendar();

            for ( var year = 1998; year <= 2030; year++ )
                Assert.Equal( calendar.GetDaysInYear( year ), target.GetDaysInYear( year ) );
        }

        [Fact]
        public void get_month_should_return_correct_offset()
        {
            var target = new GregorianFiscalCalendar( 7 );
            var mappings = new List<Tuple<int, int>>()
            {
                new Tuple<int,int>( 7, 1 ),
                new Tuple<int,int>( 8, 2 ),
                new Tuple<int,int>( 9, 3 ),
                new Tuple<int,int>( 10, 4 ),
                new Tuple<int,int>( 11, 5 ),
                new Tuple<int,int>( 12, 6 ),
                new Tuple<int,int>( 1, 7 ),
                new Tuple<int,int>( 2, 8 ),
                new Tuple<int,int>( 3, 9 ),
                new Tuple<int,int>( 4, 10 ),
                new Tuple<int,int>( 5, 11 ),
                new Tuple<int,int>( 6, 12 ),
            };

            for ( var year = 2009; year <= 2012; year++ )
            {
                foreach ( var mapping in mappings )
                    Assert.Equal( mapping.Item2, target.GetMonth( new DateTime( year, mapping.Item1, 1 ) ) );
            }
        }

        [Fact]
        public void get_year_should_return_correct_result()
        {
            var target = new GregorianFiscalCalendar( 7 );

            Assert.Equal( 2011, target.GetYear( new DateTime( 2010, 7, 1 ) ) );
            Assert.Equal( 2011, target.GetYear( new DateTime( 2010, 7, 23 ) ) );
            Assert.Equal( 2011, target.GetYear( new DateTime( 2010, 8, 1 ) ) );
            Assert.Equal( 2010, target.GetYear( new DateTime( 2010, 6, 30 ) ) );
            Assert.Equal( 2011, target.GetYear( new DateTime( 2011, 6, 30 ) ) );
            Assert.Equal( 2012, target.GetYear( new DateTime( 2011, 7, 1 ) ) );
        }

        [Fact]
        public void is_leap_year_should_return_correct_result()
        {
            var target = new GregorianFiscalCalendar( 7 );
            var calendar = new GregorianCalendar();

            for ( var year = 2005; year <= 2015; year++ )
                Assert.Equal( calendar.IsLeapYear( year - 1 ) | calendar.IsLeapYear( year ), target.IsLeapYear( year ) );
        }

        [Fact]
        public void to_date_time_should_convert_from_fiscal_to_calendar_date_time()
        {
            var target = new GregorianFiscalCalendar( 7 );

            Assert.Equal( new DateTime( 2010, 7, 1 ), target.ToDateTime( 2011, 1, 1, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2010, 7, 23 ), target.ToDateTime( 2011, 1, 23, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2010, 8, 1 ), target.ToDateTime( 2011, 2, 1, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2010, 6, 30 ), target.ToDateTime( 2010, 12, 30, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2011, 6, 30 ), target.ToDateTime( 2011, 12, 30, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2011, 7, 1 ), target.ToDateTime( 2012, 1, 1, 0, 0, 0, 0 ) );
        }

        [Fact]
        public void get_week_of_year_should_return_correct_result()
        {
            var target = new GregorianFiscalCalendar( 7 );

            Assert.Equal( 1, target.GetWeekOfYear( new DateTime( 2010, 7, 1 ), CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) );
            Assert.Equal( 4, target.GetWeekOfYear( new DateTime( 2010, 7, 23 ), CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) );
            Assert.Equal( 3, target.GetWeekOfYear( new DateTime( 2010, 7, 23 ), CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday ) );
            Assert.Equal( 6, target.GetWeekOfYear( new DateTime( 2010, 8, 1 ), CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) );
            Assert.True( new[] { 52, 53 }.Contains( target.GetWeekOfYear( new DateTime( 2010, 6, 30 ), CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) ) );
            Assert.True( new[] { 52, 53 }.Contains( target.GetWeekOfYear( new DateTime( 2011, 6, 30 ), CalendarWeekRule.FirstDay, DayOfWeek.Sunday ) ) );
        }
    }
}
