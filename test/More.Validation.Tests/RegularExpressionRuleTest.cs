namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="RegularExpressionRule"/>.
    /// </summary>
    public class RegularExpressionRuleTest
    {
        public static IEnumerable<object[]> NullPatternData
        {
            get
            {
                yield return new object[] { new Action<string>( pattern => new RegularExpressionRule( pattern ) ) };
                yield return new object[] { new Action<string>( pattern => new RegularExpressionRule( pattern, "Invalid" ) ) };
            }
        }

        [Theory( DisplayName = "new regex rule should not allow null or empty pattern" )]
        [MemberData( "NullPatternData" )]
        public void ConstructorShouldNotAllowNullOrEmptyPattern( Action<string> test )
        {
            // arrange
            string pattern = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( pattern ) );

            // assert
            Assert.Equal( "pattern", ex.ParamName );
        }

        [Theory( DisplayName = "new regex rule should not allow null or empty error message" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void ConstructorShouldNotAllowNullOrEmptyErrorMessage( string errorMessage )
        {
            // arrange


            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new RegularExpressionRule( ".*", errorMessage ) );

            // assert
            Assert.Equal( "errorMessage", ex.ParamName );
        }

        [Fact( DisplayName = "regex rule should evaluate success for null value" )]
        public void EvaluateShouldReturnSuccessForNullValue()
        {
            // arrange
            var rule = new RegularExpressionRule( ".+" );
            var property = new Property<string>( "Ssn", null );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "regex rule should evaluate success for value matching pattern" )]
        public void EvaluateShouldReturnSuccessForValueMatchingPattern()
        {
            // arrange
            var rule = new RegularExpressionRule( @"\d{3}-\d{2}-\d{4}" );
            var property = new Property<string>( "Ssn", "111-22-3333" );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "regex rule should evaluate expected result for value unmatched by pattern" )]
        public void EvaluateShouldReturnExpectedResultForValueUnmatchedByPattern()
        {
            // arrange
            var rule = new RegularExpressionRule( @"\d{3}-\d{2}-\d{4}" );
            var property = new Property<string>( "Ssn", "111-xx-3333" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( @"The Ssn field must match the regular expression '\d{3}-\d{2}-\d{4}'.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Ssn", actual.MemberNames.Single() );
        }

        [Fact( DisplayName = "regex rule should evaluate with custom error message" )]
        public void EvaluateShouldReturnResultWithCustomErrorMessage()
        {
            // arrange
            var expected = "Invalid";
            var rule = new RegularExpressionRule( @"\d{3}-\d{2}-\d{4}", expected );
            var property = new Property<string>( "Ssn", "111-xx-3333" );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }
    }
}
