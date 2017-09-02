namespace More.ComponentModel
{
    using FluentAssertions;
    using More.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static HierarchicalItemSelectionModes;

    public class HierarchicalItemCollectionTTest
    {
        [Fact]
        public void new_hierarchical_collection_should_be_tree_with_single_root()
        {
            // arrange

            // act
            var collection = new HierarchicalItemCollection<string>( CreateNodeHierarchy() );

            // assert
            collection.Should().HaveCount( 1 );
            collection[0].IsSelected.Should().BeFalse();
            collection[0].Should().HaveCount( 3 );
        }

        [Fact]
        public void new_hierarchical_collection_should_be_fan_with_multiple_roots()
        {
            // arrange

            // act
            var collection = new HierarchicalItemCollection<string>( CreateNodeHierarchy().ToArray() );

            // assert
            collection.Should().HaveCount( 3 );
            collection.All( i => i.IsSelected.Value ).Should().BeFalse();
        }

        [Fact]
        public void items_should_cycle_through_selection_states()
        {
            // arrange
            var collection = new HierarchicalItemCollection<string>( CreateNodeHierarchy() ) { SelectionMode = All | Synchronize };

            // act : select all items
            collection[0].IsSelected = true;

            // assert
            collection[0].IsSelected.Should().BeTrue();
            collection[0][0].IsSelected.Should().BeTrue();
            collection[0][1].IsSelected.Should().BeTrue();
            collection[0][2].IsSelected.Should().BeTrue();
            collection.SelectedItems.Should().HaveCount( 13 );

            // act: unselect a child item
            collection[0][0][0].IsSelected = false;

            // assert
            collection[0][0][0].IsSelected.Should().BeFalse();

            collection[0][0].IsSelected.Should().BeNull();
            collection[0].IsSelected.Should().BeNull();
            collection.SelectedItems.Should().HaveCount( 10 );

            // act : unselect all items
            collection[0].IsSelected = false;
            collection[0].IsSelected.Should().BeFalse();
            collection[0][0].IsSelected.Should().BeFalse();
            collection[0][1].IsSelected.Should().BeFalse();
            collection[0][2].IsSelected.Should().BeFalse();
            collection.SelectedItems.Should().BeEmpty();
        }

        [Fact]
        public void items_should_cycle_through_selection_states_when_only_leaves_are_selected()
        {
            // arrange
            var collection = new HierarchicalItemCollection<string>( CreateNodeHierarchy() ) { SelectionMode = Leaf | Synchronize };

            // act : select all items
            collection[0].IsSelected = true;

            // assert
            collection[0].IsSelected.Should().BeTrue();
            collection[0][0].IsSelected.Should().BeTrue();
            collection[0][1].IsSelected.Should().BeTrue();
            collection[0][2].IsSelected.Should().BeTrue();
            collection[0][0][0].IsSelected.Should().BeTrue();
            collection[0][0][1].IsSelected.Should().BeTrue();
            collection[0][0][2].IsSelected.Should().BeTrue();
            collection.SelectedItems.Should().HaveCount( 9 );

            // act : unselect a child item
            collection[0][0][0].IsSelected = false;

            // assert
            collection[0][0][0].IsSelected.Should().BeFalse();
            collection[0][0].IsSelected.Should().BeNull();
            collection[0].IsSelected.Should().BeNull();
            collection.SelectedItems.Should().HaveCount( 8 );

            // act : unselect all items
            collection[0].IsSelected = false;

            // assert
            collection[0].IsSelected.Should().BeFalse();
            collection[0][0].IsSelected.Should().BeFalse();
            collection[0][1].IsSelected.Should().BeFalse();
            collection[0][2].IsSelected.Should().BeFalse();
            collection[0][0][0].IsSelected.Should().BeFalse();
            collection.SelectedItems.Should().BeEmpty();
        }

        [Fact]
        public void collection_should_be_synchronized_with_source()
        {
            // arrange
            var source = CreateNodeHierarchy();
            var newItem = new Node<string>( "Item 1.4" );
            var collection = new HierarchicalItemCollection<string>( source ) { SelectionMode = All | Synchronize };

            // act : select all items
            collection[0].IsSelected = true;

            // assert
            collection.SelectedItems.Should().HaveCount( 13 );

            // act : add new item, which should cause parent update and unselection of items due to indeterminate state
            source[0].Add( newItem );

            // assert
            collection.SelectedItems.Should().HaveCount( 11 );
            collection[0][0][3].Value.Should().Be( newItem.Value );
            collection[0].IsSelected.Should().BeNull();

            // act : select new item, which should make entire path selected
            collection[0][0][3].IsSelected = true;

            // assert
            collection.SelectedItems.Should().HaveCount( 14 );
            collection[0].IsSelected.Should().BeTrue();
            collection[0][0][3].IsSelected.Should().BeTrue();

            // act : unselect item to revert state
            collection[0][0][3].IsSelected = false;

            // assert
            collection.SelectedItems.Should().HaveCount( 11 );
            collection[0].IsSelected.Should().BeNull();

            // act : remove a source item should cause the hierarchy to update
            source[0].Remove( newItem );

            // assert
            collection.SelectedItems.Should().HaveCount( 13 );
            collection[0].IsSelected.Should().BeTrue();
        }

        static Node<string> CreateNodeHierarchy()
        {
            return new Node<string>( "Root Item" )
            {
                new Node<string>( "Item 1" )
                {
                    new Node<string>( "Item 1.1" ),
                    new Node<string>( "Item 1.2" ),
                    new Node<string>( "Item 1.3" )
                },
                new Node<string>( "Item 2" )
                {
                    new Node<string>( "Item 2.1" ),
                    new Node<string>( "Item 2.2" ),
                    new Node<string>( "Item 2.3" )
                },
                new Node<string>( "Item 3" )
                {
                    new Node<string>( "Item 3.1" ),
                    new Node<string>( "Item 3.2" ),
                    new Node<string>( "Item 3.3" )
                }
            };
        }
    }
}