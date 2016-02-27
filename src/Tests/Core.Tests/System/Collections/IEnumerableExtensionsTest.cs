using Xunit;
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

        [Fact( DisplayName = "first should return first element" )]
        public void FirstShouldReturnFirstElement()
        {
            // arrange
            var expected = new object();
            IEnumerable sequence = new[] { expected, new object(), new object() };

            // act
            var actual = sequence.First();

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "first should throw exception when sequence is empty" )]
        public void FirstShouldThrowExceptionWhenSequenceIsEmpty()
        {
            // arrange
            IEnumerable sequence = new object[0];
            var expected = "Sequence contains no elements.";

            // act
            var ex = Assert.Throws<InvalidOperationException>( () => sequence.First() );
            var actual = ex.Message;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "first or default should return expected result" )]
        [InlineData( new Type[0], null )]
        [InlineData( new[] { typeof( object ), typeof( string ), typeof( int ) }, typeof( object ) )]
        public void FirstOrDefaultShouldReturnExpectedResult( IEnumerable sequence, object expected )
        {
            // arrange


            // act
            var actual = sequence.FirstOrDefault();

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "last should return last element" )]
        public void LastShouldReturnLastElement()
        {
            // arrange
            var expected = new object();
            IEnumerable sequence = new[] { new object(), new object(), expected };

            // act
            var actual = sequence.Last();

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "last should throw exception when sequence is empty" )]
        public void LastShouldThrowExceptionWhenSequenceIsEmpty()
        {
            // arrange
            IEnumerable sequence = new object[0];
            var expected = "Sequence contains no elements.";

            // act
            var ex = Assert.Throws<InvalidOperationException>( () => sequence.Last() );
            var actual = ex.Message;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "last or default should return expected result" )]
        [InlineData( new Type[0], null )]
        [InlineData( new[] { typeof( string ), typeof( int ), typeof( object ) }, typeof( object ) )]
        public void LastOrDefaultShouldReturnExpectedResult( IEnumerable sequence, object expected )
        {
            // arrange


            // act
            var actual = sequence.LastOrDefault();

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "single should return exactly one element" )]
        public void SingleShouldReturnExactlyOneElement()
        {
            // arrange
            var expected = new object();
            IEnumerable sequence = new[] { expected };

            // act
            var actual = sequence.Single();

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "single should throw exception when sequence is not exactly one element" )]
        [InlineData( new object[0], "Sequence contains no elements." )]
        [InlineData( new[] { 1, 2, 3 }, "Sequence contains more than one matching element." )]
        public void SingleShouldThrowExceptionWhenSequenceIsNotExactlyOneElement( IEnumerable sequence, string expected )
        {
            // arrange

            // act
            var ex = Assert.Throws<InvalidOperationException>( () => sequence.Single() );
            var actual = ex.Message;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "single or default should return expected result" )]
        [InlineData( new Type[0], null )]
        [InlineData( new[] { typeof( object ) }, typeof( object ) )]
        public void SingleOrDefaultShouldReturnExpectedResult( IEnumerable sequence, object expected )
        {
            // arrange


            // act
            var actual = sequence.SingleOrDefault();

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "single or default should throw exception when sequence is more than one element" )]
        public void SingleOrDefaultShouldThrowExceptionWhenSequenceIsMoreThanOneElement()
        {
            // arrange
            var sequence = new[] { 1, 2, 3 };
            var expected = "Sequence contains more than one matching element.";

            // act
            var ex = Assert.Throws<InvalidOperationException>( () => sequence.SingleOrDefault() );
            var actual = ex.Message;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "to array should return expected result" )]
        public void ToArrayShouldReturnExpectedResult()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            IEnumerable sequence = (IEnumerable) expected.Clone();

            // act
            var actual = sequence.ToArray();

            // assert
            Assert.Equal( expected.Length, actual.Length );
            Assert.Equal( expected[0], actual[0] );
            Assert.Equal( expected[1], actual[1] );
            Assert.Equal( expected[2], actual[2] );
        }

        [Fact( DisplayName = "to list should return expected result" )]
        public void ToListShouldReturnExpectedResult()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            IEnumerable sequence = (IEnumerable) expected.Clone();

            // act
            var actual = sequence.ToList();

            // assert
            Assert.Equal( expected.Length, actual.Count );
            Assert.Equal( expected[0], actual[0] );
            Assert.Equal( expected[1], actual[1] );
            Assert.Equal( expected[2], actual[2] );
        }

        [Fact( DisplayName = "reverse should return expected result" )]
        public void ReverseShouldReturnExpectedResult()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            IEnumerable sequence = new[] { expected[2], expected[1], expected[0] };
            var actual = new ArrayList();

            // act
            foreach ( object item in sequence.Reverse() )
                actual.Add( item );

            // assert
            Assert.Equal( expected.Length, actual.Count );
            Assert.Equal( expected[0], actual[0] );
            Assert.Equal( expected[1], actual[1] );
            Assert.Equal( expected[2], actual[2] );
        }

        [Fact( DisplayName = "skip should bypass expected elements" )]
        public void SkipShouldBypassExpectedElements()
        {
            // arrange
            var expected = new[] { new object(), new object() };
            IEnumerable sequence = new[] { new object(), new object(), new object(), expected[0], expected[1] };
            var actual = new ArrayList();

            // act
            foreach ( object item in sequence.Skip( 3 ) )
                actual.Add( item );

            // assert
            Assert.Equal( expected.Length, actual.Count );
            Assert.Equal( expected[0], actual[0] );
            Assert.Equal( expected[1], actual[1] );
        }

        [Fact( DisplayName = "take should return expected elements" )]
        public void TakeShouldReturnExpectedElements()
        {
            // arrange
            var expected = new[] { new object(), new object() };
            IEnumerable sequence = new[] { expected[0], expected[1], new object(), new object(), new object() };
            var actual = new ArrayList();

            // act
            foreach ( object item in sequence.Take( 2 ) )
                actual.Add( item );

            // assert
            Assert.Equal( expected.Length, actual.Count );
            Assert.Equal( expected[0], actual[0] );
            Assert.Equal( expected[1], actual[1] );
        }

        [Theory( DisplayName = "sequence equal should return expected result" )]
        [InlineData( new object[0], new object[0], true )]
        [InlineData( new object[] { 1 }, new object[0], false )]
        [InlineData( new object[0], new object[] { 1 }, false )]
        [InlineData( new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, true )]
        public void SequenceEqualShouldReturnExpectedResult( IEnumerable first, IEnumerable second, bool expected )
        {
            // arrange


            // act
            var actual = first.SequenceEqual( second );

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
