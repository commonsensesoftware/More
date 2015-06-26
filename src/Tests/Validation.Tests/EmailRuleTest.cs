namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="EmailRule"/>.
    /// </summary>
    public class EmailRuleTest
    {
        [Theory( DisplayName = "new email rule should not allow null or empty error message" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void EvaluateShouldNotAllowNullOrEmptyErrorMessage( string errorMessage )
        {
            // arrange

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new EmailRule( errorMessage ) );

            // assert
            Assert.Equal( "errorMessage", ex.ParamName );
        }

        [Fact( DisplayName = "email rule should evaluate success for null value" )]
        public void EvaluateShouldReturnSuccessForNullOrEmpty()
        {
            // arrange
            string value = null;
            var rule = new EmailRule();
            var property = new Property<string>( "Email", value );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "email rule should evaluate invalid value" )]
        public void EvaluateShouldReturnExpectedResultForInvalidValue()
        {
            // arrange
            var rule = new EmailRule();
            var property = new Property<string>( "Email", "foo" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Email field is not a valid e-mail address.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Email", actual.MemberNames.Single() );
        }

        [Fact( DisplayName = "email rule should use custom error message" )]
        public void EvaluateShouldUseCustomErrorMessage()
        {
            // arrange
            var expected = "Test";
            var rule = new EmailRule( expected );
            var property = new Property<string>( "Email", "invalid" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }

        [Fact( DisplayName = "email rule should evaluate valid value" )]
        public void EvaluateShouldReturnExpectedResultForValidValue()
        {
            // arrange
            var rule = new EmailRule();
            var property = new Property<string>( "Email", "test@somewhere.com" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
