namespace More.Windows.Interactivity
{
    using Controls;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.System;
    using global::Windows.UI.Xaml.Navigation;
    using static global::Windows.UI.Core.CoreAcceleratorKeyEventType;
    using static global::Windows.System.VirtualKey;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class NavigationBehavior
    {
        private void OnAcceleratorKeyActivated( CoreDispatcher sender, AcceleratorKeyEventArgs args )
        {
            Contract.Requires( args != null );

            // only investigate further when Left, Right, or the dedicated Previous or Next keys are pressed
            var virtualKey = args.VirtualKey;
            var shouldHandle = ( args.EventType == SystemKeyDown || args.EventType == KeyDown ) && ( virtualKey == Left || virtualKey == Right || (int) virtualKey == 166 || (int) virtualKey == 167 );

            if ( !shouldHandle )
                return;

            var window = Window.Current.CoreWindow;
            var down = CoreVirtualKeyStates.Down;
            var menuKey = ( window.GetKeyState( Menu ) & down ) == down;
            var controlKey = ( window.GetKeyState( Control ) & down ) == down;
            var shiftKey = ( window.GetKeyState( Shift ) & down ) == down;
            var noModifiers = !menuKey && !controlKey && !shiftKey;
            var onlyAlt = menuKey && !controlKey && !shiftKey;

            if ( ( ( (int) virtualKey == 166 && noModifiers ) || ( virtualKey == Left && onlyAlt ) ) && NavigationService.CanGoBack )
            {
                // navigate back when the previous key or Alt+Left are pressed
                NavigationService.GoBack();
                args.Handled = true;
            }
            else if ( ( ( (int) virtualKey == 167 && noModifiers ) || ( virtualKey == Right && onlyAlt ) ) && NavigationService.CanGoBack )
            {
                // navigate forward when the next key or Alt+Right are pressed
                NavigationService.GoForward();
                args.Handled = true;
            }
        }

        private void OnPointerPressed( CoreWindow sender, PointerEventArgs args )
        {
            Contract.Requires( args != null );

            var properties = args.CurrentPoint.Properties;

            // ignore button chords with the Left, Right, and Middle buttons
            if ( properties.IsLeftButtonPressed || properties.IsRightButtonPressed || properties.IsMiddleButtonPressed )
                return;

            // if Back or Forward are pressed (but not both) navigate appropriately
            var backPressed = properties.IsXButton1Pressed;
            var forwardPressed = properties.IsXButton2Pressed;
            var mutuallyExclusive = backPressed ^ forwardPressed;

            if ( !mutuallyExclusive )
                return;

            if ( backPressed && NavigationService.CanGoBack )
            {
                NavigationService.GoBack();
                args.Handled = true;
            }
            else if ( forwardPressed && NavigationService.CanGoForward )
            {
                NavigationService.GoForward();
                args.Handled = true;
            }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            var page = AssociatedObject;

            page.NavigationCacheMode = NavigationCacheMode.Required;
            navigationService = new FrameNavigationAdapter( AssociatedObject.Frame );
            
            var window = Window.Current;
            var bounds = window.Bounds;

            // keyboard and mouse navigation only apply when occupying the entire window
            if ( page.ActualHeight != bounds.Height || page.ActualWidth != bounds.Width )
                return;

            var coreWindow = window.CoreWindow;
            coreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
            coreWindow.PointerPressed += OnPointerPressed;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            var window = Window.Current.CoreWindow;
            window.Dispatcher.AcceleratorKeyActivated -= OnAcceleratorKeyActivated;
            window.PointerPressed -= OnPointerPressed;
            navigationService = null;
            base.OnDetaching();
        }
    }
}
