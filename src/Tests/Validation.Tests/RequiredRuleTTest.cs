namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="RequiredRule{T}"/>.
    /// </summary>
    public class RequiredRuleTTest
    {
        [Theory( DisplayName = "new required rule should not allow null or empty error message" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void ConstructorShouldNotAllowNullOrEmptyErrorMessage( string errorMessage )
        {
            // arrange


            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new RequiredRule<object>( errorMessage ) );

            // assert
            Assert.Equal( "errorMessage", ex.ParamName );
        }

        [Fact( DisplayName = "required rule should return success for non-null value" )]
        public void EvaluateShouldReturnSuccessForNonNullValue()
        {
            // arrange
            var rule = new RequiredRule<object>();
            var property = new Property<object>( "Object", new object() );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "required rule should return success for non-null string" )]
        public void EvaluateShouldReturnSuccessForNonNullString()
        {
            // arrange
            var rule = new RequiredRule<string>();
            var property = new Property<string>( "Text", "Valid" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "required rule should return success for empty string" )]
        public void EvaluateShouldReturnSuccessForEmptyString()
        {
            // arrange
            var rule = new RequiredRule<string>() { AllowEmptyStrings = true };
            var property = new Property<string>( "Text", "" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "required rule should return expected result for null value" )]
        public void EvaluateShouldReturnExpectedResultForNullValue()
        {
            // arrange
            var rule = new RequiredRule<object>();
            var property = new Property<object>( "Object", null );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Object field is required.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Object", actual.MemberNames.Single() );
        }

        [Theory( DisplayName = "required rule should return expected result for null or empty string" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void EvaluateShouldReturnExpectedResultForNullOrEmptyString( string value )
        {
            // arrange
            var rule = new RequiredRule<string>();
            var property = new Property<string>( "Text", value );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Text field is required.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Text", actual.MemberNames.Single() );
        }

        [Fact( DisplayName = "required rule should return result with custom error message" )]
        public void EvaluateShouldReturnResultWithCustomErrorMessage()
        {
            // arrange
            var expected = "Invalid";
            var rule = new RequiredRule<object>( expected );
            var property = new Property<object>( "Object", null );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }

        [Fact( DisplayName = "required rule should return result with custom error message" )]
        public void EvaluateShouldReturnResultWithCustomErrorMessageForString()
        {
            // arrange
            var expected = "Invalid";
            var rule = new RequiredRule<string>( expected );
            var property = new Property<string>( "Text", null );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }
    }
}
