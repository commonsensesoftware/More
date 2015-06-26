namespace More.Collections.Generic
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="QueueDebugView{T}"/> class.
    /// </summary>
    public class QueueDebugViewTTest
    {
        [Fact( DisplayName = "queue debug view should initialize items" )]
        public void ItemsPropertyShouldReturnArrayOfQueueItems()
        {
            // arrange
            var queue = new Queue<string>().Adapt();

            queue.Enqueue( "test" );

            // act
            var target = new QueueDebugView<string>( queue );

            // assert
            Assert.NotNull( target.Items );
            Assert.Equal( 1, target.Items.Length );
            Assert.Equal( queue.Peek(), target.Items[0] );
        }
    }
}
