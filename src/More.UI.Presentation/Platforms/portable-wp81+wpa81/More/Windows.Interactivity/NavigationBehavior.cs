namespace More.Windows.Interactivity
{
    using global::Windows.Phone.UI.Input;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class NavigationBehavior
    {
        void OnBackPressed( object sender, BackPressedEventArgs e )
        {
            var frame = AssociatedObject.Frame;

            if ( !frame.CanGoBack )
            {
                return;
            }

            frame.GoBack();
            e.Handled = true;
        }

        /// <summary>
        /// Called after the behavior is attached to an <see cref="P:AssociatedObject"/>.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.NavigationCacheMode = NavigationCacheMode;
            HardwareButtons.BackPressed += OnBackPressed;
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="P:AssociatedObject"/>, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            HardwareButtons.BackPressed -= OnBackPressed;
            base.OnDetaching();
        }
    }
}