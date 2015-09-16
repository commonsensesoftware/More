namespace More.Windows.Interactivity
{
    using More.Windows.Controls;
    using System;
    using global::Windows.Phone.UI.Input;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Navigation;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class NavigationBehavior
    {
        private void OnBackPressed( object sender, BackPressedEventArgs e )
        {
            if ( !navigationService.CanGoBack )
                return;

            navigationService.GoBack();
            e.Handled = true;
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.NavigationCacheMode = NavigationCacheMode.Required;
            navigationService = new FrameNavigationAdapter( AssociatedObject.Frame );
            HardwareButtons.BackPressed += OnBackPressed;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            HardwareButtons.BackPressed -= OnBackPressed;
            navigationService = null;
            base.OnDetaching();
        }
    }
}
