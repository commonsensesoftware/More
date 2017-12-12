namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
#if UAP10_0
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Markup;
#else
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Represents a value conversion sequence, which is a composite of other value converters.
    /// </summary>
    /// <example>This example demonstrates how to create a value converter that represents a sequence of multiple conversions, which
    /// converts a <see cref="string"/> to a <see cref="bool"/> and a <see cref="bool"/> to <see cref="Visibility"/>.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:More="clr-namespace:System.Windows.Data;assembly=System.Windows.More">
    ///
    /// <More:ValueConversionSequence x:Key="TextToVisibilityConverter">
    ///  <More:ValueConversionStep TargetType="System.Boolean">
    ///   <More:ValueConversionStep.Converter>
    ///    <More:IsNullConverter Negate="True" />
    ///   </More:ValueConversionStep.Converter>
    ///  </More:ValueConversionStep>
    ///  <More:ValueConversionStep TargetType="System.Windows.Visibility">
    ///   <More:ValueConversionStep.Converter>
    ///    <More:VisibilityConverter />
    ///   </More:ValueConversionStep.Converter>
    ///  </More:ValueConversionStep>
    /// </More:ValueConversionSequence>
    ///
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="{Binding Name}" Visibility="{Binding Name, Converter={StaticResource TextToVisibilityConverter}}" />
    /// </Grid>
    ///
    /// </UserControl>
    /// ]]></code></example>
#if UAP10_0
    [CLSCompliant( false )]
    [ContentProperty( Name = nameof( Steps ) )]
#else
    [ContentProperty( nameof( Steps ) )]
#endif
    public class ValueConversionSequence : IValueConverter
    {
        readonly ObservableCollection<ValueConversionStep> steps = new ObservableCollection<ValueConversionStep>();

        /// <summary>
        /// Gets a collection of conversion steps executed during the conversion sequence.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> object.</value>
        public Collection<ValueConversionStep> Steps
        {
            get
            {
                Contract.Ensures( steps != null );
                return steps;
            }
        }

#if UAP10_0
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx_core"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <include file='IValueConverter.xml' path='//member[@name="Convert" and @platform="netfx"]/*'/>
        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            if ( !Steps.Any() )
            {
                return value;
            }

            // the target type must match the type defined for the final converter
            if ( !Equals( Steps.Last().TargetType, targetType ) )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }

            foreach ( var step in Steps )
            {
#if UAP10_0
                value = step.Converter.Convert( value, step.TargetType, step.ConverterParameter, language );
#else
                value = step.Converter.Convert( value, step.TargetType, step.ConverterParameter, culture );
#endif
            }

            return value;
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
            if ( !Steps.Any() )
            {
                return value;
            }

            // the target type must match the type defined for the final converter
            if ( !Equals( Steps.First().TargetType, targetType ) )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }

            foreach ( var step in Steps.Reverse() )
            {
#if UAP10_0
                value = step.Converter.Convert( value, step.TargetType, step.ConverterParameter, language );
#else
                value = step.Converter.Convert( value, step.TargetType, step.ConverterParameter, culture );
#endif
            }

            return value;
        }
    }
}