namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ISpecificationExtensions"/>.
    /// </summary>
    public class ISpecificationExtensionsTest
    {
        [Fact( DisplayName = "specification should append logical 'and'" )]
        public void AndShouldPerformLogicalAndOnSpecification()
        {
            // arrange
            var target = new Specification<int>( x => x >= 0 ).And( x => x <= 100 );
            
            // act
            var satisfied = target.IsSatisfiedBy( 1 );
            var unsatisfied = target.IsSatisfiedBy( -1 );

            // assert
            Assert.True( satisfied );
            Assert.False( unsatisfied );
        }

        [Fact( DisplayName = "specification should append logical 'or'" )]
        public void OrShouldPerformLogicalOrOnSpecification()
        {
            // arrange
            var target = new Specification<int>( x => x < 0 ).Or( x => x > 0 );

            // act
            var satisfied = target.IsSatisfiedBy( 1 );
            var unsatisfied = target.IsSatisfiedBy( 0 );

            // assert
            Assert.True( satisfied );
            Assert.False( unsatisfied );
        }
    }
}
