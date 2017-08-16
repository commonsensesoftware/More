namespace More.Collections.Generic
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    public class VariantObservableCollectionT1T2Test
    {
        [Theory]
        [InlineData( typeof( IList<object> ) )]
        [InlineData( typeof( ICollection<object> ) )]
        [InlineData( typeof( IEnumerable<object> ) )]
        [InlineData( typeof( IList<string> ) )]
        [InlineData( typeof( ICollection<string> ) )]
        [InlineData( typeof( IEnumerable<string> ) )]
        public void new_variant_observable_collection_should_be_covariant( Type covariantType )
        {
            // arrange
            var collection = new VariantObservableCollection<string, object>();

            // act

            // assert
            collection.Should().BeAssignableTo( covariantType );
        }

        [Theory]
        [InlineData( "test", 0 )]
        [InlineData( "other test", -1 )]
        public void index_of_should_return_expected_value( string value, int expected )
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>() { "test" };

            // act
            var index = collection.IndexOf( value );

            // assert
            index.Should().Be( expected );
        }

        [Fact]
        public void insert_should_add_item()
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>();

            // act
            collection.Insert( 0, "test" );

            // assert
            collection.Should().Equal( new[] { "test" } );
        }

        [Fact]
        public void remove_at_should_remove_item()
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>() { "test" };

            // act
            collection.RemoveAt( 0 );

            // assert
            collection.Should().BeEmpty();
        }

        [Fact]
        public void indexer_should_write_expected_item()
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>() { string.Empty };

            // act
            collection[0] = "test";

            // assert
            collection.Should().Equal( new[] { "test" } );
        }

        [Fact]
        public void add_should_append_item()
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>();

            // act
            collection.Add( "test" );

            // assert
            collection.Should().Equal( new[] { "test" } );
        }

        [Fact]
        public void clear_should_remove_all_items()
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>() { "test" };

            // act
            collection.Clear();

            // assert
            collection.Should().BeEmpty();
        }

        [Fact]
        public void contains_should_return_expected_result()
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>() { "test" };

            // act
            var contains = collection.Contains( "test" );

            // assert
            contains.Should().BeTrue();
        }

        [Fact]
        public void copy_to_should_copy_items()
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>() { "test1", "test2", "test3" };
            var items = new string[3];

            // act
            collection.CopyTo( items, 0 );

            // assert
            items.Should().Equal( collection );
        }

        [Fact]
        public void readX2Donly_should_return_expected_value()
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>();

            // act
            var readOnly = collection.IsReadOnly;

            // assert
            readOnly.Should().BeFalse();
        }

        [Theory]
        [InlineData( "test", true )]
        [InlineData( "other test", false )]
        public void remove_should_remove_item( string item, bool expected )
        {
            // arrange
            IList<string> collection = new VariantObservableCollection<string, object>() { "test" };

            // act
            var removed = collection.Remove( item );

            // assert
            removed.Should().Be( expected );
        }

        [Fact]
        public void variant_observable_collection_should_enumerate_in_sequence()
        {
            // arrange
            var collection = new VariantObservableCollection<string, object>() { "test1", "test2", "test3" };

            // act
            IEnumerable<string> sequence = collection;

            // assert
            sequence.Should().Equal( collection );
        }

        [Fact]
        public void collection_should_behave_as_expected_with_readX2Donly_adapter()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var collection = new VariantObservableCollection<string, object>();
            var readOnlyCollection = new ReadOnlyObservableCollection<object>( collection );

            ( (INotifyPropertyChanged) collection ).PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            collection.Add( "test" );

            // assert
            actualProperties.Should().Equal( expectedProperties );
            readOnlyCollection[0].Should().Be( "test" );
        }
    }
}