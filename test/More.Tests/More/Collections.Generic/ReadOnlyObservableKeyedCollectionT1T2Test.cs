namespace More.Collections.Generic
{
    using FluentAssertions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;

    public class ReadOnlyObservableKeyedCollectionT1T2Test
    {
        [Fact]
        public void indexer_should_return_expected_item_by_index()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            var collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );
            var expected = new object();

            inner.Add( expected );

            // act
            var item = collection[0];

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void indexer_should_return_expected_item_by_key()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            var collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );
            var expected = new object();

            inner.Add( expected );

            // act
            var item = collection[expected.ToString()];

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void index_of_should_return_expected_value()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            var collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );
            var item = new object();

            inner.Add( item );

            // act
            var index = collection.IndexOf( item );

            // assert
            index.Should().Be( 0 );
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_insert()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList<object> collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            // act
            Action insert = () => collection.Insert( 0, new object() );

            // assert
            insert.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_remove()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList<object> collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            inner.Add( new object() );

            // act
            Action removeAt = () => collection.RemoveAt( 0 );

            // assert
            removeAt.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_replacing_item()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList<object> collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            inner.Add( new object() );

            // act
            Action setItem = () => collection[0] = new object();

            // assert
            setItem.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_add()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            ICollection<object> collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            // act
            Action add = () => collection.Add( new object() );

            // assert
            add.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_clear()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            ICollection<object> collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            inner.Add( new object() );

            // act
            Action clear = () => collection.Clear();

            // assert
            clear.ShouldThrow<NotSupportedException>();
        }

        [Theory]
        [InlineData( 1, true )]
        [InlineData( 2, false )]
        public void contains_should_return_expected_result( int value, bool expected )
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, int>( i => i.ToString() );
            var collection = new ReadOnlyObservableKeyedCollection<string, int>( inner );

            inner.Add( 1 );

            // act
            var result = collection.Contains( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void copy_to_should_copy_source_items()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.GetHashCode().ToString() );
            var collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );
            var expected = new[] { new object(), new object(), new object() };
            var items = new object[3];

            inner.AddRange( expected );

            // act
            collection.CopyTo( items, 0 );

            // assert
            items.Should().Equal( expected );
        }

        [Fact]
        public void readX2Donly_should_always_return_true()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            ICollection<object> collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            // act

            // assert
            collection.IsReadOnly.Should().BeTrue();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_collection_remove()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            ICollection<object> collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );
            var item = new object();

            inner.Add( item );

            // act
            Action remove = () => collection.Remove( item );

            // assert
            remove.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_list_add()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            // act
            Action add = () => collection.Add( new object() );

            // assert
            add.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_list_insert()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            // act
            Action insert = () => collection.Insert( 0, new object() );

            // assert
            insert.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_list_clear()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            inner.Add( new object() );

            // act
            Action clear = () => collection.Clear();

            // assert
            clear.ShouldThrow<NotSupportedException>();
        }

        [Theory]
        [InlineData( 1, true )]
        [InlineData( 2, false )]
        public void list_contains_should_return_expected_result( int value, bool expected )
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, int>( i => i.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, int>( inner );

            inner.Add( value );

            // act
            var result = collection.Contains( 1 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void list_index_of_should_return_index_when_found()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );
            var item = new object();
            var expected = 0;

            inner.Add( item );

            // act
            var index = collection.IndexOf( item );

            // assert
            index.Should().Be( expected );
        }

        [Fact]
        public void list_index_of_should_return_X2D1_when_not_found()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, string>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, string>( inner );

            inner.Add( "test" );

            // act
            var result = collection.IndexOf( new object() );

            // assert
            result.Should().Be( -1 );
        }

        [Fact]
        public void list_readX2Donly_should_always_return_true()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            // act

            // assert
            collection.IsReadOnly.Should().BeTrue();
        }

        [Fact]
        public void list_readX2Donly_keyed_collection_should_not_allow_remove()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );
            var item = new object();

            inner.Add( item );

            // act
            Action remove = () => collection.Remove( item );

            // assert
            remove.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void readX2Donly_keyed_collection_should_not_allow_list_remove_at()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            inner.Add( new object() );

            // act
            Action removeAt = () => collection.RemoveAt( 0 );

            // assert
            removeAt.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void indexer_should_not_allow_item_replacement()
        {
            // arrange
            var inner = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList collection = new ReadOnlyObservableKeyedCollection<string, object>( inner );

            inner.Add( new object() );

            // act
            Action setItem = () => collection[0] = new object();

            // assert
            setItem.ShouldThrow<NotSupportedException>();
        }
    }
}