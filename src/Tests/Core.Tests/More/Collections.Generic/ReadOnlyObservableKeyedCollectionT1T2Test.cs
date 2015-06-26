namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ReadOnlyObservableKeyedCollection{TKey,TItem}"/>.
    /// </summary>
    public class ReadOnlyObservableKeyedCollectionT1T2Test
    {
        [Fact( DisplayName = "indexer should return expected item" )]
        public void IndexerShouldReturnExpectedValue()
        {
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            var target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            var expected = new object();
            
            source.Add( expected );

            var actual = target[0];
            Assert.Equal( expected, actual );

            actual = target[expected.ToString()];
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "index of should return expected value" )]
        public void IndexOfShouldReturnExpectedValue()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            var target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            var item = new object();
            var expected = 0;

            source.Add( item );

            // act
            var actual = target.IndexOf( item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "read-only keyed collection should not allow insert" )]
        public void IListOfTInsertShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList<object> target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            // act
            Assert.Throws<NotSupportedException>( () => target.Insert( 0, new object() ) );

            // assert
        }

        [Fact( DisplayName = "read-only keyed collection should not allow remove" )]
        public void IListOfTRemoveAtShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList<object> target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            source.Add( new object() );

            // act
            Assert.Throws<NotSupportedException>( () => target.RemoveAt( 0 ) );

            // assert
        }

        [Fact( DisplayName = "read-only keyed collection should not allow replacing item" )]
        public void IListOfTIndexerShouldThrowExceptionOnWrite()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList<object> target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            
            source.Add( new object() );

            // act
            Assert.Throws<NotSupportedException>( () => target[0] = new object() );

            // assert
        }

        [Fact( DisplayName = "read-only keyed collection should not allow add" )]
        public void ICollectionOfTAddShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            ICollection<object> target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            // act
            Assert.Throws<NotSupportedException>( () => target.Add( new object() ) );

            // assert
        }

        [Fact( DisplayName = "read-only keyed collection should not allow clear" )]
        public void ICollectionOfTClearShouldThrowException()
        {
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            ICollection<object> target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            source.Add( new object() );
            Assert.Throws<NotSupportedException>( () => target.Clear() );
        }

        [Fact( DisplayName = "contains should return expected result" )]
        public void ContainsShouldMatchSourceCollection()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            var target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            var item = new object();

            source.Add( item );

            // act

            // assert
            Assert.True( target.Contains( item ) );
            Assert.False( target.Contains( new object() ) );
        }

        [Fact( DisplayName = "copy to should copy source items" )]
        public void CopyToShouldCopySourceItems()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.GetHashCode().ToString() );
            var target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            var expected = new[] { new object(), new object(), new object() };
            var actual = new object[3];

            source.AddRange( expected );

            // act
            target.CopyTo( actual, 0 );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "read-only should always return true" )]
        public void ICollectionOfTIsReadOnlyShouldAlwaysReturnTrue()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            ICollection<object> target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            // act
            var actual = target.IsReadOnly;

            // assert
            Assert.True( actual );
        }

        [Fact( DisplayName = "read-only keyed collection should not allow remove" )]
        public void ICollectionOfTRemoveShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            ICollection<object> target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            var item = new object();

            source.Add( item );

            // act
            Assert.Throws<NotSupportedException>( () => target.Remove( item ) );

            // assert
        }

        [Fact( DisplayName = "read-only keyed collection should not allow add" )]
        public void IListAddShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            // act
            Assert.Throws<NotSupportedException>( () => target.Add( new object() ) );

            // assert
        }

        [Fact( DisplayName = "read-only keyed collection should not allow insert" )]
        public void IListInsertShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            // act
            Assert.Throws<NotSupportedException>( () => target.Insert( 0, new object() ) );

            // assert
        }

        [Fact( DisplayName = "read-only keyed collection should not allow clear" )]
        public void IListClearShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            source.Add( new object() );

            // act
            Assert.Throws<NotSupportedException>( () => target.Clear() );

            // assert
        }

        [Fact( DisplayName = "contains should return expected result" )]
        public void IListContainsShouldMatchSourceCollection()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            var item = new object();

            source.Add( item );

            // act

            // assert
            Assert.True( target.Contains( item ) );
            Assert.False( target.Contains( new object() ) );
        }

        [Fact( DisplayName = "contains should return expected result" )]
        public void IListContainsShouldNotMatchIncompatibleItemType()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, string>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, string>( source );
            var item = "test";

            source.Add( item );

            // act
            var actual = target.Contains( new object() );

            // assert
            Assert.False( actual );
        }

        [Fact( DisplayName = "index of should return expected value" )]
        public void IListIndexOfShouldReturnExpectedValue()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            var item = new object();
            var expected = 0;

            source.Add( item );

            // act
            var actual = target.IndexOf( item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "index of should return expected value" )]
        public void IListIndexOfShouldNotMatchIncompatibleItemType()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, string>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, string>( source );
            var expected = -1;

            source.Add( "test" );

            // act
            var actual = target.IndexOf( new object() );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "read-only should always return true" )]
        public void IListIsReadOnlyShouldAlwaysReturnTrue()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            // act
            var actual = target.IsReadOnly;

            // assert
            Assert.True( actual );
        }

        [Fact( DisplayName = "read-only keyed collection should not allow remove" )]
        public void IListRemoveShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );
            var item = new object();

            source.Add( item );

            // act
            Assert.Throws<NotSupportedException>( () => target.Remove( item ) );

            // assert
        }

        [Fact( DisplayName = "read-only keyed collection should not allow remove at" )]
        public void IListRemoveAtShouldThrowException()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            source.Add( new object() );

            // act
            Assert.Throws<NotSupportedException>( () => target.RemoveAt( 0 ) );

            // assert
        }

        [Fact( DisplayName = "indexer should not allow item replacement" )]
        public void IListIndexerShouldThrowExceptionOnWrite()
        {
            // arrange
            var source = new ObservableKeyedCollection<string, object>( o => o.ToString() );
            IList target = new ReadOnlyObservableKeyedCollection<string, object>( source );

            source.Add( new object() );

            // act
            Assert.Throws<NotSupportedException>( () => target[0] = new object() );

            // assert
        }
    }
}
