namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ComparerAdapter{TItem,TValue}"/>.
    /// </summary>
    public class ComparerAdapterT1T2Test
    {
        [Fact( DisplayName = "equals should return true for equal items" )]
        public void EqualsShouldReturnTrueWhenItemsAreEqual()
        {
            var t1 = new ComparerAdapter<string, int>( s => s.Length );     // ref type
            Assert.True( t1.Equals( "TEST", "test" ) );                     // compared by length

            var t2 = new ComparerAdapter<DateTime, int>( d => d.Day );      // value type
            var actual = t2.Equals( new DateTime( 2012, 1, 1 ), new DateTime( 2013, 1, 1 ) );
            Assert.True( actual );                                          // compared by day
        }

        [Fact( DisplayName = "equals should return true for equal items" )]
        public void IEqualityComparerEqualsShouldReturnTrueWhenItemsAreEqual()
        {
            IEqualityComparer t1 = new ComparerAdapter<string, int>( s => s.Length );   // ref type
            Assert.True( t1.Equals( "TEST", "test" ) );                                 // compared by length

            IEqualityComparer t2 = new ComparerAdapter<DateTime, int>( d => d.Day );    // value type
            var actual = t2.Equals( new DateTime( 2012, 1, 1 ), new DateTime( 2013, 1, 1 ) );
            Assert.True( actual );                                                      // compared by day
        }

        [Fact( DisplayName = "equals should return true for null items" )]
        public void EqualsShouldReturnTrueWhenItemsAreNull()
        {
            // arrange
            var target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.Equals( null, null );

            // assert
            Assert.True( actual );
        }

        [Fact( DisplayName = "equals should return true for null items" )]
        public void IEqualityComparerEqualsShouldReturnTrueWhenItemsAreNull()
        {
            // arrange
            var target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.Equals( null, null );

            // assert
            Assert.True( actual );
        }

        [Fact( DisplayName = "get hash code should return 0 for null" )]
        public void GetHashCodeShouldReturnZeroForNull()
        {
            // arrange
            var target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.GetHashCode( null );

            Assert.Equal( 0, actual );
        }

        [Fact( DisplayName = "get hash code should return expected value" )]
        public void GetHashCodeShouldReturnExpectedValue()
        {
            // arrange
            var value = "test";
            var expected = EqualityComparer<int>.Default.GetHashCode( value.Length );
            var target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.GetHashCode( value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get hash code should return expected value" )]
        public void IEqualityComparerGetHashCodeShouldReturnExpectedValue()
        {
            // arrange
            var value = "test";
            var expected = EqualityComparer<int>.Default.GetHashCode( value.Length );
            IEqualityComparer target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.GetHashCode( value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "compare should return zero for null items" )]
        public void CompareShouldReturnZeroWhenItemsAreNull()
        {
            // arrange
            var target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.Compare( null, null );

            // assert
            Assert.Equal( 0, actual );
        }

        [Fact( DisplayName = "compare should return zero for null items" )]
        public void IComparerCompareShouldReturnZeroWhenItemsAreNull()
        {
            // arrange
            IComparer target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.Compare( null, null );

            // assert
            Assert.Equal( 0, actual );
        }

        [Fact( DisplayName = "compare should return 0 when items are equal" )]
        public void CompareShouldReturnZeroWhenItemsAreEqual()
        {
            var t1 = new ComparerAdapter<string, int>( s => s.Length );     // ref type
            Assert.Equal( 0, t1.Compare( "TEST", "test" ) );                // compared by length

            var t2 = new ComparerAdapter<DateTime, int>( d => d.Day );      // value type
            var actual = t2.Compare( new DateTime( 2012, 1, 1 ), new DateTime( 2013, 1, 1 ) );
            Assert.Equal( 0, actual );                                      // compared by day
        }

        [Fact( DisplayName = "compare should return 0 when items are equal" )]
        public void IComparerCompareShouldReturnZeroWhenItemsAreEqual()
        {
            IComparer t1 = new ComparerAdapter<string, int>( s => s.Length );   // ref type
            Assert.Equal( 0, t1.Compare( "TEST", "test" ) );                    // compared by length

            IComparer t2 = new ComparerAdapter<DateTime, int>( d => d.Day );    // value type
            var actual = t2.Compare( new DateTime( 2012, 1, 1 ), new DateTime( 2013, 1, 1 ) );
            Assert.Equal( 0, actual );                                          // compared by day
        }

        [Fact( DisplayName = "compare should return 1 for 'longer' > 'short'" )]
        public void CompareShouldReturnOneWhenLeftSideIsGreater()
        {
            // arrange
            var target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.Compare( "longer", "short" );

            // assert
            Assert.Equal( 1, actual );
        }

        [Fact( DisplayName = "compare should return 1 for 'longer' > 'short'" )]
        public void IComparerCompareShouldReturnOneWhenLeftSideIsGreater()
        {
            // arrange
            IComparer target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.Compare( "longer", "short" );

            // assert
            Assert.Equal( 1, actual );
        }

        [Fact( DisplayName = "compare should return -1 when 'short' < 'longer'" )]
        public void IComparerCompareShouldReturnNegativeOneWhenRightSideIsGreater()
        {
            // arrange
            IComparer target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var actual = target.Compare( "short", "longer" );

            // assert
            Assert.Equal( -1, actual );
        }
    }
}
