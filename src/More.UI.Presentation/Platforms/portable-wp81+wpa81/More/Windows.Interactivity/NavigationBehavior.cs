namespace More.Windows.Interactivity
{
    using System;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a behavior which supports navigation operations
    /// </summary>
    /// <remarks>Navigation support also includes when related hardware buttons are pressed.</remarks>
    /// <example>This example demonstrates automatically handling navigation.
    /// <code lang="Xaml"><![CDATA[
    /// <Page
    ///  x:Class="Page1"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:Interactivity="using:Microsoft.Xaml.Interactivity"
    ///  xmlns:More="using:More.Windows.Interactivity">
    /// <Interactivity:Interaction.Behaviors>
    ///  <More:NavigationBehavior />
    /// </Interactivity:Interaction.Behaviors>
    /// <Grid>
    ///  <TextBlock Text="Hello world!" />
    /// </Grid>
    /// </Page>
    /// ]]></code></example>
    [CLSCompliant( false )]
    public partial class NavigationBehavior : System.Windows.Interactivity.Behavior<Page>
    {
        /// <summary>
        /// Gets the navigation cache mode dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty NavigationCacheModeProperty =
            DependencyProperty.Register( "NavigationCacheMode", typeof( NavigationCacheMode ), typeof( NavigationBehavior ), new PropertyMetadata( NavigationCacheMode.Required ) );

        /// <summary>
        /// Gets or sets the navigation cache mode for pages with the applied behavior.
        /// </summary>
        /// <value>One of the <see cref="T:NavigationCacheMode"/> values. The default value is <see cref="NavigationCacheMode.Required"/>.</value>
        public NavigationCacheMode NavigationCacheMode
        {
            get => (NavigationCacheMode) GetValue( NavigationCacheModeProperty );
            set => SetValue( NavigationCacheModeProperty, value );
        }
    }
}