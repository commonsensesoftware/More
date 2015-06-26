namespace More.Globalization
{
    using System;
    using System.Globalization;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="FiscalMonth"/>.
    /// </summary>
    public class FiscalMonthTest
    {
        [Fact( DisplayName = "new fiscal month first and last day should default to today" )]
        public void FirstAndLastDayPropertiesShouldDefaultToToday()
        {
            var target = new FiscalMonth();
            Assert.Equal( DateTime.Today, target.FirstDay );
            Assert.Equal( DateTime.Today, target.LastDay );
        }

        [Fact( DisplayName = "new fiscal month should set weeks" )]
        public void ConstructorShouldSetWeeks()
        {
            var weeks = new[] { new FiscalWeek( new DateTime( 2013, 6, 23 ) ) };
            var target = new FiscalMonth( weeks );
            Assert.Equal( 1, target.Weeks.Count );
        }

        [Fact( DisplayName = "first day, last day, and days in months should be based on weeks" )]
        public void FirstDayLastDayAndDaysInMonthPropertiesShouldBeBasedOnSuppliedWeeks()
        {
            var firstDay = new DateTime( 2013, 6, 23 );
            var lastDay = new DateTime( 2013, 6, 29 );
            var weeks = new[] { new FiscalWeek( firstDay ) };
            var target = new FiscalMonth( weeks );

            Assert.Equal( firstDay, target.FirstDay );
            Assert.Equal( lastDay, target.LastDay );
            Assert.Equal( 7, target.DaysInMonth );
        }

        [Fact( DisplayName = "fiscal month to string should return expected text" )]
        public void ToStringShouldReturnExpectedResult()
        {
            var weeks = new[] { new FiscalWeek( new DateTime( 2013, 6, 23 ) ) };
            var target = new FiscalMonth( weeks );
            var expected = string.Format( CultureInfo.CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", target.FirstDay, target.LastDay );
            var actual = target.ToString();
            Assert.Equal( expected, actual );
        }
    }
}
