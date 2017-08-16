namespace More
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using static NumberStyle;
    using static System.Decimal;
    using static System.Globalization.CultureInfo;

    /// <summary>
    /// Represents any type of number.
    /// </summary>
    [DebuggerDisplay( "Value = {Value}, NumberStyle = {NumberStyle}" )]
    public struct Number : IEquatable<Number>, IComparable<Number>, IComparable, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Byte">byte</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Integer"/>.</remarks>
        public Number( byte value ) : this( new decimal( value ), Integer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Int16">short</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Integer"/>.</remarks>
        public Number( short value ) : this( new decimal( value ), Integer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="int">int</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Integer"/>.</remarks>
        public Number( int value ) : this( new decimal( value ), Integer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Int64">long</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Integer"/>.</remarks>
        public Number( long value ) : this( new decimal( value ), Integer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="SByte">sbyte</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Integer"/>.</remarks>
        [CLSCompliant( false )]
        public Number( sbyte value ) : this( new decimal( value ), Integer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="UInt16">ushort</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Integer"/>.</remarks>
        [CLSCompliant( false )]
        public Number( ushort value ) : this( new decimal( value ), Integer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="UInt32">uint</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Integer"/>.</remarks>
        [CLSCompliant( false )]
        public Number( uint value ) : this( new decimal( value ), Integer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="UInt64">ulong</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Integer"/>.</remarks>
        [CLSCompliant( false )]
        public Number( ulong value ) : this( new decimal( value ), Integer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Single">float</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Default"/>.</remarks>
        public Number( float value ) : this( new decimal( value ), Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Double">double</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Default"/>.</remarks>
        public Number( double value ) : this( new decimal( value ), Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Decimal">decimal</see> value to encapsulate.</param>
        /// <remarks>This constructor always sets the <see cref="P:Number.NumberStyle"/> property to <see cref="T:Default"/>.</remarks>
        public Number( decimal value ) : this( value, Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Single">float</see> value to encapsulate.</param>
        /// <param name="style">One of the <see cref="NumberStyle"/> values.</param>
        public Number( float value, NumberStyle style ) : this( new decimal( value ), style ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Double">double</see> value to encapsulate.</param>
        /// <param name="style">One of the <see cref="NumberStyle"/> values.</param>
        public Number( double value, NumberStyle style ) : this( new decimal( value ), style ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="Decimal">decimal</see> value to encapsulate.</param>
        /// <param name="style">One of the <see cref="NumberStyle"/> values.</param>
        public Number( decimal value, NumberStyle style )
        {
            Value = value;
            NumberStyle = style;
        }

        /// <summary>
        /// Gets the value of the number.
        /// </summary>
        /// <value>The value in <see cref="Decimal">decimal</see> form.</value>
        public decimal Value { get; }

        /// <summary>
        /// Gets the style of the number.
        /// </summary>
        /// <value>One of the <see cref="NumberStyle"/> values.</value>
        public NumberStyle NumberStyle { get; }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent <see cref="String">string</see> representation, using the specified format.
        /// </summary>
        /// <param name="format">A <see cref="String">string</see> containing the format specification.</param>
        /// <returns>A <see cref="String">string</see> representation of the value of this instance as specified by the <paramref name="format"/>.</returns>
        [Pure]
        public string ToString( string format )
        {
            Arg.NotNullOrEmpty( format, nameof( format ) );
            Contract.Ensures( Contract.Result<string>() != null );
            return ToString( format, CurrentCulture );
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent <see cref="String">string</see> representation using the specified culture-specific format information.
        /// </summary>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The <see cref="String">string</see> representation of the value of this instance as specified by the <paramref name="formatProvider">provider</paramref>.</returns>
        [Pure]
        public string ToString( IFormatProvider formatProvider )
        {
            Contract.Ensures( Contract.Result<string>() != null );

            switch ( NumberStyle )
            {
                case Currency:
                    return Value.ToString( "C", formatProvider );
                case Integer:
                    return Value.ToString( "N0", formatProvider );
                case Percent:
                    return Value.ToString( "P", formatProvider );
            }

            return Value.ToString( formatProvider );
        }

        /// <summary>
        /// Converts a numeric <see cref="String">string</see> representation to its <see cref="Number"/> equivalent using the specified style.
        /// </summary>
        /// <param name="text">A <see cref="String">string</see> containing a number to convert.</param>
        /// <param name="style">One of the <see cref="NumberStyle"/> values indicating the style of the parsed number.</param>
        /// <returns>The <see cref="Number"/> equivalent to the value contained in the string as specified by <paramref name="style"/>.</returns>
        [Pure]
        public static Number Parse( string text, NumberStyle style )
        {
            Arg.NotNullOrEmpty( text, nameof( text ) );
            Contract.Ensures( Contract.Result<Number>().NumberStyle == style );
            return Parse( text, style, CurrentCulture );
        }

        /// <summary>
        /// Converts a numeric <see cref="String">string</see> representation its <see cref="Number"/> equivalent using the specified style and culture-specific format.
        /// </summary>
        /// <param name="text">A <see cref="String">string</see> containing a number to convert.</param>
        /// <param name="style">One of the <see cref="NumberStyle"/> values indicating the style of the parsed number.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The <see cref="Number"/> equivalent to the value contained in the string as specified by <paramref name="style"/> and <paramref name="formatProvider">provider</paramref>.</returns>
        [Pure]
        public static Number Parse( string text, NumberStyle style, IFormatProvider formatProvider )
        {
            Arg.NotNullOrEmpty( text, nameof( text ) );
            Contract.Ensures( Contract.Result<Number>().NumberStyle == style );
            return new Number( decimal.Parse( text, NumberStyles.Any, formatProvider ), style );
        }

        /// <summary>
        /// Converts a numeric <see cref="String">string</see> representation to its <see cref="Number"/> equivalent using the specified style.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="text">A <see cref="String">string</see> containing a number to convert.</param>
        /// <param name="style">One of the <see cref="NumberStyle"/> values indicating the style of the parsed number.</param>
        /// <param name="result">When this method returns, it contains the <see cref="Number"/> equivalent to the numeric value if the conversion succeeded or zero if the conversion failed.
        /// The conversion fails if the input parameter is null, is not in a compliant format, or represents a number less than <see cref="P:Decimal.MinValue"/> or greater than <see cref="P:Decimal.MaxValue"/>.
        /// This parameter is passed uninitialized.</param>
        /// <returns>True if the conversion was successful; otherwise, false.</returns>
        [Pure]
        public static bool TryParse( string text, NumberStyle style, out Number result ) => TryParse( text, style, CurrentCulture, out result );

        /// <summary>
        /// Converts a numeric <see cref="String">string</see> representation to its <see cref="Number"/> equivalent using the specified style and culture-specific format.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="text">A <see cref="String">string</see> containing a number to convert.</param>
        /// <param name="style">One of the <see cref="NumberStyle"/> values indicating the style of the parsed number.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <param name="result">When this method returns, it contains the <see cref="Number"/> equivalent to the numeric value if the conversion succeeded or zero if the conversion failed.
        /// The conversion fails if the input parameter is null, is not in a compliant format, or represents a number less than <see cref="P:Decimal.MinValue"/> or greater than <see cref="P:Decimal.MaxValue"/>.
        /// This parameter is passed uninitialized.</param>
        /// <returns>True if the conversion was successful; otherwise, false.</returns>
        [Pure]
        public static bool TryParse( string text, NumberStyle style, IFormatProvider formatProvider, out Number result )
        {
            if ( decimal.TryParse( text, NumberStyles.Any, formatProvider, out var value ) )
            {
                result = new Number( value, style );
                return true;
            }

            result = new Number( 0m, style );
            return false;
        }

        /// <summary>
        /// Adds two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the addition of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number Add( Number number1, Number number2 ) => new Number( number1.Value + number2.Value, number1.NumberStyle );

        /// <summary>
        /// Subtracts two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the difference of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number Subtract( Number number1, Number number2 ) => new Number( number1.Value - number2.Value, number1.NumberStyle );

        /// <summary>
        /// Multiplies two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the product of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number Multiply( Number number1, Number number2 ) => new Number( number1.Value * number2.Value, number1.NumberStyle );

        /// <summary>
        /// Divides two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the division of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number Divide( Number number1, Number number2 ) => new Number( number1.Value / number2.Value, number1.NumberStyle );

        /// <summary>
        /// Returns the remainder from dividing two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the remainder of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number Mod( Number number1, Number number2 ) => new Number( number1.Value % number2.Value, number1.NumberStyle );

        /// <summary>
        /// Increments the specified <see cref="Number"/>.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to increment.</param>
        /// <returns>A <see cref="Number"/> containing the value <paramref name="number"/> incremented by 1.</returns>
        [Pure]
        public static Number Increment( Number number ) => new Number( number.Value + 1m, number.NumberStyle );

        /// <summary>
        /// Decrements the specified <see cref="Number"/>.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to decrement.</param>
        /// <returns>A <see cref="Number"/> containing the value <paramref name="number"/> decremented by 1.</returns>
        [Pure]
        public static Number Decrement( Number number ) => new Number( number.Value - 1m, number.NumberStyle );

        /// <summary>
        /// Negates the value of the <see cref="Number"/> operand.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> operand.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        /// <remarks>The sign of the operand is unchanged.</remarks>
        [Pure]
        public static Number Negate( Number number ) => new Number( -number.Value, number.NumberStyle );

        /// <summary>
        /// Returns a value indicating whether the specified object equals the current instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object">object</see> to evaluate.</param>
        /// <returns>True if the current instance equals the specified object; otherwise, false.</returns>
        public override bool Equals( object obj )
        {
            if ( obj is Number other )
            {
                return Value.Equals( other.Value );
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for the current instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Returns the <see cref="String">string</see> representation of the current instance.
        /// </summary>
        /// <returns>The <see cref="String">string</see> representation of the current instance.</returns>
        public override string ToString() => ToString( CurrentCulture );

        /// <summary>
        /// Returns the implicit conversion of the current number to its <see cref="Decimal">decimal</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="Decimal">decimal</see> structure.</returns>
        [Pure]
        public static implicit operator decimal( Number number ) => number.Value;

        /// <summary>
        /// Converts an 8-bit unsigned integer into a <see cref="Number"/>.
        /// </summary>
        /// <param name="value">The <see cref="Byte">byte</see> to convert.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        [Pure]
        public static implicit operator Number( byte value ) => new Number( value );

        /// <summary>
        /// Converts a 16-bit signed integer into a <see cref="Number"/>.
        /// </summary>
        /// <param name="value">The <see cref="Int16">short</see> to convert.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        [Pure]
        public static implicit operator Number( short value ) => new Number( value );

        /// <summary>
        /// Converts a 32-bit signed integer into a <see cref="Number"/>.
        /// </summary>
        /// <param name="value">The <see cref="int">int</see> to convert.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        [Pure]
        public static implicit operator Number( int value ) => new Number( value );

        /// <summary>
        /// Converts a 64-bit signed integer into a <see cref="Number"/>.
        /// </summary>
        /// <param name="value">The <see cref="Int64">long</see> to convert.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        [Pure]
        public static implicit operator Number( long value ) => new Number( value );

        /// <summary>
        /// Converts an 8-bit signed integer into a <see cref="Number"/>.
        /// </summary>
        /// <param name="value">The <see cref="SByte">sbyte</see> to convert.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        [Pure]
        [CLSCompliant( false )]
        public static implicit operator Number( sbyte value ) => new Number( value );

        /// <summary>
        /// Converts a 16-bit unsigned integer into a <see cref="Number"/>.
        /// </summary>
        /// <param name="value">The <see cref="Int16">ushort</see> to convert.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        [Pure]
        [CLSCompliant( false )]
        public static implicit operator Number( ushort value ) => new Number( value );

        /// <summary>
        /// Converts a 32-bit unsigned integer into a <see cref="Number"/>.
        /// </summary>
        /// <param name="value">The <see cref="int">uint</see> to convert.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        [Pure]
        [CLSCompliant( false )]
        public static implicit operator Number( uint value ) => new Number( value );

        /// <summary>
        /// Converts a 64-bit unsigned integer into a <see cref="Number"/>.
        /// </summary>
        /// <param name="value">The <see cref="Int64">ulong</see> to convert.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        [Pure]
        [CLSCompliant( false )]
        public static implicit operator Number( ulong value ) => new Number( value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="Single">float</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="Single">float</see> structure.</returns>
        [Pure]
        public static explicit operator float( Number number ) => ToSingle( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="Double">double</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="Double">double</see> structure.</returns>
        [Pure]
        public static explicit operator double( Number number ) => ToDouble( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="Byte">byte</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="Byte">byte</see> structure.</returns>
        [Pure]
        public static explicit operator byte( Number number ) => ToByte( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="Int16">short</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="Int16">short</see> structure.</returns>
        [Pure]
        public static explicit operator short( Number number ) => ToInt16( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="int">int</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="int">int</see> structure.</returns>
        [Pure]
        public static explicit operator int( Number number ) => ToInt32( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="Int64">long</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="Int64">long</see> structure.</returns>
        [Pure]
        public static explicit operator long( Number number ) => ToInt64( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="SByte">sbyte</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="SByte">sbyte</see> structure.</returns>
        [Pure]
        [CLSCompliant( false )]
        public static explicit operator sbyte( Number number ) => ToSByte( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="UInt16">ushort</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="UInt16">ushort</see> structure.</returns>
        [Pure]
        [CLSCompliant( false )]
        public static explicit operator ushort( Number number ) => ToUInt16( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="UInt32">uint</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="UInt32">uint</see> structure.</returns>
        [Pure]
        [CLSCompliant( false )]
        public static explicit operator uint( Number number ) => ToUInt32( number.Value );

        /// <summary>
        /// Returns the explicit conversion of the current number to its <see cref="UInt64">ulong</see> equivalent.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to convert.</param>
        /// <returns>A <see cref="UInt64">ulong</see> structure.</returns>
        [Pure]
        [CLSCompliant( false )]
        public static explicit operator ulong( Number number ) => ToUInt64( number.Value );

        /// <summary>
        /// Returns the equality of two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The <see cref="Number"/> to compare.</param>
        /// <param name="number2">The <see cref="Number"/> to compare against.</param>
        /// <returns>True if the structures are equal; otherwise, false.</returns>
        [Pure]
        public static bool operator ==( Number number1, Number number2 ) => number1.Value.Equals( number2.Value );

        /// <summary>
        /// Returns the inequality of two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The <see cref="Number"/> to compare.</param>
        /// <param name="number2">The <see cref="Number"/> to compare against.</param>
        /// <returns>True if the structures are not equal; otherwise, false.</returns>
        [Pure]
        public static bool operator !=( Number number1, Number number2 ) => !number1.Value.Equals( number2.Value );

        /// <summary>
        /// Returns a value indicating whether the first <see cref="Number"/> is greater than the second <see cref="Number"/>.
        /// </summary>
        /// <param name="number1">The <see cref="Number"/> to compare.</param>
        /// <param name="number2">The <see cref="Number"/> to compare against.</param>
        /// <returns>True if the first structure is greater than the second structure; otherwise, false.</returns>
        [Pure]
        public static bool operator >( Number number1, Number number2 ) => number1.Value > number2.Value;

        /// <summary>
        /// Returns a value indicating whether the first <see cref="Number"/> is greater than or equal to the second <see cref="Number"/>.
        /// </summary>
        /// <param name="number1">The <see cref="Number"/> to compare.</param>
        /// <param name="number2">The <see cref="Number"/> to compare against.</param>
        /// <returns>True if the first structure is greater than or equal to the second structure; otherwise, false.</returns>
        [Pure]
        public static bool operator >=( Number number1, Number number2 ) => number1.Value >= number2.Value;

        /// <summary>
        /// Returns a value indicating whether the first <see cref="Number"/> is less than to the second <see cref="Number"/>.
        /// </summary>
        /// <param name="number1">The <see cref="Number"/> to compare.</param>
        /// <param name="number2">The <see cref="Number"/> to compare against.</param>
        /// <returns>True if the first structure is less than to the second structure; otherwise, false.</returns>
        [Pure]
        public static bool operator <( Number number1, Number number2 ) => number1.Value < number2.Value;

        /// <summary>
        /// Returns a value indicating whether the first <see cref="Number"/> is less than or equal to the second <see cref="Number"/>.
        /// </summary>
        /// <param name="number1">The <see cref="Number"/> to compare.</param>
        /// <param name="number2">The <see cref="Number"/> to compare against.</param>
        /// <returns>True if the first structure is less than or equal to the second structure; otherwise, false.</returns>
        [Pure]
        public static bool operator <=( Number number1, Number number2 ) => number1.Value <= number2.Value;

        /// <summary>
        /// Increments the specified <see cref="Number"/>.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to increment.</param>
        /// <returns>A <see cref="Number"/> containing the value <paramref name="number"/> incremented by 1.</returns>
        public static Number operator ++( Number number ) => Increment( number );

        /// <summary>
        /// Decrements the specified <see cref="Number"/>.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> to decrement.</param>
        /// <returns>A <see cref="Number"/> containing the value <paramref name="number"/> decremented by 1.</returns>
        public static Number operator --( Number number ) => Decrement( number );

        /// <summary>
        /// Adds two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the addition of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number operator +( Number number1, Number number2 ) => Add( number1, number2 );

        /// <summary>
        /// Subtracts two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the difference of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number operator -( Number number1, Number number2 ) => Subtract( number1, number2 );

        /// <summary>
        /// Negates the value of the <see cref="Number"/> operand.
        /// </summary>
        /// <param name="number">The <see cref="Number"/> operand.</param>
        /// <returns>A <see cref="Number"/> structure.</returns>
        /// <remarks>The sign of the operand is unchanged.</remarks>
        public static Number operator -( Number number ) => Negate( number );

        /// <summary>
        /// Multiplies two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the product of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number operator *( Number number1, Number number2 ) => Multiply( number1, number2 );

        /// <summary>
        /// Divides two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the division of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number operator /( Number number1, Number number2 ) => Divide( number1, number2 );

        /// <summary>
        /// Returns the remainder from dividing two <see cref="Number"/> structures.
        /// </summary>
        /// <param name="number1">The first <see cref="Number"/>.</param>
        /// <param name="number2">The second <see cref="Number"/>.</param>
        /// <returns>A <see cref="Number"/> containing the remainder of the two structures.</returns>
        /// <remarks>If the <see cref="P:Number.NumberStyle"/> do not match, the <see cref="P:Number.NumberStyle"/> from the left-hand side of the operation is preserved.</remarks>
        [Pure]
        public static Number operator %( Number number1, Number number2 ) => Mod( number1, number2 );

        /// <summary>
        /// Returns a value indicating whether the specified object equals the current instance.
        /// </summary>
        /// <param name="other">The <see cref="Number"/> to evaluate.</param>
        /// <returns>True if the current instance equals the specified object; otherwise, false.</returns>
        public bool Equals( Number other ) => Value.Equals( other.Value );

        /// <summary>
        /// Compares the current instance to the specified object.
        /// </summary>
        /// <param name="other">The <see cref="Number"/> to compare against.</param>
        /// <returns>A signed number indicating the relative values of this instance and the specified value.  A value less than zero indicates the current instance is less than the
        /// specified object.  A value greater than zero indicates the current instance is greater than the specified object.  A value of zero indicates the objects are equal.</returns>
        public int CompareTo( Number other ) => Value.CompareTo( other.Value );

        /// <summary>
        /// Compares the current instance to the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Object">object</see> to compare against.</param>
        /// <returns>A signed number indicating the relative values of this instance and the specified value.  A value less than zero indicates the current instance is less than the
        /// specified object.  A value greater than zero indicates the current instance is greater than the specified object.  A value of zero indicates the objects are equal.</returns>
        public int CompareTo( object obj )
        {
            if ( obj is Number other )
            {
                return Value.CompareTo( other.Value );
            }

            return obj == null ? 1 : throw new ArgumentException( ExceptionMessage.InvalidObjectType.FormatDefault( GetType().Name ) );
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent <see cref="string">string</see> representation using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A <see cref="String">string</see> containing the format specification.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>The <see cref="String">string</see> representation of the value of this instance as specified by the <paramref name="format"/> and <paramref name="formatProvider">provider</paramref>.</returns>
        public string ToString( string format, IFormatProvider formatProvider )
        {
            Arg.NotNullOrEmpty( format, nameof( format ) );
            return Value.ToString( format, formatProvider );
        }
    }
}