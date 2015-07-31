namespace More.Windows.Interactivity
{
    using More.Windows.Controls;
    using System;
    using global::Windows.Phone.UI.Input;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Represents a behavior to navigate backward when the "Back" hardware button is pressed.
    /// </summary>
    /// <remarks>If the backward navigation cannot be performed, then the application continues with the default hardware behavior.</remarks>
    /// <example>This example demonstrates automatically handle backward navigation when the "Back" hardware button is pressed.
    /// <code lang="Xaml"><![CDATA[
    /// <Page
    ///  x:Class="Page1"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:Interactivity="using:Microsoft.Xaml.Interactivity"
    ///  xmlns:More="using:More.Windows.Interactivity">
    /// <Interactivity:Interaction.Behaviors>
    ///  <More:BackBehavior />
    /// </Interactivity:Interaction.Behaviors>
    /// <Grid>
    ///  <TextBlock Text="Hello world!" />
    /// </Grid>
    /// </Page>
    /// ]]></code></example>
    [CLSCompliant( false )]
    public class BackBehavior : System.Windows.Interactivity.Behavior<DependencyObject>
    {
        private void OnBackPressed( object sender, BackPressedEventArgs e )
        {
            INavigationService navigationService;

            if ( Util.TryResolveNavigationService( AssociatedObject, out navigationService ) && navigationService.CanGoBack )
            {
                navigationService.GoBack();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            HardwareButtons.BackPressed += OnBackPressed;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            HardwareButtons.BackPressed -= OnBackPressed;
            base.OnDetaching();
        }
    }
}
