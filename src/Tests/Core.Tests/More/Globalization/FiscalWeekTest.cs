namespace More.Globalization
{
    using System;
    using System.Globalization;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="FiscalWeek"/>.
    /// </summary>
    public class FiscalWeekTest
    {
        [Fact( DisplayName = "new fiscal week should set first and last day" )]
        public void ConstructorShouldSetFirstAndLastDay()
        {
            var firstDay = new DateTime( 2013, 6, 23 );
            var lastDay = new DateTime( 2013, 6, 29 );
            var target = new FiscalWeek( firstDay, lastDay );
            Assert.Equal( firstDay, target.FirstDay );
            Assert.Equal( lastDay, target.LastDay );
        }

        [Fact( DisplayName = "new fiscal week should derive last day from first day" )]
        public void ConstructorShouldDeriveLastDayFromFirstDay()
        {
            var firstDay = new DateTime( 2013, 6, 23 );
            var expected = new DateTime( 2013, 6, 29 );
            var target = new FiscalWeek( firstDay );
            var actual = target.LastDay;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new fiscal week should require 7 day date range" )]
        public void ConstructorShouldNotAllowDateRangeOtherThanSevenDays()
        {
            var day1 = new DateTime( 2013, 6, 23 );
            var day2 = new DateTime( 2013, 6, 28 );

            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => new FiscalWeek( day2, day1 ) );
            Assert.Equal( "firstDay", ex.ParamName );

            ex = Assert.Throws<ArgumentOutOfRangeException>( () => new FiscalWeek( day1, day2 ) );
            Assert.Equal( "lastDay", ex.ParamName );
        }

        [Fact( DisplayName = "fiscal week to string should return expected text" )]
        public void ToStringShouldReturnExpectedResult()
        {
            var target = new FiscalWeek( new DateTime( 2013, 6, 23 ) );
            var expected = string.Format( CultureInfo.CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", target.FirstDay, target.LastDay );
            var actual = target.ToString();
            Assert.Equal( expected, actual );
        }
    }
}
