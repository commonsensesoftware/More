namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SizeRule{T}"/>.
    /// </summary>
    public class SizeRuleTTest
    {
        public static IEnumerable<object[]> ConstructorMinCountData
        {
            get
            {
                yield return new object[] { new Action<int>( min => new SizeRule<IEnumerable>( min ) ) };
                yield return new object[] { new Action<int>( min => new SizeRule<IEnumerable>( min, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ConstructorMinAndMaxCountData
        {
            get
            {
                yield return new object[] { new Action<int, int>( ( min, max ) => new SizeRule<IEnumerable>( min, max ) ) };
                yield return new object[] { new Action<int, int>( ( min, max ) => new SizeRule<IEnumerable>( min, max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ErrorMessageData
        {
            get
            {
                yield return new object[] { new Func<string, SizeRule<IEnumerable<int>>>( msg => new SizeRule<IEnumerable<int>>( 1, msg ) ) };
                yield return new object[] { new Func<string, SizeRule<IEnumerable<int>>>( msg => new SizeRule<IEnumerable<int>>( 1, 10, msg ) ) };
            }
        }

        [Theory( DisplayName = "new size rule should not allow min count < 0" )]
        [MemberData( "ConstructorMinCountData" )]
        public void ConstructorShouldNotAllowMaxCountLessThanZero( Action<int> test )
        {
            // arrange
            var min = -1;

            // act
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => test( min ) );

            // assert
            Assert.Equal( "minimumCount", ex.ParamName );
        }

        [Theory( DisplayName = "new size rule should not allow max count < min count" )]
        [MemberData( "ConstructorMinAndMaxCountData" )]
        public void ConstructorShouldNotAllowMaxCountLessThanMinCount( Action<int, int> test )
        {
            // arrange
            var min = 0;
            var max = -1;

            // act
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => test( min, max ) );

            // assert
            Assert.Equal( "maximumCount", ex.ParamName );
        }

        [Fact( DisplayName = "size rule should evaluate success for null value" )]
        public void EvaluateShouldReturnSuccessForNullValue()
        {
            // arrange
            var rule = new SizeRule<IEnumerable<int>>( 1 );
            var property = new Property<IEnumerable<int>>( "Points", null );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "size rule should evaluate success for valid value" )]
        public void EvaluateShouldReturnSuccessForValidValue()
        {
            // arrange
            var rule = new SizeRule<IEnumerable<int>>( 1 );
            var property = new Property<IEnumerable<int>>( "Points", new[] { 1 } );
            var expected = ValidationResult.Success;

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "size rule should evaluate value out of range" )]
        [InlineData( 0 )]
        [InlineData( 11 )]
        public void EvaluateShouldReturnExpectedResultForInvalidValue( int count )
        {
            // arrange
            var value = Enumerable.Range( 1, count );
            var rule = new SizeRule<IEnumerable<int>>( 1, 10 );
            var property = new Property<IEnumerable<int>>( "Points", value );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( "The Points field must be a sequence with a minimum count of 1 and a maximum count of 10.", actual.ErrorMessage );
            Assert.Equal( 1, actual.MemberNames.Count() );
            Assert.Equal( "Points", actual.MemberNames.Single() );
        }

        [Theory( DisplayName = "size rule should evaluate with custom error message" )]
        [MemberData( "ErrorMessageData" )]
        public void EvaluateShouldReturnCustomErrorMessage( Func<string, SizeRule<IEnumerable<int>>> @new )
        {
            // arrange
            var expected = "Invalid";
            var rule = @new( expected );
            var property = new Property<IEnumerable<int>>( "Counter", Enumerable.Empty<int>() );

            // act
            var actual = rule.Evaluate( property );

            // assert
            Assert.Equal( expected, actual.ErrorMessage );
        }
    }
}
