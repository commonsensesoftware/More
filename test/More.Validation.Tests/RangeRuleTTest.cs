namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="RangeRule{T}"/>.
    /// </summary>
    public class RangeRuleTTest
    {
        public static IEnumerable<object[]> ConstructorMaxValueData
        {
            get
            {
                yield return new object[] { new Action<int>( max => new RangeRule<int>( max ) ) };
                yield return new object[] { new Action<int>( max => new RangeRule<int>( max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ConstructorMinAndMaxValueData
        {
            get
            {
                yield return new object[] { new Action<int, int>( ( min, max ) => new RangeRule<int>( min, max ) ) };
                yield return new object[] { new Action<int, int>( ( min, max ) => new RangeRule<int>( min, max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ErrorMessageData
        {
            get
            {
                yield return new object[] { new Func<string, RangeRule<int>>( msg => new RangeRule<int>( 10, msg ) ) };
                yield return new object[] { new Func<string, RangeRule<int>>( msg => new RangeRule<int>( 1, 10, msg ) ) };
            }
        }
        
        [Theory( DisplayName = "new range rule should not allow max < default" )]
        [MemberData( "ConstructorMaxValueData" )]
        public void ConstructorShouldNotAllowMaxValueLessThanDefaultValue( Action<int> test )
        {
            // arrange
            var max = -1;

            // act
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => test( max ) );

            // assert
            Assert.Equal( "maximum", ex.ParamName );
        }

        [Theory( DisplayName = "new range rule should not allow max < min" )]
        [MemberData( "ConstructorMinAndMaxValueData" )]
        public void ConstructorShouldNotAllowMaxValueLessThanDefaultValue( Action<int, int> test )
        {
            // arrange
            var min = 0;
            var max = -1;

            // act
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => test( min, max ) );

            // assert
            Assert.Equal( "maximum", ex.ParamName );
        }

        [Fact( DisplayName = "range rule should evaluate success for valid value" )]
        public void EvaluateShouldReturnSuccessForValidValue()
        {
            // arrange
            var rule = new RangeRule<int>( 10 );
            var property = new Property<int>( "Counter", 5 );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "range rule should evaluate value out of range" )]
        [InlineData( 0 )]
        [InlineData( 11 )]
        public void EvaluateShouldReturnExpectedResultForInvalidValue( int value )
        {
            // arrange
            var rule = new RangeRule<int>( 1, 10 );
            var property = new Property<int>( "Counter", value );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Counter field must be between 1 and 10.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Counter", actual.MemberNames.Single() );
        }

        [Theory( DisplayName = "range rule should evaluate with custom error message" )]
        [MemberData( "ErrorMessageData" )]
        public void EvaluateShouldReturnCustomErrorMessage( Func<string, RangeRule<int>> @new )
        {
            // arrange
            var expected = "Invalid";
            var rule = @new( expected );
            var property = new Property<int>( "Counter", 11 );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }
    }
}
