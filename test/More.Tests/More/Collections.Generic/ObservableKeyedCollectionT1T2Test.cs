namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ObservableKeyedCollectionT1T2Test
    {
        [Fact]
        public void clear_should_remove_items()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var collection = new ObservableKeyedCollection<string, int>( i => i.ToString() );

            collection.Add( 1 );
            collection.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            collection.Clear();

            // assert
            collection.Should().BeEmpty();
            actualProperties.Should().Equal( expectedProperties );
        }

        [Fact]
        public void add_should_append_item()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var collection = new ObservableKeyedCollection<string, int>( i => i.ToString() );

            collection.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            collection.Add( 1 );

            // assert
            collection[0].Should().Be( 1 );
            actualProperties.Should().Equal( expectedProperties );
        }

        [Fact]
        public void insert_should_add_item()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var collection = new ObservableKeyedCollection<string, int>( i => i.ToString() );

            collection.AddRange( new[] { 1, 2, 3 } );
            collection.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            collection.Insert( 1, 4 );

            // assert
            collection.Should().Equal( new[] { 1, 4, 2, 3 } );
            actualProperties.Should().Equal( expectedProperties );
        }

        [Fact]
        public void remove_should_remove_item()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var collection = new ObservableKeyedCollection<string, int>( i => i.ToString() );

            collection.Add( 1 );
            collection.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            collection.Remove( 1 );

            // assert
            collection.Should().BeEmpty();
            actualProperties.Should().Equal( expectedProperties );
        }

        [Fact]
        public void indexer_should_set_item()
        {
            // arrange
            var collection = new ObservableKeyedCollection<string, int>( i => i.ToString() );
            var actualProperties = new List<string>();

            collection.Add( 1 );
            collection.PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            collection[0] = 2;

            // assert
            collection[0].Should().Be( 2 );
        }
    }
}