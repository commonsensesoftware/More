namespace More.Windows.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts; 
    using System.Globalization;
    using System.Linq;
#if NETFX_CORE
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Markup;
#else
    using System.Windows.Data;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Represents a value converter for <see cref="Number"/> values.
    /// </summary>
    /// <example>This example demonstrates how to define declarative rules to select how a <see cref="Number"/> should be formatted
    /// according to its <see cref="P:Number.NumberStyle"/> property.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:More="clr-namespace:System.Windows.Data;assembly=System.Windows.More">
    ///  
    /// <More:NumberConverter x:Key="NumberConverter">
    ///   <More:NumberStyleRule NumberStyle="Integer" Format="N0" />
    ///   <More:NumberStyleRule NumberStyle="Percent" Format="#,##0.##%;(#,##0.##%)" />
    ///   <More:NumberStyleRule NumberStyle="Currency">
    ///     <More.NumberStyleRule.ValueConverter>
    ///         <More:NumericDenominationConverter DefaultFormat="0.##M;(0.##)M" DefaultDenomination="1000000" />
    ///     </More.NumberStyleRule.ValueConverter>
    ///   </More:NumberStyleRule>
    /// </More:NumberConverter>
    /// 
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="{Binding SomeNumber, Converter={StaticResource NumberConverter}}" />
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
    public class NumberConverter : IValueConverter
    {
        private readonly Lazy<ObservableCollection<NumberStyleRule>> rules = new Lazy<ObservableCollection<NumberStyleRule>>( () => new ObservableCollection<NumberStyleRule>() );

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberConverter"/> class.
        /// </summary>
        public NumberConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberConverter"/> class.
        /// </summary>
        /// <param name="defaultFormat">The default format for the converter.</param>
        public NumberConverter( string defaultFormat )
        {
            this.DefaultFormat = defaultFormat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberConverter"/> class.
        /// </summary>
        /// <param name="defaultConverter">The default <see cref="IValueConverter">value converter</see> for the new instance.</param>
        public NumberConverter( IValueConverter defaultConverter )
        {
            this.DefaultValueConverter = defaultConverter;
        }

        /// <summary>
        /// Gets or sets the default string format associated with the number style.
        /// </summary>
        /// <value>A <see cref="String">string</see> representing the number format to apply.  This property can be null.</value>
        public string DefaultFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default value converter associated with the number style.
        /// </summary>
        /// <value>An <see cref="IValueConverter"/> used to format the number. This property can be null.</value>
        public IValueConverter DefaultValueConverter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of rules for the converter.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> object.</value>
        public virtual Collection<NumberStyleRule> Rules
        {
            get
            {
                Contract.Ensures( Contract.Result<Collection<NumberStyleRule>>() != null );
                return this.rules.Value;
            }
        }

#if NETFX_CORE
        private string FormatNumber( NumberStyleRule rule, Number? number, string language )
#else
        private string FormatNumber( NumberStyleRule rule, Number? number, CultureInfo culture )
#endif
        {
            string format;
            IValueConverter converter;

            // select format and value converter
            if ( rule == null )
            {
                format = this.DefaultFormat;
                converter = this.DefaultValueConverter;
            }
            else
            {
                format = rule.Format;
                converter = rule.ValueConverter;
            }

            if ( number == null )
            {
                // give the value converter an opportunity to handle null
                if ( converter != null )
                {
#if NETFX_CORE
                    return (string) converter.Convert( null, typeof( string ), null, language );
#else
                    return (string) converter.Convert( null, typeof( string ), null, culture );
#endif
                }

                return null;
            }

            // use converter first, then format
            if ( converter != null )
            {
#if NETFX_CORE
                return (string) converter.Convert( number.Value, typeof( string ), null, language );
#else
                return (string) converter.Convert( number.Value, typeof( string ), null, culture );
#endif
            }

#if NETFX_CORE
            var culture = Util.GetCultureFromLanguage( language );
#endif
            if ( !string.IsNullOrEmpty( format ) )
                return number.Value.ToString( format, culture );

            return number.Value.ToString( culture );
        }

#if NETFX_CORE
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = typeof( object ).Equals( targetType ) || typeof( string ).Equals( targetType ) || typeof( Number ).Equals( targetType );

            if ( !supported )
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), "targetType" );

            if ( value != null )
            {
                if ( !( value is Number ) )
                    throw new ArgumentException( ExceptionMessage.UnsupportedValueType.FormatDefault( value.GetType() ), "value" );

                if ( value.GetType().Equals( targetType ) )
                    return value;
            }

            // select matching rule from rule set
            var number = value == null ? (Number?) null : new Number?( (Number) value );
            var formatRule = this.Rules.FirstOrDefault( r => r.Evaluate( number ) );

            // format number according to rule
#if NETFX_CORE
            return this.FormatNumber( formatRule, number, language );
#else
            return this.FormatNumber( formatRule, number, culture );
#endif
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
