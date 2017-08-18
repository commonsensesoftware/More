namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using Xunit;
    using static ValidationResult;

    public class RequiredRuleTTest
    {
        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_required_rule_should_not_allow_null_or_empty_error_message( string errorMessage )
        {
            // arrange


            // act
            Action @new = () => new RequiredRule<object>( errorMessage );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( errorMessage ) );
        }

        [Fact]
        public void required_rule_should_return_success_for_nonX2Dnull_value()
        {
            // arrange
            var rule = new RequiredRule<object>();
            var property = new Property<object>( "Object", new object() );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void required_rule_should_return_success_for_nonX2Dnull_string()
        {
            // arrange
            var rule = new RequiredRule<string>();
            var property = new Property<string>( "Text", "Valid" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void required_rule_should_return_success_for_empty_string()
        {
            // arrange
            var rule = new RequiredRule<string>() { AllowEmptyStrings = true };
            var property = new Property<string>( "Text", "" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void required_rule_should_return_expected_result_for_null_value()
        {
            // arrange
            var rule = new RequiredRule<object>();
            var property = new Property<object>( "Object", null );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The Object field is required.",
                    MemberNames = new[] { "Object" }
                } );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void required_rule_should_return_expected_result_for_null_or_empty_string( string value )
        {
            // arrange
            var rule = new RequiredRule<string>();
            var property = new Property<string>( "Text", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
               new
               {
                   ErrorMessage = "The Text field is required.",
                   MemberNames = new[] { "Text" }
               } );
        }

        [Fact]
        public void required_rule_should_return_result_with_custom_error_message()
        {
            // arrange
            var rule = new RequiredRule<object>( errorMessage: "Invalid" );
            var property = new Property<object>( "Object", null );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Invalid" );
        }

        [Fact]
        public void required_rule_should_return_result_with_custom_error_message_for_string()
        {
            // arrange
            var rule = new RequiredRule<string>( errorMessage: "Invalid" );
            var property = new Property<string>( "Text", null );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Invalid" );
        }
    }
}