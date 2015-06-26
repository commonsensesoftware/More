namespace System
{
    using System;
    using System.Text.RegularExpressions;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="StringExtensions"/> class.
    /// </summary>
    public class StringExtensionsTest
    {
        [Fact( DisplayName = "is match should not allow null string" )]
        public void IsMatchShouldNotAllowNullValue()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.IsMatch( null, ".+" ) );
            Assert.Equal( "value", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.IsMatch( null, ".+", RegexOptions.Singleline ) );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "is match should not allow null or empty pattern" )]
        public void IsMatchShouldNotAllowNullOrEmptyPattern()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.IsMatch( string.Empty, null ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.IsMatch( string.Empty, string.Empty ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.IsMatch( string.Empty, null, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.IsMatch( string.Empty, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );
        }

        [Fact( DisplayName = "is match should evaluate pattern" )]
        public void IsMatchShouldProcessSuppliedText()
        {
            Assert.True( StringExtensions.IsMatch( "abc", @"\w+" ) );
            Assert.False( StringExtensions.IsMatch( "!@#$", @"\w+" ) );
            Assert.True( StringExtensions.IsMatch( "abc", @"\w+", RegexOptions.IgnoreCase ) );
            Assert.False( StringExtensions.IsMatch( "!@#$", @"\w+", RegexOptions.IgnoreCase ) );
        }

        [Fact( DisplayName = "match should not allow null string" )]
        public void MatchShouldNotAllowNullValue()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Match( null, ".+" ) );
            Assert.Equal( "value", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Match( null, ".+", RegexOptions.Singleline ) );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "match should not allow null or empty pattern" )]
        public void MatchShouldNotAllowNullOrEmptyPattern()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Match( string.Empty, null ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Match( string.Empty, string.Empty ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Match( string.Empty, null, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Match( string.Empty, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );
        }

        [Fact( DisplayName = "match should evaluate pattern" )]
        public void MatchShouldProcessSuppliedText()
        {
            Assert.NotNull( StringExtensions.Match( "abc", @"\w+" ) );
            Assert.NotNull( StringExtensions.Match( "!@#$", @"\w+" ) );
            Assert.True( StringExtensions.Match( "abc", @"\w+" ).Success );
            Assert.False( StringExtensions.Match( "!@#$", @"\w+" ).Success );
            Assert.True( StringExtensions.Match( "abc", @"\w+", RegexOptions.IgnoreCase ).Success );
            Assert.False( StringExtensions.Match( "!@#$", @"\w+", RegexOptions.IgnoreCase ).Success );
        }

        [Fact( DisplayName = "matches should not allow null" )]
        public void MatchesShouldNotAllowNullValue()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Matches( null, string.Empty ) );
            Assert.Equal( "value", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Matches( null, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "matches should not allow null or empty pattern" )]
        public void MatchesShouldNotAllowNullOrEmptyPattern()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Matches( string.Empty, null ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Matches( string.Empty, string.Empty ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Matches( string.Empty, null, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Matches( string.Empty, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );
        }

        [Fact( DisplayName = "matches should evaluate pattern" )]
        public void MatchesShouldProcessSuppliedText()
        {
            Assert.NotNull( StringExtensions.Matches( "abc", @"\w+" ) );
            Assert.NotNull( StringExtensions.Matches( "!@#$", @"\w+" ) );
            Assert.True( StringExtensions.Matches( "abc", @"\w+" ).Count == 1 );
            Assert.True( StringExtensions.Matches( "!@#$", @"\w+" ).Count == 0 );
            Assert.True( StringExtensions.Matches( "abc", @"\w+", RegexOptions.IgnoreCase ).Count == 1 );
            Assert.True( StringExtensions.Matches( "!@#$", @"\w+", RegexOptions.IgnoreCase ).Count == 0 );
        }

        [Fact( DisplayName = "split should not allow null string" )]
        public void SplitShouldNotAllowNullValue()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Split( null, string.Empty ) );
            Assert.Equal( "value", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Split( null, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "split should not allow null or empty pattern" )]
        public void SplitShouldNotAllowNullOrEmptyPattern()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Split( string.Empty, null ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Split( string.Empty, string.Empty ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Split( string.Empty, null, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Split( string.Empty, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );
        }

        [Fact( DisplayName = "split should evaluate pattern" )]
        public void SplitShouldProcessSuppliedText()
        {
            Assert.NotNull( StringExtensions.Split( "a b  c", @"\s+" ) );
            Assert.NotNull( StringExtensions.Split( "!@#$", @"\s+" ) );
            Assert.True( StringExtensions.Split( "a b  c", @"\s+" ).Length == 3 );
            Assert.True( StringExtensions.Split( "!@#$", @"\s+" ).Length == 1 );
            Assert.True( StringExtensions.Split( "a b  c", @"\s+", RegexOptions.IgnoreCase ).Length == 3 );
            Assert.True( StringExtensions.Split( "!@#$", @"\s+", RegexOptions.IgnoreCase ).Length == 1 );
        }

        [Fact( DisplayName = "replace should not allow null string" )]
        public void ReplaceShouldNotAllowNullValue()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Replace( null, string.Empty, string.Empty ) );
            Assert.Equal( "value", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Replace( null, string.Empty, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "replace should not allow null or empty pattern" )]
        public void ReplaceShouldNotAllowNullOrEmptyPattern()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Replace( string.Empty, null, string.Empty ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Replace( string.Empty, string.Empty, string.Empty ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Replace( string.Empty, null, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Replace( string.Empty, string.Empty, string.Empty, RegexOptions.Singleline ) );
            Assert.Equal( "pattern", ex.ParamName );
        }

        [Fact( DisplayName = "replace should not all null replacement" )]
        public void ReplaceShouldNotAllowNullReplacement()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Replace( string.Empty, @"\s+", null ) );
            Assert.Equal( "replacement", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Replace( string.Empty, @"\s+", null, RegexOptions.Singleline ) );
            Assert.Equal( "replacement", ex.ParamName );
        }

        [Fact( DisplayName = "replace should evaluate pattern" )]
        public void ReplaceShouldProcessSuppliedText()
        {
            Assert.NotNull( StringExtensions.Replace( "a b  c", @"\s+", string.Empty ) );
            Assert.NotNull( StringExtensions.Replace( "!@#$", @"\s+", string.Empty ) );
            Assert.True( StringExtensions.Replace( "a b  c", @"\s+", string.Empty ).Length == 3 );
            Assert.True( StringExtensions.Replace( "!@#$", @"\s+", string.Empty ).Length == 4 );
            Assert.True( StringExtensions.Replace( "a b  c", @"\s+", string.Empty, RegexOptions.IgnoreCase ).Length == 3 );
            Assert.True( StringExtensions.Replace( "!@#$", @"\s+", string.Empty, RegexOptions.IgnoreCase ).Length == 4 );
        }

        [Fact( DisplayName = "format invariant should not allow null format" )]
        public void FormatInvariantShouldNotAllowNullFormat()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.FormatInvariant( null ) );
            Assert.Equal( "format", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.FormatInvariant( null, new[] { new object(), new object() } ) );
            Assert.Equal( "format", ex.ParamName );
        }

        [Fact( DisplayName = "format invariant should return expected value" )]
        public void FormatInvariantShouldReturnExpectedValue()
        {
            var expected = "Unit Test";
            var format = expected;
            var actual = StringExtensions.FormatInvariant( format );
            Assert.Equal( expected, actual );

            format = "Unit {0}";
            actual = StringExtensions.FormatInvariant( format, "Test" );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "format default should not allow null format" )]
        public void FormatDefaultShouldNotAllowNullFormat()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.FormatDefault( null ) );
            Assert.Equal( "format", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.FormatDefault( null, new[] { new object(), new object() } ) );
            Assert.Equal( "format", ex.ParamName );
        }

        [Fact( DisplayName = "format default should return expected value" )]
        public void FormatDefaultShouldReturnExpectedValue()
        {
            var expected = "Unit Test";
            var format = expected;
            var actual = StringExtensions.FormatDefault( format );
            Assert.Equal( expected, actual );

            format = "Unit {0}";
            actual = StringExtensions.FormatDefault( format, "Test" );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "left should not allow null string" )]
        public void LeftShouldNotAllowNullString()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Left( null, 0 ) );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "left should not allow length longer than string" )]
        public void LeftShouldNotAllowLengthLongerThanTheInputString()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => StringExtensions.Left( "x", 2 ) );
            Assert.Equal( "length", ex.ParamName );
        }

        [Fact( DisplayName = "left should return expected value" )]
        public void LeftShouldReturnExpectedValue()
        {
            var target = "abc xyz";
            var actual = target.Left( 3 );
            Assert.NotNull( actual );
            Assert.Equal( 3, actual.Length );
            Assert.Equal( "abc", actual );
        }

        [Fact( DisplayName = "right should not allow null string" )]
        public void RightShouldNotAllowNullString()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => StringExtensions.Right( null, 0 ) );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "right should not allow length longer than string" )]
        public void RightShouldNotAllowLengthLongerThanTheInputString()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => StringExtensions.Right( "x", 2 ) );
            Assert.Equal( "length", ex.ParamName );
        }

        [Fact( DisplayName = "right should return expected value" )]
        public void RightShouldReturnExpectedValue()
        {
            var target = "abc xyz";
            var actual = target.Right( 3 );
            Assert.NotNull( actual );
            Assert.Equal( 3, actual.Length );
            Assert.Equal( "xyz", actual );
        }
    }
}
