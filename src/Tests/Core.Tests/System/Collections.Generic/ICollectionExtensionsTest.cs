namespace System.Collections.Generic
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ICollectionExtensions"/>.
    /// </summary>
    public class ICollectionExtensionsTest
    {
        [Fact( DisplayName = "add range should copy sequence" )]
        public void AddRangeShouldCopySequence()
        {
            // arrange
            var expected = new List<object>() { new object(), new object(), new object() };
            var target = new Collection<object>();

            // act
            target.AddRange( expected );

            // assert
            Assert.Equal( expected.Count, target.Count );
            Assert.True( target.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "add range should copy partial sequence" )]
        public void AddRangeShouldCopyPartialSequence()
        {
            // arrange
            var count = 2;
            var expected = new List<object>() { new object(), new object(), new object() };
            var target = new Collection<object>();

            // act
            target.AddRange( expected, count );

            // assert
            Assert.Equal( count, target.Count );
            Assert.True( target.SequenceEqual( expected.Take( count ) ) );
        }

        [Fact( DisplayName = "add range should allow empty sequence" )]
        public void AddRangeShouldAllowEmptySequence()
        {
            // arrange
            var expected = Enumerable.Empty<object>();
            var target = new Collection<object>();

            // act
            target.AddRange( expected );

            // assert
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "remove range should remove sequence" )]
        public void RemoveRangeShouldRemoveSequence()
        {
            // arrange
            var lastItem = new object();
            var target = new Collection<object>() { new object(), new object(), lastItem };
            var items = target.Take( 2 ).ToList();

            // act
            target.RemoveRange( items );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.Equal( lastItem, target[0] );
        }

        [Fact( DisplayName = "remove range should remove sequece with comparer" )]
        public void RemoveRangeShouldRemoveSequenceWithComparer()
        {
            // arrange
            var lastItem = new object();
            var target = new Collection<object>() { new object(), new object(), lastItem };
            var items = target.Take( 2 ).ToList();

            // act
            target.RemoveRange( items, EqualityComparer<object>.Default );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.Equal( lastItem, target[0] );
        }

        [Fact( DisplayName = "remove should remove item with comparer" )]
        public void RemoveShouldRemoveItem()
        {
            // arrange
            var item = new object();
            var target = new Collection<object>() { new object(), new object(), item };

            // act
            Assert.True( target.Remove( item, EqualityComparer<object>.Default ) );

            // assert
            Assert.Equal( 2, target.Count );
            Assert.False( target.Contains( item ) );
        }

        [Fact( DisplayName = "remove should return false with comparer" )]
        public void RemoveShouldReturnFalseForUnmatchedItem()
        {
            // arrange
            var target = new Collection<object>() { new object(), new object(), new object() };

            // act
            Assert.False( target.Remove( new object(), EqualityComparer<object>.Default ) );

            // assert
            Assert.Equal( 3, target.Count );
        }

        [Fact( DisplayName = "remove all should remove items" )]
        public void RemoveAllShouldRemoveAllInstancesOfItem()
        {
            // arrange
            var item = new object();
            var target = new Collection<object>() { item, new object(), item };

            // act
            Assert.Equal( 2, target.RemoveAll( item ) );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.False( target.Contains( item ) );
        }

        [Fact( DisplayName = "remove all should remove items with comparer" )]
        public void RemoveAllShouldRemoveAllInstancesOfItemWithComparer()
        {
            // arrange
            var item = new object();
            var target = new Collection<object>() { item, new object(), item };

            // act
            Assert.Equal( 2, target.RemoveAll( item, EqualityComparer<object>.Default ) );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.False( target.Contains( item ) );
        }

        [Fact( DisplayName = "replace all should clear and add range" )]
        public void ReplaceAllShouldClearAndAddRange()
        {
            // arrange
            var oldItems = new List<object>() { new object(), new object(), new object() };
            var newItems = new List<object>() { new object(), new object(), new object() };
            var target = new Collection<object>();

            target.AddRange( oldItems );

            // act
            target.ReplaceAll( newItems );

            // assert
            Assert.Equal( newItems.Count, target.Count );
            Assert.True( target.SequenceEqual( newItems ) );
        }
    }
}
