namespace More.Collections.Generic
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class StackAdapterTTest
    {
        [Fact]
        public void peek_should_peek_source()
        {
            // arrange
            var source = new Stack<object>();
            var target = new StackAdapter<object>( source );

            // act
            target.Push( new object() );

            // assert
            target.Peek().Should().Be( source.Peek() );
        }

        [Fact]
        public void pop_should_pop_source()
        {
            // arrange
            var source = new Stack<object>();
            var target = new StackAdapter<object>( source );
            var expected = new object();

            target.Push( expected );

            // act
            var item = target.Pop();

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void clear_should_clear_source()
        {
            // arrange
            var source = new Stack<object>();
            var target = new StackAdapter<object>( source );

            target.Push( new object() );

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
            var source = new Stack<object>( expected );
            var target = new StackAdapter<object>( source );
            var items = new object[3];

            // act
            target.CopyTo( items, 0 );

            // assert
            items.Should().BeEquivalentTo( expected.Reverse() );
        }
    }
}