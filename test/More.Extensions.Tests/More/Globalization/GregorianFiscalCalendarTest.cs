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
        [Fact( DisplayName = "new gregorian fiscal calendar should not allow invalid epoch" )]
        public void ConstructorsShouldNotAcceptInvalidEpochMonth()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => new GregorianFiscalCalendar( 0 ) );
            Assert.Equal( "epochMonth", ex.ParamName );

            ex = Assert.Throws<ArgumentOutOfRangeException>( () => new GregorianFiscalCalendar( 13 ) );
            Assert.Equal( "epochMonth", ex.ParamName );
        }

        [Fact( DisplayName = "new gregorian fiscal calendar should set expected epoch" )]
        public void EpochMonthPropertyShouldEqualConstructorParameter()
        {
            Assert.Equal( 7, new GregorianFiscalCalendar( 7 ).EpochMonth );
        }

        [Fact( DisplayName = "get day of year should return correct day" )]
        public void GetDayOfYearShouldReturnCorrectDay()
        {
            var target = new GregorianFiscalCalendar( 7 );

            Assert.Equal( 1, target.GetDayOfYear( new DateTime( 2010, 7, 1 ) ) );
            Assert.Equal( 23, target.GetDayOfYear( new DateTime( 2010, 7, 23 ) ) );
            Assert.Equal( 32, target.GetDayOfYear( new DateTime( 2010, 8, 1 ) ) );
            Assert.Equal( 365, target.GetDayOfYear( new DateTime( 2010, 6, 30 ) ) );
            Assert.Equal( 365, target.GetDayOfYear( new DateTime( 2011, 6, 30 ) ) );
        }

        [Fact( DisplayName = "get days in month should return correct result" )]
        public void GetDaysInMonthShouldReturnCorrectCount()
        {
            var target = new GregorianFiscalCalendar( 7 );
            var calendar = new GregorianCalendar();

            Assert.Equal( calendar.GetDaysInMonth( 2010, 7 ), target.GetDaysInMonth( 2011, 1 ) );
            Assert.Equal( calendar.GetDaysInMonth( 2010, 8 ), target.GetDaysInMonth( 2011, 2 ) );
            Assert.Equal( calendar.GetDaysInMonth( 2010, 6 ), target.GetDaysInMonth( 2010, 12 ) );
            Assert.Equal( calendar.GetDaysInMonth( 2011, 6 ), target.GetDaysInMonth( 2011, 12 ) );
        }

        [Fact( DisplayName = "get days in year should return correct result" )]
        public void GetDaysInYearShouldReturnCorrectCount()
        {
            var target = new GregorianFiscalCalendar( 7 );
            var calendar = new GregorianCalendar();

            for ( var year = 1998; year <= 2030; year++ )
                Assert.Equal( calendar.GetDaysInYear( year ), target.GetDaysInYear( year ) );
        }

        [Fact( DisplayName = "get month should return correct offset" )]
        public void GetMonthShouldReturnCorrectOffset()
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

        [Fact( DisplayName = "get year should return correct result" )]
        public void GetYearShouldReturnCorrectFiscalYear()
        {
            var target = new GregorianFiscalCalendar( 7 );

            Assert.Equal( 2011, target.GetYear( new DateTime( 2010, 7, 1 ) ) );
            Assert.Equal( 2011, target.GetYear( new DateTime( 2010, 7, 23 ) ) );
            Assert.Equal( 2011, target.GetYear( new DateTime( 2010, 8, 1 ) ) );
            Assert.Equal( 2010, target.GetYear( new DateTime( 2010, 6, 30 ) ) );
            Assert.Equal( 2011, target.GetYear( new DateTime( 2011, 6, 30 ) ) );
            Assert.Equal( 2012, target.GetYear( new DateTime( 2011, 7, 1 ) ) );
        }

        [Fact( DisplayName = "is leap year should return correct result" )]
        public void IsLeapYearShouldReturnCorrectValueForFiscalYear()
        {
            var target = new GregorianFiscalCalendar( 7 );
            var calendar = new GregorianCalendar();

            for ( var year = 2005; year <= 2015; year++ )
                Assert.Equal( calendar.IsLeapYear( year - 1 ) | calendar.IsLeapYear( year ), target.IsLeapYear( year ) );
        }

        [Fact( DisplayName = "to date time should convert from fiscal to calendar date time" )]
        public void ToDateTimeShouldConvertFromFiscalToCalendarDateTime()
        {
            var target = new GregorianFiscalCalendar( 7 );

            Assert.Equal( new DateTime( 2010, 7, 1 ), target.ToDateTime( 2011, 1, 1, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2010, 7, 23 ), target.ToDateTime( 2011, 1, 23, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2010, 8, 1 ), target.ToDateTime( 2011, 2, 1, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2010, 6, 30 ), target.ToDateTime( 2010, 12, 30, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2011, 6, 30 ), target.ToDateTime( 2011, 12, 30, 0, 0, 0, 0 ) );
            Assert.Equal( new DateTime( 2011, 7, 1 ), target.ToDateTime( 2012, 1, 1, 0, 0, 0, 0 ) );
        }

        [Fact( DisplayName = "get week of year should return correct result" )]
        public void GetWeekOfYearShouldReturnCorrectValueForFiscalYear()
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
