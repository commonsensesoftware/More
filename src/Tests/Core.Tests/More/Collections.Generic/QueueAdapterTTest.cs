namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="QueueAdapter{T}"/>.
    /// </summary>
    public class QueueAdapterTTest
    {
        [Fact( DisplayName = "peek should peek source" )]
        public void PeekShouldPeekSourceQueue()
        {
            // arrange
            var source = new Queue<object>();
            var target = new QueueAdapter<object>( source );

            // act
            target.Enqueue( new object() );

            // assert
            Assert.Equal( source.Peek(), target.Peek() );
        }

        [Fact( DisplayName = "dequeue should dequeue from source" )]
        public void DequeueShouldDequeueSourceQueue()
        {
            // arrange
            var source = new Queue<object>();
            var target = new QueueAdapter<object>( source );

            target.Enqueue( new object() );

            var expected = source.Peek();

            // act
            var actual = target.Dequeue();

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "clear should clear source" )]
        public void ClearShouldClearSourceQueue()
        {
            // arrange
            var source = new Queue<object>();
            var target = new QueueAdapter<object>( source );

            target.Enqueue( new object() );

            // act
            target.Clear();

            // assert
            Assert.Equal( 0, source.Count );
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "copy to should copy from source" )]
        public void CopyToShouldCopySourceQueue()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            var source = new Queue<object>( expected );
            var target = new QueueAdapter<object>( source );
            var actual = new object[3];

            // act
            target.CopyTo( actual, 0 );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }
    }
}
