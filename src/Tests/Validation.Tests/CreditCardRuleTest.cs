namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="CreditCardRule"/>.
    /// </summary>
    public class CreditCardRuleTest
    {
        [Theory( DisplayName = "new credit card rule should not allow null or empty error message" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void EvaluateShouldNotAllowNullOrEmptyErrorMessage( string errorMessage )
        {
            // arrange

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new CreditCardRule( errorMessage ) );

            // assert
            Assert.Equal( "errorMessage", ex.ParamName );
        }

        [Fact( DisplayName = "credit card rule should evaluate success for null value" )]
        public void EvaluateShouldReturnSuccessForNull()
        {
            // arrange
            string value = null;
            var rule = new CreditCardRule();
            var property = new Property<string>( "CreditCard", value );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "credit card rule should evaluate invalid value" )]
        public void EvaluateShouldReturnExpectedResultForInvalidValue()
        {
            // arrange
            var rule = new CreditCardRule();
            var property = new Property<string>( "CreditCard", "1234-5678-9012-3456" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The CreditCard field is not a valid credit card number.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "CreditCard", actual.MemberNames.Single() );
        }

        [Fact( DisplayName = "credit card rule should use custom error message" )]
        public void EvaluateShouldUseCustomErrorMessage()
        {
            // arrange
            var expected = "Test";
            var rule = new CreditCardRule( expected );
            var property = new Property<string>( "CreditCard", "1234-5678-9012-3456" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }

        [Fact( DisplayName = "credit card rule should evaluate valid value" )]
        public void EvaluateShouldReturnExpectedResultForValidValue()
        {
            // arrange
            var rule = new CreditCardRule();
            var property = new Property<string>( "CreditCard", "4028-4816-3063-7116" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
