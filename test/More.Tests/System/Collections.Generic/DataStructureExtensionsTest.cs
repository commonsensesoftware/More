namespace System.Collections.Generic
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class DataStructureExtensionsTest
    {
        [Fact]
        public void adapt_should_not_allow_null_stack()
        {
            // arrange
            var stack = default( Stack<object> );

            // act
            Action adapt = () => stack.Adapt();

            // assert
            adapt.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( stack ) );
        }

        [Fact]
        public void adapt_should_not_allow_null_queue()
        {
            // arrange
            var queue = default( Queue<object> );

            // act
            Action adapt = () => queue.Adapt();

            // assert
            adapt.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( queue ) );
        }
    }
}