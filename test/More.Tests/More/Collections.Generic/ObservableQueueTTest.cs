namespace More.Collections.Generic
{
    using FluentAssertions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ObservableQueueTTest
    {
        [Fact]
        public void new_observable_queue_should_initialize_items()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };

            // act
            var queue = new ObservableQueue<string>( expected );

            // assert
            queue.Should().Equal( expected );
        }

        [Fact]
        public void enqueue_should_raise_events()
        {
            // arrange
            var expected = "1";
            var queue = new ObservableQueue<string>();

            queue.MonitorEvents();

            // act
            queue.Enqueue( expected );

            // assert
            queue.Peek().Should().Be( expected );
            queue.ShouldRaisePropertyChangeFor( q => q.Count );
        }

        [Fact]
        public void dequeue_should_raise_events()
        {
            // arrange
            var expected = "1";
            var queue = new ObservableQueue<string>();

            queue.Enqueue( expected );
            queue.MonitorEvents();

            // act
            queue.Dequeue();

            // assert
            queue.Should().BeEmpty();
            queue.ShouldRaisePropertyChangeFor( q => q.Count );
        }

        [Fact]
        public void dequeue_should_not_be_allowed_when_empty()
        {
            // arrange
            var queue = new ObservableQueue<string>();

            // act
            Action dequeue = () => queue.Dequeue();

            // assert
            dequeue.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void peek_should_return_first_item()
        {
            // arrange
            var queue = new ObservableQueue<string>();

            queue.Enqueue( "2" );
            queue.Enqueue( "1" );
            queue.Enqueue( "3" );

            // act
            var result = queue.Peek();

            // assert
            result.Should().Be( "2" );
        }

        [Fact]
        public void peek_should_not_be_allowed_when_empty()
        {
            // arrange
            var queue = new ObservableQueue<string>();

            // act
            Action peek = () => queue.Peek();

            // assert
            peek.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void to_array_should_return_items_in_seqence()
        {
            // arrange
            var queue = new ObservableQueue<string>();
            var expected = new[] { "1", "2", "3" };

            queue.Enqueue( "1" );
            queue.Enqueue( "2" );
            queue.Enqueue( "3" );

            // act
            var items = queue.ToArray();

            // assert
            items.Should().Equal( expected );
        }

        [Fact]
        public void trim_should_remove_excess()
        {
            // arrange
            var queue = new ObservableQueue<string>( 10 );

            queue.Enqueue( "1" );
            queue.Enqueue( "2" );
            queue.Enqueue( "3" );

            // act
            Action trimExcess = queue.TrimExcess;

            // assert
            trimExcess.ShouldNotThrow();
        }

        [Theory]
        [InlineData( "Two", true )]
        [InlineData( "Four", false )]
        [InlineData( null, false )]
        public void contains_should_return_expected_result( string value, bool expected )
        {
            // arrange
            var queue = new ObservableQueue<string>();

            queue.Enqueue( "One" );
            queue.Enqueue( "Two" );
            queue.Enqueue( "Three" );

            // act
            var result = queue.Contains( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void copy_to_should_copy_items()
        {
            // arrange
            var expected = new[] { "1", "2" };
            var queue = new ObservableQueue<string>( expected );

            queue.Enqueue( "1" );
            queue.Enqueue( "2" );

            var items = new string[2];

            // act
            queue.CopyTo( items, 0 );

            // assert
            items.Should().Equal( expected );
        }

        [Fact]
        public void copy_to_should_copy_items_with_offset()
        {
            // arrange
            var expected = new[] { "1", "2" };
            var queue = new ObservableQueue<string>( expected );

            queue.Enqueue( "1" );
            queue.Enqueue( "2" );

            var items = new string[4];

            // act
            queue.CopyTo( items, 2 );

            // assert
            items.Skip( 2 ).Should().Equal( expected );
        }

        [Fact]
        public void copy_to_should_copy_untyped_items()
        {
            // arrange
            var queue = new ObservableQueue<string>();
            var collection = (ICollection) queue;
            var expected = new[] { "1", "2" };
            var items = new string[2];

            queue.Enqueue( "1" );
            queue.Enqueue( "2" );

            // act
            collection.CopyTo( items, 0 );

            // assert
            items.Should().Equal( expected );
        }

        [Fact]
        public void clear_should_raise_events()
        {
            // arrange
            var queue = new ObservableQueue<string>();

            queue.Enqueue( "1" );
            queue.MonitorEvents();

            // act
            queue.Clear();

            // assert
            queue.Should().BeEmpty();
            queue.ShouldRaisePropertyChangeFor( q => q.Count );
        }

        [Fact]
        public void observable_queue_should_not_be_synchronized()
        {
            // arrange
            var queue = (ICollection) new ObservableQueue<string>();

            // act

            // assert
            queue.IsSynchronized.Should().BeFalse();
            queue.SyncRoot.Should().NotBeNull();
        }

        [Fact]
        public void observable_queue_should_enumerate_in_typed_sequence()
        {
            // arrange
            var queue = new ObservableQueue<string>();

            queue.Enqueue( "1" );
            queue.Enqueue( "2" );
            queue.Enqueue( "3" );

            // act
            IEnumerable<string> items = queue;

            // assert
            items.Should().Equal( new []{ "1", "2", "3" } );
        }

        [Fact]
        public void observable_queue_should_enumerate_in_untyped_sequence()
        {
            // arrange
            var queue = new ObservableQueue<string>();

            queue.Enqueue( "1" );
            queue.Enqueue( "2" );
            queue.Enqueue( "3" );

            // act
            IEnumerable items = queue;

            // assert
            items.Should().Equal( new[] { "1", "2", "3" } );
        }

        [Fact]
        public void observable_queue_should_grow_dynamically()
        {
            // arrange
            var queue = new ObservableQueue<string>( 3 );

            // act
            for ( var i = 0; i < 10; i++ )
            {
                queue.Enqueue( ( i + 1 ).ToString() );
            }

            // assert
            queue.Clear();
            queue.TrimExcess();
        }
    }
}