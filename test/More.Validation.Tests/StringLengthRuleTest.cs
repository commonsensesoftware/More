namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static ValidationResult;

    public class StringLengthRuleTest
    {
        [Theory]
        [MemberData( nameof( ConstructorMaxLengthData ) )]
        public void new_string_length_rule_should_not_allow_max_length_X3C_0( Action<int> newStringLengthRule )
        {
            // arrange
            var maximumLength = -1;

            // act
            Action @new = () => newStringLengthRule( maximumLength );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( maximumLength ) );
        }

        [Theory]
        [MemberData( nameof( ConstructorMinAndMaxLengthData ) )]
        public void new_string_length_rule_should_not_allow_min_length_X3C_0( Action<int, int> test )
        {
            // arrange
            var minimumLength = -1;
            var maximumLength = 10;

            // act
            Action @new = () => test( minimumLength, maximumLength );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( minimumLength ) );
        }

        [Theory]
        [MemberData( nameof( ConstructorMinAndMaxLengthData ) )]
        public void new_string_length_rule_should_not_allow_max_length_X3C_min_length( Action<int, int> newStringLengthRule )
        {
            // arrange
            var minimumLength = 0;
            var maximumLength = -1;

            // act
            Action @new = () => newStringLengthRule( minimumLength, maximumLength );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( maximumLength ) );
        }

        [Fact]
        public void string_length_rule_should_evaluate_success_for_null_string()
        {
            // arrange
            var rule = new StringLengthRule( 10 );
            var property = new Property<string>( "Text", null );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void string_length_rule_should_evaluate_success_for_valid_value()
        {
            // arrange
            var rule = new StringLengthRule( 10 );
            var property = new Property<string>( "Text", "Valid" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void string_length_rule_should_evaluate_value_out_of_range_with_maximum()
        {
            // arrange
            var value = new string( 'x', 11 );
            var rule = new StringLengthRule( 10 );
            var property = new Property<string>( "Text", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The Text field must be a string with a maximum length of 10.",
                    MemberNames = new[] { "Text" }
                } );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 11 )]
        public void string_length_rule_should_evaluate_value_out_of_range_with_minimum_and_maximum( int count )
        {
            // arrange
            var value = new string( 'x', count );
            var rule = new StringLengthRule( 1, 10 );
            var property = new Property<string>( "Text", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The Text field must be a string with a minimum length of 1 and a maximum length of 10.",
                    MemberNames = new[] { "Text" }
                } );
        }

        [Theory]
        [MemberData( nameof( ErrorMessageData ) )]
        public void string_length_rule_should_evaluate_with_custom_error_message( Func<string, StringLengthRule> newStringLengthRule )
        {
            // arrange
            var value = new string( 'x', 25 );
            var rule = newStringLengthRule( "Invalid" );
            var property = new Property<string>( "Text", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Invalid" );
        }

        public static IEnumerable<object[]> ConstructorMaxLengthData
        {
            get
            {
                yield return new object[] { new Action<int>( max => new StringLengthRule( max ) ) };
                yield return new object[] { new Action<int>( max => new StringLengthRule( max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ConstructorMinAndMaxLengthData
        {
            get
            {
                yield return new object[] { new Action<int, int>( ( min, max ) => new StringLengthRule( min, max ) ) };
                yield return new object[] { new Action<int, int>( ( min, max ) => new StringLengthRule( min, max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ErrorMessageData
        {
            get
            {
                yield return new object[] { new Func<string, StringLengthRule>( msg => new StringLengthRule( 10, msg ) ) };
                yield return new object[] { new Func<string, StringLengthRule>( msg => new StringLengthRule( 1, 10, msg ) ) };
            }
        }
    }
}