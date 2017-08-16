namespace System
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Xunit;
    using static System.String;
    using static System.Text.RegularExpressions.RegexOptions;

    public class StringExtensionsTest
    {
        public static IEnumerable<object[]> IsMatchValueData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.IsMatch( default( string ), ".+" ) ) };
                yield return new object[] { new Action( () => StringExtensions.IsMatch( default( string ), ".+", Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( IsMatchValueData ) )]
        public void is_match_should_not_allow_null_string( Action isMatch )
        {
            // arrange
            var value = default( string );

            // act

            // assert
            isMatch.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        public static IEnumerable<object[]> IsMatchPatternData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.IsMatch( Empty, null ) ) };
                yield return new object[] { new Action( () => StringExtensions.IsMatch( Empty, Empty ) ) };
                yield return new object[] { new Action( () => StringExtensions.IsMatch( Empty, null, Singleline ) ) };
                yield return new object[] { new Action( () => StringExtensions.IsMatch( Empty, Empty, Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( IsMatchPatternData ) )]
        public void is_match_should_not_allow_null_or_empty_pattern( Action isMatch )
        {
            // arrange
            var pattern = default( string );

            // act

            // assert
            isMatch.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( pattern ) );
        }

        public static IEnumerable<object[]> IsMatchData
        {
            get
            {
                yield return new object[] { new Func<bool>( () => "abc".IsMatch( @"\w+" ) ), true };
                yield return new object[] { new Func<bool>( () => "!@#$".IsMatch( @"\w+" ) ), false };
                yield return new object[] { new Func<bool>( () => "abc".IsMatch( @"\w+", IgnoreCase ) ), true };
                yield return new object[] { new Func<bool>( () => "!@#$".IsMatch( @"\w+", IgnoreCase ) ), false };

            }
        }

        [Theory]
        [MemberData( nameof( IsMatchData ) )]
        public void is_match_should_evaluate_pattern( Func<bool> isMatch, bool expected )
        {
            // arrange

            // act
            var result = isMatch();

            // assert
            result.Should().Be( expected );
        }

        public static IEnumerable<object[]> MatchValueData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.Match( default( string ), ".+" ) ) };
                yield return new object[] { new Action( () => StringExtensions.Match( default( string ), ".+", Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( MatchValueData ) )]
        public void match_should_not_allow_null_string( Action match )
        {
            // arrange
            var value = default( string );

            // act

            // assert
            match.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        public static IEnumerable<object[]> MatchPatternData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.Match( Empty, null ) ) };
                yield return new object[] { new Action( () => StringExtensions.Match( Empty, Empty ) ) };
                yield return new object[] { new Action( () => StringExtensions.Match( Empty, null, Singleline ) ) };
                yield return new object[] { new Action( () => StringExtensions.Match( Empty, Empty, Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( MatchPatternData ) )]
        public void match_should_not_allow_null_or_empty_pattern( Action match )
        {
            // arrange
            var pattern = default( string );

            // act

            // assert
            match.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( pattern ) );
        }

        public static IEnumerable<object[]> MatchData
        {
            get
            {
                yield return new object[] { new Func<Match>( () => "abc".Match( @"\w+" ) ), true };
                yield return new object[] { new Func<Match>( () => "!@#$".Match( @"\w+" ) ), false };
                yield return new object[] { new Func<Match>( () => "abc".Match( @"\w+", IgnoreCase ) ), true };
                yield return new object[] { new Func<Match>( () => "!@#$".Match( @"\w+", IgnoreCase ) ), false };
            }
        }

        [Theory]
        [MemberData( nameof( MatchData ) )]
        public void match_should_evaluate_pattern( Func<Match> match, bool expected )
        {
            // arrange


            // act
            var result = match();

            // assert
            result.Success.Should().Be( expected );
        }

        public static IEnumerable<object[]> MatchesValueData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.Match( null, Empty ) ) };
                yield return new object[] { new Action( () => StringExtensions.Match( null, Empty, Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( MatchesValueData ) )]
        public void matches_should_not_allow_null( Action matches )
        {
            // arrange
            var value = default( string );

            // act

            // assert
            matches.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        public static IEnumerable<object[]> MatchesPatternData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.Match( Empty, null ) ) };
                yield return new object[] { new Action( () => StringExtensions.Match( Empty, Empty ) ) };
                yield return new object[] { new Action( () => StringExtensions.Match( Empty, null, Singleline ) ) };
                yield return new object[] { new Action( () => StringExtensions.Match( Empty, Empty, Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( MatchesPatternData ) )]
        public void matches_should_not_allow_null_or_empty_pattern( Action matches )
        {
            // arrange
            var pattern = default( string );

            // act

            // assert
            matches.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( pattern ) );
        }

        public static IEnumerable<object[]> MatchesData
        {
            get
            {
                yield return new object[] { new Func<MatchCollection>( () => "abc".Matches( @"\w+" ) ), 1 };
                yield return new object[] { new Func<MatchCollection>( () => "!@#$".Matches( @"\w+" ) ), 0 };
                yield return new object[] { new Func<MatchCollection>( () => "abc".Matches( @"\w+", IgnoreCase ) ), 1 };
                yield return new object[] { new Func<MatchCollection>( () => "!@#$".Matches( @"\w+", IgnoreCase ) ), 0 };
            }
        }

        [Theory]
        [MemberData( nameof( MatchesData ) )]
        public void matches_should_evaluate_pattern( Func<MatchCollection> matches, int count )
        {
            // arrange

            // act
            var collection = matches();

            // assert
            collection.Should().HaveCount( count );
        }

        public static IEnumerable<object[]> SplitValueData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.Split( null, Empty ) ) };
                yield return new object[] { new Action( () => StringExtensions.Split( null, Empty, Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( SplitValueData ) )]
        public void split_should_not_allow_null_string( Action split )
        {
            // arrange
            var value = default( string );

            // act

            // assert
            split.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        public static IEnumerable<object[]> SplitPatternData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.Split( Empty, null ) ) };
                yield return new object[] { new Action( () => StringExtensions.Split( Empty, Empty ) ) };
                yield return new object[] { new Action( () => StringExtensions.Split( Empty, null, Singleline ) ) };
                yield return new object[] { new Action( () => StringExtensions.Split( Empty, Empty, Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( SplitPatternData ) )]
        public void split_should_not_allow_null_or_empty_pattern( Action split )
        {
            // arrange
            var pattern = default( string );

            // act

            // assert
            split.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( pattern ) );
        }

        public static IEnumerable<object[]> SplitData
        {
            get
            {
                yield return new object[] { new Func<string[]>( () => "a b  c".Split( @"\s+" ) ), 3 };
                yield return new object[] { new Func<string[]>( () => "!@#$".Split( @"\s+" ) ), 1 };
                yield return new object[] { new Func<string[]>( () => "a b  c".Split( @"\s+", IgnoreCase ) ), 3 };
                yield return new object[] { new Func<string[]>( () => "!@#$".Split( @"\s+", IgnoreCase ) ), 1 };
            }
        }

        [Theory]
        [MemberData( nameof( SplitData ) )]
        public void split_should_evaluate_pattern( Func<string[]> split, int count )
        {
            // arrange

            // act
            var results = split();

            // assert
            results.Length.Should().Be( count );
        }

        [Fact]
        public void replace_should_not_allow_null_string()
        {
            // arrange
            var value = default( string );

            // act
            Action replace = () => StringExtensions.Replace( null, Empty, Empty, Singleline );

            // assert
            replace.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        public static IEnumerable<object[]> ReplacePatternData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.Replace( Empty, null, Empty, Singleline ) ) };
                yield return new object[] { new Action( () => StringExtensions.Replace( Empty, Empty, Empty, Singleline ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( ReplacePatternData ) )]
        public void replace_should_not_allow_null_or_empty_pattern( Action replace )
        {
            // arrange
            var pattern = default( string );

            // act

            // assert
            replace.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( pattern ) );
        }

        [Fact]
        public void replace_should_not_allow_null_replacement()
        {
            // arrange
            var replacement = default( string );

            // act
            Action replace = () => StringExtensions.Replace( Empty, @"\s+", null, Singleline );

            // assert
            replace.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( replacement ) );
        }

        public static IEnumerable<object[]> ReplaceData
        {
            get
            {
                yield return new object[] { new Func<string>( () => "!@#$".Replace( @"\s+", Empty ) ), 4 };
                yield return new object[] { new Func<string>( () => "a b  c".Replace( @"\s+", Empty, IgnoreCase ) ), 3 };
                yield return new object[] { new Func<string>( () => "!@#$".Replace( @"\s+", Empty, IgnoreCase ) ), 4 };
            }
        }

        [Theory]
        [MemberData( nameof( ReplaceData ) )]
        public void replace_should_evaluate_pattern( Func<string> replace, int expected )
        {
            // arrange


            // act
            var result = replace();

            // assert
            result.Length.Should().Be( expected );
        }

        [Fact]
        public void replace_should_use_string_comparison()
        {
            // arrange
            var text = "test EQUALS test";

            // act
            var result = text.Replace( "equals", "equals", StringComparison.OrdinalIgnoreCase );

            // assert
            result.Should().Be( "test equals test" );
        }

        public static IEnumerable<object[]> FormatInvariantFormatData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.FormatInvariant( null ) ) };
                yield return new object[] { new Action( () => StringExtensions.FormatInvariant( null, new[] { new object() } ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( FormatInvariantFormatData ) )]
        public void format_invariant_should_not_allow_null_format( Action formatInvariant )
        {
            // arrange
            var format = default( string );

            // act

            // assert
            formatInvariant.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( format ) );
        }

        [Theory]
        [InlineData( "Unit Test", new object[0] )]
        [InlineData( "Unit {0}", new object[] { "Test" } )]
        public void format_invariant_should_return_expected_value( string format, object[] args )
        {
            // arrange


            // act
            var result = StringExtensions.FormatInvariant( format, args );

            // assert
            result.Should().Be( "Unit Test" );
        }

        public static IEnumerable<object[]> FormatDefaultFormatData
        {
            get
            {
                yield return new object[] { new Action( () => StringExtensions.FormatDefault( null ) ) };
                yield return new object[] { new Action( () => StringExtensions.FormatDefault( null, new[] { new object() } ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( FormatDefaultFormatData ) )]
        public void format_default_should_not_allow_null_format( Action formatDefault )
        {
            // arrange
            var format = default( string );

            // act

            // assert
            formatDefault.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( format ) );
        }

        [Theory]
        [InlineData( "Unit Test", new object[0] )]
        [InlineData( "Unit {0}", new object[] { "Test" } )]
        public void format_default_should_return_expected_value( string format, object[] args )
        {
            // arrange


            // act
            var result = StringExtensions.FormatDefault( format, args );

            // assert
            result.Should().Be( "Unit Test" );
        }

        [Fact]
        public void left_should_not_allow_null_string()
        {
            // arrange
            var value = default( string );

            // act
            Action left = () => StringExtensions.Left( null, 0 );

            // assert
            left.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void left_should_not_allow_length_longer_than_string()
        {
            // arrange
            var length = 2;

            // act
            Action left = () => StringExtensions.Left( "x", 2 );

            // assert
            left.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( length ) );
        }

        [Fact]
        public void left_should_return_expected_value()
        {
            // arrange
            var @string = "abc xyz";

            // act
            var result = @string.Left( 3 );

            // assert
            result.Should().Be( "abc" );
        }

        [Fact]
        public void right_should_not_allow_null_string()
        {
            // arrange
            var value = default( string );

            // act
            Action right = () => StringExtensions.Right( null, 0 );

            // assert
            right.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void right_should_not_allow_length_longer_than_string()
        {
            // arrange
            var length = 2;

            // act
            Action right = () => StringExtensions.Right( "x", length );

            // assert
            right.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( length ) );
        }

        [Fact]
        public void right_should_return_expected_value()
        {
            // arrange
            var @string = "abc xyz";

            // act
            var result = @string.Right( 3 );

            // assert
            result.Should().Be( "xyz" );
        }
    }
}