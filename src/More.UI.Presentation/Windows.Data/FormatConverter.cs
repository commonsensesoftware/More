namespace More.Windows.Data
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Represents a custom format converter.
    /// </summary>
    public class FormatConverter : IFormattableValueConverter
    {
        /// <summary>
        /// Gets or sets the custom format.
        /// </summary>
        /// <value>The custom format.</value>
        public virtual string Format { get; set; }

        /// <summary>
        /// Gets or sets the default value used to format null values.
        /// </summary>
        /// <value>The default value for null values.</value>
        public virtual string DefaultNullValue { get; set; }

#if UAP10_0
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = typeof( object ).Equals( targetType ) || typeof( string ).Equals( targetType );

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }
#if UAP10_0
            var culture = Util.GetCultureFromLanguage( language );
#endif
            if ( value == null )
            {
                if ( ( value = DefaultNullValue ) == null )
                {
                    return null;
                }
            }


            if ( value is IFormattable formattable )
            {
                return formattable.ToString( Format, culture );
            }
            else if ( value.GetType() == targetType )
            {
                return value;
            }
            else if ( !string.IsNullOrEmpty( Format ) )
            {
                return string.Format( culture, Format, value );
            }

            return ( value ?? string.Empty ).ToString();
        }

#if UAP10_0
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx_core"]/*'/>
        public virtual object ConvertBack( object value, Type targetType, object parameter, string language ) =>
#else
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx"]/*'/>
        public virtual object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) =>
#endif
            throw new NotSupportedException( ExceptionMessage.ConvertBackUnsupported );
    }
}