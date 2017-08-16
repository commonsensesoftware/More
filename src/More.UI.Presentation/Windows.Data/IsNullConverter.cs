namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
#if UAP10_0
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

    /// <summary>
    /// Represents a value converter for null equality.
    /// </summary>
    /// <example>This example demonstrates how to convert a null value to a Boolean.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:More="clr-namespace:System.Windows.Data;assembly=System.Windows.More">
    ///  
    /// <More:IsNullConverter x:Key="NullConverter" />
    /// 
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="{Binding Name, Converter={StaticResource NullConverter}, StringFormat='Name is null = \{0\}'}" />
    /// </Grid>
    /// 
    /// </UserControl>
    /// ]]></code></example>
    /// <example>This example demonstrates how to convert a null value to a Boolean and negating the result.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:More="clr-namespace:System.Windows.Data;assembly=System.Windows.More">
    ///  
    /// <More:IsNullConverter x:Key="NullConverter" Negate="True" />
    /// 
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="{Binding Name, Converter={StaticResource NullConverter}, StringFormat='Name is not null = \{0\}'}" />
    /// </Grid>
    /// 
    /// </UserControl>
    /// ]]></code></example>
    public class IsNullConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNullConverter"/> class.
        /// </summary>
        public IsNullConverter() => TreatEmptyStringAsNull = true;

        /// <summary>
        /// Gets or sets a value indicating whether the result of the conversion should be negated.
        /// </summary>
        /// <value>True if the result of the conversion is negated; otherwise, false.</value>
        /// <remarks>Negating the result supports test for non-null.</remarks>
        public bool Negate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether empty strings are treated as null.
        /// </summary>
        /// <value>True if empty strings are treated as null; otherwise, false.  The default value is true.</value>
        /// <remarks>If the value to be converted is not a <see cref="string">string</see>, this property has no effect.</remarks>
        public bool TreatEmptyStringAsNull { get; set; }

        bool ShouldNegate( object parameter, CultureInfo culture )
        {
            if ( parameter == null )
            {
                return Negate;
            }
            else if ( parameter is bool )
            {
                return (bool) parameter;
            }

            var message = string.Format( culture, ExceptionMessage.InvalidConverterParameter, typeof( bool ) );

            if ( !( parameter is string ) )
            {
                throw new ArgumentException( message, nameof( parameter ) );
            }

            if ( !bool.TryParse( parameter.ToString(), out var negate ) )
            {
                throw new ArgumentException( message, nameof( parameter ) );
            }

            return negate;
        }

#if UAP10_0
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supportedType = typeof( bool ).Equals( targetType ) || typeof( object ).Equals( targetType );

            if ( !supportedType )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }

#if UAP10_0
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var negate = ShouldNegate( parameter, culture );

            if ( value == null )
            {
                return negate ? false : true;
            }

            if ( ( value is string ) && value.ToString().Length == 0 && TreatEmptyStringAsNull )
            {
                return negate ? false : true;
            }

            return negate ? true : false;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "All conversions performed by this type are one-way." )]
#if UAP10_0
        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, string language ) =>
            throw new NotSupportedException( ExceptionMessage.ConvertBackUnsupported );
#else
        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) =>
            throw new NotSupportedException( ExceptionMessage.ConvertBackUnsupported );
#endif
    }
}