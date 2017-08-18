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
    public class CalendarExtensionsTest
    {
        [Fact]
        public void first_day_of_week_in_month_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 4 );
            var actual = calendar.FirstDayOfWeekInMonth( date, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void last_date_of_week_in_month_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 29 );
            var actual = calendar.LastDayOfWeekInMonth( date, DayOfWeek.Friday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void day_of_week_in_month_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 11 );
            var actual = calendar.DayOfWeekInMonth( date, 2, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_week_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 2, 24 );
            var actual = calendar.StartOfWeek( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_week_with_first_day_of_week_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 2, 25 );
            var actual = calendar.StartOfWeek( date, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_month_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 26 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 1 );
            var actual = calendar.StartOfMonth( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_quarter_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = calendar.StartOfQuarter( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_semester_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );
            var actual = calendar.StartOfSemester( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void start_of_year_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2012, 7, 1 ); // because fiscal calendar starts in July
            var actual = calendar.StartOfYear( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_week_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 2 );
            var actual = calendar.EndOfWeek( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_week_with_first_day_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 3 );
            var actual = calendar.EndOfWeek( date, DayOfWeek.Monday );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_month_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = calendar.EndOfMonth( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_quarter_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );
            var actual = calendar.EndOfQuarter( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_semester_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 ); // because fiscal calendar starts in July
            var actual = calendar.EndOfSemester( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void end_of_year_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 ); // because fiscal calendar starts in July
            var actual = calendar.EndOfYear( date );
            Assert.Equal( expected.Date, actual.Date );
        }

        [Fact]
        public void week_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Week( date );
            Assert.Equal( 35, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void week_with_first_day_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Week( date, DayOfWeek.Sunday );
            Assert.Equal( 35, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void quarter_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Quarter( date );
            Assert.Equal( 3, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void semester_should_return_correct_result()
        {
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Semester( date );
            Assert.Equal( 2, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void year_should_return_correct_result()
        {
            var date = new DateTime( 2013, 7, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var actual = calendar.Year( date );
            Assert.Equal( 2014, actual ); // because fiscal calendar starts in July
        }

        [Fact]
        public void first_month_of_year_should_return_correct_result()
        {
            var actual = CultureInfo.CurrentCulture.Calendar.FirstMonthOfYear();
            Assert.Equal( 1, actual );

            actual = new GregorianFiscalCalendar( 7 ).FirstMonthOfYear();
            Assert.Equal( 7, actual );
        }

        [Fact]
        public void current_end_of_month_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 6, 30 );
                var target = new GregorianCalendar();
                var actual = target.CurrentEndOfMonth();

                Assert.Equal( expected.Date, actual.Date );
            }
        }

        [Fact]
        public void current_end_of_quarter_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 6, 30 );
                var target = new GregorianCalendar();
                var actual = target.CurrentEndOfQuarter();

                Assert.Equal( expected.Date, actual.Date );
            }
        }

        [Fact]
        public void curent_end_of_semester_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 6, 30 );
                var target = new GregorianCalendar();
                var actual = target.CurrentEndOfSemester();

                Assert.Equal( expected.Date, actual.Date );
            }
        }

        [Fact]
        public void current_end_of_week_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 2 );

                var expected = new DateTime( 2013, 6, 8 );
                var target = new GregorianCalendar();
                var actual = target.CurrentEndOfWeek();

                Assert.Equal( expected.Date, actual.Date );
            }
        }

        [Fact]
        public void current_end_of_week_with_start_day_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 2 );

                var expected = new DateTime( 2013, 6, 7 );
                var target = new GregorianCalendar();
                var actual = target.CurrentEndOfWeek( DayOfWeek.Saturday );

                Assert.Equal( expected.Date, actual.Date );
            }
        }

        [Fact]
        public void current_end_of_year_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 12, 31 );
                var target = new GregorianCalendar();
                var actual = target.CurrentEndOfYear();

                Assert.Equal( expected.Date, actual.Date );
            }
        }

        [Fact]
        public void current_first_day_of_week_in_month_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 6, 3 );
                var target = new GregorianCalendar();
                var actual = target.CurrentFirstDayOfWeekInMonth( DayOfWeek.Monday );

                Assert.Equal( expected.Date, actual.Date );
            }
        }

        [Fact]
        public void current_last_day_of_week_in_month_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 6, 28 );
                var target = new GregorianCalendar();
                var actual = target.CurrentLastDayOfWeekInMonth( DayOfWeek.Friday );

                Assert.Equal( expected.Date, actual.Date );
            }
        }

        [Fact]
        public void current_quarter_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = 2;
                var target = new GregorianCalendar();
                var actual = target.CurrentQuarter();

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_semester_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = 1;
                var target = new GregorianCalendar();
                var actual = target.CurrentSemester();

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_start_of_month_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 15 );

                var expected = new DateTime( 2013, 6, 1 );
                var target = new GregorianCalendar();
                var actual = target.CurrentStartOfMonth();

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_start_of_quarter_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 4, 1 );
                var target = new GregorianCalendar();
                var actual = target.CurrentStartOfQuarter();

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_start_of_semester_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 1, 1 );
                var target = new GregorianCalendar();
                var actual = target.CurrentStartOfSemester();

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_start_of_week_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2013, 5, 26 );
                var target = new GregorianCalendar();
                var actual = target.CurrentStartOfWeek();

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_start_of_week_with_start_day_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 8 );

                var expected = new DateTime( 2013, 6, 3 );
                var target = new GregorianCalendar();
                var actual = target.CurrentStartOfWeek( DayOfWeek.Monday );

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_start_of_year_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = new DateTime( 2012, 7, 1 );
                var target = new GregorianFiscalCalendar( 7 ); // because fiscal calendar starts in July
                var actual = target.CurrentStartOfYear();

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_week_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = 22;
                var target = new GregorianCalendar();
                var actual = target.CurrentWeek();

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_week_with_start_day_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                ShimDateTime.TodayGet = () => new DateTime( 2013, 6, 1 );

                var expected = 23;
                var target = new GregorianCalendar();
                var actual = target.CurrentWeek( DayOfWeek.Saturday );

                Assert.Equal( expected, actual );
            }
        }

        [Fact]
        public void current_year_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
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
}
