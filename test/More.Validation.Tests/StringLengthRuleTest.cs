namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="StringLengthRule"/>.
    /// </summary>
    public class StringLengthRuleTest
    {
        public static IEnumerable<object[]> ConstructorMaxLengthData
        {
            get
            {
                yield return new object[] { new Action<int>( max => new StringLengthRule( max ) ) };
                yield return new object[] { new Action<int>( max => new StringLengthRule( max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ConstructorMinAndMaxLengthData
        {
            get
            {
                yield return new object[] { new Action<int, int>( ( min, max ) => new StringLengthRule( min, max ) ) };
                yield return new object[] { new Action<int, int>( ( min, max ) => new StringLengthRule( min, max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ErrorMessageData
        {
            get
            {
                yield return new object[] { new Func<string, StringLengthRule>( msg => new StringLengthRule( 10, msg ) ) };
                yield return new object[] { new Func<string, StringLengthRule>( msg => new StringLengthRule( 1, 10, msg ) ) };
            }
        }

        [Theory( DisplayName = "new string length rule should not allow max length < 0" )]
        [MemberData( "ConstructorMaxLengthData" )]
        public void ConstructorShouldNotAllowMaxLengthLessThanZero( Action<int> test )
        {
            // arrange
            var max = -1;

            // act
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => test( max ) );

            // assert
            Assert.Equal( "maximumLength", ex.ParamName );
        }

        [Theory( DisplayName = "new string length rule should not allow min length < 0" )]
        [MemberData( "ConstructorMinAndMaxLengthData" )]
        public void ConstructorShouldNotAllowMinLengthLessThanZero( Action<int, int> test )
        {
            // arrange
            var min = -1;
            var max = 10;

            // act
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => test( min, max ) );

            // assert
            Assert.Equal( "minimumLength", ex.ParamName );
        }

        [Theory( DisplayName = "new string length rule should not allow max length < min length" )]
        [MemberData( "ConstructorMinAndMaxLengthData" )]
        public void ConstructorShouldNotAllowMaxLengthLessThanMinLength( Action<int, int> test )
        {
            // arrange
            var min = 0;
            var max = -1;

            // act
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => test( min, max ) );

            // assert
            Assert.Equal( "maximumLength", ex.ParamName );
        }

        [Fact( DisplayName = "string length rule should evaluate success for null string" )]
        public void EvaluateShouldReturnSuccessForNullValue()
        {
            // arrange
            var rule = new StringLengthRule( 10 );
            var property = new Property<string>( "Text", null );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "string length rule should evaluate success for valid value" )]
        public void EvaluateShouldReturnSuccessForValidValue()
        {
            // arrange
            var rule = new StringLengthRule( 10 );
            var property = new Property<string>( "Text", "Valid" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "string length rule should evaluate value out of range" )]
        public void EvaluateShouldReturnExpectedResultForInvalidValue()
        {
            // arrange
            var value = new string( 'x', 11 );
            var rule = new StringLengthRule( 10 );
            var property = new Property<string>( "Text", value );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Text field must be a string with a maximum length of 10.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Text", actual.MemberNames.Single() );
        }

        [Theory( DisplayName = "string length rule should evaluate value out of range" )]
        [InlineData( 0 )]
        [InlineData( 11 )]
        public void EvaluateWithMinLengthShouldReturnExpectedResultForInvalidValue( int count )
        {
            // arrange
            var value = new string( 'x', count );
            var rule = new StringLengthRule( 1, 10 );
            var property = new Property<string>( "Text", value );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Text field must be a string with a minimum length of 1 and a maximum length of 10.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Text", actual.MemberNames.Single() );
        }

        [Theory( DisplayName = "string length rule should evaluate with custom error message" )]
        [MemberData( "ErrorMessageData" )]
        public void EvaluateShouldReturnCustomErrorMessage( Func<string, StringLengthRule> @new )
        {
            // arrange
            var value = new string( 'x', 25 );
            var expected = "Invalid";
            var rule = @new( expected );
            var property = new Property<string>( "Text", value );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }
    }
}
