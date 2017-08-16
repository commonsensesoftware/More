namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class QueueDebugViewTTest
    {
        [Fact]
        public void queue_debug_view_should_initialize_items()
        {
            // arrange
            var queue = new Queue<string>().Adapt();

            queue.Enqueue( "test" );

            // act
            var target = new QueueDebugView<string>( queue );

            // assert
            target.Items.Should().Equal( queue );
        }
    }
}