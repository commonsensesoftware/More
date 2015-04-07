namespace More.Windows.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
#if NETFX_CORE
    using global::Windows.UI;
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Markup;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Represents a conditional brush value converter.
    /// </summary>
    /// <example>This example demonstrates how to define declarative rules to select a <see cref="Brush"/> object.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:More="clr-namespace:System.Windows.Data;assembly=System.Windows.More">
    ///  
    /// <More:ConditionalBrushConverter x:Key="BrushConverter" DefaultBrush="White">
    ///   <More:ConditionalBrushRule Brush="Yellow">
    ///    <More:EqualToRule Value="{x:Null}" />
    ///   </More:ConditionalBrushRule>
    ///   <More:ConditionalBrushRule Brush="Green">
    ///    <More:GreaterThanRule Value="0" />
    ///   </More:ConditionalBrushRule>
    ///   <More:ConditionalBrushRule Brush="White">
    ///    <More:EqualToRule Value="0" />
    ///   </More:ConditionalBrushRule>
    ///   <More:ConditionalBrushRule Brush="Red">
    ///    <More:LessThanRule Value="0" />
    ///   </More:ConditionalBrushRule>
    /// </More:ConditionalBrushConverter>
    /// 
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="{Binding SomeNumber, StringFormat='\{0:N2\}'}"
    ///             Foreground="{Binding SomeNumber, Converter={StaticResource BrushConverter}}" />
    /// </Grid>
    /// 
    /// </UserControl>
    /// ]]></code></example>
#if NETFX_CORE
    [CLSCompliant( false )]
    [ContentProperty( Name = "Rules" )]
#else
    [ContentProperty( "Rules" )]
#endif
    public class ConditionalBrushConverter : IValueConverter
    {
        private readonly Lazy<ObservableCollection<ConditionalBrushRule>> rules = new Lazy<ObservableCollection<ConditionalBrushRule>>( () => new ObservableCollection<ConditionalBrushRule>() );
        private Brush brush;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalBrushConverter"/> class.
        /// </summary>
        public ConditionalBrushConverter()
        {
            this.brush = new SolidColorBrush( Colors.Gray );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalBrushConverter"/> class.
        /// </summary>
        /// <param name="defaultBrush">The default <see cref="Brush">brush</see> for the converter.</param>
        public ConditionalBrushConverter( Brush defaultBrush )
        {
            Contract.Requires<ArgumentNullException>( defaultBrush != null, "defaultBrush" );
            this.brush = defaultBrush;
        }

        /// <summary>
        /// Gets or sets the default brush associated with the converter.
        /// </summary>
        /// <value>A <see cref="Brush"/> object.  The default value is <see cref="P:Brushes.Gray"/>.</value>
        public Brush DefaultBrush
        {
            get
            {
                Contract.Ensures( Contract.Result<Brush>() != null );
                return this.brush;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.brush = value;
            }
        }

        /// <summary>
        /// Gets a collection of rules for the converter.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> object.</value>
        public virtual Collection<ConditionalBrushRule> Rules
        {
            get
            {
                Contract.Ensures( Contract.Result<Collection<ConditionalBrushRule>>() != null );
                return this.rules.Value;
            }
        }

#if NETFX_CORE
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = typeof( object ).Equals( targetType ) || typeof( Brush ).Equals( targetType );

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );

            if ( value != null && value.GetType().Equals( targetType ) )
                return value;

#if NETFX_CORE
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var item = value == null ? (decimal?) null : new decimal?( System.Convert.ToDecimal( value, culture ) );

            // select matching color from rule set
            foreach ( var rule in this.Rules )
            {
                if ( rule.Evaluate( item ) )
                    return rule.Brush;
            }

            // if no rules are satisfied, use the default color
            return this.DefaultBrush;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "All conversions performed by this type are one-way." )]
#if NETFX_CORE
        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, string language )
        {
            throw new NotSupportedException( ExceptionMessage.ConvertBackUnsupported );
        }
#else
        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotSupportedException( ExceptionMessage.ConvertBackUnsupported );
        }
#endif
    }
}
