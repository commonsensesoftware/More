namespace More.ComponentModel
{
    using FluentAssertions;
    using System.ComponentModel;
    using Xunit;

    public class SelectableItemCollectionTTest
    {
        [Fact]
        public void value_in_collection_should_select_item()
        {
            // arrange
            var collection = new SelectableItemCollection<string>( new[] { "1", "2", "3" } );

            // act
            collection[1].IsSelected = true;

            // assert
            collection.SelectedValues.Should().Equal( new[] { "2" } );
            collection.SelectedItems.ShouldBeEquivalentTo( new[] { new { Value = "2" } }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void value_in_collection_should_unselect_item()
        {
            // arrange
            var collection = new SelectableItemCollection<string>( new[] { "1", "2", "3" } );

            collection[1].IsSelected = true;

            // act
            collection.SelectedItems[0].IsSelected = false;

            // assert
            collection.SelectedValues.Should().BeEmpty();
            collection.SelectedItems.Should().BeEmpty();
        }

        [Fact]
        public void selected_value_should_select_item()
        {
            // arrange
            var collection = new SelectableItemCollection<string>( new[] { "1", "2", "3" } );
            var selectedValues = collection.SelectedValues;

            selectedValues.MonitorEvents<INotifyPropertyChanged>();

            // act            
            collection.SelectedValues.Add( "2" );

            // assert
            selectedValues.Should().Equal( new[] { "2" } );
            selectedValues.ShouldRaisePropertyChangeFor( c => c.Count );
            selectedValues.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
            collection.SelectedItems.ShouldBeEquivalentTo( new[] { new { Value = "2" } }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void clear_should_unselect_values()
        {
            // arrange
            var collection = new SelectableItemCollection<string>( new[] { "1", "2", "3" } );
            var selectedValues = collection.SelectedValues;

            collection.SelectedValues.Add( "2" );
            selectedValues.MonitorEvents<INotifyPropertyChanged>();

            // act
            collection.SelectedValues.Clear();

            // assert
            collection.SelectedValues.Should().BeEmpty();
            collection.SelectedItems.Should().BeEmpty();
            selectedValues.ShouldRaisePropertyChangeFor( c => c.Count );
            selectedValues.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == "Item[]" );
        }

        [Fact]
        public void item_should_select_all_items_in_collection()
        {
            // arrange
            var collection = new SelectableItemCollection<string>( new[] { "1", "2", "3" }, "All" );

            // act
            collection[0].IsSelected = true;

            // assert
            collection[0].IsSelected.Should().BeTrue();
            collection.SelectedValues.Should().HaveCount( 3 );
            collection.SelectedItems.Should().HaveCount( 3 );
        }

        [Fact]
        public void item_should_unselect_all_items_in_collection()
        {
            // arrange
            var collection = new SelectableItemCollection<string>( new[] { "1", "2", "3" }, "All" );

            collection[0].IsSelected = true;

            // act
            collection[0].IsSelected = false;

            // assert
            collection[0].IsSelected.Should().BeFalse();
            collection.SelectedValues.Should().BeEmpty();
            collection.SelectedItems.Should().BeEmpty();
        }

        [Fact]
        public void item_should_be_indeterminate_when_a_childe_item_is_unselected_in_collection()
        {
            // arrange
            var collection = new SelectableItemCollection<string>( new[] { "1", "2", "3" }, "All" );

            collection[0].IsSelected = true;

            // act
            collection.SelectedItems[0].IsSelected = false;

            // assert
            collection[0].IsSelected.Should().BeNull();
            collection.SelectedValues.Should().HaveCount( 2 );
            collection.SelectedItems.Should().HaveCount( 2 );
        }
    }
}