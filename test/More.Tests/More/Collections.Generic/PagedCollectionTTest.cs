namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.ObjectModel;
    using Xunit;
    using static System.Linq.Enumerable;

    public class PagedCollectionTTest
    {
        [Fact]
        public void new_paged_collection_from_sequence_should_set_total_count()
        {
            // arrange
            var collection = new PagedCollection<object>( Empty<object>(), 3L );

            // act
            var totalCount = collection.TotalCount;

            // assert
            totalCount.Should().Be( 3L );
        }

        [Fact]
        public void new_paged_collection_from_observable_collection_should_set_total_count()
        {
            // act
            var collection = new PagedCollection<object>( new ObservableCollection<object>(), 3L );

            // act
            var totalCount = collection.TotalCount;

            // assert
            totalCount.Should().Be( 3L );
        }

        [Fact]
        public void new_paged_collection_from_sequence_should_copy_items()
        {
            // arrange
            var expected = new []{ new object(), new object(), new object() };

            // act
            var collection = new PagedCollection<object>( expected, 3L );

            // assert
            collection.Should().Equal( expected );
        }

        [Fact]
        public void new_paged_collection_from_observable_collection_should_copy_items()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };

            // act
            var collection = new PagedCollection<object>( new ObservableCollection<object>( expected ), 3L );

            // assert
            collection.Should().Equal( expected );
        }
    }
}