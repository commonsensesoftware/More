namespace System.Globalization
{
    using Microsoft.QualityTools.Testing.Fakes;
    using More.Globalization;
    using System;
    using System.Fakes;
    using System.Globalization;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="CalendarExtensions"/>.
    /// </summary>
    [Collection( "Fakes" )]
    public class CalendarExtensionsTest
    {
        [Fact( DisplayName = "first day of week in month should return correct result" )]
        public void FirstDayOfWeekInMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 4 );
            var actual = calendar.FirstDayOfWeekInMonth( date, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "last date of week in month should return correct result" )]
        public void LastDateOfWeekInMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 29 );
            var actual = calendar.LastDayOfWeekInMonth( date, DayOfWeek.Friday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "day of week in month should return correct result" )]
        public void DayOfWeekInMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 11 );
            var actual = calendar.DayOfWeekInMonth( date, 2, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of week should return correct result" )]
        public void StartOfWeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 2, 24 );
            var actual = calendar.StartOfWeek( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of week with first day of week should return correct result" )]
        public void StartOfWeekWithCustomFirstDayOfWeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 2, 25 );
            var actual = calendar.StartOfWeek( date, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of month should return correct result" )]
        public void StartOfMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 26 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 1 );
            var actual = calendar.StartOfMonth( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of quarter should return correct result" )]
        public void StartOfQuarterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = calendar.StartOfQuarter( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of semester should return correct result" )]
        public void StartOfSemesterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = calendar.StartOfSemester( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "start of year should return correct result" )]
        public void StartOfYearShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2012, 7, 1 ); // because fiscal calendar starts in July
            var actual = calendar.StartOfYear( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of week should return correct result" )]
        public void EndOfWeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 2 );
            var actual = calendar.EndOfWeek( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of week with first day should return correct result" )]
        public void EndOfWeekWithCustomFirstDayOfWeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 3 );
            var actual = calendar.EndOfWeek( date, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of month should return correct result" )]
        public void EndOfMonthShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = calendar.EndOfMonth( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of quarter should return correct result" )]
        public void EndOfQuarterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = calendar.EndOfQuarter( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of semester should return correct result" )]
        public void EndOfSemesterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 ); // because fiscal calendar starts in July
            var actual = calendar.EndOfSemester( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "end of year should return correct result" )]
        public void EndOfYearShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 ); // because fiscal calendar starts in July
            var actual = calendar.EndOfYear( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "week should return correct result" )]
        public void WeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Week( date );
            Assert.Equal( 35, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "week with first day should return correct result" )]
        public void WeekWithCustomFirstDayOfWeekShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Week( date, DayOfWeek.Sunday );
            Assert.Equal( 35, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "quarter should return correct result" )]
        public void QuarterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Quarter( date );
            Assert.Equal( 3, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "semester should return correct result" )]
        public void SemesterShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Semester( date );
            Assert.Equal( 2, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "year should return correct result" )]
        public void YearShouldReturnExpectedResult()
        {
            var date = new DateTime( 2013, 7, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Year( date );
            Assert.Equal( 2014, actual ); // because fiscal calendar starts in July
        }

        [Fact( DisplayName = "first month of year should return correct result" )]
        public void FirstMonthOfYearShouldReturnExpectedResult()
        {
            var actual = CultureInfo.CurrentCulture.Calendar.FirstMonthOfYear();
            Assert.Equal( 1, actual );

            actual = new GregorianFiscalCalendar( 7 ).FirstMonthOfYear();
            Assert.Equal( 7, actual );
        }

        [Fact( DisplayName = "current end of month should return correct result" )]
        public void CurrentEndOfMonthShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 6, 30 );
            var target = new GregorianCalendar();
            var actual = target.CurrentEndOfMonth();

            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "current end of quarter should return correct result" )]
        public void CurrentEndOfQuarterShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 6, 30 );
            var target = new GregorianCalendar();
            var actual = target.CurrentEndOfQuarter();

            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "curent end of semester should return correct result" )]
        public void CurrentEndOfSemesterShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 6, 30 );
            var target = new GregorianCalendar();
            var actual = target.CurrentEndOfSemester();

            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "current end of week should return correct result" )]
        public void CurrentEndOfWeekShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 2 );

            var expected = new DateTime( 2013, 6, 8 );
            var target = new GregorianCalendar();
            var actual = target.CurrentEndOfWeek();

            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "current end of week with start day should return correct result" )]
        public void CurrentEndOfWeekWithCustomStartShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 2 );

            var expected = new DateTime( 2013, 6, 7 );
            var target = new GregorianCalendar();
            var actual = target.CurrentEndOfWeek( DayOfWeek.Saturday );

            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "current end of year should return correct result" )]
        public void CurrentEndOfYearShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 12, 31 );
            var target = new GregorianCalendar();
            var actual = target.CurrentEndOfYear();

            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "current first day of week in month should return correct result" )]
        public void CurrentFirstDayOfWeekInMonthShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 6, 3 );
            var target = new GregorianCalendar();
            var actual = target.CurrentFirstDayOfWeekInMonth( DayOfWeek.Monday );

            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "current last day of week in month should return correct result" )]
        public void CurrentLastDayOfWeekInMonthShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 6, 28 );
            var target = new GregorianCalendar();
            var actual = target.CurrentLastDayOfWeekInMonth( DayOfWeek.Friday );

            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact( DisplayName = "current quarter should return correct result" )]
        public void CurrentQuarterShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = 2;
            var target = new GregorianCalendar();
            var actual = target.CurrentQuarter();

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current semester should return correct result" )]
        public void CurrentSemesterShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = 1;
            var target = new GregorianCalendar();
            var actual = target.CurrentSemester();

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current start of month should return correct result" )]
        public void CurrentStartOfMonthShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 15 );

            var expected = new DateTime( 2013, 6, 1 );
            var target = new GregorianCalendar();
            var actual = target.CurrentStartOfMonth();

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current start of quarter should return correct result" )]
        public void CurrentStartOfQuarterShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 4, 1 );
            var target = new GregorianCalendar();
            var actual = target.CurrentStartOfQuarter();

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current start of semester should return correct result" )]
        public void CurrentStartOfSemesterShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 1, 1 );
            var target = new GregorianCalendar();
            var actual = target.CurrentStartOfSemester();

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current start of week should return correct result" )]
        public void CurrentStartOfWeekShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2013, 5, 26 );
            var target = new GregorianCalendar();
            var actual = target.CurrentStartOfWeek();

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current start of week with start day should return correct result" )]
        public void CurrentStartOfWeekWithCustomStartShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 8 );

            var expected = new DateTime( 2013, 6, 3 );
            var target = new GregorianCalendar();
            var actual = target.CurrentStartOfWeek( DayOfWeek.Monday );

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current start of year should return correct result" )]
        public void CurrentStartOfYearShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = new DateTime( 2012, 7, 1 );
            var target = new GregorianFiscalCalendar( 7 ); // because fiscal calendar starts in July
            var actual = target.CurrentStartOfYear();

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current week should return correct result" )]
        public void CurrentWeekShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = 22;
            var target = new GregorianCalendar();
            var actual = target.CurrentWeek();

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current week with start day should return correct result" )]
        public void CurrentWeekWithCustomStartShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

            var expected = 23;
            var target = new GregorianCalendar();
            var actual = target.CurrentWeek( DayOfWeek.Saturday );

            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "current year should return correct result" )]
        public void CurrentYearShouldReturnExpectedResult()
        {
            ShimDateTime.TodayGet = () => new DateTime( 2012, 7, 1 );

            // this test sets the fiscal year to start in july. the calendar will
            // report the "fiscal year" as the year reported by the true calendar
            // year at the end of the current "fiscal year"; therefore, although
            // DateTime.Today is 7/1/2012, 2013 will be reported because the
            // current year ends on 6/30/2013.

            var target = new GregorianFiscalCalendar( 7 );
            var actual = target.CurrentYear();
            var expected = 2013;

            Assert.Equal( expected, actual );
        }
    }
}
