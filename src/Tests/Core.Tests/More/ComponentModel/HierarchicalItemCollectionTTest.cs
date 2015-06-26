namespace More.ComponentModel
{
    using More.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="HierarchicalItemCollection{T}"/> class.
    /// </summary>
    public class HierarchicalItemCollectionTTest
    {
        private static Node<string> CreateNodeHierarchy()
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

        [Fact( DisplayName = "new hierarchical collection should be tree with single root" )]
        public void CollectionShouldResembleTreeWhenRootNodeIsProvided()
        {
            var target = new HierarchicalItemCollection<string>( CreateNodeHierarchy() );
            Assert.Equal( 1, target.Count );
            Assert.Equal( false, target[0].IsSelected );
            Assert.Equal( 3, target[0].Count );
        }

        [Fact( DisplayName = "new hierarchical collection should be fan with multiple roots" )]
        public void CollectionShouldResembleFanWhenMultipleRootNodesAreProvided()
        {
            var target = new HierarchicalItemCollection<string>( CreateNodeHierarchy().ToList() );
            Assert.Equal( 3, target[0].Count );
            Assert.Equal( false, target[0].IsSelected );
            Assert.Equal( false, target[1].IsSelected );
            Assert.Equal( false, target[2].IsSelected );
        }

        [Fact( DisplayName = "items should have correct selection state" )]
        public void ItemsShouldHaveTheCorrectSelectionStateInTheCollection()
        {
            var target = new HierarchicalItemCollection<string>( CreateNodeHierarchy() );
            target.SelectionMode = HierarchicalItemSelectionModes.All | HierarchicalItemSelectionModes.Synchronize;

            // select all items
            target[0].IsSelected = true;

            Assert.Equal( true, target[0].IsSelected );
            Assert.Equal( true, target[0][0].IsSelected );
            Assert.Equal( true, target[0][1].IsSelected );
            Assert.Equal( true, target[0][2].IsSelected );
            Assert.Equal( 13, target.SelectedItems.Count );

            // unselect a child item
            target[0][0][0].IsSelected = false;

            Assert.Equal( false, target[0][0][0].IsSelected );
            Assert.Null( target[0][0].IsSelected );
            Assert.Null( target[0].IsSelected );
            Assert.Equal( 10, target.SelectedItems.Count );

            // unselect all items
            target[0].IsSelected = false;

            Assert.Equal( false, target[0].IsSelected );
            Assert.Equal( false, target[0][0].IsSelected );
            Assert.Equal( false, target[0][1].IsSelected );
            Assert.Equal( false, target[0][2].IsSelected );
            Assert.Equal( 0, target.SelectedItems.Count );
        }

        [Fact( DisplayName = "only leaves should be selected" )]
        public void OnlyLeafItemsShouldBeSelectedInCollection()
        {
            var target = new HierarchicalItemCollection<string>( CreateNodeHierarchy() );
            target.SelectionMode = HierarchicalItemSelectionModes.Leaf | HierarchicalItemSelectionModes.Synchronize;

            // select all items
            target[0].IsSelected = true;

            Assert.Equal( true, target[0].IsSelected );
            Assert.Equal( true, target[0][0].IsSelected );
            Assert.Equal( true, target[0][1].IsSelected );
            Assert.Equal( true, target[0][2].IsSelected );
            Assert.Equal( true, target[0][0][0].IsSelected );
            Assert.Equal( true, target[0][0][1].IsSelected );
            Assert.Equal( true, target[0][0][2].IsSelected );
            Assert.Equal( 9, target.SelectedItems.Count );

            // unselect a child item
            target[0][0][0].IsSelected = false;

            Assert.Equal( false, target[0][0][0].IsSelected );
            Assert.Null( target[0][0].IsSelected );
            Assert.Null( target[0].IsSelected );
            Assert.Equal( 8, target.SelectedItems.Count );

            // unselect all items
            target[0].IsSelected = false;

            Assert.Equal( false, target[0].IsSelected );
            Assert.Equal( false, target[0][0].IsSelected );
            Assert.Equal( false, target[0][1].IsSelected );
            Assert.Equal( false, target[0][2].IsSelected );
            Assert.Equal( false, target[0][0][0].IsSelected );
            Assert.Equal( 0, target.SelectedItems.Count );
        }

        [Fact( DisplayName = "collection should be in-sync with source" )]
        public void CollectionShouldBeInSyncWithSourceNodeCollection()
        {
            var source = CreateNodeHierarchy();
            var newItem = new Node<string>( "Item 1.4" );
            var target = new HierarchicalItemCollection<string>( source )
            {
                SelectionMode = HierarchicalItemSelectionModes.All | HierarchicalItemSelectionModes.Synchronize
            };

            // select all items
            target[0].IsSelected = true;
            Assert.Equal( 13, target.SelectedItems.Count );

            // add new item (should cause parent update and unselection of items due to indeterminate state)
            source[0].Add( newItem );
            Assert.Equal( 11, target.SelectedItems.Count );
            Assert.Equal( "Item 1.4", target[0][0][3].Value );
            Assert.Null( target[0].IsSelected );

            // select new item, which should make entire path selected
            target[0][0][3].IsSelected = true;
            Assert.Equal( 14, target.SelectedItems.Count );
            Assert.Equal( true, target[0][0][3].IsSelected );
            Assert.Equal( true, target[0].IsSelected );

            // unselect item to revert state
            target[0][0][3].IsSelected = false;
            Assert.Equal( 11, target.SelectedItems.Count );
            Assert.Null( target[0].IsSelected );

            // remove a source item should cause the hierarchy to update
            source[0].Remove( newItem );
            Assert.Equal( 13, target.SelectedItems.Count );
            Assert.Equal( true, target[0].IsSelected );
        }
    }
}
