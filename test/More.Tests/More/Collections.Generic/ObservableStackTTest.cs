namespace More.Collections.Generic
{
    using FluentAssertions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ObservableStackTTest
    {
        [Fact]
        public void new_observable_stack_should_initialize_items()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };

            // act
            var stack = new ObservableStack<string>( expected );

            // assert
            stack.Should().Equal( expected.Reverse() );
        }

        [Fact]
        public void push_should_raise_events()
        {
            // arrange
            var expected = "1";
            var stack = new ObservableStack<string>();

            stack.MonitorEvents();

            // act
            stack.Push( expected );

            // assert
            stack.Peek().Should().Be( expected );
            stack.ShouldRaisePropertyChangeFor( s => s.Count );
        }

        [Fact]
        public void pop_should_raise_events()
        {
            // arrange
            var expected = "1";
            var stack = new ObservableStack<string>();

            stack.Push( expected );
            stack.MonitorEvents();

            // act
            var result = stack.Pop();

            // assert
            result.Should().Be( expected );
            stack.ShouldRaisePropertyChangeFor( s => s.Count );
        }

        [Fact]
        public void pop_should_not_be_allowed_when_empty()
        {
            // arrange
            var stack = new ObservableStack<string>();

            // act
            Action pop = () => stack.Pop();

            // assert
            pop.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void peek_should_return_last_item()
        {
            // arrange
            var stack = new ObservableStack<string>();

            stack.Push( "2" );
            stack.Push( "1" );
            stack.Push( "3" );

            // act
            var result = stack.Peek();

            // assert
            result.Should().Be( "3" );
        }

        [Fact]
        public void peek_should_not_be_allowed_when_empty()
        {
            // arrange
            var stack = new ObservableStack<string>();

            // act
            Action peek = () => stack.Peek();

            // assert
            peek.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void to_array_should_return_items_in_sequence()
        {
            // arrange
            var stack = new ObservableStack<string>();
            var expected = new[] { "3", "2", "1" };

            stack.Push( "1" );
            stack.Push( "2" );
            stack.Push( "3" );

            // act
            var items = stack.ToArray();

            // assert
            items.Should().Equal( expected );
        }

        [Fact]
        public void trim_should_remove_excess()
        {
            // arrange
            var stack = new ObservableStack<string>( 10 );

            stack.Push( "1" );
            stack.Push( "2" );
            stack.Push( "3" );

            // act
            Action trimExcess = stack.TrimExcess;

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
            var stack = new ObservableStack<string>();

            stack.Push( "One" );
            stack.Push( "Two" );
            stack.Push( "Three" );

            // act
            var result = stack.Contains( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void copy_should_copy_items()
        {
            // arrange
            var expected = new[] { "1", "2" };
            var stack = new ObservableStack<string>( expected );
            var items = new string[2];

            // act
            stack.CopyTo( items, 0 );

            // assert
            items.Should().Equal( expected.Reverse() );
        }

        [Fact]
        public void copy_should_copy_items_with_offset()
        {
            // arrange
            var expected = new[] { "1", "2" };
            var stack = new ObservableStack<string>( expected );
            var items = new string[4];

            // act
            stack.CopyTo( items, 2 );

            // assert
            items.Skip( 2 ).Should().Equal( expected.Reverse() );
        }

        [Fact]
        public void copy_should_copy_untyped_items()
        {
            // arrange
            var stack = new ObservableStack<string>();
            var collection = (ICollection) stack;
            var expected = new[] { "1", "2" };
            var items = new string[2];

            stack.Push( "1" );
            stack.Push( "2" );

            // act
            collection.CopyTo( items, 0 );

            // assert
            items.Should().Equal( expected.Reverse() );
        }

        [Fact]
        public void clear_should_raise_events()
        {
            // arrange
            var stack = new ObservableStack<string>();

            stack.Push( "1" );
            stack.MonitorEvents();

            // act
            stack.Clear();

            // assert
            stack.Should().BeEmpty();
            stack.ShouldRaisePropertyChangeFor( s => s.Count );
        }

        [Fact]
        public void observable_stack_should_not_be_synchronized()
        {
            // arrange
            var stack = (ICollection) new ObservableStack<string>();

            // act

            // assert
            stack.IsSynchronized.Should().BeFalse();
            stack.SyncRoot.Should().NotBeNull();
        }

        [Fact]
        public void observable_stack_should_enumerate_in_typed_sequence()
        {
            // arrange
            var stack = new ObservableStack<string>();

            stack.Push( "1" );
            stack.Push( "2" );
            stack.Push( "3" );

            // act
            IEnumerable<string> items = stack;

            // assert
            items.Should().BeEquivalentTo( new[] { "1", "2", "3" } );
        }

        [Fact]
        public void observable_stack_should_enumerate_in_untyped_sequence()
        {
            // arrange
            var stack = new ObservableStack<string>();

            stack.Push( "1" );
            stack.Push( "2" );
            stack.Push( "3" );

            // act
            IEnumerable items = stack;

            // assert
            items.Should().BeEquivalentTo( new[] { "1", "2", "3" } );
        }

        [Fact]
        public void observable_stack_should_grow_dynamically()
        {
            // arrange
            var stack = new ObservableStack<string>( 3 );

            // act
            for ( var i = 0; i < 10; i++ )
            {
                stack.Push( ( i + 1 ).ToString() );
            }

            // assert
            stack.Clear();
            stack.TrimExcess();
        }
    }
}