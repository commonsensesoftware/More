namespace More.Windows.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
#if UAP10_0
    using global::Windows.UI;
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Markup;
    using global::Windows.UI.Xaml.Media;
    using Color = global::Windows.UI.Color;
#else
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Represents a conditional color value converter.
    /// </summary>
    /// <example>This example demonstrates how define a declarative to select a <see cref="Color"/> object.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:More="clr-namespace:System.Windows.Data;assembly=System.Windows.More">
    ///
    /// <More:ConditionalColorConverter x:Key="ColorConverter" DefaultColor="White">
    ///   <More:ConditionalColorRule Color="Yellow">
    ///    <More:EqualToRule Value="{x:Null}" />
    ///   </More:ConditionalColorRule>
    ///   <More:ConditionalColorRule Color="Green">
    ///    <More:GreaterThanRule Value="0" />
    ///   </More:ConditionalColorRule>
    ///   <More:ConditionalColorRule Color="White">
    ///    <More:EqualToRule Value="0" />
    ///   </More:ConditionalColorRule>
    ///   <More:ConditionalColorRule Color="Red">
    ///    <More:LessThanRule Value="0" />
    ///   </More:ConditionalColorRule>
    /// </More:ConditionalColorConverter>
    ///
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="{Binding SomeNumber, StringFormat='\{0:N2\}'}"
    ///             Foreground="{Binding SomeNumber, Converter={StaticResource ColorConverter}}" />
    /// </Grid>
    ///
    /// </UserControl>
    /// ]]></code></example>
#if UAP10_0
    [ContentProperty( Name = nameof( Rules ) )]
#else
    [ContentProperty( nameof( Rules ) )]
#endif
    public class ConditionalColorConverter : IValueConverter
    {
        readonly Lazy<ObservableCollection<ConditionalColorRule>> rules = new Lazy<ObservableCollection<ConditionalColorRule>>( () => new ObservableCollection<ConditionalColorRule>() );

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalColorConverter"/> class.
        /// </summary>
        public ConditionalColorConverter() => DefaultColor = Colors.Black;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalColorConverter"/> class.
        /// </summary>
        /// <param name="defaultColor">The default <see cref="Color">color</see> for the converter.</param>
        public ConditionalColorConverter( Color defaultColor ) => DefaultColor = defaultColor;

        /// <summary>
        /// Gets or sets the default color associated with the converter.
        /// </summary>
        /// <value>A <see cref="Color"/> structure.  The default value is <see cref="Colors.Black">black</see>.</value>
        public Color DefaultColor { get; set; }

        /// <summary>
        /// Gets a collection of rules for the converter.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> object.</value>
        public virtual Collection<ConditionalColorRule> Rules
        {
            get
            {
                Contract.Ensures( Contract.Result<Collection<ConditionalColorRule>>() != null );
                return rules.Value;
            }
        }

        static object ConvertToObject( Color color, Type targetType )
        {
            Contract.Requires( targetType != null );
            Contract.Ensures( Contract.Result<object>() != null );

            if ( typeof( Color ).Equals( targetType ) )
            {
                return color;
            }

            return new SolidColorBrush( color );
        }

#if UAP10_0
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = typeof( object ).Equals( targetType ) || typeof( Color ).Equals( targetType ) || typeof( Brush ).Equals( targetType );

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }

            if ( value != null && value.GetType().Equals( targetType ) )
            {
                return value;
            }
#if UAP10_0
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var item = value == null ? (decimal?) null : new decimal?( System.Convert.ToDecimal( value, culture ) );

            foreach ( var rule in Rules )
            {
                if ( rule.Evaluate( item ) )
                {
                    return ConvertToObject( rule.Color, targetType );
                }
            }

            return ConvertToObject( DefaultColor, targetType );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "All conversions performed by this type are one-way." )]
#if UAP10_0
        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, string language ) =>
#else
        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) =>
#endif
            throw new NotSupportedException( ExceptionMessage.ConvertBackUnsupported );
    }
}