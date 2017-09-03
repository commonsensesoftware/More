namespace More
{
    using FluentAssertions;
    using More.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xunit;

    public class DateTimeFormatProviderTest
    {
        [Theory]
        [MemberData( nameof( NullDateTimeFormatData ) )]
        public void date_time_format_provider_should_not_allow_null_format( Action<DateTimeFormatInfo> test )
        {
            // arrange
            var dateTimeFormat = default( DateTimeFormatInfo );

            // act
            Action @new = () => test( dateTimeFormat );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( dateTimeFormat ) );
        }

        [Theory]
        [MemberData( nameof( NullCalendarData ) )]
        public void date_time_provider_should_not_allow_null_calendar( Action<Calendar> test )
        {
            // arrange
            var calendar = default( Calendar );

            // act
            Action @new = () => test( calendar );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( calendar ) );
        }

        [Theory]
        [MemberData( nameof( UnsupportedFormatTypesData ) )]
        public void get_format_should_return_null_for_unsupported_types( DateTimeFormatProvider provider, Type formatType )
        {
            // arrange

            // act
            var format = provider.GetFormat( formatType );

            // assert
            format.Should().BeNull();
        }

        [Theory]
        [MemberData( nameof( GetFormatData ) )]
        public void get_format_should_return_expected_provider( DateTimeFormatProvider provider, Type formatType, object expected )
        {
            // arrange

            // act
            var format = provider.GetFormat( formatType );

            // assert
            format.Should().BeSameAs( expected );
        }

        [Theory]
        [MemberData( nameof( DateTimeFormatProvidersData ) )]
        public void format_should_allow_null_format_string( DateTimeFormatProvider provider )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var culture = CultureInfo.CurrentCulture;

            // act
            var format = provider.Format( null, date, culture );

            // assert
            format.Should().Be( date.ToString( culture ) );
        }

        [Theory]
        [MemberData( nameof( DateTimeFormatProvidersData ) )]
        public void format_should_allow_empty_format_string( DateTimeFormatProvider provider )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var culture = CultureInfo.CurrentCulture;

            // act
            var format = provider.Format( string.Empty, date, culture );

            // assert
            format.Should().Be( date.ToString( culture ) );
        }

        [Theory]
        [MemberData( nameof( DateTimeFormatProvidersData ) )]
        public void format_should_return_original_format_when_argument_is_null( DateTimeFormatProvider provider )
        {
            // arrange
            var culture = CultureInfo.CurrentCulture;

            // act
            var format = provider.Format( "d", null, culture );

            // assert
            format.Should().Be( "d" );
        }

        [Theory]
        [MemberData( nameof( DateTimeFormatProvidersData ) )]
        public void format_should_return_original_format_when_argument_cannot_be_formatted( DateTimeFormatProvider provider )
        {
            // arrange
            var arg = new object();
            var culture = CultureInfo.CurrentCulture;

            // act
            var format = provider.Format( "d", arg, culture );

            // assert
            format.Should().Be( arg.ToString() );
        }

        [Theory]
        [MemberData( nameof( MalformedLiteralStringsData ) )]
        public void format_should_not_allow_malformed_literal_strings( DateTimeFormatProvider provider, string formatString )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            Action format = () => provider.Format( formatString, date, null );

            // assert
            format.ShouldThrow<FormatException>();
        }

        [Theory]
        [MemberData( nameof( BuiltInCustomFormatStringData ) )]
        public void format_should_return_expected_builtX2Din_custom_format_string( DateTimeFormatProvider provider, string format )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var result = provider.Format( format, date, null );

            // assert
            result.Should().Be( date.ToString( format ) );
        }

        [Theory]
        [MemberData( nameof( SemesterFormatData ) )]
        public void format_semester_should_return_expected_string( DateTimeFormatProvider provider, string format, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var result = provider.Format( format, date, null );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( QuarterFormatData ) )]
        public void format_quarter_should_return_expected_string( DateTimeFormatProvider provider, string format, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var result = provider.Format( format, date, null );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( CustomFormatData ) )]
        public void format_should_return_expected_custom_format_string( Func<DateTime, string> test, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var result = test( date );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( CustomFormatAndCalendarData ) )]
        public void format_should_return_expected_custom_format_string_with_custom_calendar( Func<DateTime, DateTimeFormatProvider, string> test, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var provider = new DateTimeFormatProvider( new GregorianFiscalCalendar( 7 ) );

            // act
            var result = test( date, provider );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( MultipleFormatParameterData ) )]
        public void string_format_should_return_custom_format_string_with_multiple_parameters( DateTimeFormatProvider provider, string format, object arg, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var args = new object[] { date, arg };

            // act
            var result = string.Format( provider, format, args );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void format_should_return_custom_string_with_escape_sequence()
        {
            // arrange
            var provider = new DateTimeFormatProvider( new GregorianFiscalCalendar( 7 ) );
            var date = new DateTime( 2010, 9, 29 );

            // act
            var result = provider.Format( "'FY'\\'yy", date, null );

            // assert
            result.Should().Be( "FY'11" );
        }

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
    }
}