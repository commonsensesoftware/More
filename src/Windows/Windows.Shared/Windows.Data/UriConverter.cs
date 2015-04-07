namespace More.Windows.Data
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents a formattable <see cref="Uri"/> value converter.
    /// </summary>
    public class UriConverter : FormatConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UriConverter"/> class.
        /// </summary>
        public UriConverter()
        {
            this.UriKind = System.UriKind.Absolute;
        }

        /// <summary>
        /// Gets or sets the kind of URI the converter handles.
        /// </summary>
        /// <value>One of the <see cref="UriKind"/> values. The default value is <see cref="T:UriKind.Absolute"/>.</value>
        public UriKind UriKind
        {
            get;
            set;
        }

#if NETFX_CORE
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public override object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public override object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = typeof( object ).Equals( targetType ) || typeof( Uri ).Equals( targetType );

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );

            if ( value == null )
            {
                if ( string.IsNullOrEmpty( this.DefaultNullValue ) )
                    return null;
                else
                    value = this.DefaultNullValue;
            }

#if NETFX_CORE
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var formattable = value as IFormattable;

            if ( formattable != null )
                return new Uri( formattable.ToString( this.Format, culture ), this.UriKind );
            else if ( !string.IsNullOrEmpty( this.Format ) )
                return new Uri( string.Format( culture, this.Format, value ), this.UriKind );

            return new Uri( value.ToString(), this.UriKind );
        }

#if NETFX_CORE
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx_core"]/*'/>
        public override object ConvertBack( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx"]/*'/>
        public override object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = typeof( object ).Equals( targetType ) || typeof( string ).Equals( targetType );

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );

            if ( value == null || StringComparer.Ordinal.Equals( value.ToString(), this.DefaultNullValue ) )
                return null;

            return value.ToString();
        }
    }
}