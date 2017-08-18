namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using Xunit;
    using static ValidationResult;

    public class UrlRuleTest
    {
        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_url_rule_should_not_allow_null_or_empty_error_message( string errorMessage )
        {
            // arrange

            // act
            Action @new = () => new UrlRule( errorMessage );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( errorMessage ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_url_rule_with_kind_should_not_allow_null_or_empty_error_message( string errorMessage )
        {
            // arrange
            var kind = UriKind.RelativeOrAbsolute;

            // act
            Action @new = () => new UrlRule( kind, errorMessage );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( errorMessage ) );
        }
        [Fact]
        public void url_rule_should_evaluate_success_for_null_value()
        {
            // arrange
            string value = null;
            var rule = new UrlRule();
            var property = new Property<string>( "Url", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void url_rule_should_evaluate_invalid_value()
        {
            // arrange
            var rule = new UrlRule();
            var property = new Property<string>( "Url", "foo" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The Url field is not a valid fully-qualified http, https, or ftp URL.",
                    MemberNames = new[] { "Url" }
                } );
        }

        [Fact]
        public void url_rule_should_use_custom_error_message()
        {
            // arrange
            var rule = new UrlRule( errorMessage: "Invalid" );
            var property = new Property<string>( "Url", "tempuri.org" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Invalid" );
        }

        [Fact]
        public void url_rule_with_kind_should_use_custom_error_message()
        {
            // arrange
            var rule = new UrlRule( UriKind.Absolute, errorMessage: "Invalid" );
            var property = new Property<string>( "Url", "tempuri.org" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Invalid" );
        }

        [Fact]
        public void url_rule_should_evaluate_valid_value()
        {
            // arrange
            var rule = new UrlRule();
            var property = new Property<string>( "Url", "http://tempuri.org" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void url_rule_with_kind_should_evaluate_valid_value()
        {
            // arrange
            var rule = new UrlRule( UriKind.Relative );
            var property = new Property<string>( "Url", "/api/helloworld" );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }
    }
}