namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using Xunit;
    using static ValidationResult;

    public class EmailRuleTest
    {
        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_email_rule_should_not_allow_null_or_empty_error_message( string errorMessage )
        {
            // arrange

            // act
            Action @new = () => new EmailRule( errorMessage );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( errorMessage ) );
        }

        [Fact]
        public void email_rule_should_evaluate_success_for_null_value()
        {
            // arrange
            var value = default( string );
            var rule = new EmailRule();
            var property = new Property<string>( "Email", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void email_rule_should_evaluate_invalid_value()
        {
            // arrange
            var rule = new EmailRule();
            var property = new Property<string>( "Email", "foo" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The Email field is not a valid e-mail address.",
                    MemberNames = new[] { "Email" }
                } );
        }

        [Fact]
        public void email_rule_should_use_custom_error_message()
        {
            // arrange
            var rule = new EmailRule( errorMessage: "Test" );
            var property = new Property<string>( "Email", "invalid" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Test" );
        }

        [Fact]
        public void email_rule_should_evaluate_valid_value()
        {
            // arrange
            var rule = new EmailRule();
            var property = new Property<string>( "Email", "test@somewhere.com" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }
    }
}