namespace System.Collections.Generic
{
    using FluentAssertions;
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

    public class IListExtensionsTest
    {
        [Fact]
        public void as_variant_should_return_self_when_covariant()
        {
            // arrange
            var list = new List<object>();

            // act
            var newList = list.AsVariant<object, object>();

            // assert
            newList.Should().BeSameAs( list );
        }

        [Fact]
        public void as_variant_should_return_new_list()
        {
            // arrange
            var list = new List<string>();

            // act
            var newList = list.AsVariant<string, object>();

            // assert
            newList.Should().BeOfType<VariantListAdapter<string, object>>();
        }

        [Fact]
        public void as_variant_should_return_observable_list_for_observable_source()
        {
            // arrange
            var list = new ObservableCollection<string>();

            // act
            var newList = list.AsVariant<string, object>();

            // assert
            newList.Should().BeOfType<ObservableVariantListAdapter<string, object>>();
        }

        [Fact]
        public void as_readX2Donly_should_return_self_when_readX2Donly()
        {
            // arrange
            IList<object> list = new ReadOnlyCollection<object>( new List<object>() );

            // act
            var newList = list.AsReadOnly();

            // assert
            newList.Should().BeSameAs( list );
        }

        [Fact]
        public void as_readX2Donly_should_return_new_collection()
        {
            // arrange
            var mock = new Mock<IList<object>>();

            mock.SetupGet( d => d.IsReadOnly ).Returns( true );
            mock.As<IReadOnlyList<object>>();

            var list = mock.Object;

            // act
            var newList = list.AsReadOnly();

            // assert
            newList.Should().BeSameAs( list );
        }

        [Fact]
        public void as_readX2Donly_should_return_observable_collection_for_observable_source()
        {
            // arrange
            IList<object> list = new ObservableCollection<object>();

            // act
            var newList = list.AsReadOnly();

            // assert
            newList.Should().BeAssignableTo<INotifyCollectionChanged>();
            newList.Should().BeAssignableTo<INotifyPropertyChanged>();
        }

        [Fact]
        public void sort_should_order_items_correctly()
        {
            // arrange
            var expected = new[] { "a", "b", "c", "d", "e" };
            var list = new[] { "c", "e", "b", "a", "d" };

            // act
            list.Sort();

            // assert
            list.Should().Equal( expected );
        }

        [Fact]
        public void binary_search_should_return_expected_result()
        {
            // arrange
            var list = new[] { "a", "b", "c", "d", "e" };

            // act
            var index = list.BinarySearch( "d" );

            // assert
            index.Should().Be( 3 );
        }

        [Fact]
        public void insert_range_should_add_expected_items()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            var list = new Collection<object>() { new object(), new object(), new object() };

            // act
            list.InsertRange( expected, 3 );

            // assert
            list.Should().HaveCount( 6 );
            list.Skip( 3 ).Should().Equal( expected );
        }

        [Fact]
        public void insert_range_should_add_expected_partial_sequence()
        {
            // arrange
            var item = new object();
            var expected = new[] { item, new object(), new object() };
            var list = new Collection<object>() { new object(), new object(), new object() };

            // act
            list.InsertRange( expected, 3, 1 );

            // assert
            list.Should().HaveCount( 4 );
            list.Last().Should().Be( item );
        }
    }
}