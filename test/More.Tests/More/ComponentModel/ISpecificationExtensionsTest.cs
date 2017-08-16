namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class ISpecificationExtensionsTest
    {
        [Theory]
        [InlineData( 1, true )]
        [InlineData( -1, false )]
        public void specification_should_append_logical_X27andX27( int value, bool expected )
        {
            // arrange
            var specification = new Specification<int>( x => x >= 0 ).And( x => x <= 100 );

            // act
            var result = specification.IsSatisfiedBy( value );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 1, true )]
        [InlineData( 0, false )]
        public void specification_should_append_logical_X27orX27( int value, bool expected )
        {
            // arrange
            var specification = new Specification<int>( x => x < 0 ).Or( x => x > 0 );

            // act
            var result = specification.IsSatisfiedBy( value );

            // assert
            result.Should().Be( expected );
        }
    }
}