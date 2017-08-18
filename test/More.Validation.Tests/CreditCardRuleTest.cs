namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using Xunit;
    using static ValidationResult;

    public class CreditCardRuleTest
    {
        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_credit_card_rule_should_not_allow_null_or_empty_error_message( string errorMessage )
        {
            // arrange

            // act
            Action @new = () => new CreditCardRule( errorMessage );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( errorMessage ) );
        }

        [Fact]
        public void credit_card_rule_should_evaluate_success_for_null_value()
        {
            // arrange
            var value = default( string );
            var rule = new CreditCardRule();
            var property = new Property<string>( "CreditCard", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void credit_card_rule_should_evaluate_invalid_value()
        {
            // arrange
            var rule = new CreditCardRule();
            var property = new Property<string>( "CreditCard", "1234-5678-9012-3456" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The CreditCard field is not a valid credit card number.",
                    MemberNames = new[] { "CreditCard" }
                } );
        }

        [Fact]
        public void credit_card_rule_should_use_custom_error_message()
        {
            // arrange
            var rule = new CreditCardRule( errorMessage: "Test" );
            var property = new Property<string>( "CreditCard", "1234-5678-9012-3456" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Test" );
        }

        [Fact]
        public void credit_card_rule_should_evaluate_valid_value()
        {
            // arrange
            var rule = new CreditCardRule();
            var property = new Property<string>( "CreditCard", "4028-4816-3063-7116" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }
    }
}