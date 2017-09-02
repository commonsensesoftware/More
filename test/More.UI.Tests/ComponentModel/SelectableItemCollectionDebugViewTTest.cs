namespace More.ComponentModel
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class SelectableItemCollectionDebugViewTTest
    {
        [Fact]
        public void items_should_return_expected_results()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };
            var collection = new SelectableItemCollection<string>( expected );
            var debugView = new SelectableItemCollectionDebugView<string>( collection );

            // act
            var values = debugView.Items.Select( i => i.Value );

            // assert
            values.Should().Equal( expected );
        }

        [Fact]
        public void selected_values_should_return_expected_results()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };
            var collection = new SelectableItemCollection<string>( expected );
            var debugView = new SelectableItemCollectionDebugView<string>( collection );

            collection.ForEach( i => i.IsSelected = true );

            // act
            var selectedValues = debugView.SelectedValues;

            // assert
            selectedValues.Should().Equal( expected );
        }

        [Fact]
        public void selected_items_should_return_expected_results()
        {
            // arrange
            var items = new[] { "1", "2", "3" };
            var collection = new SelectableItemCollection<string>( items );
            var debugView = new SelectableItemCollectionDebugView<string>( collection );

            collection.ForEach( i => i.IsSelected = true );

            // act
            var selectedItems = debugView.SelectedItems;

            // assert
            selectedItems.Should().Equal( collection );
        }
    }
}