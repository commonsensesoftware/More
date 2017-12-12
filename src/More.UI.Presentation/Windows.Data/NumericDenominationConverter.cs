namespace More.Windows.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
#if UAP10_0
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Markup;
#else
    using System.Windows.Data;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Represents a numeric denomination value converter.
    /// </summary>
    /// <example>This example demonstrates how to format numbers using a single denomination.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:More="clr-namespace:System.Windows.Data;assembly=System.Windows.More">
    ///
    /// <More:NumericDenominationConverter x:Key="NumberConverter" DefaultFormat="0.##M;(0.##)M" DefaultDenomination="1000000" />
    ///
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="{Binding SomeNumber, Converter={StaticResource NumberConverter}}" />
    /// </Grid>
    ///
    /// </UserControl>
    /// ]]></code></example>
    /// <example>This example demonstrates how to define declarative rules to define the conditions and formats for different numeric denominations.
    /// This example formats cents, hundreds, thousands, millions, and billions.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:More="clr-namespace:System.Windows.Data;assembly=System.Windows.More">
    ///
    /// <More:NumericDenominationConverter x:Key="NumberConverter" DefaultFormat="C">
    ///  <More:DenominationConvertRule Denomination="1000000000" Format="$0.## b;($0.##) b">
    ///   <More:GreaterThanRule Value="1000000000">
    ///    <More:EqualToRule Value="1000000000">
    ///     <More:LessThanRule Value="-1000000000">
    ///      <More:EqualToRule Value="-1000000000" />
    ///     </More:LessThanRule>
    ///    </More:EqualToRule>
    ///   </More:GreaterThanRule>
    ///  </More:DenominationConvertRule>
    ///  <More:DenominationConvertRule Denomination="1000000" Format="$0.## m;($0.##) m">
    ///   <More:GreaterThanRule Value="1000000">
    ///    <More:EqualToRule Value="1000000">
    ///     <More:LessThanRule Value="-1000000">
    ///      <More:EqualToRule Value="-1000000" />
    ///     </More:LessThanRule>
    ///    </More:EqualToRule>
    ///   </More:GreaterThanRule>
    ///  </More:DenominationConvertRule>
    ///  <More:DenominationConvertRule Denomination="1000" Format="$0.## k;($0.##) k">
    ///   <More:GreaterThanRule Value="1000">
    ///    <More:EqualToRule Value="1000">
    ///     <More:LessThanRule Value="-1000">
    ///      <More:EqualToRule Value="-1000" />
    ///     </More:LessThanRule>
    ///    </More:EqualToRule>
    ///   </More:GreaterThanRule>
    ///  </More:DenominationConvertRule>
    ///  <More:DenominationConvertRule Denomination="1" Format="C">
    ///   <More:EqualToRule Value="0" />
    ///  </More:DenominationConvertRule>
    ///  <More:DenominationConvertRule Denomination="1" Format="$0.0# ¢;($0.0#) ¢">
    ///   <More:LessThanRule Value="1" />
    ///   <More:GreaterThanRule Value="-1" />
    ///  </More:DenominationConvertRule>
    /// </More:NumericDenominationConverter>
    ///
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="{Binding SomeNumber, Converter={StaticResource NumberConverter}}" />
    /// </Grid>
    ///
    /// </UserControl>
    /// ]]></code></example>
#if UAP10_0
    [ContentProperty( Name = nameof( ConvertRules ) )]
#else
    [ContentProperty( nameof( ConvertRules ) )]
#endif
    public class NumericDenominationConverter : IValueConverter
    {
        readonly Lazy<ObservableCollection<DenominationConvertRule>> convertRules = new Lazy<ObservableCollection<DenominationConvertRule>>( () => new ObservableCollection<DenominationConvertRule>() );
        readonly Lazy<ObservableCollection<DenominationConvertBackRule>> convertBackRules = new Lazy<ObservableCollection<DenominationConvertBackRule>>( () => new ObservableCollection<DenominationConvertBackRule>() );
        decimal denomination;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericDenominationConverter"/> class.
        /// </summary>
        public NumericDenominationConverter() => denomination = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericDenominationConverter"/> class.
        /// </summary>
        /// <param name="defaultFormat">The default format for the converter.</param>
        /// <param name="defaultDenomination">The default denomination for the converter.</param>
        public NumericDenominationConverter( string defaultFormat, decimal defaultDenomination )
        {
            Arg.GreaterThan( defaultDenomination, 0m, nameof( defaultDenomination ) );
            DefaultFormat = defaultFormat;
            denomination = defaultDenomination;
        }

        /// <summary>
        /// Gets or sets the default number format.
        /// </summary>
        /// <value>The default number format.</value>
        public string DefaultFormat { get; set; }

        /// <summary>
        /// Gets or sets the default denomination associated with the converter.
        /// </summary>
        /// <value>The default denomination associated with the rule.  The default value is one.</value>
        public decimal DefaultDenomination
        {
            get
            {
                Contract.Ensures( Contract.Result<decimal>() > 0m );
                return denomination;
            }
            set
            {
                Arg.GreaterThan( value, 0m, nameof( value ) );
                denomination = value;
            }
        }

        /// <summary>
        /// Gets a collection of conversion rules for the converter.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> of <see cref="DenominationConvertRule">conversion rules</see>.</value>
        public virtual Collection<DenominationConvertRule> ConvertRules
        {
            get
            {
                Contract.Ensures( Contract.Result<Collection<DenominationConvertRule>>() != null );
                return convertRules.Value;
            }
        }

        /// <summary>
        /// Gets a collection of inverse conversion rules for the converter.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> of <see cref="DenominationConvertBackRule">inverse conversion rules</see>.</value>
        public virtual Collection<DenominationConvertBackRule> ConvertBackRules
        {
            get
            {
                Contract.Ensures( Contract.Result<Collection<DenominationConvertBackRule>>() != null );
                return convertBackRules.Value;
            }
        }

        static object CoerceValue( string value, decimal denomination, Type targetType, CultureInfo culture )
        {
            Contract.Requires( !string.IsNullOrEmpty( value ) );
            Contract.Requires( denomination > 0 );
            Contract.Requires( targetType != null );

            var text = Regex.Replace( value, @"[^\d\.]+", string.Empty, RegexOptions.Singleline );
            var result = System.Convert.ToDecimal( text, culture ) * denomination;
            return System.Convert.ChangeType( result, targetType, culture );
        }

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

            if ( value == null )
            {
                return value;
            }
            else if ( value.GetType().Equals( targetType ) )
            {
                return value;
            }

            // find rule for matching format and denomination
#if UAP10_0
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var result = System.Convert.ToDecimal( value, culture );
            var rule = ConvertRules.FirstOrDefault( r => r.Evaluate( result ) );
            var scale = DefaultDenomination;
            var format = DefaultFormat;

            if ( rule != null )
            {
                scale = rule.Denomination;
                format = rule.Format;
            }

            // apply denomination
            result /= scale;

            if ( string.IsNullOrEmpty( format ) )
            {
                return result.ToString( culture );
            }

            return result.ToString( format, culture );
        }

#pragma warning disable SA1606 // Element documentation should have summary text
#if UAP10_0
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx_core"]/*'/>
        public virtual object ConvertBack( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="ConvertBack" and @platform="netfx"]/*'/>
        public virtual object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
#endif
#pragma warning restore SA1606 // Element documentation should have summary text
        {
            // must have convert back rules if there are convert rules
            if ( ConvertRules.Any() && !ConvertBackRules.Any() )
            {
                throw new NotSupportedException( ExceptionMessage.ConvertBackUnsupported );
            }

            if ( value == null || value.GetType().Equals( targetType ) )
            {
                return value;
            }

            var item = value.ToString();
            var scale = DefaultDenomination;
#if UAP10_0
            var culture = Util.GetCultureFromLanguage( language );
#endif

            // use matching rule or failover to default converter
            if ( ConvertBackRules.Any() )
            {
                try
                {
                    // must match exactly one rule
                    var rule = ConvertBackRules.Single( r => r.Evaluate( item ) );
                    scale = rule.Denomination;
                }
                catch ( InvalidOperationException ex )
                {
                    throw new InvalidOperationException( string.Format( culture, ExceptionMessage.NoConvertBackRule, item ), ex );
                }
            }

            return CoerceValue( item, scale, targetType, culture );
        }
    }
}