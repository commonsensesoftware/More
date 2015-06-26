namespace More.Collections.Generic
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="VariantListAdapter{TFrom,TTo}"/>.
    /// </summary>
    public class VariantListAdapterT1T2Test
    {
        [Fact( DisplayName = "index of should return expected value" )]
        public void IndexOfShouldReturnCorrectValue()
        {
            // arrange
            var target = new VariantListAdapter<string, object>( new List<string>() );

            target.Add( "test" );

            // act
            var actual = target.IndexOf( "test" );

            // assert
            Assert.Equal( 0, actual );
        }

        [Fact( DisplayName = "insert should add value" )]
        public void InsertShouldAddCorrectValue()
        {
            // arrange
            var target = new VariantListAdapter<string, object>( new List<string>() );

            // act
            target.Insert( 0, "test" );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.Equal( "test", target[0] );
        }

        [Fact( DisplayName = "remove at should remove item" )]
        public void RemoveAtShouldRemoveItem()
        {
            // arrange
            var target = new VariantListAdapter<string, object>( new List<string>() );

            target.Add( "test" );

            // act
            target.RemoveAt( 0 );

            // assert
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "indexer should set expected value" )]
        public void IndexerShouldReadWriteCorrectValue()
        {
            // arrange
            var expected = "test1";
            var target = new VariantListAdapter<string, object>( new List<string>() );

            target.Add( expected );
            var actual = target[0];
            Assert.Equal( expected, actual );

            expected = "test2";

            // act
            actual = target[0] = expected;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "clear should remove items" )]
        public void ClearShouldRemoveAllItems()
        {
            // arrange
            var target = new VariantListAdapter<string, object>( new List<string>() );

            target.Add( "test" );

            // act
            target.Clear();

            // assert
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "contains should return expected result" )]
        public void ContainsShouldReturnExpectedResult()
        {
            // arrange
            var target = new VariantListAdapter<string, object>( new List<string>() );

            target.Add( "test" );

            // act

            // assert
            Assert.True( target.Contains( "test" ) );
            Assert.False( target.Contains( "other test" ) );
        }

        [Fact( DisplayName = "copy to should copy source items" )]
        public void CopyToShouldCreateCovariantArray()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };
            var actual = new object[3];
            var target = new VariantListAdapter<string, object>( expected );

            // act
            target.CopyTo( actual, 0 );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "read-only should return source value" )]
        public void IsReadOnlyPropertyShouldMatchAdaptedList()
        {
            // arrange
            var collection = new ReadOnlyCollection<string>( new List<string>() );
            var target = new VariantListAdapter<string, object>( collection );
            var expected = ( (ICollection<string>) collection ).IsReadOnly;

            // act
            var actual = target.IsReadOnly;

            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "remove should remove from source" )]
        [InlineData( "test", true )]
        [InlineData( "other test", false )]
        public void RemoveShouldRemoveFromSource( string value, bool expected )
        {
            // arrange
            var target = new VariantListAdapter<string, object>( new List<string>() );

            target.Add( "test" );

            // act
            var actual = target.Remove( value );

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
