namespace More.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="FiscalYear"/>.
    /// </summary>
    public class FiscalYearTest
    {
        [Fact( DisplayName = "new fiscal year first and last day should default to today" )]
        public void FirstAndLastDayPropertiesShouldDefaultToToday()
        {
            var target = new FiscalYear();
            Assert.Equal( DateTime.Today, target.FirstDay );
            Assert.Equal( DateTime.Today, target.LastDay );
        }

        [Fact( DisplayName = "new fiscal year should set months" )]
        public void ConstructorShouldSetMonths()
        {
            var weeks = new[] { new FiscalWeek( new DateTime( 2013, 6, 23 ) ) };
            var months = new Dictionary<int, FiscalMonth>() { { 1, new FiscalMonth( weeks ) } };
            var target = new FiscalYear( months );
            Assert.Equal( 1, target.Months.Count );
        }

        [Fact( DisplayName = "first day, last day, and days in year should be based on months" )]
        public void FirstDayLastDayAndDaysInYearPropertiesShouldBeBasedOnSuppliedMonths()
        {
            var firstDay = new DateTime( 2013, 6, 23 );
            var lastDay = new DateTime( 2013, 6, 29 );
            var weeks = new[] { new FiscalWeek( firstDay ) };
            var months = new Dictionary<int, FiscalMonth>() { { 1, new FiscalMonth( weeks ) } };
            var target = new FiscalYear( months );

            Assert.Equal( firstDay, target.FirstDay );
            Assert.Equal( lastDay, target.LastDay );
            Assert.Equal( 7, target.DaysInYear );
        }

        [Fact( DisplayName = "fiscal year to string should return expected text" )]
        public void ToStringShouldReturnExpectedResult()
        {
            var weeks = new[] { new FiscalWeek( new DateTime( 2013, 6, 23 ) ) };
            var months = new Dictionary<int, FiscalMonth>() { { 1, new FiscalMonth( weeks ) } };
            var target = new FiscalYear( months );
            var expected = string.Format( CultureInfo.CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", target.FirstDay, target.LastDay );
            var actual = target.ToString();
            Assert.Equal( expected, actual );
        }
    }
}
