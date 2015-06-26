namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="UrlRule"/>.
    /// </summary>
    public class UrlRuleTest
    {
        [Theory( DisplayName = "new url rule should not allow null or empty error message" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void EvaluateShouldNotAllowNullOrEmptyErrorMessage( string errorMessage )
        {
            // arrange

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new UrlRule( errorMessage ) );

            // assert
            Assert.Equal( "errorMessage", ex.ParamName );
        }

        [Theory( DisplayName = "new url rule with kind should not allow null or empty error message" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void EvaluateWithKindShouldNotAllowNullOrEmptyErrorMessage( string errorMessage )
        {
            // arrange
            var kind = UriKind.RelativeOrAbsolute;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new UrlRule( kind, errorMessage ) );

            // assert
            Assert.Equal( "errorMessage", ex.ParamName );
        }
        [Fact( DisplayName = "url rule should evaluate success for null value" )]
        public void EvaluateShouldReturnSuccessForNullOrEmpty()
        {
            // arrange
            string value = null;
            var rule = new UrlRule();
            var property = new Property<string>( "Url", value );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "url rule should evaluate invalid value" )]
        public void EvaluateShouldReturnExpectedResultForInvalidValue()
        {
            // arrange
            var rule = new UrlRule();
            var property = new Property<string>( "Url", "foo" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Url field is not a valid fully-qualified http, https, or ftp URL.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Url", actual.MemberNames.Single() );
        }

        [Fact( DisplayName = "url rule should use custom error message" )]
        public void EvaluateShouldUseCustomErrorMessage()
        {
            // arrange
            var expected = "Test";
            var rule = new UrlRule( expected );
            var property = new Property<string>( "Url", "tempuri.org" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }

        [Fact( DisplayName = "url rule should use custom error message" )]
        public void EvaluateWithKindShouldUseCustomErrorMessage()
        {
            // arrange
            var expected = "Test";
            var rule = new UrlRule( UriKind.Absolute, expected );
            var property = new Property<string>( "Url", "tempuri.org" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }

        [Fact( DisplayName = "url rule should evaluate valid value" )]
        public void EvaluateShouldReturnExpectedResultForValidValue()
        {
            // arrange
            var rule = new UrlRule();
            var property = new Property<string>( "Url", "http://tempuri.org" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "url rule with kind should evaluate valid value" )]
        public void EvaluateWithKindShouldReturnExpectedResultForValidValue()
        {
            // arrange
            var rule = new UrlRule( UriKind.Relative );
            var property = new Property<string>( "Url", "/api/helloworld" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
