namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static ValidationResult;

    public class NullableRangeRuleTTest
    {
        [Theory]
        [MemberData( nameof( ConstructorMaxValueData ) )]
        public void new_nullable_range_rule_should_not_allow_max_X3C_default( Action<int> newNullableRange )
        {
            // arrange
            var maximum = -1;

            // act
            Action @new = () => newNullableRange( maximum );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( maximum ) );
        }

        [Theory]
        [MemberData( nameof( ConstructorMinAndMaxValueData ) )]
        public void new_nullable_range_rule_should_not_allow_max_X3C_min( Action<int, int> newNullableRange )
        {
            // arrange
            var minimum = 0;
            var maximum = -1;

            // act
            Action @new = () => newNullableRange( minimum, maximum );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( maximum ) );
        }

        [Fact]
        public void nullable_range_rule_should_evaluate_success_for_null_value()
        {
            // arrange
            var rule = new NullableRangeRule<int>( 0 );
            var property = new Property<int?>( "Counter", null );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void nullable_range_rule_should_evaluate_success_for_valid_value()
        {
            // arrange
            var rule = new NullableRangeRule<int>( 10 );
            var property = new Property<int?>( "Counter", 5 );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 11 )]
        public void nullable_range_rule_should_evaluate_value_out_of_range( int value )
        {
            // arrange
            var rule = new NullableRangeRule<int>( 1, 10 );
            var property = new Property<int?>( "Counter", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The Counter field must be between 1 and 10.",
                    MemberNames = new[] { "Counter" }
                } );
        }

        [Theory]
        [MemberData( nameof( ErrorMessageData ) )]
        public void nullable_range_rule_should_evaluate_with_custom_error_message( Func<string, NullableRangeRule<int>> @new )
        {
            // arrange
            var rule = @new( "Invalid" );
            var property = new Property<int?>( "Counter", 11 );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Invalid" );
        }

        public static IEnumerable<object[]> ConstructorMaxValueData
        {
            get
            {
                yield return new object[] { new Action<int>( max => new NullableRangeRule<int>( max ) ) };
                yield return new object[] { new Action<int>( max => new NullableRangeRule<int>( max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ConstructorMinAndMaxValueData
        {
            get
            {
                yield return new object[] { new Action<int, int>( ( min, max ) => new NullableRangeRule<int>( min, max ) ) };
                yield return new object[] { new Action<int, int>( ( min, max ) => new NullableRangeRule<int>( min, max, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> ErrorMessageData
        {
            get
            {
                yield return new object[] { new Func<string, NullableRangeRule<int>>( msg => new NullableRangeRule<int>( 10, msg ) ) };
                yield return new object[] { new Func<string, NullableRangeRule<int>>( msg => new NullableRangeRule<int>( 1, 10, msg ) ) };
            }
        }
    }
}