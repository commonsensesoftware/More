namespace More
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a custom date and time format provider, which supports the standard date and time format specifiers as well as
    /// custom specifiers for quarter and semester.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This format provider supports all of the built-in <see href="http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx">custom date and time format strings</see>,
    /// in addition to the following custom date and time format strings:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Format specifier</term>
    ///         <description>Description</description>
    ///         <description>Examples</description>
    ///     </listheader>
    ///     <item>
    ///         <term>"q"</term>
    ///         <description>The quarter of the year, from 1 through 4.</description>
    ///         <description>
    ///             <para>2/1/2009 1:45:30 PM -> 1</para>
    ///             <para>7/1/2009 1:45:30 PM -> 3</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>"qq"</term>
    ///         <description>The quarter of the year, from 01 through 04.</description>
    ///         <description>
    ///             <para>2/1/2009 1:45:30 PM -> 01</para>
    ///             <para>7/1/2009 1:45:30 PM -> 03</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>"qqq"</term>
    ///         <description>The abbreviated quarter of the year.</description>
    ///         <description>
    ///             <para>2/1/2009 1:45:30 PM -> Q1</para>
    ///             <para>7/1/2009 1:45:30 PM -> Q3</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>"qqqq"</term>
    ///         <description>The full name of the quarter of the year.</description>
    ///         <description>
    ///             <para>2/1/2009 1:45:30 PM -> Quarter 1</para>
    ///             <para>7/1/2009 1:45:30 PM -> Quarter 3</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>"S"</term>
    ///         <description>The semester of the year, from 1 through 2.</description>
    ///         <description>
    ///             <para>2/1/2009 1:45:30 PM -> 1</para>
    ///             <para>7/1/2009 1:45:30 PM -> 2</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>"SS"</term>
    ///         <description>The semester of the year, from 01 through 02.</description>
    ///         <description>
    ///             <para>2/1/2009 1:45:30 PM -> 01</para>
    ///             <para>7/1/2009 1:45:30 PM -> 02</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>"SSS"</term>
    ///         <description>The abbreviated semester of the year.</description>
    ///         <description>
    ///             <para>2/1/2009 1:45:30 PM -> H1</para>
    ///             <para>7/1/2009 1:45:30 PM -> H2</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>"SSSS"</term>
    ///         <description>The full name of the semester of the year.</description>
    ///         <description>
    ///             <para>2/1/2009 1:45:30 PM -> Semester 1</para>
    ///             <para>7/1/2009 1:45:30 PM -> Semester 3</para>
    ///         </description>
    ///     </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>This example demonstrates how using the an implementation of ToString which will leverage the <see cref="ICustomFormatter"/> interface.
    /// <code lang="C#">
    /// <![CDATA[
    /// using System;
    /// 
    /// var formatProvider = new DateTimeFormatProvider();
    /// var dateTime = new DateTime( 2010, 7, 1 );
    /// 
    /// Console.WriteLine( string.Format( formatProvider, "{0:'FY'y, 'H'S, 'Q'q, 'M'M, 'D'd}", dateTime ) );
    /// Console.WriteLine( string.Format( formatProvider, "{0:'FY'yy, 'H'SS, 'Q'qq, 'M'MM, 'D'dd}", dateTime ) );
    /// Console.WriteLine( string.Format( formatProvider, "{0:'FY'yyy, SSS, qqq, MMM, 'D'd}", dateTime ) );
    /// Console.WriteLine( string.Format( formatProvider, "{0:'Fiscal Year' yyyy, SSSS, qqqq, MMMM, d}", dateTime ) );
    /// 
    /// // prints:
    /// // "FY10, H2, Q3, M7, D1"
    /// // "FY10, H02, Q03, M07, D01"
    /// // "FY10, H2, Q3, Jul, D1"
    /// // "Fiscal Year 2010, Semester 2, Quarter 3, July, 1"
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class DateTimeFormatProvider : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// Represents a parsed string format token.
        /// </summary>
        private sealed class DateTimeFormatToken
        {
            internal readonly string Format;
            internal readonly bool IsLiteral;
            internal readonly bool IsInvalid;

            internal DateTimeFormatToken( string format )
                : this( format, false, false )
            {
                Contract.Requires( format != null );
            }

            internal DateTimeFormatToken( string format, bool literal )
                : this( format, literal, false )
            {
                Contract.Requires( format != null );
            }

            internal DateTimeFormatToken( string format, bool literal, bool invalid )
            {
                Contract.Requires( format != null );
                this.Format = format;
                this.IsLiteral = literal;
                this.IsInvalid = invalid;
            }
        }

        /// <summary>
        /// Represents a simple utility class to tokenize a date and time format string.
        /// </summary>
        private static class DateTimeFormatTokenizer
        {
            private static bool IsLiteralDelimiter( char ch )
            {
                return ch == '\'' || ch == '\"';
            }

            private static bool IsFormatSpecifier( char ch )
            {
                switch ( ch )
                {
                    case 'F':
                    case 'H':
                    case 'K':
                    case 'M':
                    case 'S':
                    case 'd':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'm':
                    case 'q':
                    case 's':
                    case 't':
                    case 'y':
                    case 'z':
                        return true;
                }

                return false;
            }

            private static bool IsEscapeSequence( string sequence )
            {
                Contract.Requires( sequence != null );
                Contract.Requires( sequence.Length == 2 );

                switch ( sequence )
                {
                    case @"\'":
                    case @"\\":
                    case @"\F":
                    case @"\H":
                    case @"\K":
                    case @"\M":
                    case @"\S":
                    case @"\d":
                    case @"\f":
                    case @"\g":
                    case @"\h":
                    case @"\m":
                    case @"\q":
                    case @"\s":
                    case @"\t":
                    case @"\y":
                    case @"\z":
                        return true;
                }

                return false;
            }

            private static bool IsSingleCustomFormatSpecifier( string sequence )
            {
                Contract.Requires( sequence != null );
                Contract.Requires( sequence.Length == 2 );

                switch ( sequence )
                {
                    case "%d":
                    case "%f":
                    case "%g":
                    case "%h":
                    case "%m":
                    case "%q":
                    case "%s":
                    case "%t":
                    case "%y":
                    case "%z":
                    case "%F":
                    case "%H":
                    case "%K":
                    case "%M":
                    case "%S":
                        return true;
                }

                return false;
            }

            private static void EnsureCurrentLiteralSequenceTerminated( IList<DateTimeFormatToken> tokens, StringBuilder token )
            {
                Contract.Requires( tokens != null );
                Contract.Requires( token != null );

                if ( token.Length > 0 )
                {
                    tokens.Add( new DateTimeFormatToken( token.ToString(), true ) );
                    token.Length = 0;
                }
            }

            internal static IEnumerable<DateTimeFormatToken> Tokenize( string format )
            {
                Contract.Requires( !string.IsNullOrEmpty( format ) );
                Contract.Ensures( Contract.Result<IEnumerable<DateTimeFormatToken>>() != null );
                var tokens = new List<DateTimeFormatToken>();

                var token = new StringBuilder();

                for ( var i = 0; i < format.Length; i++ )
                {
                    var ch = format[i];

                    if ( IsLiteralDelimiter( ch ) )
                    {
                        EnsureCurrentLiteralSequenceTerminated( tokens, token );

                        var delimiter = ch;
                        var current = '\0';

                        while ( ( ++i < format.Length ) && ( ( current = format[i] ) != delimiter ) )
                            token.Append( current );

                        // literal string
                        tokens.Add( new DateTimeFormatToken( token.ToString(), true, current != delimiter ) );
                        token.Length = 0;
                    }
                    else if ( ( ch == '\\' ) && ( i < format.Length - 1 ) && IsEscapeSequence( format.Substring( i, 2 ) ) )
                    {
                        EnsureCurrentLiteralSequenceTerminated( tokens, token );

                        // escape sequence of \<c>
                        tokens.Add( new DateTimeFormatToken( format.Substring( ++i, 1 ), true ) );
                        token.Length = 0;
                    }
                    else if ( ( ch == '%' ) && ( i < format.Length - 1 ) && IsSingleCustomFormatSpecifier( format.Substring( i, 2 ) ) )
                    {
                        EnsureCurrentLiteralSequenceTerminated( tokens, token );

                        // single specifier custom format of %<c>
                        tokens.Add( new DateTimeFormatToken( format.Substring( ++i, 1 ) ) );
                        token.Length = 0;
                    }
                    else if ( IsFormatSpecifier( ch ) )
                    {
                        EnsureCurrentLiteralSequenceTerminated( tokens, token );

                        token.Append( ch );

                        var last = ch;

                        while ( ( ++i < format.Length ) && ( ( ch = format[i] ) == last ) )
                            token.Append( ch );

                        // format specifier sequence <c>{1,}
                        tokens.Add( new DateTimeFormatToken( token.ToString() ) );
                        token.Length = 0;

                        // if this isn't the last character, go back one
                        if ( i != format.Length )
                            --i;
                    }
                    else
                    {
                        // any literal character
                        token.Append( ch );
                    }
                }

                return tokens;
            }
        }

        private readonly DateTimeFormatInfo dateTimeFormat;
        private readonly Calendar calendar;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeFormatProvider"/> class.
        /// </summary>
        public DateTimeFormatProvider()
            : this( DateTimeFormatInfo.CurrentInfo, DateTimeFormatInfo.CurrentInfo.Calendar )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeFormatProvider"/> class.
        /// </summary>
        /// <param name="dateTimeFormat">The <see cref="DateTimeFormatInfo"/> used by the format provider.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public DateTimeFormatProvider( DateTimeFormatInfo dateTimeFormat )
            : this( dateTimeFormat, dateTimeFormat.Calendar )
        {
            Arg.NotNull( dateTimeFormat, "dateTimeFormat" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeFormatProvider"/> class.
        /// </summary>
        /// <param name="calendar">The <see cref="Calendar"/> used by the format provider.</param>
        public DateTimeFormatProvider( Calendar calendar )
            : this( DateTimeFormatInfo.CurrentInfo, calendar )
        {
            Arg.NotNull( calendar, "calendar" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeFormatProvider"/> class.
        /// </summary>
        /// <param name="dateTimeFormat">The <see cref="DateTimeFormatInfo"/> used by the format provider.</param>
        /// <param name="calendar">The <see cref="Calendar"/> used by the format provider.</param>
        public DateTimeFormatProvider( DateTimeFormatInfo dateTimeFormat, Calendar calendar )
        {
            Arg.NotNull( dateTimeFormat, "dateTimeFormat" );
            Arg.NotNull( calendar, "calendar" );
            this.dateTimeFormat = dateTimeFormat;
            this.calendar = calendar;
        }

        /// <summary>
        /// Gets the underlying date and time format information.
        /// </summary>
        /// <value>A <see cref="DateTimeFormatInfo"/> object.</value>
        protected DateTimeFormatInfo DateTimeFormat
        {
            get
            {
                Contract.Ensures( this.dateTimeFormat != null );
                return this.dateTimeFormat;
            }
        }

        /// <summary>
        /// Gets the calendar associated with the format provider.
        /// </summary>
        /// <value>A <see cref="Calendar"/> object.</value>
        /// <remarks>The <see cref="P:DateTimeFormatInfo.Calendar"/> cannot be assigned to a custom calendar.  This property provides the
        /// ability to use a custom calendar if one is specified in the constructor.</remarks>
        protected Calendar Calendar
        {
            get
            {
                Contract.Ensures( this.calendar != null );
                return this.calendar;
            }
        }

        private static string GetDefaultFormat( string format, object arg, IFormatProvider formatProvider )
        {
            if ( arg == null )
                return format ?? string.Empty;

            if ( !string.IsNullOrEmpty( format ) )
            {
                var formattable = arg as IFormattable;

                if ( formattable != null )
                    return formattable.ToString( format, formatProvider );
            }

            return arg.ToString();
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        private string GetCustomFormat( DateTime value, string format, IFormatProvider formatProvider )
        {
            Contract.Requires( !string.IsNullOrEmpty( format ) );
            Contract.Ensures( Contract.Result<string>() != null );

            switch ( format[0] )
            {
                case 'M':
                    return this.FormatMonthPart( value, format, formatProvider );
                case 'S':
                    return this.FormatSemesterPart( value, format, formatProvider );
                case 'q':
                    return this.FormatQuarterPart( value, format, formatProvider );
                case 'y':
                    return this.FormatYearPart( value, format, formatProvider );
            }

            return this.FormatDatePart( value, format, formatProvider );
        }

        private static int ToDateTimeMonth( Calendar calendar, DateTime value )
        {
            Contract.Requires( calendar != null );
            Contract.Ensures( Contract.Result<int>() > 0 );

            var epoch = calendar.FirstMonthOfYear();
            var month = calendar.GetMonth( value );
            var actual = ( epoch + month ) - 1;

            return actual > 12 ? ( actual - 12 ) : actual;
        }

        /// <summary>
        /// Formats the year part of the specified date using the provided format.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to format the year for.</param>
        /// <param name="format">The format string for the year segment of the date.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> used to apply the format.</param>
        /// <returns>A formatted <see cref="String">string</see> representing the year.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        protected virtual string FormatYearPart( DateTime value, string format, IFormatProvider formatProvider )
        {
            Arg.NotNullOrEmpty( format, "format" );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            switch ( format.Length )
            {
                case 1: // y
                    return ( this.Calendar.Year( value ) % 100 ).ToString( formatProvider );
                case 2: // yy
                    return ( this.Calendar.Year( value ) % 100 ).ToString( "00", formatProvider );
                case 3: // yyy
                    return this.Calendar.Year( value ).ToString( "000", formatProvider );
            }

            // yyyy*
            return this.Calendar.Year( value ).ToString( formatProvider );
        }

        /// <summary>
        /// Formats the semester part of the specified date using the provided format.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to format the semester for.</param>
        /// <param name="format">The format string for the semester segment of the date.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> used to apply the format.</param>
        /// <returns>A formatted <see cref="String">string</see> representing the semester.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        protected virtual string FormatSemesterPart( DateTime value, string format, IFormatProvider formatProvider )
        {
            Arg.NotNullOrEmpty( format, "format" );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            switch ( format.Length )
            {
                case 1: // S
                    return value.Semester( this.Calendar ).ToString( formatProvider );
                case 2: // SS
                    return value.Semester( this.Calendar ).ToString( "00", formatProvider );
                case 3: // SSS
                    return string.Format( formatProvider, SR.ShortSemesterFormat, value.Semester( this.Calendar ) );
            }

            // SSSS*
            return string.Format( formatProvider, SR.SemesterFormat, value.Semester( this.Calendar ) );
        }

        /// <summary>
        /// Formats the quarter part of the specified date using the provided format.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to format the quarter for.</param>
        /// <param name="format">The format string for the quarter segment of the date.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> used to apply the format.</param>
        /// <returns>A formatted <see cref="String">string</see> representing the quarter.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        protected virtual string FormatQuarterPart( DateTime value, string format, IFormatProvider formatProvider )
        {
            Arg.NotNullOrEmpty( format, "format" );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            switch ( format.Length )
            {
                case 1: // q
                    return value.Quarter( this.Calendar ).ToString( formatProvider );
                case 2: // qq
                    return value.Quarter( this.Calendar ).ToString( "00", formatProvider );
                case 3: // qqq
                    return string.Format( formatProvider, SR.ShortQuarterFormat, value.Quarter( this.Calendar ) );
            }

            // qqqq*
            return string.Format( formatProvider, SR.QuarterFormat, value.Quarter( this.Calendar ) );
        }

        /// <summary>
        /// Formats the month part of the specified date using the provided format.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to format the month for.</param>
        /// <param name="format">The format string for the month segment of the date.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> used to apply the format.</param>
        /// <returns>A formatted <see cref="String">string</see> representing the month.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        protected virtual string FormatMonthPart( DateTime value, string format, IFormatProvider formatProvider )
        {
            Arg.NotNullOrEmpty( format, "format" );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            // NOTE: the DateTimeFormatInfo cannot be overridden or supplied an alternate calendar.
            // When the ordinal month value is needed, we differ to the calendar; otherwise, we
            // just use the default DateTime.Month because the DateTimeFormatInfo does not understand
            // translated months (ex: a custom calendar such as GregorianFiscalCalendar)

            switch ( format.Length )
            {
                case 1: // M
                    return this.Calendar.GetMonth( value ).ToString( formatProvider );
                case 2: // MM
                    return this.Calendar.GetMonth( value ).ToString( "00", formatProvider );
                case 3: // MMM
                    return this.DateTimeFormat.GetAbbreviatedMonthName( ToDateTimeMonth( this.Calendar, value ) );
            }

            // MMMM*
            return this.DateTimeFormat.GetMonthName( ToDateTimeMonth( this.Calendar, value ) );
        }

        /// <summary>
        /// Formats the date part of the specified date using the provided format.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> to format.</param>
        /// <param name="format">The format string for the date segment.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> used to apply the format.</param>
        /// <returns>A formatted <see cref="String">string</see> representing the date part indicated by the format string.</returns>
        /// <remarks>This method is invoked for all standard date part formats that are not overriden such as day, time, and time zone.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        protected virtual string FormatDatePart( DateTime value, string format, IFormatProvider formatProvider )
        {
            Arg.NotNullOrEmpty( format, "format" );
            Contract.Ensures( Contract.Result<string>() != null );

            // since we're going to let the DateTime do its normal thing, we need to prepend the %
            // when the format string is a single character; otherwise, it will be confused with
            // one of the standard format strings
            if ( format.Length == 1 )
                format = "%" + format;

            // everything else; use the DateTimeFormatInfo supplied to the format provider
            return value.ToString( format, this.DateTimeFormat ) ?? string.Empty;
        }

        /// <summary>
        /// Returns the formatter for the requested type.
        /// </summary>
        /// <param name="formatType">The <see cref="Type">type</see> of requested formatter.</param>
        /// <returns>A <see cref="DateTimeFormatInfo"/>, <see cref="ICustomFormatter"/>, or <c>null</c> depending on the requested <paramref name="formatType">format type</paramref>.</returns>
        public object GetFormat( Type formatType )
        {
            if ( typeof( DateTimeFormatInfo ).Equals( formatType ) )
                return this.DateTimeFormat;
            else if ( typeof( ICustomFormatter ).Equals( formatType ) )
                return this;

            return null;
        }

        /// <summary>
        /// Formats the provided argument with the specified format and provider.
        /// </summary>
        /// <param name="format">The format string to apply to the argument.</param>
        /// <param name="arg">The argument to format.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> used to format the argument.</param>
        /// <returns>A <see cref="String">string</see> represeting the formatted argument.</returns>
        public virtual string Format( string format, object arg, IFormatProvider formatProvider )
        {
            // we don't know how to format anything else except for DateTime
            if ( !( arg is DateTime ) )
                return GetDefaultFormat( format, arg, formatProvider );
            else if ( string.IsNullOrEmpty( format ) )
                return ( (DateTime) arg ).ToString( formatProvider );

            var tokens = DateTimeFormatTokenizer.Tokenize( format );

            if ( tokens.Any( t => t.IsInvalid ) )
                throw new FormatException( ExceptionMessage.InvalidFormatString );

            var date = (DateTime) arg;
            var text = new StringBuilder();
            formatProvider = formatProvider == null || object.ReferenceEquals( this, formatProvider ) ? CultureInfo.CurrentCulture : formatProvider;

            foreach ( var token in tokens )
                text.Append( token.IsLiteral ? token.Format : this.GetCustomFormat( date, token.Format, formatProvider ) );

            return text.ToString();
        }
    }
}