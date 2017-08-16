namespace More.Collections.Generic
{
    using FluentAssertions;
    using System;
    using System.Collections;
    using Xunit;

    public class DynamicComparerTTest
    {
        [Fact]
        public void equals_should_match_comparison()
        {
            // arrange
            var comparer = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) );

            // act
            var result = comparer.Equals( "test", "test" );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_match_hash_code()
        {
            // arrange
            var comparer = new DynamicComparer<string>( s => s.GetHashCode() );

            // act
            var result = comparer.Equals( "test", "test" );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_return_true()
        {
            // arrange
            IEqualityComparer comparer = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) );

            // act
            var result = comparer.Equals( "test", "test" );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void get_hash_code_should_use_default_method()
        {
            // arrange
            var value = "test";
            var expected = value.GetHashCode();
            var comparer = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) );

            // act
            var result = comparer.GetHashCode( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void get_hash_code_should_use_custom_method_with_custom_comparison()
        {
            // arrange
            var value = "test";
            var expected = 1;
            var comparer = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ), s => s == "test" ? 1 : s.GetHashCode() );

            // act
            var result = comparer.GetHashCode( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void get_hash_code_should_use_custom_method()
        {
            // arrange
            var value = "test";
            var expected = 1;
            var comparer = new DynamicComparer<string>( s => s == "test" ? 1 : s.GetHashCode() );

            // act
            var result = comparer.GetHashCode( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void get_hash_code_should_return_expected_result()
        {
            // arrage
            var value = "test";
            var expected = 1;
            IEqualityComparer comparer = new DynamicComparer<string>( s => s == "test" ? 1 : s.GetHashCode() );

            // act
            var result = comparer.GetHashCode( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void compare_should_return_0_for_typed_reference_type_items()
        {
            // arrange
            var comparer = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) );

            // act
            var result = comparer.Compare( "test", "test" );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_0_for_typed_value_types_items()
        {
            // arrange
            var comparer = new DynamicComparer<DateTime>( ( d1, d2 ) => d1.CompareTo( d2 ) );
            var date = new DateTime( 2013, 6, 27 );

            // act
            var result = comparer.Compare( date, date );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_0_for_untyped_reference_type_items()
        {
            // arrange
            IComparer comparer = new DynamicComparer<string>( ( s1, s2 ) => s1.CompareTo( s2 ) );

            // act
            var result = comparer.Compare( "test", "test" );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_0_for_typed_value_type_items()
        {
            // arrange
            IComparer comparer = new DynamicComparer<DateTime>( ( d1, d2 ) => d1.CompareTo( d2 ) );
            var date = new DateTime( 2013, 6, 27 );

            // act
            var result = comparer.Compare( date, date );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_X27longerX27_gt_X27shortX27()
        {
            // arrange
            var comparer = new DynamicComparer<string>( ( s1, s2 ) => s1.Length.CompareTo( s2.Length ) );

            // act
            var result = comparer.Compare( "longer", "short" );

            // assert
            result.Should().Be( 1 );
        }

        [Fact]
        public void compare_should_return_X27shortX27_lt_X27longerX27()
        {
            // arrange
            var comparer = new DynamicComparer<string>( ( s1, s2 ) => s1.Length.CompareTo( s2.Length ) );

            // act
            var result = comparer.Compare( "short", "longer" );

            // assert
            result.Should().Be( -1 );
        }
    }
}