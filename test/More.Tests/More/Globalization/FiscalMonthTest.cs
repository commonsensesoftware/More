namespace More.Globalization
{
    using FluentAssertions;
    using System;
    using Xunit;
    using static System.DateTime;
    using static System.Globalization.CultureInfo;
    using static System.String;

    public class FiscalMonthTest
    {
        [Fact]
        public void new_fiscal_month_first_and_last_day_should_default_to_today()
        {
            // arrange

            // act
            var month = new FiscalMonth();

            // assert
            month.ShouldBeEquivalentTo( new { FirstDay = Today, LastDay = Today }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void new_fiscal_month_should_set_weeks()
        {
            // arrange
            var weeks = new[] { new FiscalWeek( new DateTime( 2013, 6, 23 ) ) };

            // act
            var month = new FiscalMonth( weeks );

            // assert
            month.Weeks.Should().Equal( weeks );
        }

        [Fact]
        public void first_dayX2C_last_dayX2C_and_days_in_months_should_be_based_on_weeks()
        {
            // arrange
            var firstDay = new DateTime( 2013, 6, 23 );
            var lastDay = new DateTime( 2013, 6, 29 );
            var weeks = new[] { new FiscalWeek( firstDay ) };

            // act
            var month = new FiscalMonth( weeks );

            // assert
            month.ShouldBeEquivalentTo( new { FirstDay = firstDay, LastDay = lastDay, DaysInMonth = 7 }, o => o.Excluding( m => m.Weeks ) );
        }

        [Fact]
        public void fiscal_month_to_string_should_return_expected_text()
        {
            // arrange
            var weeks = new[] { new FiscalWeek( new DateTime( 2013, 6, 23 ) ) };
            var month = new FiscalMonth( weeks );
            var expected = Format( CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", month.FirstDay, month.LastDay );

            // act
            var text = month.ToString();

            // assert
            text.Should().Be( expected );
        }
    }
}