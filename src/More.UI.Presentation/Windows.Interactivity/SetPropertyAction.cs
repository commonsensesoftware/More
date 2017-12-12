namespace More.Windows.Interactivity
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Interactivity;
#if UAP10_0
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// Represents an action that assigns a value to the property of a bound object.
    /// </summary>
    /// <example>This example demonstrates how to define an action to set an object property.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:Interactivity="http://schemas.microsoft.com/expression/2010/interactivity"
    ///  xmlns:More="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.More">
    ///
    /// <Grid x:Name="LayoutRoot">
    ///  <TextBlock Text="Mouse over me to show a popup.">
    ///   <Interactivity:Interaction.Triggers>
    ///    <Interactivity:EventTrigger EventName="MouseEnter">
    ///     <More:SetPropertyAction Target="{Binding ElementName=MessageBox}" PropertyName="IsOpen" PropertyValue="True" />
    ///    </Interactivity:EventTrigger>
    ///    <Interactivity:EventTrigger EventName="MouseLeave">
    ///     <More:SetPropertyAction Target="{Binding ElementName=MessageBox}" PropertyName="IsOpen" PropertyValue="False" />
    ///    </Interactivity:EventTrigger>
    ///   </Interactivity:Interaction.Triggers>
    ///  </TextBlock>
    ///  <Popup x:Name="MessageBox" IsOpen="False" HorizontalOffset="20" VerticalOffset="20">
    ///   <TextBlock Text="Hello world!" />
    ///  </Popup>
    /// </Grid>
    ///
    /// </UserControl>
    /// ]]></code></example>
    public partial class SetPropertyAction
    {
        /// <summary>
        /// Gets the target object dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register( nameof( TargetObject ), typeof( object ), typeof( SetPropertyAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the property name dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register( nameof( PropertyName ), typeof( string ), typeof( SetPropertyAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the property value dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty PropertyValueProperty =
            DependencyProperty.Register( nameof( PropertyValue ), typeof( object ), typeof( SetPropertyAction ), new PropertyMetadata( (object) null ) );

        Type targetObjectType;
        PropertyDescriptor targetProperty;

        /// <summary>
        /// Gets or sets the object representing the target to set the property for.
        /// </summary>
        /// <value>The target <see cref="object">object</see>.</value>
        public object TargetObject
        {
            get => GetValue( TargetObjectProperty );
            set => SetValue( TargetObjectProperty, value );
        }

        /// <summary>
        /// Gets or sets the name of the property on the target to set.
        /// </summary>
        /// <value>The name of the property on the target to set.</value>
        public string PropertyName
        {
            get => (string) GetValue( PropertyNameProperty );
            set => SetValue( PropertyNameProperty, value );
        }

        /// <summary>
        /// Gets or sets the value to assign to the property.
        /// </summary>
        /// <value>The property value.</value>
        public object PropertyValue
        {
            get => GetValue( PropertyValueProperty );
            set => SetValue( PropertyValueProperty, value );
        }
    }
}