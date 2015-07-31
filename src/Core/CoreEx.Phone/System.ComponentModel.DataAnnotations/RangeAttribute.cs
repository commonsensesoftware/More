namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Globalization;
    using global::System.Reflection;
    using Diagnostics.CodeAnalysis;

    /// <summary>
    /// Specifies the numeric range constraints for the value of a data field.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.RangeAttribute.</remarks>
    [SuppressMessage( "Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Ported from System.ComponentModel.DataAnnotations." )]
    [SuppressMessage( "Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Ported from System.ComponentModel.DataAnnotations." )]
    [AttributeUsage( AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false )]
    public class RangeAttribute : ValidationAttribute
    {
        private RangeAttribute()
            : base( (Func<string>) ( () => DataAnnotationsResources.RangeAttribute_ValidationError ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.DataAnnotations.RangeAttribute class by using the specified minimum and maximum values.
        /// </summary>
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
        public RangeAttribute( double minimum, double maximum )
            : this()
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = typeof( double );
        }

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.DataAnnotations.RangeAttribute class by using the specified minimum and maximum values.
        /// </summary>
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
        public RangeAttribute( int minimum, int maximum )
            : this()
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = typeof( int );
        }

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.DataAnnotations.RangeAttribute class by using the specified minimum and maximum values and the specific type.
        /// </summary>
        /// <param name="type">Specifies the type of the object to test.</param>
        /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
        /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
        public RangeAttribute( Type type, string minimum, string maximum )
            : this()
        {
            OperandType = type;
            Minimum = minimum;
            Maximum = maximum;
        }

        private Func<object, object> Conversion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the maximum allowed field value.
        /// </summary>
        /// <value>The maximum value that is allowed for the data field.</value>
        public object Maximum
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the minimum allowed field value.
        /// </summary>
        /// <value>The minimum value that is allowed for the data field.</value>
        public object Minimum
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the data field whose value must be validated.
        /// </summary>
        /// <value>The type of the data field whose value must be validated.</value>
        public Type OperandType
        {
            get;
            private set;
        }

        private void Initialize( IComparable minimum, IComparable maximum, Func<object, object> conversion )
        {
            if ( minimum.CompareTo( maximum ) > 0 )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.RangeAttribute_MinGreaterThanMax, new object[] { maximum, minimum } ) );
            }
            Minimum = minimum;
            Maximum = maximum;
            Conversion = conversion;
        }

        private void SetupConversion()
        {
            if ( Conversion != null )
                return;

            object minimum = Minimum;
            object maximum = Maximum;

            if ( ( minimum == null ) || ( maximum == null ) )
            {
                throw new InvalidOperationException( DataAnnotationsResources.RangeAttribute_Must_Set_Min_And_Max );
            }

            var type = minimum.GetType();

            if ( type == typeof( int ) )
            {
                Initialize( (int) minimum, (int) maximum, v => Convert.ToInt32( v, CultureInfo.InvariantCulture ) );
            }
            else if ( type == typeof( double ) )
            {
                Initialize( (double) minimum, (double) maximum, v => Convert.ToDouble( v, CultureInfo.InvariantCulture ) );
            }
            else
            {
                type = OperandType;

                if ( type == null )
                {
                    throw new InvalidOperationException( DataAnnotationsResources.RangeAttribute_Must_Set_Operand_Type );
                }

                var type2 = typeof( IComparable );

                if ( !type2.GetTypeInfo().IsAssignableFrom( type.GetTypeInfo() ) )
                {
                    throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.RangeAttribute_ArbitraryTypeNotIComparable, new object[] { type.FullName, type2.FullName } ) );
                }

                var culture = CultureInfo.CurrentCulture;
                var comparable = (IComparable) Convert.ChangeType( minimum, type, culture );
                var comparable2 = (IComparable) Convert.ChangeType( maximum, type, culture );
                Func<object, object> conversion = delegate( object value )
                {
                    if ( ( value != null ) && ( value.GetType() == type ) )
                    {
                        return value;
                    }
                    return Convert.ChangeType( value, type, culture );
                };

                Initialize( comparable, comparable2, conversion );
            }
        }

        /// <summary>
        /// Formats the error message that is displayed when range validation fails.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted error message.</returns>
        public override string FormatErrorMessage( string name )
        {
            SetupConversion();
            return string.Format( CultureInfo.CurrentCulture, base.ErrorMessageString, new object[] { name, Minimum, Maximum } );
        }

        /// <summary>
        /// Checks that the value of the data field is in the specified range.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>True if the specified value is in the range; otherwise, false.</returns>
        public override bool IsValid( object value )
        {
            SetupConversion();

            if ( value == null )
            {
                return true;
            }

            var str = value as string;

            if ( ( str != null ) && string.IsNullOrEmpty( str ) )
            {
                return true;
            }

            object obj2 = null;

            try
            {
                obj2 = Conversion( value );
            }
            catch ( FormatException )
            {
                return false;
            }
            catch ( InvalidCastException )
            {
                return false;
            }
            catch ( NotSupportedException )
            {
                return false;
            }

            var minimum = (IComparable) Minimum;
            var maximum = (IComparable) Maximum;

            return ( ( minimum.CompareTo( obj2 ) <= 0 ) && ( maximum.CompareTo( obj2 ) >= 0 ) );
        }
    }

}
