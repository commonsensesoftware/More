namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="DynamicComparer{T}"/>.
    /// </summary>
    public class DynamicComparerTTest
    {
        [Fact( DisplayName = "equals should match comparison" )]
        public void EqualsShouldMatchComparison()
        {
            // arrange
            var target = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) );

            // act
            var actual = target.Equals( "test", "test" );

            // assert
            Assert.True( actual );
        }

        [Fact( DisplayName = "equals should match hash code" )]
        public void EqualsShouldMatchHashCode()
        {
            // arrange
            var target = new DynamicComparer<string>( s => s.GetHashCode() );

            // act
            var actual = target.Equals( "test", "test" );

            // assert
            Assert.True( actual );
        }

        [Fact( DisplayName = "equals should return true" )]
        public void IEqualityComparerEqualsShouldReturnTrueWhenEqual()
        {
            // arrange
            IEqualityComparer target = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) );

            // act
            var actual = target.Equals( "test", "test" );

            // assert
            Assert.True( actual );
        }

        [Fact( DisplayName = "get hash code should use default method" )]
        public void GetHashCodeShouldUseDefaultMethodWithComparison()
        {
            // arrange
            var value = "test";
            var expected = value.GetHashCode();
            var target = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) );

            // act
            var actual = target.GetHashCode( value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get hash code should use custom method" )]
        public void GetHashCodeShouldUseCustomMethodWithComparison()
        {
            // arrange
            var value = "test";
            var expected = 1;
            var target = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ), s => s == "test" ? 1 : s.GetHashCode() );

            // act
            var actual = target.GetHashCode( value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get hash code should use custom method" )]
        public void GetHashCodeShouldUseCustomMethod()
        {
            // arrange
            var value = "test";
            var expected = 1;
            var target = new DynamicComparer<string>( s => s == "test" ? 1 : s.GetHashCode() );

            // act
            var actual = target.GetHashCode( value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get hash code should return expected result" )]
        public void IEqualityComparerGetHashCodeShouldReturnExpectedResult()
        {
            // arrage
            var value = "test";
            var expected = 1;
            IEqualityComparer target = new DynamicComparer<string>( s => s == "test" ? 1 : s.GetHashCode() );

            // act
            var actual = target.GetHashCode( value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "compare should return 0" )]
        public void CompareReturnZeroWhenUsingComparisonMethod()
        {
            var t1 = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) ); // ref type
            Assert.Equal( 0, t1.Compare( "test", "test" ) );

            var t2 = new DynamicComparer<DateTime>( ( d1, d2 ) => d1.CompareTo( d2 ) ); // value type
            var date = new DateTime( 2013, 6, 27 );
            Assert.Equal( 0, t2.Compare( date, date ) );
        }

        [Fact( DisplayName = "compare should return 0" )]
        public void IComparerCompareReturnZeroWhenUsingComparisonMethod()
        {
            IComparer t1 = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) ); // ref type
            Assert.Equal( 0, t1.Compare( "test", "test" ) );

            IComparer t2 = new DynamicComparer<DateTime>( ( d1, d2 ) => d1.CompareTo( d2 ) ); // value type
            var date = new DateTime( 2013, 6, 27 );
            Assert.Equal( 0, t2.Compare( date, date ) );
        }

        [Fact( DisplayName = "compare should return 'longer' > 'short'" )]
        public void CompareReturnOneWhenUsingComparisonMethodAndLeftSideIsGreater()
        {
            var target = new DynamicComparer<string>( ( s1, s2 ) => s1.Length.CompareTo( s2.Length ) );
            Assert.Equal( 1, target.Compare( "longer", "short" ) );
        }

        [Fact( DisplayName = "compare should return 'short' < 'longer'" )]
        public void CompareReturnNegativeOneWhenUsingComparisonMethodAndRightSideIsGreater()
        {
            var target = new DynamicComparer<string>( ( s1, s2 ) => s1.Length.CompareTo( s2.Length ) );
            Assert.Equal( -1, target.Compare( "short", "longer" ) );
        }
    }
}
