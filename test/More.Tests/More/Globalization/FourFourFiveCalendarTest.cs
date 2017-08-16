namespace More.Globalization
{
    using FluentAssertions;
    using System;
    using Xunit;
    using static System.DayOfWeek;
    using static System.Globalization.CalendarWeekRule;
    using static System.Globalization.CultureInfo;
    using static System.String;

    public partial class FourFourFiveCalendarTest
    {
        FourFourFiveCalendar Calendar { get; } = CreateCalendar();

        [Fact]
        public void get_week_of_year_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action getWeekOfYear = () => Calendar.GetWeekOfYear( date );

            // assert
            getWeekOfYear.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void get_week_of_year_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2007, 7, 4 );

            // act
            var weekOfYear = Calendar.GetWeekOfYear( date );

            // assert
            weekOfYear.Should().Be( 1 );
        }

        [Fact]
        public void get_week_of_year_with_rule_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action getWeekOfYear = () => Calendar.GetWeekOfYear( date, FirstDay, Calendar.MinSupportedDateTime.DayOfWeek );

            // assert
            getWeekOfYear.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void get_week_of_year_with_rule_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2007, 7, 7 );

            // act
            var weekOfYear = Calendar.GetWeekOfYear( date, FirstDay, Calendar.MinSupportedDateTime.DayOfWeek );

            // assert
            weekOfYear.Should().Be( 2 );
        }

        [Fact]
        public void epoch_month_should_return_expected_value()
        {
            // arrange
            const int July = 7;
            ICalendarEpoch epoch = Calendar;

            // act
            var month = epoch.Month;

            // assert
            month.Should().Be( July );
        }

        [Fact]
        public void eras_should_always_return_single_era()
        {
            // arrange

            // act
            var eras = Calendar.Eras;

            // assert
            eras.Should().Equal( new[] { 1 } );
        }

        [Fact]
        public void add_months_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action addMonths = () => Calendar.AddMonths( date, 1 );

            // assert
            addMonths.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void add_months_with_positive_value_should_return_correct_result()
        {
            // arrange
            var expected = new DateTime( 2007, 12, 29 );
            var date = new DateTime( 2007, 6, 30 );

            // act
            var result = Calendar.AddMonths( date, 6 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void add_months_with_negative_value_should_return_correct_result()
        {
            // assert
            var expected = new DateTime( 2007, 6, 30 );
            var date = new DateTime( 2007, 12, 29 );

            // act
            var result = Calendar.AddMonths( date, -6 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void add_years_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action addYears = () => Calendar.AddYears( date, 1 );

            // assert
            addYears.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void add_years_with_positive_value_should_return_correct_result()
        {
            // arrange
            var expected = new DateTime( 2008, 6, 28 );
            var date = new DateTime( 2007, 6, 30 );

            // act
            var result = Calendar.AddYears( date, 1 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void add_years_with_negative_value_should_return_correct_result()
        {
            // arrange
            var expected = new DateTime( 2007, 6, 30 );
            var date = new DateTime( 2008, 6, 28 );

            // act
            var result = Calendar.AddYears( date, -1 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void get_day_of_month_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2006, 1, 1 );

            // act
            Action getDayOfMonth = () => Calendar.GetDayOfMonth( date );

            // assert
            getDayOfMonth.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void get_day_of_month_should_return_correct_result()
        {
            // NOTE: using 4-4-5, first day of month could be another month

            // arrange
            var firstDay = new DateTime( 2007, 6, 30 );

            // act
            var day = Calendar.GetDayOfMonth( firstDay );

            // assert
            day.Should().Be( 1 );
        }

        [Fact]
        public void get_day_of_week_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action getDayOfWeek = () => Calendar.GetDayOfWeek( date );

            // assert
            getDayOfWeek.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void get_day_of_week_should_return_correct_result()
        {
            // arrange
            var firstDay = new DateTime( 2007, 6, 30 );

            // act
            var dayOfWeek = Calendar.GetDayOfWeek( firstDay );

            // assert
            dayOfWeek.Should().Be( Saturday );
        }

        [Fact]
        public void get_day_of_eyar_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action getDayOfYear = () => Calendar.GetDayOfYear( date );

            // assert
            getDayOfYear.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void get_day_of_year_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2007, 6, 30 );

            // act
            var day = Calendar.GetDayOfYear( date );

            // assert
            day.Should().Be( 1 );
        }

        [Fact]
        public void get_days_in_month_should_not_allow_year_out_of_range()
        {
            // arrange
            const int year = 2006;

            // act
            Action getDaysInMonth = () => Calendar.GetDaysInMonth( year, 6, 1 );

            // assert
            getDaysInMonth.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( year ) );
        }

        [Fact]
        public void get_days_in_month_should_not_allow_month_out_of_range()
        {
            // arrange
            const int month = 5;

            // act
            Action getDaysInMonth = () => Calendar.GetDaysInMonth( 2007, month, 1 );

            // assert
            getDaysInMonth.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( month ) );
        }

        [Fact]
        public void get_days_in_month_should_return_correct_result()
        {
            // arrange
            var expected = 28;

            // act
            var days = Calendar.GetDaysInMonth( 2008, 1, 1 );

            // assert
            days.Should().Be( expected );
        }

        [Fact]
        public void get_days_in_year_should_not_allow_year_out_of_range()
        {
            // arrange
            const int year = 2006;

            // act
            Action getDaysInYear = () => Calendar.GetDaysInYear( year, 1 );

            // assert
            getDaysInYear.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( year ) );
        }

        [Fact]
        public void get_days_in_year_should_return_correct_result()
        {
            // arrange
            var expected = 364;

            // act
            var days = Calendar.GetDaysInYear( 2008, 1 );

            // assert
            days.Should().Be( expected );
        }

        [Fact]
        public void get_era_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action getEra = () => Calendar.GetEra( date );

            // assert
            getEra.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void get_era_should_return_correct_result()
        {
            // arrange
            var firstDay = new DateTime( 2007, 6, 30 );

            // act
            var era = Calendar.GetEra( firstDay );

            // assert
            era.Should().Be( 1 );
        }

        [Fact]
        public void get_month_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action getMonth = () => Calendar.GetMonth( date );

            // assert
            getMonth.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void get_month_should_return_correct_result()
        {
            // arrange
            var firstDay = new DateTime( 2007, 6, 30 );

            // act
            var month = Calendar.GetMonth( firstDay );

            // assert
            month.Should().Be( 1 );
        }

        [Fact]
        public void get_months_in_year_should_not_allow_year_out_of_range()
        {
            // arrange
            const int year = 2006;

            // act
            Action getMonthsInYear = () => Calendar.GetMonthsInYear( year, 1 );

            // assert
            getMonthsInYear.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( year ) );
        }

        [Fact]
        public void get_months_in_year_should_return_correct_result()
        {
            // arrange


            // act
            var months = Calendar.GetMonthsInYear( 2008, 1 );

            // assert
            months.Should().Be( 12 );
        }

        [Fact]
        public void get_year_should_not_allow_date_out_of_range()
        {
            // arrange
            var date = new DateTime( 2007, 1, 1 );

            // act
            Action getYear = () => Calendar.GetYear( date );

            // assert
            getYear.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( date ) );
        }

        [Fact]
        public void get_year_should_return_correct_result()
        {
            // NOTE: when fiscal years span calendar years, the calendar year from the last day in the fiscal calendar is reported

            // arrange
            var firstDay = new DateTime( 2007, 6, 30 );

            // act
            var year = Calendar.GetYear( firstDay );

            // asset
            year.Should().Be( 2008 );
        }

        [Fact]
        public void is_leap_day_should_not_allow_date_out_of_range()
        {
            // arrange
            const int year = 2006;

            // act
            Action isLeapDay = () => Calendar.IsLeapDay( year, 1, 1, 1 );

            // assert
            isLeapDay.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( year ) );
        }

        [Fact]
        public void is_leap_day_should_not_allow_month_out_of_range()
        {
            // arrange
            const int month = 5;

            // act
            Action isLeapDay = () => Calendar.IsLeapDay( 2007, month, 1, 1 );

            // assert
            isLeapDay.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( month ) );
        }

        [Fact]
        public void is_leap_month_should_not_allow_year_out_of_range()
        {
            // arrange
            const int year = 2006;

            // act
            Action isLeapMonth = () => Calendar.IsLeapMonth( year, 1, 1 );

            // assert
            isLeapMonth.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( year ) );
        }

        [Fact]
        public void is_leap_month_should_not_allow_month_out_of_range()
        {
            // arrange
            const int month = 5;

            // act
            Action isLeapMonth = () => Calendar.IsLeapMonth( 2007, month, 1 );

            // assert
            isLeapMonth.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( month ) );
        }

        [Fact]
        public void is_leap_year_should_not_allow_year_out_of_range()
        {
            // arrange
            const int year = 2006;

            // act
            Action isLeapYear = () => Calendar.IsLeapYear( year, 1 );

            // assert
            isLeapYear.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( year ) );
        }

        [Fact]
        public void to_date_time_should_not_allow_year_out_of_range()
        {
            // arrange
            const int year = 2006;

            // act
            Action toDateTime = () => Calendar.ToDateTime( year, 1, 1, 0, 0, 0, 0 );

            // assert
            toDateTime.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( year ) );
        }

        [Fact]
        public void to_date_time_should_not_allow_month_out_of_range()
        {
            // arrange
            const int month = 5;

            // act
            Action toDateTime = () => Calendar.ToDateTime( 2007, month, 1, 0, 0, 0, 0 );

            // assert
            toDateTime.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( month ) );
        }

        [Fact]
        public void to_date_time_should_return_correct_result()
        {
            // arrange
            var expected = new DateTime( 2007, 6, 30 );

            // act
            var date = Calendar.ToDateTime( 2008, 1, 1, 0, 0, 0, 0 );

            // assert
            date.Should().Be( expected );
        }

        [Fact]
        public void X34X2D4X2D5_calendar_to_string_should_return_expected_text()
        {
            // arrange
            var expected = Format( CurrentCulture, "MinSupportedDateTime = {0:d}, MaxSupportedDateTime = {1:d}", Calendar.MinSupportedDateTime, Calendar.MaxSupportedDateTime );

            // act
            var text = Calendar.ToString();

            // assert
            text.Should().Be( expected );
        }
    }
}