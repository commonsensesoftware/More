namespace More.Windows.Data
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Represents a <see cref="bool"/> converter.
    /// </summary>
    /// <remarks>This converter also supports converting Boolean values to their integer equivalents in order to use
    /// the value with conditional format strings.  When a format is not specified and the target type is <see cref="string">string</see>,
    /// the default Boolean string value is returned.</remarks>
    public class BooleanConverter : FormatConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether the result of the conversion should be negated.
        /// </summary>
        /// <value>True if the result of the conversion is negated; otherwise, false.</value>
        public bool Negate { get; set; }

        bool ShouldNegate( object parameter, CultureInfo culture )
        {
            if ( parameter == null )
            {
                return Negate;
            }
            else if ( parameter is bool boolean )
            {
                return boolean;
            }

            var message = ExceptionMessage.InvalidConverterParameter.FormatDefault( typeof( bool ) );

            if ( parameter is string @string && bool.TryParse( @string, out var negate ) )
            {
                return negate;
            }

            throw new ArgumentException( message, nameof( parameter ) );
        }
#if UAP10_0
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public override object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public override object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = value == null || value is bool || value is bool?;

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedValueType.FormatDefault( value == null ? "null" : value.GetType().ToString() ), nameof( value ) );
            }

            var boolean = typeof( bool ).Equals( targetType );
            supported = typeof( object ).Equals( targetType ) || typeof( string ).Equals( targetType ) || boolean;

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }
#if UAP10_0
            var culture = Util.GetCultureFromLanguage( language );
#endif
            if ( value == null )
            {
                if ( string.IsNullOrEmpty( Format ) )
                {
                    return DefaultNullValue;
                }

                return string.Format( culture, Format, DefaultNullValue );
            }

            var val = (bool) value;

            if ( ShouldNegate( parameter, culture ) )
            {
                val = !val;
            }

            if ( boolean )
            {
                return val;
            }
#if UAP10_0
            if ( string.IsNullOrEmpty( Format ) )
            {
                return val.ToString();
            }

            var @int = System.Convert.ToInt32( val, culture );
#else
            var convertible = (IConvertible) val;

            if ( string.IsNullOrEmpty( Format ) )
            {
                return convertible.ToString( culture );
            }

            var @int = convertible.ToInt32( culture );
#endif
            return string.Format( culture, Format, @int );
        }

#if UAP10_0
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx_core"]/*'/>
        public override object ConvertBack( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx"]/*'/>
        public override object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = value == null || value is bool || value is bool?;

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedValueType.FormatDefault( value == null ? "null" : value.GetType().ToString() ), nameof( value ) );
            }

            supported = typeof( object ).Equals( targetType ) || typeof( bool ).Equals( targetType );

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }
#if UAP10_0
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var val = (bool) value;

            return ShouldNegate( parameter, culture ) ? !val : val;
        }
    }
}