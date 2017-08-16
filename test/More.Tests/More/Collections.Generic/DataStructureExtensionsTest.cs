namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class DataStructureExtensionsTest
    {
        [Fact]
        public void adapt_should_wrap_stack()
        {
            // arrange
            var target = new Stack<object>();

            // act
            var actual = target.Adapt();

            // assert
            actual.Should().BeOfType<StackAdapter<object>>();
        }

        [Fact]
        public void adapt_should_wrap_queue()
        {
            // arrange
            var target = new Queue<object>();
            
            // act
            var actual = target.Adapt();

            // assert
            actual.Should().BeOfType<QueueAdapter<object>>();
        }
    }
}