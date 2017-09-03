namespace System.Globalization
{
    using FluentAssertions;
    using Microsoft.QualityTools.Testing.Fakes;
    using More.Globalization;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static System.DayOfWeek;
    using static System.Fakes.ShimDateTime;

    public class CalendarExtensionsTest
    {
        [Fact]
        public void first_day_of_week_in_month_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 4 );

            // act
            var firstDayOfWeekInMonth = calendar.FirstDayOfWeekInMonth( date, Monday ).Date;

            // assert
            firstDayOfWeekInMonth.Should().Be( expected );
        }

        [Fact]
        public void last_date_of_week_in_month_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 29 );

            // act
            var lastDayOfWeekInMonth = calendar.LastDayOfWeekInMonth( date, Friday ).Date;

            // assert
            lastDayOfWeekInMonth.Should().Be( expected );
        }

        [Fact]
        public void day_of_week_in_month_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 11 );

            // act
            var dayOfWeekInMonth = calendar.DayOfWeekInMonth( date, 2, Monday ).Date;

            // assert
            dayOfWeekInMonth.Should().Be( expected );
        }

        [Fact]
        public void start_of_week_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 2, 24 );

            // act
            var startOfWeek = calendar.StartOfWeek( date ).Date;

            // assert
            startOfWeek.Should().Be( expected );
        }

        [Fact]
        public void start_of_week_with_first_day_of_week_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 2, 25 );

            // act
            var startOfWeek = calendar.StartOfWeek( date, Monday ).Date;

            // assert
            startOfWeek.Should().Be( expected );
        }

        [Fact]
        public void start_of_month_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 26 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 1 );

            // act
            var startOfMonth = calendar.StartOfMonth( date ).Date;

            // assert
            startOfMonth.Should().Be( expected );
        }

        [Fact]
        public void start_of_quarter_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );

            // act
            var startOfQuarter = calendar.StartOfQuarter( date ).Date;

            // assert
            startOfQuarter.Should().Be( expected );
        }

        [Fact]
        public void start_of_semester_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 1, 1 );

            // act
            var startOfSemester = calendar.StartOfSemester( date ).Date;

            // assert
            startOfSemester.Should().Be( expected );
        }

        [Fact]
        public void start_of_year_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2012, 7, 1 );

            // act
            var startOfYear = calendar.StartOfYear( date ).Date;

            // assert
            startOfYear.Should().Be( expected );
        }

        [Fact]
        public void end_of_week_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 2 );

            // act
            var endOfWeek = calendar.EndOfWeek( date ).Date;

            // assert
            endOfWeek.Should().Be( expected );
        }

        [Fact]
        public void end_of_week_with_first_day_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 3 );

            // act
            var endOfWeek = calendar.EndOfWeek( date, Monday ).Date;

            // assert
            endOfWeek.Should().Be( expected );
        }

        [Fact]
        public void end_of_month_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );

            // act
            var endOfMonth = calendar.EndOfMonth( date ).Date;

            // assert
            endOfMonth.Should().Be( expected );
        }

        [Fact]
        public void end_of_quarter_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 3, 31 );

            // act
            var endOfQuarter = calendar.EndOfQuarter( date ).Date;

            // assert
            endOfQuarter.Should().Be( expected );
        }

        [Fact]
        public void end_of_semester_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 );

            // act
            var endOfSemester = calendar.EndOfSemester( date ).Date;

            // assert
            endOfSemester.Should().Be( expected );
        }

        [Fact]
        public void end_of_year_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );
            var expected = new DateTime( 2013, 6, 30 );

            // act
            var endOfYear = calendar.EndOfYear( date ).Date;

            // assert
            endOfYear.Should().Be( expected );
        }

        [Fact]
        public void week_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var week = calendar.Week( date );

            // assert
            week.Should().Be( 35 );
        }

        [Fact]
        public void week_with_first_day_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var week = calendar.Week( date, Sunday );

            // assert
            week.Should().Be( 35 );
        }

        [Fact]
        public void quarter_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var quarter = calendar.Quarter( date );

            // assert
            quarter.Should().Be( 3 );
        }

        [Fact]
        public void semester_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 3, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var semester = calendar.Semester( date );

            // assert
            semester.Should().Be( 2 );
        }

        [Fact]
        public void year_should_return_correct_result()
        {
            // arrange
            var date = new DateTime( 2013, 7, 1 );
            var calendar = new GregorianFiscalCalendar( 7 );

            // act
            var year = calendar.Year( date );

            // assert
            year.Should().Be( 2014 );
        }

        public static IEnumerable<object[]> FirstMonthOfYearData
        {
            get
            {
                yield return new object[] { CultureInfo.CurrentCulture.Calendar, 1 };
                yield return new object[] { new GregorianFiscalCalendar( 7 ), 7 };
            }
        }

        [Theory]
        [MemberData( nameof( FirstMonthOfYearData ) )]
        public void first_month_of_year_should_return_correct_result( Calendar calendar, int expected )
        {
            // arrange

            // act
            var firstMonthOfYear = calendar.FirstMonthOfYear();

            // assert
            firstMonthOfYear.Should().Be( expected );
        }

        [Fact]
        public void current_end_of_month_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 6, 30 );
                var calendar = new GregorianCalendar();

                // act
                var endOfMonth = calendar.CurrentEndOfMonth().Date;

                // assert
                endOfMonth.Should().Be( expected );
            }
        }

        [Fact]
        public void current_end_of_quarter_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 6, 30 );
                var calendar = new GregorianCalendar();

                // act
                var endOfQuarter = calendar.CurrentEndOfQuarter().Date;

                // assert
                endOfQuarter.Should().Be( expected );
            }
        }

        [Fact]
        public void curent_end_of_semester_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 6, 30 );
                var calendar = new GregorianCalendar();

                // act
                var endOfSemester = calendar.CurrentEndOfSemester().Date;

                // assert
                endOfSemester.Should().Be( expected );
            }
        }

        [Fact]
        public void current_end_of_week_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 2 );
                var expected = new DateTime( 2013, 6, 8 );
                var calendar = new GregorianCalendar();

                // act
                var endOfWeek = calendar.CurrentEndOfWeek().Date;

                // assert
                endOfWeek.Should().Be( expected );
            }
        }

        [Fact]
        public void current_end_of_week_with_start_day_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 2 );
                var expected = new DateTime( 2013, 6, 7 );
                var calendar = new GregorianCalendar();

                // act
                var endOfWeek = calendar.CurrentEndOfWeek( Saturday ).Date;

                // assert
                endOfWeek.Should().Be( expected );
            }
        }

        [Fact]
        public void current_end_of_year_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 12, 31 );
                var calendar = new GregorianCalendar();

                // act
                var endOfYear = calendar.CurrentEndOfYear().Date;

                // assert
                endOfYear.Should().Be( expected );
            }
        }

        [Fact]
        public void current_first_day_of_week_in_month_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 6, 3 );
                var calendar = new GregorianCalendar();

                // act
                var firstDayOfWeekInMonth = calendar.CurrentFirstDayOfWeekInMonth( Monday ).Date;

                // assert
                firstDayOfWeekInMonth.Should().Be( expected );
            }
        }

        [Fact]
        public void current_last_day_of_week_in_month_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 6, 28 );
                var calendar = new GregorianCalendar();

                // act
                var lastDayOfWeekInMonth = calendar.CurrentLastDayOfWeekInMonth( Friday ).Date;

                // assert
                lastDayOfWeekInMonth.Should().Be( expected );
            }
        }

        [Fact]
        public void current_quarter_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var calendar = new GregorianCalendar();

                // act
                var quarter = calendar.CurrentQuarter();

                // assert
                quarter.Should().Be( 2 );
            }
        }

        [Fact]
        public void current_semester_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var calendar = new GregorianCalendar();

                // act
                var semester = calendar.CurrentSemester();

                // assert
                semester.Should().Be( 1 );
            }
        }

        [Fact]
        public void current_start_of_month_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 15 );
                var expected = new DateTime( 2013, 6, 1 );
                var calendar = new GregorianCalendar();

                // act
                var startOfMonth = calendar.CurrentStartOfMonth().Date;

                // assert
                startOfMonth.Should().Be( expected );
            }
        }

        [Fact]
        public void current_start_of_quarter_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 4, 1 );
                var calendar = new GregorianCalendar();

                // act
                var startOfQuarter = calendar.CurrentStartOfQuarter().Date;

                // assert
                startOfQuarter.Should().Be( expected );
            }
        }

        [Fact]
        public void current_start_of_semester_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 1, 1 );
                var calendar = new GregorianCalendar();

                // act
                var startOfSemester = calendar.CurrentStartOfSemester().Date;

                // assert
                startOfSemester.Should().Be( expected );
            }
        }

        [Fact]
        public void current_start_of_week_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2013, 5, 26 );
                var calendar = new GregorianCalendar();

                // act
                var startOfWeek = calendar.CurrentStartOfWeek().Date;

                // assert
                startOfWeek.Should().Be( expected );
            }
        }

        [Fact]
        public void current_start_of_week_with_start_day_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 8 );
                var expected = new DateTime( 2013, 6, 3 );
                var calendar = new GregorianCalendar();

                // act
                var startOfWeek = calendar.CurrentStartOfWeek( Monday ).Date;

                // assert
                startOfWeek.Should().Be( expected );
            }
        }

        [Fact]
        public void current_start_of_year_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var expected = new DateTime( 2012, 7, 1 );
                var calendar = new GregorianFiscalCalendar( 7 );

                // act
                var startOfYear = calendar.CurrentStartOfYear().Date;

                // assert
                startOfYear.Should().Be( expected );
            }
        }

        [Fact]
        public void current_week_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var calendar = new GregorianCalendar();

                // act
                var week = calendar.CurrentWeek();

                // assert
                week.Should().Be( 22 );
            }
        }

        [Fact]
        public void current_week_with_start_day_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2013, 6, 1 );
                var calendar = new GregorianCalendar();

                // act
                var week = calendar.CurrentWeek( Saturday );

                // assert
                week.Should().Be( 23 );
            }
        }

        [Fact]
        public void current_year_should_return_correct_result()
        {
            using ( ShimsContext.Create() )
            {
                // arrange
                TodayGet = () => new DateTime( 2012, 7, 1 );
                var calendar = new GregorianFiscalCalendar( 7 );

                // act
                var year = calendar.CurrentYear();

                // assert
                year.Should().Be( 2013 );
            }
        }
    }
}