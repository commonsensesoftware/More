namespace More.Collections.Generic
{
    using FluentAssertions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;

    public class ComparerAdapterT1T2Test
    {
        [Fact]
        public void equals_should_return_true_for_equal_reference_type_items()
        {
            // arrange
            var comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.Equals( "TEST", "test" );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_return_true_for_equal_value_type_items()
        {
            // arrange
            var comparer = new ComparerAdapter<DateTime, int>( d => d.Day );

            // act
            var result = comparer.Equals( new DateTime( 2012, 1, 1 ), new DateTime( 2013, 1, 1 ) );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_return_true_for_equal_untyped_reference_type_items()
        {
            // arrange
            IEqualityComparer comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.Equals( "TEST", "test" );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_return_true_for_equal_untyped_value_type_items()
        {
            // arrange
            IEqualityComparer comparer = new ComparerAdapter<DateTime, int>( d => d.Day );

            // act
            var result = comparer.Equals( new DateTime( 2012, 1, 1 ), new DateTime( 2013, 1, 1 ) );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_return_true_for_typed_null_items()
        {
            // arrange
            var target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = target.Equals( null, null );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_return_true_for_untyped_null_items()
        {
            // arrange
            IEqualityComparer target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = target.Equals( null, null );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void get_hash_code_should_return_0_for_null()
        {
            // arrange
            var comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.GetHashCode( null );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void get_hash_code_should_return_expected_value()
        {
            // arrange
            var value = "test";
            var expected = EqualityComparer<int>.Default.GetHashCode( value.Length );
            var comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.GetHashCode( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void get_hash_code_for_typed_object_should_return_expected_value()
        {
            // arrange
            var value = "test";
            var expected = EqualityComparer<int>.Default.GetHashCode( value.Length );
            IEqualityComparer comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.GetHashCode( value );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void compare_should_return_zero_for_null_items()
        {
            // arrange
            var comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.Compare( null, null );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_zero_for_untyped_null_items()
        {
            // arrange
            IComparer comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.Compare( null, null );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_0_when_typed_reference_type_items_are_equal()
        {
            // arrange
            var comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.Compare( "TEST", "test" );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_0_when_typed_value_type_items_are_equal()
        {
            // arrange
            var comparer = new ComparerAdapter<DateTime, int>( d => d.Day );

            // act
            var result = comparer.Compare( new DateTime( 2012, 1, 1 ), new DateTime( 2013, 1, 1 ) );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_0_when_untyped_reference_type_items_are_equal()
        {
            // arrange
            IComparer comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.Compare( "TEST", "test" );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_0_when_untyped_value_type_items_are_equal()
        {
            // arrange
            IComparer comparer = new ComparerAdapter<DateTime, int>( d => d.Day );

            // act
            var result = comparer.Compare( new DateTime( 2012, 1, 1 ), new DateTime( 2013, 1, 1 ) );

            // assert
            result.Should().Be( 0 );
        }

        [Fact]
        public void compare_should_return_1_for_X27longerX27_gt_X27shortX27()
        {
            // arrange
            var comparer = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = comparer.Compare( "longer", "short" );

            // assert
            result.Should().Be( 1 );
        }

        [Fact]
        public void compare_should_return_1_when_untyped_for_X27longerX27_gt_X27shortX27()
        {
            // arrange
            IComparer target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = target.Compare( "longer", "short" );

            // assert
            result.Should().Be( 1 );
        }

        [Fact]
        public void compare_should_return_X2D1_when_X27shortX27_lt_X27longerX27()
        {
            // arrange
            IComparer target = new ComparerAdapter<string, int>( s => s.Length );

            // act
            var result = target.Compare( "short", "longer" );

            // assert
            result.Should().Be( -1 );
        }
    }
}