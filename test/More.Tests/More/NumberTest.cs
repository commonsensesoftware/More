namespace More
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static NumberStyle;
    using static System.Globalization.CultureInfo;

    public class NumberTest
    {
        [Fact]
        public void number_should_set_expected_properties_for_byte()
        {
            // arrange
            var value = (byte) 1;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Integer } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_short()
        {
            // arrange
            var value = (short) 1;

            // arrange
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Integer } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_int()
        {
            // arrange
            var value = 1;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Integer } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_long()
        {
            // arrange
            var value = 1L;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Integer } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_sbyte()
        {
            // arrange
            var value = (sbyte) 1;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Integer } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_ushort()
        {
            // arrange
            var value = (ushort) 1;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Integer } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_uint()
        {
            // arrange
            var value = 1U;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Integer } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_ulong()
        {
            // arrange
            var value = 1UL;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Integer } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_float()
        {
            // arrange
            var value = 1f;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Default } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_double()
        {
            // arrange
            var value = 1d;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Default } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_decimal()
        {
            // arrange
            var value = 1m;

            // act
            var number = new Number( value );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Default } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_float_with_style()
        {
            // arrange
            var value = 1f;

            // act
            var number = new Number( value, Percent );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Percent } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_double_with_style()
        {
            // arrange
            var value = 1d;

            // act
            var number = new Number( value, Percent );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Percent } );
        }

        [Fact]
        public void number_should_set_expected_properties_for_decimal_with_style()
        {
            // arrange
            var value = 1m;

            // act
            var number = new Number( value, Percent );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Percent } );
        }

        [Theory]
        [InlineData( NumberStyle.Decimal, "1" )]
        [InlineData( Currency, "$1.00" )]
        [InlineData( Default, "1" )]
        [InlineData( Integer, "1" )]
        [InlineData( Percent, "100.00 %" )]
        public void number_to_string_should_return_text_by_style( NumberStyle numberStyle, string expected )
        {
            // arrange
            var number = new Number( 1m, numberStyle );

            // act
            var text = number.ToString();

            // assert
            text.Should().Be( expected );
        }

        [Theory]
        [InlineData( NumberStyle.Decimal, "1" )]
        [InlineData( Currency, "$1.00" )]
        [InlineData( Default, "1" )]
        [InlineData( Integer, "1" )]
        [InlineData( Percent, "100.00 %" )]
        public void number_to_string_should_return_text_by_style_with_format_provider( NumberStyle numberStyle, string expected )
        {
            // arrange
            var number = new Number( 1m, numberStyle );

            // act
            var text = number.ToString( CurrentCulture );

            // assert
            text.Should().Be( expected );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void number_to_string_should_not_allow_null_or_empty_format( string format )
        {
            // arrange

            // act
            Action @new = () => new Number().ToString( format );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( format ) );
        }

        [Theory]
        [InlineData( NumberStyle.Decimal, "N2" )]
        [InlineData( Currency, "C3" )]
        [InlineData( Default, "N2" )]
        [InlineData( Integer, "N1" )]
        [InlineData( Percent, "P3" )]
        public void number_to_string_should_return_text_with_format( NumberStyle numberStyle, string format )
        {
            // arrange
            var value = 1m;
            var number = new Number( value, numberStyle );

            // act
            var text = number.ToString( format );

            // assert
            text.Should().Be( value.ToString( format ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void number_to_string_with_format_provider_should_not_allow_null_or_empty_format( string format )
        {
            // arrange

            // act
            Action @new = () => new Number().ToString( format, CurrentCulture );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( format ) );
        }

        [Theory]
        [InlineData( NumberStyle.Decimal, "N2" )]
        [InlineData( Currency, "C3" )]
        [InlineData( Default, "N2" )]
        [InlineData( Integer, "N1" )]
        [InlineData( Percent, "P3" )]
        public void number_to_string_should_return_expected_text_with_format_and_provider( NumberStyle numberStyle, string format )
        {
            // arrange
            var value = 1m;
            var number = new Number( value, numberStyle );

            // act
            var text = number.ToString( format, CurrentCulture );

            // assert
            text.Should().Be( value.ToString( format, CurrentCulture ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void parse_should_not_allow_null_or_empty( string text )
        {
            // arrange

            // act
            Action parse = () => Number.Parse( text, Default );

            // assert
            parse.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( text ) );
        }

        [Fact]
        public void parse_should_return_expected_number()
        {
            // arrange
            var text = "1";

            // act
            var number = Number.Parse( text, Percent );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Percent } );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void parse_with_provider_should_not_allow_null_or_empty_format( string text )
        {
            // arrange

            // act
            Action parse = () => Number.Parse( text, Default, CurrentCulture );

            // assert
            parse.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( text ) );
        }

        [Fact]
        public void parse_should_return_expected_number_with_format_provider()
        {
            // arrange
            var text = "1";

            // act
            var number = Number.Parse( text, Currency, CurrentCulture );

            // assert
            number.ShouldBeEquivalentTo( new { Value = 1m, NumberStyle = Currency } );
        }

        [Theory]
        [InlineData( null, 0, Default, false )]
        [InlineData( "", 0, Default, false )]
        [InlineData( "abc", 0, Default, false )]
        [InlineData( "1", 1, Integer, true )]
        public void try_parse_should_return_expected_number( string text, decimal value, NumberStyle numberStyle, bool expected )
        {
            // arrange

            // act
            var result = Number.TryParse( text, numberStyle, out var number );

            // assert
            result.Should().Be( expected );
            number.ShouldBeEquivalentTo( new { Value = value, NumberStyle = numberStyle } );
        }

        [Theory]
        [InlineData( null, 0, Default, false )]
        [InlineData( "", 0, Default, false )]
        [InlineData( "abc", 0, Default, false )]
        [InlineData( "1", 1, Integer, true )]
        public void try_parse_should_return_expected_number_with_format_provider( string text, decimal value, NumberStyle numberStyle, bool expected )
        {
            // arrange

            // act
            var result = Number.TryParse( text, numberStyle, CurrentCulture, out var number );

            // assert
            result.Should().Be( expected );
            number.ShouldBeEquivalentTo( new { Value = value, NumberStyle = numberStyle } );
        }

        [Fact]
        public void number_get_hash_code_should_return_expected_value()
        {
            // arrange
            var number = new Number( 5m, Default );
            var expected = number.Value.GetHashCode();

            // act
            var hashCode = number.GetHashCode();

            // assert
            hashCode.Should().Be( expected );
        }

        public static IEnumerable<object[]> UntypedEqualsData
        {
            get
            {
                yield return new object[] { default( object ), false };
                yield return new object[] { new object(), false };
                yield return new object[] { new Number( 10m ), false };
                yield return new object[] { new Number( 1m ), true };
            }
        }

        [Theory]
        [MemberData( nameof( UntypedEqualsData ) )]
        public void number_equals_override_should_return_expected_result( object obj, bool expected )
        {
            // arrange
            var number = new Number( 1m );

            // act
            var result = number.Equals( obj );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 10, false )]
        [InlineData( 1, true )]
        public void number_equals_should_return_expected_result( decimal value, bool expected )
        {
            // arrange
            var number = new Number( 1m );
            var other = new Number( value );

            // act
            var result = number.Equals( other );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_should_implicitly_convert_to_decimal()
        {
            // arrange
            var expected = 5m;
            var number = new Number( expected );

            // act
            decimal value = number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void byte_should_implicitly_convert_to_number()
        {
            // arrange
            var value = (byte) 5;
            var expected = new Number( value );

            // act
            Number number = value;

            // assert
            number.Should().Be( expected );
        }

        [Fact]
        public void short_should_implicitly_convert_to_number()
        {
            // arrange
            var value = (short) 5;
            var expected = new Number( value );

            // act
            Number number = value;

            // assert
            number.Should().Be( expected );
        }

        [Fact]
        public void int_should_implicitly_convert_to_number()
        {
            // arrange
            var expected = new Number( 5 );

            // act
            Number number = 5;

            // assert
            number.Should().Be( expected );
        }

        [Fact]
        public void long_should_implicitly_convert_to_number()
        {
            // arrange
            var expected = new Number( 5L );

            // act
            Number number = 5L;

            // assert
            number.Should().Be( expected );
        }

        [Fact]
        public void sbyte_should_implicitly_convert_to_number()
        {
            // arrange
            var value = (sbyte) 5;
            var expected = new Number( value );

            // act
            Number number = value;

            // assert
            number.Should().Be( expected );
        }

        [Fact]
        public void ushort_should_implicitly_convert_to_number()
        {
            // arrange
            var value = (ushort) 5;
            var expected = new Number( value );

            // act
            Number number = value;

            // assert
            number.Should().Be( expected );
        }

        [Fact]
        public void uint_should_implicitly_convert_to_number()
        {
            // arrange
            var expected = new Number( 5U );

            // act
            Number number = 5U;

            // assert
            number.Should().Be( expected );
        }

        [Fact]
        public void ulong_should_implicitly_convert_to_number()
        {
            // arrange
            var expected = new Number( 5UL );

            // act
            Number number = 5UL;

            // assert
            number.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_float()
        {
            // arrange
            var expected = 5f;
            var number = new Number( expected );

            // act
            var value = (float) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_double()
        {
            // arrange
            var expected = 5d;
            var number = new Number( expected );

            // act
            var value = (double) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_byte()
        {
            // arrange
            var expected = (byte) 5;
            var number = new Number( expected );

            // act
            var value = (byte) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_short()
        {
            // arrange
            var expected = (short) 5;
            var number = new Number( expected );

            // act
            var value = (short) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_int()
        {
            // arrange
            var expected = 5;
            var number = new Number( expected );

            // act
            var value = (int) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_long()
        {
            // arrange
            var expected = 5L;
            var number = new Number( expected );

            // act
            var value = (long) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_sbyte()
        {
            // arrange
            var expected = (sbyte) 5;
            var number = new Number( expected );

            // act
            var value = (sbyte) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_ushort()
        {
            // arrange
            var expected = (ushort) 5;
            var number = new Number( expected );

            // act
            var value = (ushort) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_uint()
        {
            // arrange
            var expected = 5U;
            var number = new Number( expected );

            // act
            var value = (uint) number;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void number_should_explicitly_convert_to_ulong()
        {
            // arrange
            var expected = 5UL;
            var number = new Number( expected );

            // act
            var value = (ulong) number;

            // assert
            value.Should().Be( expected );
        }

        [Theory]
        [InlineData( 1, 1, true )]
        [InlineData( 1, 2, false )]
        public void number_X3DX3D_should_return_expected_result( int value1, int value2, bool expected )
        {
            // arrange
            var number1 = new Number( value1 );
            var number2 = new Number( value2 );

            // act
            var result = number1 == number2;

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 1, 1, false )]
        [InlineData( 1, 2, true )]
        public void number_X21X3D_should_return_expected_result( int value1, int value2, bool expected )
        {
            // arrange
            var number1 = new Number( value1 );
            var number2 = new Number( value2 );

            // act
            var result = number1 != number2;

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 2, 1, true )]
        [InlineData( 1, 2, false )]
        public void number_X3E_should_return_expected_result( int value1, int value2, bool expected )
        {
            // arrange
            var number1 = new Number( value1 );
            var number2 = new Number( value2 );

            // act
            var result = number1 > number2;

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 2, 1, true )]
        [InlineData( 2, 2, true )]
        [InlineData( 1, 2, false )]
        public void number_X3EX3D_should_return_expected_result( int value1, int value2, bool expected )
        {
            // arrange
            var number1 = new Number( value1 );
            var number2 = new Number( value2 );

            // act
            var result = number1 >= number2;

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 1, 2, true )]
        [InlineData( 2, 1, false )]
        public void number_X3C_should_return_expected_result( int value1, int value2, bool expected )
        {
            // arrange
            var number1 = new Number( value1 );
            var number2 = new Number( value2 );

            // act
            var result = number1 < number2;

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 1, 2, true )]
        [InlineData( 2, 2, true )]
        [InlineData( 2, 1, false )]
        public void number_X3CX3D_should_return_expected_result( int value1, int value2, bool expected )
        {
            // arrange
            var number1 = new Number( value1 );
            var number2 = new Number( value2 );

            // act
            var result = number1 <= number2;

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_increment_should_return_expected_result()
        {
            // arrange
            var number = new Number( 1 );
            var expected = new Number( 2 );

            // act
            var result = Number.Increment( number );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_X2BX2B_should_return_expected_result()
        {
            // arrange
            var number = new Number( 1 );
            var expected = new Number( 2 );

            // act
            var result = ++number;

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_decrement_should_return_expected_result()
        {
            // arrange
            var number = new Number( 2 );
            var expected = new Number( 1 );

            // act
            var result = Number.Decrement( number );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_X2DX2D_should_return_expected_result()
        {
            // arrange
            var number = new Number( 2 );
            var expected = new Number( 1 );

            // act
            var result = --number;

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_add_should_return_expected_result()
        {
            // arrange
            var number = new Number( 1 );
            var expected = new Number( 2 );

            // act
            var result = Number.Add( number, new Number( 1 ) );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_X2B_should_return_expected_result()
        {
            // arrange
            var number = new Number( 1 );
            var expected = new Number( 2 );

            // act
            var result = number + new Number( 1 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_subtract_should_return_expected_result()
        {
            // arrange
            var number = new Number( 2 );
            var expected = new Number( 1 );

            // act
            var result = Number.Subtract( number, new Number( 1 ) );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_X2D_should_return_expected_result()
        {
            // arrange
            var number = new Number( 2 );
            var expected = new Number( 1 );

            // act
            var result = number - new Number( 1 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_negate_should_return_expected_result()
        {
            // arrange
            var number = new Number( 1 );
            var expected = new Number( -1 );

            // act
            var result = Number.Negate( number );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_unary_X2D_should_return_expected_result()
        {
            // arrange
            var number = new Number( 1 );
            var expected = new Number( -1 );

            // act
            var result = -number;

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_multiply_should_return_expected_result()
        {
            // arrange
            var number = new Number( 2 );
            var expected = new Number( 4 );

            // act
            var result = Number.Multiply( number, new Number( 2 ) );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_X2A_should_return_expected_result()
        {
            // arrange
            var number = new Number( 2 );
            var expected = new Number( 4 );

            // act
            var result = number * new Number( 2 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_divide_should_return_expected_result()
        {
            // arrange
            var number = new Number( 4 );
            var expected = new Number( 2 );

            // act
            var result = Number.Divide( number, new Number( 2 ) );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_X2F_should_return_expected_result()
        {
            // arrange
            var number = new Number( 4 );
            var expected = new Number( 2 );

            // act
            var result = number / new Number( 2 );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_modulus_should_return_expected_result()
        {
            // arrange
            var number = new Number( 3 );
            var expected = new Number( 1 );

            // act
            var result = Number.Mod( number, new Number( 2 ) );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void number_X25_should_return_expected_result()
        {
            // arrange
            var number = new Number( 3 );
            var expected = new Number( 1 );

            // act
            var result = number % new Number( 2 );

            // assert
            result.Should().Be( expected );
        }

        [Theory]
        [InlineData( 1, 1, 0 )]
        [InlineData( 2, 1, 1 )]
        [InlineData( 1, 2, -1 )]
        public void generic_number_compare_should_return_expected_result( int value1, int value2, int expected )
        {
            // arrange
            var number1 = new Number( value1 );
            var number2 = new Number( value2 );

            // act
            var result = number1.CompareTo( number2 );

            // assert
            result.Should().Be( expected );
        }

        public static IEnumerable<object[]> UntypedCompareToData
        {
            get
            {
                yield return new object[] { 1, new Number( 1 ), 0 };
                yield return new object[] { 2, new Number( 1 ), 1 };
                yield return new object[] { 1, new Number( 2 ), -1 };
                yield return new object[] { 1, default( object ), 1 };
            }
        }

        [Theory]
        [MemberData( nameof( UntypedCompareToData ) )]
        public void compare_should_return_expected_result( int value, object obj, int expected )
        {
            // arrange
            var number = new Number( value );

            // act
            var result = number.CompareTo( obj );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void compare_should_throw_exception_for_invalid_nonX2Dnull_object()
        {
            // arrange
            var number = new Number( 1 );

            // act
            Action compareTo = () => number.CompareTo( new object() );

            // assert
            compareTo.ShouldThrow<ArgumentException>().And.Message.Should().Be( "Object must be of type Number." );
        }

        [Fact]
        public void sequence_of_numbers_should_be_sortable()
        {
            // arrange
            var list = new List<Number>()
            {
                new Number( 20m, Currency ),
                new Number( 5m, Currency ),
                new Number( 10m, Currency )
            };

            // act
            list.Sort();

            // assert
            list.Select( i => (decimal) i ).Should().Equal( new[] { 5m, 10m, 20m } );
        }

        [Fact]
        public void sequence_of_numbers_should_be_sortable_by_comparison()
        {
            // arrange
            var list = new List<Tuple<Number>>()
            {
                Tuple.Create( new Number( 20m, Currency ) ),
                Tuple.Create( new Number( 5m, Currency ) ),
                Tuple.Create( new Number( 10m, Currency ) )
            };

            // act
            list.Sort( ( t1, t2 ) => t1.Item1.CompareTo( t2.Item1 ) );

            // assert
            list.Select( t => (decimal) t.Item1 ).Should().Equal( new[] { 5m, 10m, 20m } );
        }
    }
}