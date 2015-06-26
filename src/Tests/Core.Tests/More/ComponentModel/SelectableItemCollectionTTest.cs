namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="SelectableItemCollection{T}"/> class.
    /// </summary>
    public class SelectableItemCollectionTTest
    {
        [Fact( DisplayName = "item in collection should select and unselect" )]
        public void ShouldSelectItemInCollection()
        {
            // arrange
            var target = new SelectableItemCollection<string>( new[] { "1", "2", "3" } );

            // act - select
            target[1].IsSelected = true;

            // assert - selected
            Assert.Equal( 1, target.SelectedItems.Count );
            Assert.Equal( 1, target.SelectedValues.Count );
            Assert.Equal( "2", target.SelectedItems[0].Value );
            Assert.Equal( "2", target.SelectedValues[0] );
            
            // act - unselect
            target.SelectedItems[0].IsSelected = false;

            // assert - unselected
            Assert.Equal( 0, target.SelectedItems.Count );
            Assert.Equal( 0, target.SelectedValues.Count );
        }

        [Fact( DisplayName = "selected value should select item" )]
        public void ShouldSelectValueInCollection()
        {
            // arrange
            var target = new SelectableItemCollection<string>( new[] { "1", "2", "3" } );
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();

            ( (INotifyPropertyChanged) target.SelectedValues ).PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act            
            target.SelectedValues.Add( "2" );

            // assert
            Assert.Equal( 1, target.SelectedItems.Count );
            Assert.Equal( "2", target.SelectedValues[0] );
            Assert.Equal( "2", target.SelectedItems[0].Value );
            Assert.True( actualProperties.SequenceEqual( expectedProperties ) );
        }

        [Fact( DisplayName = "clear should unselect values" )]
        public void ShouldUnselectValueInCollectionOnClear()
        {
            // arrange
            var target = new SelectableItemCollection<string>( new[] { "1", "2", "3" } );
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();

            target.SelectedValues.Add( "2" );
            ( (INotifyPropertyChanged) target.SelectedValues ).PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            target.SelectedValues.Clear();

            // assert
            Assert.Equal( 0, target.SelectedItems.Count );
            Assert.True( actualProperties.SequenceEqual( expectedProperties ) );
        }

        [Fact( DisplayName = "item should select all items in collection" )]
        public void ShouldSelectAllItemsInCollection()
        {
            // arrange
            var target = new SelectableItemCollection<string>( new[] { "1", "2", "3" }, "All" );

            // assert - initial state
            Assert.Equal( "All", target[0].Value );
            Assert.Equal( 4, target.Count );
            Assert.Equal( 0, target.SelectedItems.Count );
            Assert.Equal( 0, target.SelectedValues.Count );

            // act - select all items
            target[0].IsSelected = true;

            // assert - all items selected
            Assert.Equal( true, target[0].IsSelected );
            Assert.Equal( 3, target.SelectedItems.Count );
            Assert.Equal( 3, target.SelectedValues.Count );

            // act - unselect one item, which causes the 'All' item to be indeterminate (null)
            target.SelectedItems[0].IsSelected = false;

            // assert - item unselected
            Assert.Null( target[0].IsSelected );
            Assert.Equal( 2, target.SelectedItems.Count );
            Assert.Equal( 2, target.SelectedValues.Count );

            // act - unselect all items
            target[0].IsSelected = false;

            // assert - all items unselected
            Assert.Equal( false, target[0].IsSelected );
            Assert.Equal( 0, target.SelectedItems.Count );
            Assert.Equal( 0, target.SelectedValues.Count );
        }
    }
}
