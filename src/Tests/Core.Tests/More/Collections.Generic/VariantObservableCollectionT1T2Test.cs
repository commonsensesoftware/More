namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="VariantObservableCollection{TFrom,TTo}"/>.
    /// </summary>
    public class VariantObservableCollectionT1T2Test
    {
        [Theory( DisplayName = "new variant observable collection should be covariant" )]
        [InlineData( typeof( IList<object> ) )]
        [InlineData( typeof( ICollection<object> ) )]
        [InlineData( typeof( IEnumerable<object> ) )]
        [InlineData( typeof( IList<string> ) )]
        [InlineData( typeof( ICollection<string> ) )]
        [InlineData( typeof( IEnumerable<string> ) )]
        public void CollectionShouldBeCovariant( Type covariantType )
        {
            // arrange
            var target = new VariantObservableCollection<string, object>();

            // act

            // assert
            Assert.IsAssignableFrom( covariantType, target );
        }

        [Fact( DisplayName = "index of should return expected value" )]
        public void IListIndexOfShouldReturnCorrectValue()
        {
            IList<string> target = new VariantObservableCollection<string, object>();
            var expected = "test";
            Assert.Equal( -1, target.IndexOf( expected ) );
            target.Add( expected );
            Assert.Equal( 0, target.IndexOf( expected ) );
        }

        [Fact( DisplayName = "insert should add item" )]
        public void IListShouldPerformInsert()
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();
            var expected = "test";

            // act
            target.Insert( 0, expected );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.Equal( expected, target[0] );
        }

        [Fact( DisplayName = "remove at should remove item" )]
        public void IListShouldPerformRemoveAt()
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();
            target.Add( "test" );
            Assert.Equal( 1, target.Count );

            // act
            target.RemoveAt( 0 );

            // assert
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "indexer should write expected item" )]
        public void IListIndexerShouldBeReadWrite()
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();
            var expected = "test";
            
            target.Add( string.Empty );
            Assert.Equal( 1, target.Count );

            // act
            target[0] = expected;

            // assert
            Assert.Equal( expected, target[0] );
        }

        [Fact( DisplayName = "add should append item" )]
        public void IListShouldPerformAdd()
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();

            // act
            target.Add( "test" );

            // assert
            Assert.Equal( 1, target.Count );
        }

        [Fact( DisplayName = "clear should remove all items" )]
        public void IListShouldPerformClear()
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();
            target.Add( "test" );
            Assert.Equal( 1, target.Count );

            // act
            target.Clear();

            // assert
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "contains should return expected result" )]
        public void IListShouldPerformContains()
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();
            var expected = "test";

            // act
            target.Add( expected );

            // assert
            Assert.True( target.Contains( expected ) );
        }

        [Fact( DisplayName = "copy to should copy items" )]
        public void IListShouldPerformCopyTo()
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();
            var expected = new string[3];

            target.Add( "test1" );
            target.Add( "test2" );
            target.Add( "test3" );

            // act
            target.CopyTo( expected, 0 );

            // assert
            Assert.True( target.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "read-only should return expected value" )]
        public void IListShouldReturnReadOnlyState()
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();

            // act
            var actual = target.IsReadOnly;

            // assert
            Assert.False( actual );
        }

        [Theory( DisplayName = "remove should remove item" )]
        [InlineData( "test", true )]
        [InlineData( "other test", false )]
        public void IListShouldPerformRemove( string item, bool expected )
        {
            // arrange
            IList<string> target = new VariantObservableCollection<string, object>();
            
            target.Add( "test" );

            // act
            var actual = target.Remove( item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "variant observable collection should enumerate in sequence" )]
        public void IListShouldReturnEnumerator()
        {
            IList<string> target = new VariantObservableCollection<string, object>();
            var expected = new[] { "test1", "test2", "test3" };

            foreach ( var item in expected )
                target.Add( item );

            IEnumerable<string> actual = target;

            Assert.True( actual.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "collection should behave as expected with read-only adapter" )]
        public void CollectionShouldBehaveAsExpectedWhenSuppliedToReadOnlyAdapter()
        {
            // arrange
            var expectedProperties = new[] { "Count", "Item[]" };
            var actualProperties = new List<string>();
            var target = new VariantObservableCollection<string, object>();
            var expected = new ReadOnlyObservableCollection<object>( target );

            ( (INotifyPropertyChanged) target ).PropertyChanged += ( s, e ) => actualProperties.Add( e.PropertyName );

            // act
            target.Add( "test" );

            // assert
            Assert.Equal( expectedProperties.Length, actualProperties.Count );
            Assert.True( actualProperties.SequenceEqual( expectedProperties ) );
            Assert.Equal( expected.Count, target.Count );
            Assert.Equal( expected[0], target[0] );
            Assert.IsType<string>( target[0] );
            Assert.IsType<string>( expected[0] );
        }
    }
}
