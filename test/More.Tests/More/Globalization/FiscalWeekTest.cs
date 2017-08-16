namespace More.Globalization
{
    using FluentAssertions;
    using System;
    using Xunit;
    using static System.DateTime;
    using static System.Globalization.CultureInfo;
    using static System.String;

    public class FiscalWeekTest
    {
        [Fact]
        public void new_fiscal_week_should_set_first_and_last_day()
        {
            // arrange
            var firstDay = new DateTime( 2013, 6, 23 );
            var lastDay = new DateTime( 2013, 6, 29 );

            // act
            var week = new FiscalWeek( firstDay, lastDay );

            // assert
            week.ShouldBeEquivalentTo( new { FirstDay = firstDay, LastDay = lastDay } );
        }

        [Fact]
        public void new_fiscal_week_should_derive_last_day_from_first_day()
        {
            // arrange
            var firstDay = new DateTime( 2013, 6, 23 );
            var expected = new DateTime( 2013, 6, 29 );

            // act
            var week = new FiscalWeek( firstDay );

            // assert
            week.LastDay.Should().Be( expected );
        }

        [Theory]
        [InlineData( "06-28-2013", "06-23-2013", "firstDay" )]
        [InlineData( "06-23-2013", "06-28-2013", "lastDay" )]
        public void new_fiscal_week_should_require_7_day_date_range( string date1, string date2, string paramName )
        {
            // arrange
            var firstDay = Parse( date1 );
            var lastDay = Parse( date2 );

            // act
            Action @new = () => new FiscalWeek( firstDay, lastDay );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( paramName );
        }

        [Fact]
        public void fiscal_week_to_string_should_return_expected_text()
        {
            // arrange
            var week = new FiscalWeek( new DateTime( 2013, 6, 23 ) );
            var expected = Format( CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", week.FirstDay, week.LastDay );

            // act
            var text = week.ToString();

            // assert
            text.Should().Be( expected );
        }
    }
}