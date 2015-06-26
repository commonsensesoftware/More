namespace System.Collections.Generic
{
    using Moq;
    using More.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IListExtensions"/>.
    /// </summary>
    public class IListExtensionsTest
    {
        [Fact( DisplayName = "as variant should return self when covariant" )]
        public void AsVariantShouldReturnSelfWhenFromAndToTypesAreEqual()
        {
            // arrange
            var target = new List<object>();

            // act
            var actual = target.AsVariant<object, object>();

            // assert
            Assert.Same( target, actual );
        }

        [Fact( DisplayName = "as variant should return new list" )]
        public void AsVariantShouldReturnVariantList()
        {
            // arrange
            var target = new List<string>();

            // act
            var actual = target.AsVariant<string, object>();

            // assert
            Assert.IsType<VariantListAdapter<string, object>>( actual );
        }

        [Fact( DisplayName = "as variant should return observable list for observable source" )]
        public void AsVariantShouldReturnObservableVariantListWhenSupported()
        {
            // arrange
            var target = new ObservableCollection<string>();

            // act
            var actual = target.AsVariant<string, object>();

            // assert
            Assert.IsType<ObservableVariantListAdapter<string, object>>( actual );
        }

        [Fact( DisplayName = "as read-only should return self when read-only" )]
        public void AsReadOnlyShouldReturnSelfWhenReadOnly()
        {
            // arrange
            IList<object> target = new ReadOnlyCollection<object>( new List<object>() );

            // act
            var actual = target.AsReadOnly();

            // assert
            Assert.Same( target, actual );
        }

        [Fact( DisplayName = "as read-only should return new collection" )]
        public void AsReadOnlyShouldReturnReadOnlyCollection()
        {
            // arrange
            var mock = new Mock<IList<object>>();

            mock.SetupGet( d => d.IsReadOnly ).Returns( true );
            mock.As<IReadOnlyList<object>>();

            var target = mock.Object;

            // act
            var actual = target.AsReadOnly();

            // assert
            Assert.Same( target, actual );
        }

        [Fact( DisplayName = "as read-only should return observable collection for observable source" )]
        public void AsReadOnlyShouldReturnReadOnlyObservableCollectionWhenSupported()
        {
            // arrange
            IList<object> target = new ObservableCollection<object>();

            // act
            var actual = target.AsReadOnly();

            // assert
            Assert.IsAssignableFrom<INotifyCollectionChanged>( actual );
            Assert.IsAssignableFrom<INotifyPropertyChanged>( actual );
        }

        [Fact( DisplayName = "sort should order items correctly" )]
        public void SortShouldOrderItemsCorrectly()
        {
            // arrange
            var expected = new[] { "a", "b", "c", "d", "e" };
            var target = new[] { "c", "e", "b", "a", "d" };

            // act
            target.Sort();

            // assert
            Assert.True( target.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "binary search should return expected result" )]
        public void BinarySearchShouldReturnExpectedResult()
        {
            // arrange
            var expected = 3;
            var target = new[] { "a", "b", "c", "d", "e" };

            // act
            var actual = target.BinarySearch( "d" );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "insert range should add expected items" )]
        public void InsertRangeShouldAddExpectedItems()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            var target = new Collection<object>() { new object(), new object(), new object() };

            // act
            target.InsertRange( expected, 3 );

            // assert
            Assert.Equal( 6, target.Count );
            Assert.Equal( expected[0], target[3] );
            Assert.Equal( expected[1], target[4] );
            Assert.Equal( expected[2], target[5] );
        }

        [Fact( DisplayName = "insert range should add expected partial sequence" )]
        public void InsertRangeShouldAddExpectedSubsetOfItems()
        {
            // arrange
            var item = new object();
            var expected = new[] { item, new object(), new object() };
            var target = new Collection<object>() { new object(), new object(), new object() };

            // act
            target.InsertRange( expected, 3, 1 );

            // assert
            Assert.Equal( 4, target.Count );
            Assert.Equal( item, target.Last() );
        }
    }
}
