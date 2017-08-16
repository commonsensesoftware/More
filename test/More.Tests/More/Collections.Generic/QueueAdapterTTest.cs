namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class QueueAdapterTTest
    {
        [Fact]
        public void peek_should_peek_source()
        {
            // arrange
            var source = new Queue<object>();
            var target = new QueueAdapter<object>( source );

            // act
            target.Enqueue( new object() );

            // assert
            source.Peek().Should().Be( target.Peek() );
        }

        [Fact]
        public void dequeue_should_dequeue_from_source()
        {
            // arrange
            var source = new Queue<object>();
            var target = new QueueAdapter<object>( source );
            var expected = new object();

            target.Enqueue( expected );

            // act
            var result = target.Dequeue();

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void clear_should_clear_source()
        {
            // arrange
            var source = new Queue<object>();
            var target = new QueueAdapter<object>( source );

            target.Enqueue( new object() );

            // act
            target.Clear();

            // assert
            target.Should().BeEmpty();
            source.Should().BeEmpty();
        }

        [Fact]
        public void copy_to_should_copy_from_source()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            var source = new Queue<object>( expected );
            var target = new QueueAdapter<object>( source );
            var items = new object[3];

            // act
            target.CopyTo( items, 0 );

            // assert
            items.Should().Equal( expected );
        }
    }
}