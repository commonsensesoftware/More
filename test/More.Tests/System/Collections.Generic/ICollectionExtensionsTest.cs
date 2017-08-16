namespace System.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Xunit;

    public class ICollectionExtensionsTest
    {
        [Fact]
        public void add_range_should_copy_sequence()
        {
            // arrange
            var expected = new List<object>() { new object(), new object(), new object() };
            var collection = new Collection<object>();

            // act
            collection.AddRange( expected );

            // assert
            collection.Should().BeEquivalentTo( expected );
        }

        [Fact]
        public void add_range_should_copy_partial_sequence()
        {
            // arrange
            var count = 2;
            var expected = new List<object>() { new object(), new object(), new object() };
            var collection = new Collection<object>();

            // act
            collection.AddRange( expected, count );

            // assert
            collection.Should().BeEquivalentTo( expected.Take( count ) );
        }

        [Fact]
        public void add_range_should_allow_empty_sequence()
        {
            // arrange
            var expected = Enumerable.Empty<object>();
            var collection = new Collection<object>();

            // act
            collection.AddRange( expected );

            // assert
            collection.Should().BeEmpty();
        }

        [Fact]
        public void remove_range_should_remove_sequence()
        {
            // arrange
            var lastItem = new object();
            var collection = new Collection<object>() { new object(), new object(), lastItem };
            var items = collection.Take( 2 ).ToArray();

            // act
            collection.RemoveRange( items );

            // assert
            collection.Should().Equal( new[] { lastItem } );
        }

        [Fact]
        public void remove_range_should_remove_sequece_with_comparer()
        {
            // arrange
            var lastItem = new object();
            var collection = new Collection<object>() { new object(), new object(), lastItem };
            var items = collection.Take( 2 ).ToArray();

            // act
            collection.RemoveRange( items, EqualityComparer<object>.Default );

            // assert
            collection.Should().Equal( new[] { lastItem } );
        }

        [Fact]
        public void remove_should_remove_item_with_comparer()
        {
            // arrange
            var item = new object();
            var collection = new Collection<object>() { new object(), new object(), item };

            // act
            var removed = collection.Remove( item, EqualityComparer<object>.Default );

            // assert
            removed.Should().BeTrue();
            collection.Should().HaveCount( 2 );
            collection.Contains( item ).Should().BeFalse();
        }

        [Fact]
        public void remove_should_return_false_with_comparer()
        {
            // arrange
            var collection = new Collection<object>() { new object(), new object(), new object() };

            // act
            var removed = collection.Remove( new object(), EqualityComparer<object>.Default );

            // assert
            removed.Should().BeFalse();
            collection.Should().HaveCount( 3 );
        }

        [Fact]
        public void remove_all_should_remove_items()
        {
            // arrange
            var item = new object();
            var collection = new Collection<object>() { item, new object(), item };

            // act
            var itemsRemoved = collection.RemoveAll( item );

            // assert
            itemsRemoved.Should().Be( 2 );
            collection.Should().HaveCount( 1 );
            collection.Contains( item ).Should().BeFalse();
        }

        [Fact]
        public void remove_all_should_remove_items_with_comparer()
        {
            // arrange
            var item = new object();
            var collection = new Collection<object>() { item, new object(), item };

            // act
            var itemsRemoved = collection.RemoveAll( item, EqualityComparer<object>.Default );

            // assert
            itemsRemoved.Should().Be( 2 );
            collection.Should().HaveCount( 1 );
            collection.Contains( item ).Should().BeFalse();
        }

        [Fact]
        public void replace_all_should_clear_and_add_range()
        {
            // arrange
            var oldItems = new List<object>() { new object(), new object(), new object() };
            var newItems = new List<object>() { new object(), new object(), new object() };
            var collection = new Collection<object>();

            collection.AddRange( oldItems );

            // act
            collection.ReplaceAll( newItems );

            // assert
            collection.Should().BeEquivalentTo( newItems );
        }
    }
}