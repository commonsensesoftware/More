namespace More.Windows.Data
{
    using System;
    using System.Globalization;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows;
    using System.Windows.Data;
#endif

    /// <summary>
    /// Provides conversions between <see cref="Boolean"/> and <see cref="Visibility"/>.
    /// </summary>
    public class VisibilityConverter : IValueConverter
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
                return this.Negate;
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
        public virtual object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            if ( value == null )
                return value;

            var valueType = value.GetType();

            // the input and output types are the same; no conversion required
            if ( valueType.Equals( targetType ) )
                return value;

            // if the target type is System.Object, then assume it's the inverse type of the value type
            if ( typeof( object ).Equals( targetType ) )
            {
                // only map to alternate type if the value type is a supported input type
                if ( typeof( bool ).Equals( valueType ) )
                    targetType = typeof( Visibility );
                else if ( typeof( Visibility ).Equals( valueType ) )
                    targetType = typeof( bool );
            }

            // coerce the negate parameter; defaults to false if unspecified
#if NETFX_CORE
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var negate = this.ShouldNegate( parameter, culture );

            // convert to appropriate Boolean or Visibility
            if ( ( value is bool || value is bool? ) && typeof( Visibility ).Equals( targetType ) )
            {
                if ( negate )
                    return ( (bool) value ) ? Visibility.Collapsed : Visibility.Visible;
                else
                    return ( (bool) value ) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if ( value is Visibility && ( typeof( bool ).Equals( targetType ) || typeof( bool? ).Equals( targetType ) ) )
            {
                if ( negate )
                    return !( ( (Visibility) value ) == Visibility.Visible );
                else
                    return ( (Visibility) value ) == Visibility.Visible;
            }

            throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );
        }

#if NETFX_CORE
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public virtual object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return this.Convert( value, targetType, parameter, language );
        }
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public virtual object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return this.Convert( value, targetType, parameter, culture );
        }
#endif
    }
}
