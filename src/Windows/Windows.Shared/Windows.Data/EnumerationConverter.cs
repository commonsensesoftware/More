namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;

    /// <summary>
    /// Represents an <see cref="Enum">enumeration</see> value converter.
    /// </summary>
    public partial class EnumerationConverter : FormatConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether enumeration values are formatted using
        /// their numeric equivalents.
        /// </summary>
        /// <value>True if enumeration values are formatted in numeric format; otherwise, false.</value>
        public bool UseNumericFormat
        {
            get;
            set;
        }

        private static object GetEnumFromString( Type type, string text )
        {
            Contract.Requires( type != null );
            Contract.Requires( !string.IsNullOrEmpty( text ) );

            // find a field with a matching name or description
            var comparer = StringComparer.Ordinal;
            var culture = CultureInfo.InvariantCulture;
            var enumType = Enum.GetUnderlyingType( type );
            var enumValues = Enum.GetValues( type ).Cast<Enum>();
            var enums = ( from enumValue in enumValues
                        let displayName = enumValue.GetDisplayName()
                        let desc = displayName == enumValue.ToString() ? enumValue.GetDescription() : displayName
                        let value = System.Convert.ChangeType( enumValue, enumType, culture ).ToString()
                        where comparer.Equals( desc, text ) || comparer.Equals( value, text )
                        select enumValue ).ToArray();

            // must be exactly one match
            if ( enums.Length   == 1 )
                return enums[0];

            throw new InvalidCastException( ExceptionMessage.CannotConvertEnumValue.FormatDefault( text, type ) );
        }

        private static Enum GetParameterAsEnum( Type enumType, object parameter )
        {
            if ( parameter == null )
                throw new ArgumentException( ExceptionMessage.EnumConverterParameterExpected, "parameter" );

            Enum value;

            try
            {
                // parse value
                value = (Enum) Enum.Parse( enumType, parameter.ToString(), true );
            }
            catch ( ArgumentException )
            {
                // parsing will fail for invalid strings
                throw new ArgumentException( ExceptionMessage.EnumConverterParameterInvalid.FormatDefault( enumType, parameter ), "parameter" );
            }

            // parsing will still succeed for invalid numeric strings; ensure the enum is valid
            if ( !Enum.IsDefined( enumType, value ) )
                throw new ArgumentException( ExceptionMessage.EnumConverterParameterInvalid.FormatDefault( enumType, parameter ), "parameter" );

            return value;
        }

        /// <summary>
        /// Gets or sets the value format.
        /// </summary>
        /// <value>The value format.</value>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.<p>- or -</p>
        /// <paramref name="value"/> is an empty string.</exception>
        /// <exception cref="FormatException"><paramref name="value"/> is not <b>G</b> or <b>D</b>.</exception>
        public override string Format
        {
            get
            {
                if ( UseNumericFormat )
                    return "D";

                return "G";
            }
            set
            {
                if ( value == null )
                    throw new ArgumentNullException( "value" );

                var formatChar = value.ToUpperInvariant();
      
                if ( ( formatChar != "G" ) && ( formatChar != "D" ) )
                    throw new FormatException( ExceptionMessage.EnumConverterException );

                UseNumericFormat = formatChar == "D";
            }
        }

#if NETFX_CORE
        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="language">The language used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="targetType"/> is not type <see cref="String"/>.</exception>
        public override object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="targetType"/> is not type <see cref="String"/>.</exception>
        public override object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var boolean = IsBooleanType( targetType );
            var supported = typeof( object ).Equals( targetType ) || typeof( string ).Equals( targetType ) || typeof( Enum ).GetTypeInfo().IsAssignableFrom( targetType.GetTypeInfo() ) || boolean;
     
            bool nullable;
            Type underlyingType;
            var numeric = IsNumericType( targetType, out underlyingType, out nullable );

            if ( !supported && numeric )
            {
                // conversion to numeric formats are only allowed if the format is set to numeric
                if ( !UseNumericFormat )
                    throw new ArgumentException( ExceptionMessage.NumericFormatMustBeUsedToConvertEnumToNumber.FormatDefault( targetType ), "targetType" );

                supported = true;
            }

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );

#if NETFX_CORE
            var culture = Util.GetCultureFromLanguage( language );
#endif
            // handle numbers
            if ( numeric )
            {
                if ( value == null )
                    return nullable ? null : Activator.CreateInstance( underlyingType );

                var number = System.Convert.ChangeType( value, underlyingType, culture );
                return nullable ? Activator.CreateInstance( typeof( Nullable<> ).MakeGenericType( underlyingType ), number ) : number;
            }

            // handle null values
            if ( value == null )
                return boolean ? null : DefaultNullValue;

            // echo value
            if ( value.GetType() == underlyingType )
                return value;

            var actualValue = (Enum) value;

            // handle booleans
            if ( boolean )
                return object.Equals( actualValue, GetParameterAsEnum( actualValue.GetType(), parameter ) );

            // handle strings with or without formatting
            if ( UseNumericFormat )
                return ( (IFormattable) actualValue ).ToString( Format, culture );

            var displayName = actualValue.GetDisplayName();

            if ( displayName == actualValue.ToString() )
                return actualValue.GetDescription();

            return displayName;
        }

#if NETFX_CORE
        /// <summary>
        /// Converts a previously converted value back to the specified type. 
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="language">The language used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetType"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="targetType"/> is not an enumeration type.</exception>
        public override object ConvertBack( object value, Type targetType, object parameter, string language )
#else
        /// <summary>
        /// Converts a previously converted value back to the specified type. 
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetType"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="targetType"/> is not an enumeration type.</exception>
        public override object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            if ( targetType == null )
                throw new ArgumentNullException( "targetType" );

            if ( !targetType.GetTypeInfo().IsEnum )
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );

            if ( value == null )
                return null;

            var sourceType = value.GetType();

            // check booleans first
            if ( IsBooleanType( sourceType ) )
                return (bool) value ? GetParameterAsEnum( targetType, parameter ) : null;

            // determine if value is numeric
            bool nullable;
            Type underlyingType;
            var numeric = IsNumericType( sourceType, out underlyingType, out nullable );

            if ( !numeric )
            {
                var text = value as string;

                // parse string
                if ( text != null )
                {
                    if ( string.Equals( text, DefaultNullValue, StringComparison.Ordinal ) )
                        return null;

                    return GetEnumFromString( targetType, text );
                }
            }
            else
            {
                // convert value to enumeration numeric equivalent (ex: Nullable<int> -> int)
#if NETFX_CORE
                var culture = Util.GetCultureFromLanguage( language );
#endif
                value = System.Convert.ChangeType( value, Enum.GetUnderlyingType( targetType ), culture );
            }

            // convert to enumeration
            return Enum.ToObject( targetType, value );
        }
    }
}
