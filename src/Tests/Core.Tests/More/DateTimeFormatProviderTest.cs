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

        [Theory( DisplayName = "date time format provider should not allow null format" )]
        [MemberData( "NullDateTimeFormatData" )]
        public void ConstructorsShouldNotAllowNullDateTimeFormat( Action<DateTimeFormatInfo> test )
        {
            // arrange
            DateTimeFormatInfo dateTimeFormat = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( dateTimeFormat ) );

            // assert
            Assert.Equal( "dateTimeFormat", ex.ParamName );
        }

        [Theory( DisplayName = "date time provider should not allow null calendar" )]
        [MemberData( "NullCalendarData" )]
        public void ConstructorsShouldNotAllowNullCalendar( Action<Calendar> test )
        {
            // arrange
            Calendar calendar = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( calendar ) );

            // assert
            Assert.Equal( "calendar", ex.ParamName );
        }

        [Theory( DisplayName = "get format should return null for unsupported types" )]
        [MemberData( "UnsupportedFormatTypesData" )]
        public void GetFormatShouldReturnNullForUnsupportedFormatTypes( DateTimeFormatProvider provider, Type formatType )
        {
            // arrange

            // act
            var actual = provider.GetFormat( formatType );

            // assert
            Assert.Null( actual );
        }

        [Theory( DisplayName = "get format should return expected provider" )]
        [MemberData( "GetFormatData" )]
        public void GetFormatShouldReturnCorrectFormatProvider( DateTimeFormatProvider provider, Type formatType, object expected )
        {
            // arrange

            // act
            var actual = provider.GetFormat( formatType );

            // assert
            Assert.Same( expected, actual );
        }

        [Theory( DisplayName = "format should allow null or empty format string" )]
        [MemberData( "DateTimeFormatProvidersData" )]
        public void FormatShouldAllowNullOrEmptyFormatString( DateTimeFormatProvider provider )
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

        [Theory( DisplayName = "format should return original format when argument cannot be formatted" )]
        [MemberData( "DateTimeFormatProvidersData" )]
        public void FormatShouldReturnOriginalStringFormatWhenArgumentCannotBeFormatted( DateTimeFormatProvider provider )
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

        [Theory( DisplayName = "format should not allow malformed literal strings" )]
        [MemberData( "MalformedLiteralStringsData" )]
        public void FormatShouldNotAllowMalformedLiteralStrings( DateTimeFormatProvider provider, string format )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act and assert
            Assert.Throws<FormatException>( () => provider.Format( format, date, null ) );
        }

        [Theory( DisplayName = "format should return expected built-in custom format string" )]
        [MemberData( "BuiltInCustomFormatStringData" )]
        public void FormatShouldReturnCorrectBuiltInCustomFormatString( DateTimeFormatProvider provider, string format )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var expected = date.ToString( format );

            // act
            var actual = provider.Format( format, date, null );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "format semester should return expected string" )]
        [MemberData( "SemesterFormatData" )]
        public void FormatShouldReturnCorrectSemesterFormatString( DateTimeFormatProvider provider, string format, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var actual = provider.Format( format, date, null );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "format quarter should return expected string" )]
        [MemberData( "QuarterFormatData" )]
        public void FormatShouldReturnCorrectQuarterFormatString( DateTimeFormatProvider provider, string format, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var actual = provider.Format( format, date, null );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "format should return expected custom format string" )]
        [MemberData( "CustomFormatData" )]
        public void FormatShouldReturnCorrectCustomFormatString( Func<DateTime, string> test, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );

            // act
            var actual = test( date );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "format should return expected custom format string with custom calendar" )]
        [MemberData( "CustomFormatAndCalendarData" )]
        public void FormatShouldReturnCorrectCustomFormatStringWithCustomCalendar( Func<DateTime, DateTimeFormatProvider, string> test, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var provider = new DateTimeFormatProvider( new GregorianFiscalCalendar( 7 ) );

            // act
            var actual = test( date, provider );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "string format should return custom format string with multiple parameters" )]
        [MemberData( "MultipleFormatParameterData" )]
        public void StringFormatShouldReturnCorrectCustomFormatStringWithMultipleFormatParameters( DateTimeFormatProvider provider, string format, object arg, string expected )
        {
            // arrange
            var date = new DateTime( 2010, 9, 29 );
            var args = new object[] { date, arg };

            // act
            var actual = string.Format( provider, format, args );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "format should return custom string with escape sequence" )]
        public void FormatShouldReturnCorrectCustomStringFormatContainingEscapeSequence()
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
