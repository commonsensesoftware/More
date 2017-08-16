namespace More.Globalization
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static System.DateTime;
    using static System.Globalization.CultureInfo;
    using static System.String;

    public class FiscalYearTest
    {
        [Fact]
        public void new_fiscal_year_first_and_last_day_should_default_to_today()
        {
            // arrange

            // act
            var year = new FiscalYear();

            // assert
            year.ShouldBeEquivalentTo( new { FirstDay = Today, LastDay = Today }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void new_fiscal_year_should_set_months()
        {
            // arrange
            var weeks = new[] { new FiscalWeek( new DateTime( 2013, 6, 23 ) ) };
            var months = new Dictionary<int, FiscalMonth>() { { 1, new FiscalMonth( weeks ) } };

            // act
            var year = new FiscalYear( months );

            // assert
            year.Months.Should().HaveCount( 1 );
        }

        [Fact]
        public void first_dayX2C_last_dayX2C_and_days_in_year_should_be_based_on_months()
        {
            // arrange
            var firstDay = new DateTime( 2013, 6, 23 );
            var lastDay = new DateTime( 2013, 6, 29 );
            var weeks = new[] { new FiscalWeek( firstDay ) };
            var months = new Dictionary<int, FiscalMonth>() { { 1, new FiscalMonth( weeks ) } };

            // act
            var year = new FiscalYear( months );

            // assert
            year.ShouldBeEquivalentTo( new { FirstDay = firstDay, LastDay = lastDay, DaysInYear = 7 }, o => o.Excluding( y => y.Months ) );
        }

        [Fact]
        public void fiscal_year_to_string_should_return_expected_text()
        {
            // arrange
            var weeks = new[] { new FiscalWeek( new DateTime( 2013, 6, 23 ) ) };
            var months = new Dictionary<int, FiscalMonth>() { { 1, new FiscalMonth( weeks ) } };
            var year = new FiscalYear( months );
            var expected = Format( CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", year.FirstDay, year.LastDay );

            // act
            var text = year.ToString();

            // assert
            text.Should().Be( expected );
        }
    }
}