namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class SpecificationTTest
    {
        [Fact]
        public void new_specification_should_not_allow_null_callback()
        {
            // arrange
            var evaluate = default( Func<string, bool> );

            // act
            Action @new = () => new Specification<string>( evaluate );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( evaluate ) );
        }

        [Theory]
        [InlineData( "unit test", true )]
        [InlineData( "ad hoc test", false )]
        public void is_satisfied_by_should_invoke_callback( string item, bool expected )
        {
            // arrange
            var specification = new Specification<string>( s => s == "unit test" );

            // act
            var result = specification.IsSatisfiedBy( item );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void and_should_not_allow_null_specification()
        {
            // arrange
            var other = default( ISpecification<string> );
            var specification = new Specification<string>( s => true );

            // act
            Action and = () => specification.And( other );

            // assert
            and.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( other ) );
        }

        [Fact]
        public void and_should_return_combined_specification()
        {
            // arrange
            var specification = new Specification<string>( s => true );
            var other = new Specification<string>( s => false );

            // act
            var combinedSpecification = specification.And( other );

            // assert
            combinedSpecification.Should().NotBeNull();
        }

        [Fact]
        public void or_should_not_allow_null_specification()
        {
            // arrange
            var other = default( ISpecification<string> );
            var specification = new Specification<string>( s => true );

            // act
            Action or = () => specification.Or( other );

            // assert
            or.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( other ) );
        }

        [Fact]
        public void or_should_return_combined_specification()
        {
            // arrange
            var specficiation = new Specification<string>( s => true );
            var other = new Specification<string>( s => false );

            // act
            var combineSpecification = specficiation.Or( other );

            // assert
            combineSpecification.Should().NotBeNull();
        }

        [Fact]
        public void not_should_negate_specification()
        {
            // arrange
            var specification = new Specification<string>( s => true );

            // act
            var not = specification.Not();

            // assert
            not.Should().NotBeNull();
        }

        [Theory]
        [InlineData( 25, false )]
        [InlineData( -1, false )]
        [InlineData( 100, true )]
        [InlineData( 25, false )]
        [InlineData( -1, false )]
        [InlineData( 100, true )]
        public void is_satisfied_by_should_evaluate_specification( int item, bool expected )
        {
            // NOTE: this test verifies the behavior of the following types:
            //  Specification<T>
            //  LogicalAndSpecification<T>
            //  LogicalOrSpecification<T>
            //  LogicalNotSpecification<T>

            // arrange
            var specification = new Specification<int>( x => x > 5 )
                          .And( new Specification<int>( x => x < 50 ) )
                          .Or( new Specification<int>( x => x == -1 ) )
                          .Not();

            // act
            var result = specification.IsSatisfiedBy( item );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 25 )]
        [InlineData( -1 )]
        [InlineData( 100 )]
        public void evaluate_should_equal_is_satisfied_by_result( int item )
        {
            // specification is a specialized form of rule (IRule<T,bool>)
            // the implementations should yield the same results

            // arrange
            var specification = new Specification<int>( x => x > 5 )
                          .And( new Specification<int>( x => x < 50 ) )
                          .Or( new Specification<int>( x => x == -1 ) )
                          .Not();
            var expected = specification.IsSatisfiedBy( item );

            // act
            var result = specification.Evaluate( item );

            result.Should().Be( expected );
        }
    }
}