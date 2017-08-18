namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using Xunit;
    using static ValidationResult;

    public class PhoneRuleTest
    {
        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_phone_rule_should_not_allow_null_or_empty_error_message( string errorMessage )
        {
            // arrange

            // act
            Action @new = () => new PhoneRule( errorMessage );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( errorMessage ) );
        }

        [Fact]
        public void phone_rule_should_evaluate_success_for_null_value()
        {
            // arrange
            var value = default( string );
            var rule = new PhoneRule();
            var property = new Property<string>( "Phone", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void phone_rule_should_evaluate_invalid_value()
        {
            // arrange
            var rule = new PhoneRule();
            var property = new Property<string>( "Phone", "foo" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The Phone field is not a valid phone number.",
                    MemberNames = new[] { "Phone" }
                } );
        }

        [Fact]
        public void phone_rule_should_use_custom_error_message()
        {
            // arrange
            var rule = new PhoneRule( errorMessage: "Test" );
            var property = new Property<string>( "Phone", "425-555-abcde" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Test" );
        }

        [Fact]
        public void phone_rule_should_evaluate_valid_value()
        {
            // arrange
            var rule = new PhoneRule();
            var property = new Property<string>( "Phone", "916-555-5555" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }
    }
}