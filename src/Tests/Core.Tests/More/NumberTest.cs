namespace More
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="Number"/>.
    /// </summary>
    public class NumberTest
    {
        [Fact( DisplayName = "number should set expected properties for byte" )]
        public void ConstructorForByteShouldSetExpectedProperties()
        {
            var target = new Number( (byte) 1 );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Integer, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for short" )]
        public void ConstructorForShortShouldSetExpectedProperties()
        {
            var target = new Number( (short) 1 );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Integer, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for int" )]
        public void ConstructorForIntShouldSetExpectedProperties()
        {
            var target = new Number( 1 );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Integer, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for long" )]
        public void ConstructorForLongShouldSetExpectedProperties()
        {
            var target = new Number( 1L );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Integer, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for sbyte" )]
        public void ConstructorForSByteShouldSetExpectedProperties()
        {
            var target = new Number( (sbyte) 1 );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Integer, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for ushort" )]
        public void ConstructorForUShortShouldSetExpectedProperties()
        {
            var target = new Number( (ushort) 1 );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Integer, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for uint" )]
        public void ConstructorForUIntShouldSetExpectedProperties()
        {
            var target = new Number( 1U );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Integer, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for ulong" )]
        public void ConstructorForULongShouldSetExpectedProperties()
        {
            var target = new Number( 1UL );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Integer, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for float" )]
        public void ConstructorForFloatShouldSetExpectedProperties()
        {
            var target = new Number( 1f );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Default, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for double" )]
        public void ConstructorForDoubleShouldSetExpectedProperties()
        {
            var target = new Number( 1d );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Default, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for decimal" )]
        public void ConstructorForDecimalShouldSetExpectedProperties()
        {
            var target = new Number( 1m );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Default, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for float with style" )]
        public void ConstructorForFloatWithStyleShouldSetExpectedProperties()
        {
            var target = new Number( 1f, NumberStyle.Percent );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Percent, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for double with style" )]
        public void ConstructorForDoubleWithStyleShouldSetExpectedProperties()
        {
            var target = new Number( 1d, NumberStyle.Percent );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Percent, target.NumberStyle );
        }

        [Fact( DisplayName = "number should set expected properties for decimal with style" )]
        public void ConstructorForDecimalWithStyleShouldSetExpectedProperties()
        {
            var target = new Number( 1m, NumberStyle.Percent );
            Assert.Equal( 1m, target.Value );
            Assert.Equal( NumberStyle.Percent, target.NumberStyle );
        }

        [Fact( DisplayName = "number to string should return text by style" )]
        public void ToStringShouldReturnValueBasedOnNumberStyle()
        {
            var expected = new List<string>()
            {
                ( 1m ).ToString( "C" ),
                ( 1m ).ToString(),
                ( 1m ).ToString(),
                ( 1m ).ToString( "N0" ),
                ( 1m ).ToString( "P" )
            };

            var actual = new List<string>()
            {
                new Number( 1m, NumberStyle.Currency ).ToString(),
                new Number( 1m, NumberStyle.Decimal ).ToString(),
                new Number( 1m, NumberStyle.Default ).ToString(),
                new Number( 1m, NumberStyle.Integer ).ToString(),
                new Number( 1m, NumberStyle.Percent ).ToString()
            };

            Assert.True( actual.SequenceEqual( actual ) );
        }

        [Fact( DisplayName = "number to string should return text by style with format provider" )]
        public void ToStringShouldReturnValueBasedOnNumberStyleWithFormatProviderOnly()
        {
            var provider = System.Globalization.CultureInfo.CurrentCulture;
            var expected = new List<string>()
            {
                ( 1m ).ToString( "C", provider ),
                ( 1m ).ToString( provider ),
                ( 1m ).ToString( provider ),
                ( 1m ).ToString( "N0", provider ),
                ( 1m ).ToString( "P", provider )
            };

            var actual = new List<string>()
            {
                new Number( 1m, NumberStyle.Currency ).ToString( provider ),
                new Number( 1m, NumberStyle.Decimal ).ToString( provider ),
                new Number( 1m, NumberStyle.Default ).ToString( provider ),
                new Number( 1m, NumberStyle.Integer ).ToString( provider ),
                new Number( 1m, NumberStyle.Percent ).ToString( provider )
            };

            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Fact( DisplayName = "number to string should not allow null or empty format" )]
        public void ToStringShouldNotAllowNullOrEmptyFormat()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => new Number().ToString( (string) null ) );
            Assert.Equal( "format", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new Number().ToString( "" ) );
            Assert.Equal( "format", ex.ParamName );
        }

        [Fact( DisplayName = "number to string should return text with format" )]
        public void ToStringShouldReturnValueBasedOnFormat()
        {
            var expected = new List<string>()
            {
                ( 1m ).ToString( "C3" ),
                ( 1m ).ToString( "N2" ),
                ( 1m ).ToString( "N2" ),
                ( 1m ).ToString( "N1" ),
                ( 1m ).ToString( "P3" )
            };

            var actual = new List<string>()
            {
                new Number( 1m, NumberStyle.Currency ).ToString( "C3" ),
                new Number( 1m, NumberStyle.Decimal ).ToString( "N2" ),
                new Number( 1m, NumberStyle.Default ).ToString( "N2" ),
                new Number( 1m, NumberStyle.Integer ).ToString( "N1" ),
                new Number( 1m, NumberStyle.Percent ).ToString( "P3" )
            };

            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Fact( DisplayName = "number to string with format provider should not allow null or empty format" )]
        public void ToStringWithFormatProviderShouldNotAllowNullOrEmptyFormat()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => new Number().ToString( (string) null, null ) );
            Assert.Equal( "format", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new Number().ToString( "", null ) );
            Assert.Equal( "format", ex.ParamName );
        }

        [Fact( DisplayName = "number to string should return expected text with format and provider" )]
        public void ToStringShouldReturnValueBasedOnFormatAndProvider()
        {
            var provider = System.Globalization.CultureInfo.CurrentCulture;
            var expected = new List<string>()
            {
                ( 1m ).ToString( "C3", provider ),
                ( 1m ).ToString( "N2", provider ),
                ( 1m ).ToString( "N2", provider ),
                ( 1m ).ToString( "N1", provider ),
                ( 1m ).ToString( "P3", provider )
            };

            var actual = new List<string>()
            {
                new Number( 1m, NumberStyle.Currency ).ToString( "C3", provider ),
                new Number( 1m, NumberStyle.Decimal ).ToString( "N2", provider ),
                new Number( 1m, NumberStyle.Default ).ToString( "N2", provider ),
                new Number( 1m, NumberStyle.Integer ).ToString( "N1", provider ),
                new Number( 1m, NumberStyle.Percent ).ToString( "P3", provider )
            };

            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Fact( DisplayName = "parse should not allow null or empty" )]
        public void ParseShouldNotAllowNullOrEmptyText()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => Number.Parse( null, NumberStyle.Default ) );
            Assert.Equal( "text", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => Number.Parse( "", NumberStyle.Default ) );
            Assert.Equal( "text", ex.ParamName );
        }

        [Fact( DisplayName = "parse should return expected number" )]
        public void ParseShouldReturnCorrectValue()
        {
            var actual = Number.Parse( "1", NumberStyle.Percent );

            Assert.Equal( 1m, actual.Value );
            Assert.Equal( NumberStyle.Percent, actual.NumberStyle );
        }

        [Fact( DisplayName = "parse with provider should not allow null or empty format" )]
        public void ParseWithFormatProviderShouldNotAllowNullOrEmptyText()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => Number.Parse( (string) null, NumberStyle.Default, null ) );
            Assert.Equal( "text", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => Number.Parse( "", NumberStyle.Default, null ) );
            Assert.Equal( "text", ex.ParamName );
        }

        [Fact( DisplayName = "parse should return expected number with format provider" )]
        public void ParseShouldReturnCorrectValueWithFormatProvider()
        {
            var provider = System.Globalization.CultureInfo.CurrentCulture;
            var actual = Number.Parse( "1", NumberStyle.Currency, provider );

            Assert.Equal( 1m, actual.Value );
            Assert.Equal( NumberStyle.Currency, actual.NumberStyle );
        }

        [Fact( DisplayName = "try parse should return expected number" )]
        public void TryParseShouldReturnCorrectValue()
        {
            Number actual;
            Assert.False( Number.TryParse( null, NumberStyle.Default, out actual ) );
            Assert.Equal( 0m, actual.Value );
            Assert.Equal( NumberStyle.Default, actual.NumberStyle );
            Assert.False( Number.TryParse( string.Empty, NumberStyle.Default, out actual ) );
            Assert.Equal( 0m, actual.Value );
            Assert.Equal( NumberStyle.Default, actual.NumberStyle );
            Assert.False( Number.TryParse( "abc", NumberStyle.Default, out actual ) );
            Assert.Equal( 0m, actual.Value );
            Assert.Equal( NumberStyle.Default, actual.NumberStyle );
            Assert.True( Number.TryParse( "1", NumberStyle.Integer, out actual ) );
            Assert.Equal( 1m, actual.Value );
            Assert.Equal( NumberStyle.Integer, actual.NumberStyle );
        }

        [Fact( DisplayName = "try parse should return expected number with format provider" )]
        public void TryParseShouldReturnCorrectValueWithFormatProvider()
        {
            var provider = System.Globalization.CultureInfo.CurrentCulture;
            Number actual;
            Assert.False( Number.TryParse( null, NumberStyle.Default, provider, out actual ) );
            Assert.Equal( 0m, actual.Value );
            Assert.Equal( NumberStyle.Default, actual.NumberStyle );
            Assert.False( Number.TryParse( string.Empty, NumberStyle.Default, provider, out actual ) );
            Assert.Equal( 0m, actual.Value );
            Assert.Equal( NumberStyle.Default, actual.NumberStyle );
            Assert.False( Number.TryParse( "abc", NumberStyle.Default, provider, out actual ) );
            Assert.Equal( 0m, actual.Value );
            Assert.Equal( NumberStyle.Default, actual.NumberStyle );
            Assert.True( Number.TryParse( "1.25", NumberStyle.Decimal, provider, out actual ) );
            Assert.Equal( 1.25m, actual.Value );
            Assert.Equal( NumberStyle.Decimal, actual.NumberStyle );
        }

        [Fact( DisplayName = "number get hash code should return expected value" )]
        public void GetHashCodeShouldEqualUnderlyingValueHashCode()
        {
            var target = new Number( 5m, NumberStyle.Default );
            var expected = target.Value.GetHashCode();
            var actual = target.GetHashCode();
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number equals override should return expected result" )]
        public void ObjectEqualsShouldReturnCorrectValue()
        {
            var target = new Number( 1m );
            Assert.False( target.Equals( null ) );
            Assert.False( target.Equals( new object() ) );
            Assert.False( target.Equals( (object) new Number( 10m ) ) );
            Assert.True( target.Equals( (object) new Number( 1m ) ) );
        }

        [Fact( DisplayName = "number equals should return expected result" )]
        public void EqualsShouldReturnCorrectValue()
        {
            var target = new Number( 1m );
            Assert.False( target.Equals( new Number( 10m ) ) );
            Assert.True( target.Equals( new Number( 1m ) ) );
        }

        [Fact( DisplayName = "number should implicitly convert to decimal" )]
        public void NumberShouldImplicitlyConvertToDecimal()
        {
            var expected = 5m;
            var target = new Number( expected );
            decimal actual = target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "byte should implicitly convert to number" )]
        public void ByteShouldImplicitlyConvertToNumber()
        {
            var expected = new Number( (byte) 5 );
            Number actual = (byte) 5;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "short should implicitly convert to number" )]
        public void ShortShouldImplicitlyConvertToNumber()
        {
            var expected = new Number( (short) 5 );
            Number actual = (short) 5;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "int should implicitly convert to number" )]
        public void IntShouldImplicitlyConvertToNumber()
        {
            var expected = new Number( 5 );
            Number actual = 5;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "long should implicitly convert to number" )]
        public void LongShouldImplicitlyConvertToNumber()
        {
            var expected = new Number( 5L );
            Number actual = 5L;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "sbyte should implicitly convert to number" )]
        public void SByteShouldImplicitlyConvertToNumber()
        {
            var expected = new Number( (sbyte) 5 );
            Number actual = (sbyte) 5;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "ushort should implicitly convert to number" )]
        public void UShortShouldImplicitlyConvertToNumber()
        {
            var expected = new Number( (ushort) 5 );
            Number actual = (ushort) 5;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "uint should implicitly convert to number" )]
        public void UIntShouldImplicitlyConvertToNumber()
        {
            var expected = new Number( 5U );
            Number actual = 5U;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "ulong should implicitly convert to number" )]
        public void ULongShouldImplicitlyConvertToNumber()
        {
            var expected = new Number( 5UL );
            Number actual = 5UL;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to float" )]
        public void NumberShouldExplicitlyConvertToFloat()
        {
            var expected = 5f;
            var target = new Number( expected );
            var actual = (float) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to double" )]
        public void NumberShouldExplicitlyConvertToDouble()
        {
            var expected = 5d;
            var target = new Number( expected );
            var actual = (double) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to byte" )]
        public void NumberShouldExplicitlyConvertToByte()
        {
            var expected = (byte) 5;
            var target = new Number( expected );
            var actual = (byte) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to short" )]
        public void NumberShouldExplicitlyConvertToShort()
        {
            var expected = (short) 5;
            var target = new Number( expected );
            var actual = (short) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to int" )]
        public void NumberShouldExplicitlyConvertToInt()
        {
            var expected = 5;
            var target = new Number( expected );
            var actual = (int) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to long" )]
        public void NumberShouldExplicitlyConvertToLong()
        {
            var expected = 5L;
            var target = new Number( expected );
            var actual = (long) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to sbyte" )]
        public void NumberShouldExplicitlyConvertToSByte()
        {
            var expected = (sbyte) 5;
            var target = new Number( expected );
            var actual = (sbyte) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to ushort" )]
        public void NumberShouldExplicitlyConvertToUShort()
        {
            var expected = (ushort) 5;
            var target = new Number( expected );
            var actual = (ushort) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to uint" )]
        public void NumberShouldExplicitlyConvertToUInt()
        {
            var expected = 5U;
            var target = new Number( expected );
            var actual = (uint) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number should explicitly convert to ulong" )]
        public void NumberShouldExplicitlyConvertToULong()
        {
            var expected = 5UL;
            var target = new Number( expected );
            var actual = (ulong) target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number == should return expected result" )]
        public void EqualityOperatorShouldReturnCorrectValue()
        {
            Assert.True( new Number( 1 ) == new Number( 1 ) );
            Assert.False( new Number( 1 ) == new Number( 2 ) );
        }

        [Fact( DisplayName = "number != should return expected result" )]
        public void InequalityOperatorShouldReturnCorrectValue()
        {
            Assert.False( new Number( 1 ) != new Number( 1 ) );
            Assert.True( new Number( 1 ) != new Number( 2 ) );
        }

        [Fact( DisplayName = "number > should return expected result" )]
        public void GreaterThanOperatorShouldReturnCorrectValue()
        {
            Assert.True( new Number( 2 ) > new Number( 1 ) );
            Assert.False( new Number( 1 ) > new Number( 2 ) );
        }

        [Fact( DisplayName = "number >= should return expected result" )]
        public void GreaterThanOrEqualToOperatorShouldReturnCorrectValue()
        {
            Assert.True( new Number( 2 ) >= new Number( 1 ) );
            Assert.True( new Number( 2 ) >= new Number( 2 ) );
            Assert.False( new Number( 1 ) >= new Number( 2 ) );
        }

        [Fact( DisplayName = "number < should return expected result" )]
        public void LessThanOperatorShouldReturnCorrectValue()
        {
            Assert.True( new Number( 1 ) < new Number( 2 ) );
            Assert.False( new Number( 2 ) < new Number( 1 ) );
        }

        [Fact( DisplayName = "number <= should return expected result" )]
        public void LessThanOrEqualToOperatorShouldReturnCorrectValue()
        {
            Assert.True( new Number( 1 ) <= new Number( 2 ) );
            Assert.True( new Number( 2 ) <= new Number( 2 ) );
            Assert.False( new Number( 2 ) <= new Number( 1 ) );
        }

        [Fact( DisplayName = "number increment should return expected result" )]
        public void IncrementMethodShouldReturnCorrectValue()
        {
            var target = new Number( 1 );
            var expected = new Number( 2 );
            var actual = Number.Increment( target );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number ++ should return expected result" )]
        public void IncrementOperatorShouldReturnCorrectValue()
        {
            var target = new Number( 1 );
            var expected = new Number( 2 );
            var actual = ++target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number decrement should return expected result" )]
        public void DecrementMethodShouldReturnCorrectValue()
        {
            var target = new Number( 2 );
            var expected = new Number( 1 );
            var actual = Number.Decrement( target );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number -- should return expected result" )]
        public void DecrementOperatorShouldReturnCorrectValue()
        {
            var target = new Number( 2 );
            var expected = new Number( 1 );
            var actual = --target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number add should return expected result" )]
        public void AddMethodShouldReturnCorrectValue()
        {
            var target = new Number( 1 );
            var expected = new Number( 2 );
            var actual = Number.Add( target, new Number( 1 ) );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number + should return expected result" )]
        public void PlusOperatorShouldReturnCorrectValue()
        {
            var target = new Number( 1 );
            var expected = new Number( 2 );
            var actual = target + new Number( 1 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number subtract should return expected result" )]
        public void SubtractMethodShouldReturnCorrectValue()
        {
            var target = new Number( 2 );
            var expected = new Number( 1 );
            var actual = Number.Subtract( target, new Number( 1 ) );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number - should return expected result" )]
        public void MinusOperatorShouldReturnCorrectValue()
        {
            var target = new Number( 2 );
            var expected = new Number( 1 );
            var actual = target - new Number( 1 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number negate should return expected result" )]
        public void NegateMethodShouldReturnCorrectValue()
        {
            var target = new Number( 1 );
            var expected = new Number( -1 );
            var actual = Number.Negate( target );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number unary - should return expected result" )]
        public void UnaryNegationOperatorShouldReturnCorrectValue()
        {
            var target = new Number( 1 );
            var expected = new Number( -1 );
            var actual = -target;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number multiply should return expected result" )]
        public void MultiplyShouldReturnCorrectValue()
        {
            var target = new Number( 2 );
            var expected = new Number( 4 );
            var actual = Number.Multiply( target, new Number( 2 ) );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number * should return expected result" )]
        public void MultiplicationOperatorShouldReturnCorrectValue()
        {
            var target = new Number( 2 );
            var expected = new Number( 4 );
            var actual = target * new Number( 2 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number divide should return expected result" )]
        public void DivideShouldReturnCorrectValue()
        {
            var target = new Number( 4 );
            var expected = new Number( 2 );
            var actual = Number.Divide( target, new Number( 2 ) );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number / should return expected result" )]
        public void DivisionOperatorShouldReturnCorrectValue()
        {
            var target = new Number( 4 );
            var expected = new Number( 2 );
            var actual = target / new Number( 2 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number modulus should return expected result" )]
        public void ModShouldReturnCorrectValue()
        {
            var target = new Number( 3 );
            var expected = new Number( 1 );
            var actual = Number.Mod( target, new Number( 2 ) );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "number % should return expected result" )]
        public void ModulusOperatorShouldReturnCorrectValue()
        {
            var target = new Number( 3 );
            var expected = new Number( 1 );
            var actual = target % new Number( 2 );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "generic number compare should return expected result" )]
        public void GenericCompareToShouldReturnCorrectValue()
        {
            Assert.Equal( 0, new Number( 1 ).CompareTo( new Number( 1 ) ) );
            Assert.Equal( 1, new Number( 2 ).CompareTo( new Number( 1 ) ) );
            Assert.Equal( -1, new Number( 1 ).CompareTo( new Number( 2 ) ) );
        }

        [Fact( DisplayName = "number compare should return expected result" )]
        public void CompareToShouldReturnCorrectValue()
        {
            Assert.Equal( 0, new Number( 1 ).CompareTo( (object) new Number( 1 ) ) );
            Assert.Equal( 1, new Number( 2 ).CompareTo( (object) new Number( 1 ) ) );
            Assert.Equal( -1, new Number( 1 ).CompareTo( (object) new Number( 2 ) ) );
            Assert.Equal( -1, new Number( 1 ).CompareTo( (object) null ) );
            Assert.Equal( -1, new Number( 1 ).CompareTo( new object() ) );
        }

        [Fact( DisplayName = "sequence of numbers should be sortable" )]
        public void ShouldSortSequenceOfNumbers()
        {
            var list = new List<Number>()
            {
                new Number( 20m, NumberStyle.Currency ),
                new Number( 5m, NumberStyle.Currency ),
                new Number( 10m, NumberStyle.Currency )
            };

            list.Sort();
            Assert.True( list.Select( n => (decimal) n ).SequenceEqual( new[] { 5m, 10m, 20m } ) );
        }

        [Fact( DisplayName = "sequence of numbers should be sortable by comparison" )]
        public void ShouldSortSequenceOfItems()
        {
            var list = new List<Tuple<Number>>()
            {
                new Tuple<Number>( new Number( 20m, NumberStyle.Currency ) ),
                new Tuple<Number>( new Number( 5m, NumberStyle.Currency ) ),
                new Tuple<Number>( new Number( 10m, NumberStyle.Currency ) )
            };

            list.Sort( ( t1, t2 ) => t1.Item1.CompareTo( t2.Item1 ) );
            Assert.True( list.Select( t => (decimal) t.Item1 ).SequenceEqual( new[] { 5m, 10m, 20m } ) );
        }
    }
}
