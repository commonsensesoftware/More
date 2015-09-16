namespace More.Windows.Interactivity
{
    using More.Windows.Controls;
    using System;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <summary>
    /// 
    /// </summary>
    /// <example>
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
        private INavigationService navigationService;

        /// <summary>
        /// Gets the navigation service associated with behavior.
        /// </summary>
        /// <value>The <see cref="INavigationService">navigation service</see> associated with the behavior.</value>
        /// <remarks>The <see cref="INavigationService">navigation service</see> is not available in the default implementation
        /// before <see cref="OnAttached"/> or after <see cref="OnDetaching"/>.</remarks>

        protected virtual INavigationService NavigationService
        {
            get
            {
                return navigationService;
            }
        }
    }
}
