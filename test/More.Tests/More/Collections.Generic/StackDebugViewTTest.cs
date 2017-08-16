namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class StackDebugViewTTest
    {
        [Fact]
        public void stack_debug_view_should_copy_items()
        {
            // arrange
            var stack = new Stack<string>().Adapt();

            stack.Push( "test" );

            // act
            var target = new StackDebugView<string>( stack );

            // assert
            target.Items.Should().Equal( new[] { "test" } );
        }
    }
}