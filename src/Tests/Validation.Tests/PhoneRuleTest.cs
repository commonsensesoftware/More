namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="PhoneRule"/>.
    /// </summary>
    public class PhoneRuleTest
    {
        [Theory( DisplayName = "new phone rule should not allow null or empty error message" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void EvaluateShouldNotAllowNullOrEmptyErrorMessage( string errorMessage )
        {
            // arrange

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new PhoneRule( errorMessage ) );

            // assert
            Assert.Equal( "errorMessage", ex.ParamName );
        }

        [Fact( DisplayName = "phone rule should evaluate success for null value" )]
        public void EvaluateShouldReturnSuccessForNullOrEmpty()
        {
            // arrange
            string value = null;
            var rule = new PhoneRule();
            var property = new Property<string>( "Phone", value );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "phone rule should evaluate invalid value" )]
        public void EvaluateShouldReturnExpectedResultForInvalidValue()
        {
            // arrange
            var rule = new PhoneRule();
            var property = new Property<string>( "Phone", "foo" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Phone field is not a valid phone number.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Phone", actual.MemberNames.Single() );
        }

        [Fact( DisplayName = "phone rule should use custom error message" )]
        public void EvaluateShouldUseCustomErrorMessage()
        {
            // arrange
            var expected = "Test";
            var rule = new PhoneRule( expected );
            var property = new Property<string>( "Phone", "425-555-abcde" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }

        [Fact( DisplayName = "phone rule should evaluate valid value" )]
        public void EvaluateShouldReturnExpectedResultForValidValue()
        {
            // arrange
            var rule = new PhoneRule();
            var property = new Property<string>( "Phone", "916-555-5555" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
