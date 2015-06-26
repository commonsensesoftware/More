namespace System.Collections
{
    using System;
    using System.Collections;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IEnumerableExtensions"/>.
    /// </summary>
    public class IEnumerableExtensionsTest
    {
        private System.Collections.Generic.IEnumerable<object> CreateIterator( System.Collections.Generic.IEnumerable<object> items )
        {
            foreach ( var item in items )
                yield return item;
        }

        [Fact( DisplayName = "any should not allow null sequence" )]
        public void AnyShouldNotAllowNullSequence()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => IEnumerableExtensions.Any( null ) );
            Assert.Equal( "sequence", ex.ParamName );
        }

        [Fact( DisplayName = "any should return true for non-empty sequence" )]
        public void AnyShouldReturnTrueForNonEmptySequence()
        {
            var list = new System.Collections.Generic.List<object>() { new object() };
            var target = CreateIterator( list );
            Assert.True( target.Any() );
        }

        [Fact( DisplayName = "any should return false for empty sequence" )]
        public void AnyShouldReturnFalseForEmptySequence()
        {
            var list = new System.Collections.Generic.List<object>();
            var target = CreateIterator( list );
            Assert.False( target.Any() );
        }

        [Fact( DisplayName = "count should not allow null sequence" )]
        public void CountShouldNotAllowNullSequence()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => IEnumerableExtensions.Count( null ) );
            Assert.Equal( "sequence", ex.ParamName );
        }

        [Fact( DisplayName = "count should return number of elements in sequence" )]
        public void CountShouldReturnNumberOfElementsInSequence()
        {
            var expected = new System.Collections.Generic.List<object>() { new object(), new object(), new object() };
            var actual = CreateIterator( expected );
            Assert.Equal( expected.Count, actual.Count() );
        }

        [Fact( DisplayName = "index should not allow null sequence" )]
        public void IndexOfShouldNotAllowNullSequence()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => IEnumerableExtensions.IndexOf( null, new object() ) );
            Assert.Equal( "sequence", ex.ParamName );
        }

        [Fact( DisplayName = "index of should not allow null comparer" )]
        public void IndexOfShouldNotAllowNullComparer()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => IEnumerableExtensions.IndexOf( new ArrayList(), new object(), null ) );
            Assert.Equal( "comparer", ex.ParamName );
        }

        [Fact( DisplayName = "index of should return expected value" )]
        public void IndexOfShouldReturnExpectedValue()
        {
            var list = new System.Collections.Generic.List<object>() { new object(), new object(), new object() };
            var target = CreateIterator( list );
            var expected = 2;
            var item = list[expected];
            var comparer = System.Collections.Generic.EqualityComparer<object>.Default;
            var actual = target.IndexOf( item, comparer );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "index of should return -1 if not found" )]
        public void IndexOfShouldReturnNegativeOneIfNotFound()
        {
            var list = new System.Collections.Generic.List<object>() { new object() };
            var target = CreateIterator( list );
            var expected = -1;
            var item = new object();
            var actual = target.IndexOf( item );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "element at should not allow null sequence" )]
        public void ElementAtShouldNotAllowNullSequence()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => IEnumerableExtensions.ElementAt( null, 0 ) );
            Assert.Equal( "sequence", ex.ParamName );
        }

        [Fact( DisplayName = "element at should not allow index out of range" )]
        public void ElementAtShouldNotAllowIndexOutOfRange()
        {
            var target = new ArrayList();
            var ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.ElementAt( -1 ) );
            Assert.Equal( "index", ex.ParamName );

            ex = Assert.Throws<ArgumentOutOfRangeException>( () => target.ElementAt( 1 ) );
            Assert.Equal( "index", ex.ParamName );
        }

        [Fact( DisplayName = "element at should return expected value from list" )]
        public void ElementAtShouldReturnExpectedValueFromList()
        {
            var list = new System.Collections.Generic.List<object>() { new object(), new object(), new object() };
            var target = CreateIterator( list );
            var index = 2;
            var expected = list[index];
            var actual = target.ElementAt( index );
            Assert.Equal( expected, actual );

            actual = list.ElementAt( index );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "element at should return expected value from dictionary" )]
        public void ElementAtShouldReturnExpectedValueFromDictionary()
        {
            var target = new Hashtable();
            target.Add( 0, new object() );
            target.Add( 1, new object() );
            target.Add( 2, new object() );
            
            var index = 2;
            var array = Array.CreateInstance( typeof( DictionaryEntry ), 3 );
            target.CopyTo( array, 0 );
            
            var expected = array.GetValue( index );
            var actual = target.ElementAt( index );
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "element at or default should return null for index out of range" )]
        [InlineData( -1 )]
        [InlineData( 3 )]
        public void ElementAtOrDefaultShouldReturnNullForIndexOutOfRange( int index )
        {
            // arrange
            var list = new ArrayList() { new object(), new object(), new object() };

            // act
            var actual = list.ElementAtOrDefault( index );

            // assert
            Assert.Null( actual );
        }
    }
}
