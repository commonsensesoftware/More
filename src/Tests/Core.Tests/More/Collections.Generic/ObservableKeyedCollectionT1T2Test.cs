namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ObservableKeyedCollection{TKey,TItem}"/>.
    /// </summary>
    public class ObservableKeyedCollectionT1T2Test
    {
        [Fact( DisplayName = "clear should remove items" )]
        public void ClearShouldRemoveItemsFromCollection()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var target = new ObservableKeyedCollection<string, int>( i => i.ToString() );
            
            target.Add( 1 );
            target.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );
            Assert.Equal( 1, target.Count );
            
            // act
            target.Clear();

            // assert
            Assert.Equal( 0, target.Count );
            Assert.True( actualProperties.SequenceEqual( expectedProperties ) );
        }

        [Fact( DisplayName = "add should append item" )]
        public void AddShouldInsertNewItem()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var target = new ObservableKeyedCollection<string, int>( i => i.ToString() );

            target.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            target.Add( 1 );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.Equal( 1, target[0] );
            Assert.True( actualProperties.SequenceEqual( expectedProperties ) );
        }

        [Fact( DisplayName = "insert should add item" )]
        public void InsertShouldAddNewItem()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var target = new ObservableKeyedCollection<string, int>( i => i.ToString() );

            target.AddRange( new[] { 1, 2, 3 } );
            target.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            target.Insert( 1, 4 );

            // assert
            Assert.Equal( 4, target.Count );
            Assert.True( target.SequenceEqual( new[] { 1, 4, 2, 3 } ) );
            Assert.True( actualProperties.SequenceEqual( expectedProperties ) );
        }

        [Fact( DisplayName = "remove should remove item" )]
        public void RemoveShouldClearItemFromCollection()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var target = new ObservableKeyedCollection<string, int>( i => i.ToString() );

            target.Add( 1 );
            target.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );
            
            // act
            Assert.True( target.Remove( 1 ) );

            // assert
            Assert.Equal( 0, target.Count );
            Assert.True( actualProperties.SequenceEqual( expectedProperties ) );
        }

        [Fact( DisplayName = "indexer should set item" )]
        public void IndexerShouldReplaceItem()
        {
            // arrange
            var target = new ObservableKeyedCollection<string, int>( i => i.ToString() );

            target.Add( 1 );

            // act
            Assert.PropertyChanged( target, "Item[]", () => target[0] = 2 );

            // assert
            Assert.Equal( 2, target[0] );
        }
    }
}
