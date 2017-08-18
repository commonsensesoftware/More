namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static ValidationResult;

    public class SizeRuleTTest
    {
        [Theory]
        [MemberData( nameof( ConstructorMinCountData ) )]
        public void new_size_rule_should_not_allow_min_count_X3C_0( Action<int> newSizeRule )
        {
            // arrange
            var minimumCount = -1;

            // act
            Action @new = () => newSizeRule( minimumCount );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( minimumCount ) );
        }

        [Theory]
        [MemberData( nameof( ConstructorMinAndMaxCountData ) )]
        public void new_size_rule_should_not_allow_max_count_X3C_min_count( Action<int, int> newSizeRule )
        {
            // arrange
            var minimumCount = 0;
            var maximumCount = -1;

            // act
            Action @new = () => newSizeRule( minimumCount, maximumCount );

            // assert
            @new.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( maximumCount ) );
        }

        [Fact]
        public void size_rule_should_evaluate_success_for_null_value()
        {
            // arrange
            var rule = new SizeRule<IEnumerable<int>>( 1 );
            var property = new Property<IEnumerable<int>>( "Points", null );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Fact]
        public void size_rule_should_evaluate_success_for_valid_value()
        {
            // arrange
            var rule = new SizeRule<IEnumerable<int>>( 1 );
            var property = new Property<IEnumerable<int>>( "Points", new[] { 1 } );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.Should().Be( Success );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 11 )]
        public void size_rule_should_evaluate_value_out_of_range( int count )
        {
            // arrange
            var value = Enumerable.Range( 1, count );
            var rule = new SizeRule<IEnumerable<int>>( 1, 10 );
            var property = new Property<IEnumerable<int>>( "Points", value );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ShouldBeEquivalentTo(
                new
                {
                    ErrorMessage = "The Points field must be a sequence with a minimum count of 1 and a maximum count of 10.",
                    MemberNames = new[] { "Points" }
                } );
        }

        [Theory]
        [MemberData( nameof( ErrorMessageData ) )]
        public void size_rule_should_evaluate_with_custom_error_message( Func<string, SizeRule<IEnumerable<int>>> newSizeRule )
        {
            // arrange
            var rule = newSizeRule( "Invalid" );
            var property = new Property<IEnumerable<int>>( "Counter", Enumerable.Empty<int>() );

            // act
            var result = rule.Evaluate( property );

            // assert
            result.ErrorMessage.Should().Be( "Invalid" );
        }

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
    }
}