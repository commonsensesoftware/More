namespace System
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides extension methods for the <see cref="String"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns whether the current string contains the specified other string.
        /// </summary>
        /// <param name="current">The <see cref="String">string</see> to evaluate.</param>
        /// <param name="value">The <see cref="String">string</see> to search for.</param>
        /// <param name="comparisonType">One of the <see cref="StringComparison"/> values.</param>
        /// <returns>True if the current string contains the specified other string; otherwise, false.</returns>
        [Pure]
        public static bool Contains( this string current, string value, StringComparison comparisonType )
        {
            if ( string.IsNullOrEmpty( current ) || string.IsNullOrEmpty( value ) )
            {
                return false;
            }

            return current.IndexOf( value, comparisonType ) >= 0;
        }

        /// <summary>
        /// Indicates whether the regular expression finds a match in the input string, using the regular expression
        /// specified in the pattern parameter.
        /// </summary>
        /// <param name="value">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>True if the regular expression finds a match; otherwise, false.</returns>
        [Pure]
        public static bool IsMatch( this string value, string pattern )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            return Regex.IsMatch( value, pattern, RegexOptions.None );
        }

        /// <summary>
        /// Indicates whether the regular expression finds a match in the input string, using the regular expression
        /// specified in the pattern parameter and the matching options supplied in the options parameter.
        /// </summary>
        /// <param name="value">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">A bitwise OR combination of <see cref="RegexOptions"/> enumeration values.</param>
        /// <returns>True if the regular expression finds a match; otherwise, false.</returns>
        [Pure]
        public static bool IsMatch( this string value, string pattern, RegexOptions options )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            return Regex.IsMatch( value, pattern, options );
        }

        /// <summary>
        /// Searches the input string for an occurrence of the regular expression supplied in a pattern parameter.
        /// </summary>
        /// <param name="value">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>A regular expression <see cref="System.Text.RegularExpressions.Match"/> object.</returns>
        [Pure]
        public static Match Match( this string value, string pattern )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            Contract.Ensures( Contract.Result<Match>() != null );
            return Regex.Match( value, pattern, RegexOptions.None );
        }

        /// <summary>
        /// Searches the input string for an occurrence of the regular expression supplied in a pattern parameter
        /// with matching options supplied in an options parameter.
        /// </summary>
        /// <param name="value">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">A bitwise OR combination of <see cref="RegexOptions"/> enumeration values.</param>
        /// <returns>A regular expression <see cref="System.Text.RegularExpressions.Match"/> object.</returns>
        [Pure]
        public static Match Match( this string value, string pattern, RegexOptions options )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            Contract.Ensures( Contract.Result<Match>() != null );
            return Regex.Match( value, pattern, options );
        }

        /// <summary>
        /// Searches the specified input string for all occurrences of the regular expression supplied in a pattern parameter.
        /// </summary>
        /// <param name="value">The string to search for matches.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>The <see cref="MatchCollection"/> of <see cref="System.Text.RegularExpressions.Match"/> objects found by the search.</returns>
        [Pure]
        public static MatchCollection Matches( this string value, string pattern )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            Contract.Ensures( Contract.Result<MatchCollection>() != null );
            return Regex.Matches( value, pattern, RegexOptions.None );
        }

        /// <summary>
        /// Searches the specified input string for all occurrences of the regular expression supplied in a pattern
        /// parameter with matching options supplied in an options parameter.
        /// </summary>
        /// <param name="value">The string to search for matches.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">A bitwise OR combination of <see cref="RegexOptions"/> enumeration values.</param>
        /// <returns>The <see cref="MatchCollection"/> of <see cref="System.Text.RegularExpressions.Match"/> objects found by the search.</returns>
        [Pure]
        public static MatchCollection Matches( this string value, string pattern, RegexOptions options )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            Contract.Ensures( Contract.Result<MatchCollection>() != null );
            return Regex.Matches( value, pattern, options );
        }

        /// <summary>
        /// Splits the input string at the positions defined by a specified regular expression pattern.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <returns>A <see cref="String"/> array.</returns>
        [Pure]
        public static string[] Split( this string value, string pattern )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            Contract.Ensures( Contract.Result<string[]>() != null );
            return Regex.Split( value, pattern, RegexOptions.None );
        }

        /// <summary>
        /// Splits the input string at the positions defined by a specified regular expression pattern.
        /// Specified options modify the matching operation.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">A bitwise OR combination of <see cref="RegexOptions"/> enumeration values.</param>
        /// <returns>A <see cref="String"/> array.</returns>
        [Pure]
        public static string[] Split( this string value, string pattern, RegexOptions options )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            Contract.Ensures( Contract.Result<string[]>() != null );
            return Regex.Split( value, pattern, options );
        }

        /// <summary>
        /// Within a specified input string, replaces all strings that match a specified regular expression with a
        /// specified replacement string. Specified options modify the matching operation.
        /// </summary>
        /// <param name="value">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="replacement">The replacement string.</param>
        /// <param name="options">A bitwise OR combination of <see cref="RegexOptions"/> enumeration values.</param>
        /// <returns>A new <see cref="String">string</see> that is identical to the input string, except that a replacement <see cref="String">string</see>
        /// takes the place of each matched <see cref="String">string</see>.</returns>
        [Pure]
        public static string Replace( this string value, string pattern, string replacement, RegexOptions options )
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            Arg.NotNull( replacement, nameof( replacement ) );
            Contract.Ensures( Contract.Result<string>() != null );
            return Regex.Replace( value, pattern, replacement, options );
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string within the current instance are replaced with another specified string.
        /// </summary>
        /// <param name="current">The <see cref="String">string</see> to make replacements in.</param>
        /// <param name="oldValue">The old value to match.</param>
        /// <param name="newValue">The new replacement value.</param>
        /// <param name="comparisonType">The type of <see cref="StringComparison">string comparison</see> to use.</param>
        /// <returns>A new <see cref="String">string</see> containing the result of the replacement operation.</returns>
        [Pure]
        public static string Replace( this string current, string oldValue, string newValue, StringComparison comparisonType )
        {
            if ( string.IsNullOrEmpty( current ) || string.IsNullOrEmpty( oldValue ) )
            {
                return current;
            }

            if ( comparisonType == StringComparison.Ordinal )
            {
                return current.Replace( oldValue, newValue );
            }

            var replaced = new StringBuilder();
            var previousIndex = 0;
            var index = current.IndexOf( oldValue, comparisonType );

            while ( index >= 0 )
            {
                replaced.Append( current.Substring( previousIndex, index - previousIndex ) );
                replaced.Append( newValue );
                index += oldValue.Length;
                previousIndex = index;
                index = current.IndexOf( oldValue, index, comparisonType );
            }

            replaced.Append( current.Substring( previousIndex ) );

            return replaced.ToString();
        }

        /// <summary>
        /// Formats the specified string using the specified format and arguments.
        /// </summary>
        /// <param name="format">The extended format <see cref="String"/>.</param>
        /// <param name="args">An array of <see cref="Object"/> containing the ordinal format parameters, if any.</param>
        /// <returns>A formatted <see cref="String"/>.</returns>
        /// <remarks>This method uses the <see cref="CultureInfo.InvariantCulture"/> for the <see cref="IFormatProvider"/>.</remarks>
        [Pure]
        public static string FormatInvariant( this string format, params object[] args )
        {
            Arg.NotNull( format, nameof( format ) );
            Contract.Ensures( Contract.Result<string>() != null );
            return string.Format( CultureInfo.InvariantCulture, format, args );
        }

        /// <summary>
        /// Formats the specified string using the specified format and arguments.
        /// </summary>
        /// <param name="format">The extended format <see cref="String"/>.</param>
        /// <param name="args">An array of <see cref="Object"/> containing the ordinal format parameters, if any.</param>
        /// <returns>A formatted <see cref="String"/>.</returns>
        /// <remarks>This method uses the <see cref="CultureInfo.CurrentCulture"/> for the <see cref="IFormatProvider"/>.</remarks>
        [Pure]
        public static string FormatDefault( this string format, params object[] args )
        {
            Arg.NotNull( format, nameof( format ) );
            Contract.Ensures( Contract.Result<string>() != null );
            return string.Format( CultureInfo.CurrentCulture, format, args );
        }

        /// <summary>
        /// Returns the specified length of characters from the lefthand side of the provided string.
        /// </summary>
        /// <param name="value">The extended <see cref="String">string</see>.</param>
        /// <param name="length">The length of the new string.</param>
        /// <returns>A <see cref="String">string</see> containing the leftmost characters of the provided length.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static string Left( this string value, int length )
        {
            Arg.NotNull( value, nameof( value ) );
            Contract.Ensures( Contract.Result<string>() != null );
            Contract.Ensures( Contract.Result<string>().Length == length );
            Arg.LessThanOrEqualTo( length, value.Length, nameof( length ) );

            return value.Length == length ? value : value.Substring( 0, length );
        }

        /// <summary>
        /// Returns the specified length of characters from the righthand side of the provided string.
        /// </summary>
        /// <param name="value">The extended <see cref="String">string</see>.</param>
        /// <param name="length">The length of the new string.</param>
        /// <returns>A <see cref="String">string</see> containing the rightmost characters of the provided length.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static string Right( this string value, int length )
        {
            Arg.NotNull( value, nameof( value ) );
            Contract.Ensures( Contract.Result<string>() != null );
            Contract.Ensures( Contract.Result<string>().Length == length );
            Arg.LessThanOrEqualTo( length, value.Length, nameof( length ) );

            return value.Length == length ? value : value.Substring( value.Length - length, length );
        }
    }
}