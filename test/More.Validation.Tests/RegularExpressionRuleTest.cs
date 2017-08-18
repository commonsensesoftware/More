namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static ValidationResult;

    public class RegularExpressionRuleTest
    {
        public static IEnumerable<object[]> NullPatternData
        {
            get
            {
                yield return new object[] { new Action<string>( pattern => new RegularExpressionRule( pattern ) ) };
                yield return new object[] { new Action<string>( pattern => new RegularExpressionRule( pattern, "Invalid" ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( NullPatternData ) )]
        public void new_regex_rule_should_not_allow_null_or_empty_pattern( Action<string> newRegExRule )
        {
            // arrange
            var pattern = default( string );

            // act
            Action @new = () => newRegExRule( pattern );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( pattern ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_regex_rule_should_not_allow_null_or_empty_error_message( string errorMessage )
        {
            // arrange


            // act
            Action @new = () => new RegularExpressionRule( ".*", errorMessage );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( errorMessage ) );
        }

        [Fact]
        public void regex_rule_should_evaluate_success_for_null_value()
        {
            // arrange
            var rule = new RegularExpressionRule( ".+" );
            var property = new Property<string>( "Ssn", null );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void regex_rule_should_evaluate_success_for_value_matching_pattern()
        {
            // arrange
            var rule = new RegularExpressionRule( @"\d{3}-\d{2}-\d{4}" );
            var property = new Property<string>( "Ssn", "111-22-3333" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void regex_rule_should_evaluate_expected_result_for_value_unmatched_by_pattern()
        {
            // arrange
            var rule = new RegularExpressionRule( @"\d{3}-\d{2}-\d{4}" );
            var property = new Property<string>( "Ssn", "111-xx-3333" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = @"The Ssn field must match the regular expression '\d{3}-\d{2}-\d{4}'.",
                    MemberNames = new[] { "Ssn" }
                } );
        }

        [Fact]
        public void regex_rule_should_evaluate_with_custom_error_message()
        {
            // arrange
            var rule = new RegularExpressionRule( @"\d{3}-\d{2}-\d{4}", "Invalid" );
            var property = new Property<string>( "Ssn", "111-xx-3333" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Invalid" );
        }
    }
}