namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="Specification{T}"/> class.
    /// </summary>
    public class SpecificationTTest
    {
        [Fact( DisplayName = "new specification should not allow null callback" )]
        public void ConstructorShouldNotAllowNullCallback()
        {
            // arrange
            Func<string, bool> evaluate = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new Specification<string>( evaluate ) );

            // assert
            Assert.Equal( "evaluate", ex.ParamName );
        }

        [Theory( DisplayName = "is satisfied by should invoke callback" )]
        [InlineData( "unit test", true )]
        [InlineData( "ad hoc test", false )]
        public void IsSatisfiedByShouldExecuteCallback( string item, bool expected )
        {
            // arrange
            var target = new Specification<string>( s => s == "unit test" );

            // act
            var actual = target.IsSatisfiedBy( item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "and should not allow null specification" )]
        public void AndShouldNotAllowNull()
        {
            // arrange
            ISpecification<string> other = null;
            var target = new Specification<string>( s => true );
            
            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.And( other ) );
            
            // assert
            Assert.Equal( "other", ex.ParamName );
        }

        [Fact( DisplayName = "and should return combined specification" )]
        public void AndShouldReturnCombinedSpecification()
        {
            // arrange
            var s1 = new Specification<string>( s => true );
            var s2 = new Specification<string>( s => false );

            // act
            var actual = s1.And( s2 );

            // assert
            Assert.NotNull( actual );
        }

        [Fact( DisplayName = "or should not allow null specification" )]
        public void OrShouldNotAllowNull()
        {
            // arrange
            ISpecification<string> other = null;
            var target = new Specification<string>( s => true );
            
            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.Or( other ) );
            
            // assert
            Assert.Equal( "other", ex.ParamName );
        }

        [Fact( DisplayName = "or should return combined specification" )]
        public void OrShouldReturnCombinedSpecification()
        {
            // arrange
            var s1 = new Specification<string>( s => true );
            var s2 = new Specification<string>( s => false );
            
            // act
            var actual = s1.Or( s2 );

            // assert
            Assert.NotNull( actual );
        }

        [Fact( DisplayName = "not should negate specification" )]
        public void NotShouldReturnNegatedSpecification()
        {
            // arrange
            var target = new Specification<string>( s => true );

            // act
            var actual = target.Not();

            // assert
            Assert.NotNull( actual );
        }

        [Theory( DisplayName = "is satisfied by should evaluate specification" )]
        [InlineData( 25, false )]
        [InlineData( -1, false )]
        [InlineData( 100, true )]
        [InlineData( 25, false )]
        [InlineData( -1, false )]
        [InlineData( 100, true )]
        public void IsSatisfiedByShouldInvokeSpecification( int item, bool expected )
        {
            // note: this test verifies the behavior of the following types:
            //  Specification<T>
            //  LogicalAndSpecification<T>
            //  LogicalOrSpecification<T>
            //  LogicalNotSpecification<T>

            // arrange
            var target = new Specification<int>( x => x > 5 )
                            .And( new Specification<int>( x => x < 50 ) )
                            .Or( new Specification<int>( x => x == -1 ) )
                            .Not();

            // act
            var actual = target.IsSatisfiedBy( item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "evaluate should equal is satisfied by result" )]
        [InlineData( 25 )]
        [InlineData( -1 )]
        [InlineData( 100 )]
        public void EvaluateShouldReturnTheSameResultsAsIsSatisfiedBy( int item )
        {
            // specification is a specialized form of rule (IRule<T,bool>)
            // the implementations should yield the same results

            // arrange
            var target = new Specification<int>( x => x > 5 )
                            .And( new Specification<int>( x => x < 50 ) )
                            .Or( new Specification<int>( x => x == -1 ) )
                            .Not();
            var expected = target.IsSatisfiedBy( item );

            // act
            var actual = target.Evaluate( item );

            Assert.Equal( expected, actual );
        }
    }
}
