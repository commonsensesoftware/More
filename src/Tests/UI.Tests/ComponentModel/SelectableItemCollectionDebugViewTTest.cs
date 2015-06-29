namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SelectableItemCollectionDebugView{T}"/>.
    /// </summary>
    public class SelectableItemCollectionDebugViewTTest
    {
        [Fact( DisplayName = "items should return expected results" )]
        public void ItemsPropertyShouldReturnExpectedResults()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };
            var collection = new SelectableItemCollection<string>( expected );
            var target = new SelectableItemCollectionDebugView<string>( collection );

            // act
            var actual = target.Items.Select( i => i.Value );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "selected values should return expected results" )]
        public void SelectedValuesPropertyShouldReturnExpectedResults()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };
            var collection = new SelectableItemCollection<string>( expected );
            var target = new SelectableItemCollectionDebugView<string>( collection );

            collection.ForEach( i => i.IsSelected = true );

            // act
            var actual = target.SelectedValues;

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "selected items should return expected results" )]
        public void SelectedItemsPropertyShouldReturnExpectedResults()
        {
            // arrange
            var items = new[] { "1", "2", "3" };
            var collection = new SelectableItemCollection<string>( items );
            var target = new SelectableItemCollectionDebugView<string>( collection );

            collection.ForEach( i => i.IsSelected = true );

            // act
            var actual = target.SelectedItems;

            // assert
            Assert.True( actual.SequenceEqual( collection ) );
        }
    }
}
