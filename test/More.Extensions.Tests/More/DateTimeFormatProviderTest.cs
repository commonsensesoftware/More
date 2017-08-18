namespace More
{
    using More.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="DateTimeFormatProvider"/>.
    /// </summary>
    public class DateTimeFormatProviderTest
    {
        public static IEnumerable<object[]> NullDateTimeFormatData
        {
            get
            {
                yield return new object[] { new Action<DateTimeFormatInfo>( dti => new DateTimeFormatProvider( dti ) ) };
                yield return new object[] { new Action<DateTimeFormatInfo>( dti => new DateTimeFormatProvider( dti, new GregorianCalendar() ) ) };
            }
        }

        public static IEnumerable<object[]> NullCalendarData
        {
            get
            {
                yield return new object[] { new Action<Calendar>( c => new DateTimeFormatProvider( c ) ) };
                yield return new object[] { new Action<Calendar>( c => new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo, c ) ) };
            }
        }

        public static IEnumerable<object[]> DateTimeFormatProvidersData
        {
            get
            {
                yield return new object[] { new DateTimeFormatProvider() };
                yield return new object[] { new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo ) };
                yield return new object[] { new DateTimeFormatProvider( new GregorianCalendar() ) };
                yield return new object[] { new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo, new GregorianCalendar() ) };
            }
        }

        public static IEnumerable<object[]> UnsupportedFormatTypesData
        {
            get
            {
                yield return new object[] { new DateTimeFormatProvider(), null };
                yield return new object[] { new DateTimeFormatProvider(), typeof( string ) };
                yield return new object[] { new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo ), null };
                yield return new object[] { new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo ), typeof( string ) };
                yield return new object[] { new DateTimeFormatProvider( new GregorianCalendar() ), null };
                yield return new object[] { new DateTimeFormatProvider( new GregorianCalendar() ), typeof( string ) };
                yield return new object[] { new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo, new GregorianCalendar() ), null };
                yield return new object[] { new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo, new GregorianCalendar() ), typeof( string ) };
            }
        }

        public static IEnumerable<object[]> GetFormatData
        {
            get
            {
                yield return new object[] { new DateTimeFormatProvider(), typeof( DateTimeFormatInfo ), DateTimeFormatInfo.CurrentInfo };

                var expected = new DateTimeFormatProvider();
                yield return new object[] { expected, typeof( ICustomFormatter ), expected };

                yield return new object[] { new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo ), typeof( DateTimeFormatInfo ), DateTimeFormatInfo.CurrentInfo };

                expected = new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo );
                yield return new object[] { expected, typeof( ICustomFormatter ), expected };

                yield return new object[] { new DateTimeFormatProvider( new GregorianCalendar() ), typeof( DateTimeFormatInfo ), DateTimeFormatInfo.CurrentInfo };

                expected = new DateTimeFormatProvider( new GregorianCalendar() );
                yield return new object[] { expected, typeof( ICustomFormatter ), expected };

                yield return new object[] { new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo, new GregorianCalendar() ), typeof( DateTimeFormatInfo ), DateTimeFormatInfo.CurrentInfo };

                expected = new DateTimeFormatProvider( DateTimeFormatInfo.CurrentInfo, new GregorianCalendar() );
                yield return new object[] { expected, typeof( ICustomFormatter ), expected };
            }
        }

        public static IEnumerable<object[]> MalformedLiteralStringsData
        {
            get
            {
                foreach ( var provider in DateTimeFormatProvidersData.Select( d => d[0] ).Cast<DateTimeFormatProvider>() )
                {
                    // missing closing ' in custom format string
                    yield return new object[] { provider, "'MM-dd-yyyy" };

                    // missing opening ' in custom format string
                    yield return new object[] { provider, "MM-dd-yyyy'" };

                    // missing closing " in custom format string
                    yield return new object[] { provider, "\"MM-dd-yyyy" };

                    // missing opening " in custom format string
                    yield return new object[] { provider, "MM-dd-yyyy\"" };
                }
            }
        }

        public static IEnumerable<object[]> BuiltInCustomFormatStringData
        {
            get
            {
                // custom string formats with one character must be prefixed with % to prevent confusion with the standard format strings
                // REF: http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx#UsingSingleSpecifiers
                var formats = new[]
                {
                    "%d", "dd", "ddd", "dddd", "%f", "ff", "fff", "ffff", "fffff", "ffffff", "fffffff",
                    "%F", "FF", "FFF", "FFFF", "FFFFF", "FFFFFF", "FFFFFFF", "%g", "gg", "%h", "hh", "%H",
                    "HH", "%K", "%m", "mm", "%M", "MM", "MMM", "MMMM", "%s", "ss", "%t", "tt", "%y", "yy",
                    "yyy", "yyyy", "%z", "zz", "zzz"
                };

                foreach ( var provider in DateTimeFormatProvidersData.Select( d => d[0] ).Cast<DateTimeFormatProvider>() )
                {
                    foreach ( var format in formats )
                        yield return new object[] { provider, format };
                }
            }
        }

        public static IEnumerable<object[]> SemesterFormatData
        {
            get
            {
                foreach ( var provider in DateTimeFormatProvidersData.Select( d => d[0] ).Cast<DateTimeFormatProvider>() )
                {
                    yield return new object[] { provider, "S", "2" };
                    yield return new object[] { provider, "SS", "02" };
                    yield return new object[] { provider, "SSS", "H2" };
                    yield return new object[] { provider, "SSSS", "Semester 2" };
                }
            }
        }

        public static IEnumerable<object[]> QuarterFormatData
        {
            get
            {
                foreach ( var provider in DateTimeFormatProvidersData.Select( d => d[0] ).Cast<DateTimeFormatProvider>() )
                {
                    yield return new object[] { provider, "q", "3" };
                    yield return new object[] { provider, "qq", "03" };
                    yield return new object[] { provider, "qqq", "Q3" };
                    yield return new object[] { provider, "qqqq", "Quarter 3" };
                }
            }
        }

        public static IEnumerable<object[]> CustomFormatData
        {
            get
            {
                foreach ( var provider in DateTimeFormatProvidersData.Select( d => d[0] ).Cast<DateTimeFormatProvider>() )
                {
                    yield return new object[] { new Func<DateTime, string>( d => provider.Format( "'Year' yyyy, 'Semester' S, 'Quarter' q, 'Month' M, 'Day' d", d, null ) ), "Year 2010, Semester 2, Quarter 3, Month 9, Day 29" };
                    yield return new object[] { new Func<DateTime, string>( d => d.ToString( provider, "'Year' yyyy, 'Semester' S, 'Quarter' q, 'Month' M, 'Day' d" ) ), "Year 2010, Semester 2, Quarter 3, Month 9, Day 29" };
                    yield return new object[] { new Func<DateTime, string>( d => string.Format( provider, "{0:'Year' yyyy, 'Semester' S, 'Quarter' q, 'Month' M, 'Day' d}", d ) ), "Year 2010, Semester 2, Quarter 3, Month 9, Day 29" };
                    yield return new object[] { new Func<DateTime, string>( d => provider.Format( "'Year' yyyy, 'Semester' SS, 'Quarter' qq, 'Month' MM, 'Day' dd", d, null ) ), "Year 2010, Semester 02, Quarter 03, Month 09, Day 29" };
                    yield return new object[] { new Func<DateTime, string>( d => d.ToString( provider, "'Year' yyyy, 'Semester' SS, 'Quarter' qq, 'Month' MM, 'Day' dd" ) ), "Year 2010, Semester 02, Quarter 03, Month 09, Day 29" };
                    yield return new object[] { new Func<DateTime, string>( d => string.Format( provider, "{0:'Year' yyyy, 'Semester' SS, 'Quarter' qq, 'Month' MM, 'Day' dd}", d ) ), "Year 2010, Semester 02, Quarter 03, Month 09, Day 29" };
                    yield return new object[] { new Func<DateTime, string>( d => provider.Format( "\"Year\" yyyy, SSS, qqq, MMM, dd", d, null ) ), "Year 2010, H2, Q3, Sep, 29" };
                    yield return new object[] { new Func<DateTime, string>( d => d.ToString( provider, "\"Year\" yyyy, SSS, qqq, MMM, dd" ) ), "Year 2010, H2, Q3, Sep, 29" };
                    yield return new object[] { new Func<DateTime, string>( d => string.Format( provider, "{0:\"Year\" yyyy, SSS, qqq, MMM, dd}", d ) ), "Year 2010, H2, Q3, Sep, 29" };
                    yield return new object[] { new Func<DateTime, string>( d => provider.Format( "\"Year\" yyyy, SSSS, qqqq, MMMM, dd", d, null ) ), "Year 2010, Semester 2, Quarter 3, September, 29" };
                    yield return new object[] { new Func<DateTime, string>( d => d.ToString( provider, "\"Year\" yyyy, SSSS, qqqq, MMMM, dd" ) ), "Year 2010, Semester 2, Quarter 3, September, 29" };
                    yield return new object[] { new Func<DateTime, string>( d => string.Format( provider, "{0:\"Year\" yyyy, SSSS, qqqq, MMMM, dd}", d ) ), "Year 2010, Semester 2, Quarter 3, September, 29" };
                }
            }
        }

        public static IEnumerable<object[]> CustomFormatAndCalendarData
        {
            get
            {
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => p.Format( "'Year' yyyy, 'Semester' S, 'Quarter' q, 'Month' M, 'Day' d", d, null ) ), "Year 2011, Semester 1, Quarter 1, Month 3, Day 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => d.ToString( p, "'Year' yyyy, 'Semester' S, 'Quarter' q, 'Month' M, 'Day' d" ) ), "Year 2011, Semester 1, Quarter 1, Month 3, Day 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => string.Format( p, "{0:'Year' yyyy, 'Semester' S, 'Quarter' q, 'Month' M, 'Day' d}", d ) ), "Year 2011, Semester 1, Quarter 1, Month 3, Day 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => p.Format( "'Year' yyyy, 'Semester' SS, 'Quarter' qq, 'Month' MM, 'Day' dd", d, null ) ), "Year 2011, Semester 01, Quarter 01, Month 03, Day 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => d.ToString( p, "'Year' yyyy, 'Semester' SS, 'Quarter' qq, 'Month' MM, 'Day' dd" ) ), "Year 2011, Semester 01, Quarter 01, Month 03, Day 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => string.Format( p, "{0:'Year' yyyy, 'Semester' SS, 'Quarter' qq, 'Month' MM, 'Day' dd}", d ) ), "Year 2011, Semester 01, Quarter 01, Month 03, Day 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => p.Format( "\"Year\" yyyy, SSS, qqq, MMM, dd", d, null ) ), "Year 2011, H1, Q1, Sep, 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => d.ToString( p, "\"Year\" yyyy, SSS, qqq, MMM, dd" ) ), "Year 2011, H1, Q1, Sep, 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => string.Format( p, "{0:\"Year\" yyyy, SSS, qqq, MMM, dd}", d ) ), "Year 2011, H1, Q1, Sep, 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => p.Format( "\"Year\" yyyy, SSSS, qqqq, MMMM, dd", d, null ) ), "Year 2011, Semester 1, Quarter 1, September, 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => d.ToString( p, "\"Year\" yyyy, SSSS, qqqq, MMMM, dd" ) ), "Year 2011, Semester 1, Quarter 1, September, 29" };
                yield return new object[] { new Func<DateTime, DateTimeFormatProvider, string>( ( d, p ) => string.Format( p, "{0:\"Year\" yyyy, SSSS, qqqq, MMMM, dd}", d ) ), "Year 2011, Semester 1, Quarter 1, September, 29" };
            }
        }

        public static IEnumerable<object[]> MultipleFormatParameterData
        {
            get
            {
                foreach ( var provider in DateTimeFormatProvidersData.Select( d => d[0] ).Cast<DateTimeFormatProvider>() )
                {
                    yield return new object[] { provider, "{0:} - {1}", "A", "9/29/2010 12:00:00 AM - A" };
                    yield return new object[] { provider, "{0:'FY'yy} - {1}", "A", "FY10 - A" };
                    yield return new object[] { provider, "{0:qqq} - {1:N1}", 1, "Q3 - 1.0" };
                }
            }
        }

        [Theory]
        [MemberData( "NullDateTimeFormatData" )]
        public void date_time_format_provider_should_not_allow_null_format( Action<DateTimeFormatInfo> test )
        {
            // arrange
            DateTimeFormatInfo dateTimeFormat = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( dateTimeFormat ) );

            // assert
            Assert.Equal( "dateTimeFormat", ex.ParamName );
        }

        [Theory]
        [MemberData( "NullCalendarData" )]
        public void date_time_provider_should_not_allow_null_calendar( Action<Calendar> test )
        {
            // arrange
            Calendar calendar = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( calendar ) );

            // assert
            Assert.Equal( "calendar", ex.ParamName );
        }

        [Theory]
        [MemberData( "UnsupportedFormatTypesData" )]
        public void get_format_should_return_null_for_unsupported_types( DateTimeFormatProvider provider, Type formatType )
        {
            // arrange

            // act
            var actual = provider.GetFormat( formatType );

            // assert
            Assert.Null( actual );
        }

        [Theory]
        [MemberData( "GetFormatData" )]
        public void get_format_should_return_expected_provider( DateTimeFormatProvider provider, Type formatType, object expected )
        {
            // arrange

            // act
            var actual = provider.GetFormat( formatType );

            // assert
            Assert.Same( expected, actual );
        }

        [Theory]
        [MemberData( "DateTimeFormatProvidersData" )]
        public void format_should_allow_null_or_empty_format_string( DateTimeFormatProvider provider )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var culture = CultureInfo.CurrentCulture;
            var expected = date.ToString( culture );
            var actual = new string[2];

            // act
            actual[0] = provider.Format( null, date, culture );
            actual[1] = provider.Format( string.Empty, date, culture );

            // assert
            Assert.Equal( expected, actual[0] );
            Assert.Equal( expected, actual[1] );
        }

        [Theory]
        [MemberData( "DateTimeFormatProvidersData" )]
        public void format_should_return_original_format_when_argument_cannot_be_formatted( DateTimeFormatProvider provider )
        {
            // arrange
            var expected = new string[] { "d", new object().ToString() };
            var actual = new string[2];
            var culture = CultureInfo.CurrentCulture;

            // act
            actual[0] = provider.Format( "d", null, culture );
            actual[1] = provider.Format( "d", new object(), culture );

            // assert
            Assert.Equal( expected[0], actual[0] );
            Assert.Equal( expected[1], actual[1] );
        }

        [Theory]
        [MemberData( "MalformedLiteralStringsData" )]
        public void format_should_not_allow_malformed_literal_strings( DateTimeFormatProvider provider, string format )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act and assert
            Assert.Throws<FormatException>( () => provider.Format( format, date, null ) );
        }

        [Theory]
        [MemberData( "BuiltInCustomFormatStringData" )]
        public void format_should_return_expected_builtX2Din_custom_format_string( DateTimeFormatProvider provider, string format )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var expected = date.ToString( format );

            // act
            var actual = provider.Format( format, date, null );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory]
        [MemberData( "SemesterFormatData" )]
        public void format_semester_should_return_expected_string( DateTimeFormatProvider provider, string format, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var actual = provider.Format( format, date, null );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory]
        [MemberData( "QuarterFormatData" )]
        public void format_quarter_should_return_expected_string( DateTimeFormatProvider provider, string format, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var actual = provider.Format( format, date, null );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory]
        [MemberData( "CustomFormatData" )]
        public void format_should_return_expected_custom_format_string( Func<DateTime, string> test, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var actual = test( date );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory]
        [MemberData( "CustomFormatAndCalendarData" )]
        public void format_should_return_expected_custom_format_string_with_custom_calendar( Func<DateTime, DateTimeFormatProvider, string> test, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var provider = new DateTimeFormatProvider( new GregorianFiscalCalendar( 7 ) );

            // act
            var actual = test( date, provider );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory]
        [MemberData( "MultipleFormatParameterData" )]
        public void string_format_should_return_custom_format_string_with_multiple_parameters( DateTimeFormatProvider provider, string format, object arg, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var args = new object[] { date, arg };

            // act
            var actual = string.Format( provider, format, args );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact]
        public void format_should_return_custom_string_with_escape_sequence()
        {
            // arrange
            var provider = new DateTimeFormatProvider( new GregorianFiscalCalendar( 7 ) );
            var date = new DateTime( 2010, 9, 29 );
            var expected = "FY'11";

            // act
            var actual = provider.Format( "'FY'\\'yy", date, null );

            Assert.Equal( expected, actual );
        }
    }
}
