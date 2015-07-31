namespace More.Windows.Data
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Represents a <see cref="Boolean"/> converter.
    /// </summary>
    /// <remarks>This converter also supports converting Boolean values to their integer equivalents in order to use
    /// the value with conditional format strings.  When a format is not specified and the target type is <see cref="String">string</see>,
    /// the default Boolean string value is returned.</remarks>
    public class BooleanConverter : FormatConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether the result of the conversion should be negated.
        /// </summary>
        /// <value>True if the result of the conversion is negated; otherwise, false.</value>
        public bool Negate
        {
            get;
            set;
        }

        private bool ShouldNegate( object parameter, CultureInfo culture )
        {
            if ( parameter == null )
                return Negate;
            else if ( parameter is bool )
                return (bool) parameter;

            var message = string.Format( culture, ExceptionMessage.InvalidConverterParameter, typeof( bool ) );

            if ( !( parameter is string ) )
                throw new ArgumentException( message, "parameter" );

            bool negate;

            if ( !bool.TryParse( parameter.ToString(), out negate ) )
                throw new ArgumentException( message, "parameter" );

            return negate;
        }

        
#if NETFX_CORE
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public override object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public override object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = value == null || value is bool || value is bool?;

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedValueType.FormatDefault( value == null ? "null" : value.GetType().ToString() ), "value" );

            var boolean = typeof( bool ).Equals( targetType );
            supported = typeof( object ).Equals( targetType ) || typeof( string ).Equals( targetType ) || boolean;

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );

#if NETFX_CORE
            var culture = Util.GetCultureFromLanguage( language );
#endif
            // handle null first
            if ( value == null )
            {
                if ( string.IsNullOrEmpty( Format ) )
                    return DefaultNullValue;

                return string.Format( culture, Format, DefaultNullValue );
            }

            var val = (bool) value;

            // negate value if necessary
            if ( ShouldNegate( parameter, culture ) )
                val = !val;

            // handle Boolean return values next
            if ( boolean )
                return val;

#if NETFX_CORE
            // no format; use Boolean.ToString
            if ( string.IsNullOrEmpty( Format ) )
                return val.ToString();

            var intValue = System.Convert.ToInt32( val, culture );
#else
            var convertible = (IConvertible) val;

            // no format; use Boolean.ToString
            if ( string.IsNullOrEmpty( Format ) )
                return convertible.ToString( culture );

            var intValue = convertible.ToInt32( culture );
#endif
            // use integer equivalent in order to support conditional formatting
            return string.Format( culture, Format, intValue );
        }

#if NETFX_CORE
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx_core"]/*'/>
        public override object ConvertBack( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx"]/*'/>
        public override object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = value == null || value is bool || value is bool?;

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedValueType.FormatDefault( value == null ? "null" : value.GetType().ToString() ), "value" );

            supported = typeof( object ).Equals( targetType ) || typeof( bool ).Equals( targetType );

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );

#if NETFX_CORE
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var val = (bool) value;

            return ShouldNegate( parameter, culture ) ? !val : val;
        }
    }
}