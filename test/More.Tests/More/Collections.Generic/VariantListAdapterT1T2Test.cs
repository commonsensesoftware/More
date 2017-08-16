namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Xunit;

    public class VariantListAdapterT1T2Test
    {
        [Fact]
        public void index_of_should_return_expected_value()
        {
            // arrange
            var list = new VariantListAdapter<string, object>( new List<string>() );

            list.Add( "test" );

            // act
            var index = list.IndexOf( "test" );

            // assert
            index.Should().Be( 0 );
        }

        [Fact]
        public void insert_should_add_value()
        {
            // arrange
            var list = new VariantListAdapter<string, object>( new List<string>() );

            // act
            list.Insert( 0, "test" );

            // assert
            list[0].Should().Be( "test" );
        }

        [Fact]
        public void remove_at_should_remove_item()
        {
            // arrange
            var list = new VariantListAdapter<string, object>( new List<string>() ) { "test" };

            // act
            list.RemoveAt( 0 );

            // assert
            list.Should().BeEmpty();
        }

        [Fact]
        public void indexer_should_set_expected_value()
        {
            // arrange
            var list = new VariantListAdapter<string, object>( new List<string>() ) { "test1" };

            // act
            list[0] = "test2";

            // assert
            list[0].Should().Be( "test2" );
        }

        [Fact]
        public void clear_should_remove_items()
        {
            // arrange
            var list = new VariantListAdapter<string, object>( new List<string>() ) { "test" };

            // act
            list.Clear();

            // assert
            list.Should().BeEmpty();
        }

        [Theory]
        [InlineData( "test", true )]
        [InlineData( "other test", false )]
        public void contains_should_return_expected_result( string value, bool expected )
        {
            // arrange
            var list = new VariantListAdapter<string, object>( new List<string>() ) { "test" };

            // act
            var result = list.Contains( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void copy_to_should_copy_source_items()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };
            var items = new object[3];
            var list = new VariantListAdapter<string, object>( expected );

            // act
            list.CopyTo( items, 0 );

            // assert
            items.Should().Equal( expected );
        }

        [Fact]
        public void readX2Donly_should_return_source_value()
        {
            // arrange
            var collection = new ReadOnlyCollection<string>( new List<string>() );
            var list = new VariantListAdapter<string, object>( collection );
            var expected = ( (ICollection<string>) collection ).IsReadOnly;

            // act
            var readOnly = list.IsReadOnly;

            // assert
            readOnly.Should().Be( expected );
        }

        [Theory]
        [InlineData( "test", true )]
        [InlineData( "other test", false )]
        public void remove_should_remove_from_source( string value, bool expected )
        {
            // arrange
            var list = new VariantListAdapter<string, object>( new List<string>() ) { "test" };

            // act
            var removed = list.Remove( value );

            // assert
            removed.Should().Be( expected );
        }
    }
}