namespace More.Windows.Interactivity
{
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using static global::Windows.System.VirtualKey;
    using static global::Windows.UI.Core.CoreAcceleratorKeyEventType;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class NavigationBehavior
    {
        void OnAcceleratorKeyActivated( CoreDispatcher sender, AcceleratorKeyEventArgs args )
        {
            Contract.Requires( args != null );

            // only investigate further when Left, Right, or the dedicated Previous or Next keys are pressed
            var virtualKey = args.VirtualKey;
            var shouldHandle = ( args.EventType == SystemKeyDown || args.EventType == KeyDown ) && ( virtualKey == Left || virtualKey == Right || (int) virtualKey == 166 || (int) virtualKey == 167 );

            if ( !shouldHandle )
            {
                return;
            }

            var window = Window.Current.CoreWindow;
            var down = CoreVirtualKeyStates.Down;
            var menuKey = ( window.GetKeyState( Menu ) & down ) == down;
            var controlKey = ( window.GetKeyState( Control ) & down ) == down;
            var shiftKey = ( window.GetKeyState( Shift ) & down ) == down;
            var noModifiers = !menuKey && !controlKey && !shiftKey;
            var onlyAlt = menuKey && !controlKey && !shiftKey;
            var frame = AssociatedObject.Frame;

            if ( ( ( (int) virtualKey == 166 && noModifiers ) || ( virtualKey == Left && onlyAlt ) ) && frame.CanGoBack )
            {
                // navigate back when the previous key or Alt+Left are pressed
                frame.GoBack();
                args.Handled = true;
            }
            else if ( ( ( (int) virtualKey == 167 && noModifiers ) || ( virtualKey == Right && onlyAlt ) ) && frame.CanGoBack )
            {
                // navigate forward when the next key or Alt+Right are pressed
                frame.GoForward();
                args.Handled = true;
            }
        }

        void OnPointerPressed( CoreWindow sender, PointerEventArgs args )
        {
            Contract.Requires( args != null );

            var properties = args.CurrentPoint.Properties;

            // ignore button chords with the Left, Right, and Middle buttons
            if ( properties.IsLeftButtonPressed || properties.IsRightButtonPressed || properties.IsMiddleButtonPressed )
            {
                return;
            }

            // if Back or Forward are pressed (but not both) navigate appropriately
            var backPressed = properties.IsXButton1Pressed;
            var forwardPressed = properties.IsXButton2Pressed;
            var mutuallyExclusive = backPressed ^ forwardPressed;

            if ( !mutuallyExclusive )
            {
                return;
            }

            var frame = AssociatedObject.Frame;

            if ( backPressed && frame.CanGoBack )
            {
                frame.GoBack();
                args.Handled = true;
            }
            else if ( forwardPressed && frame.CanGoForward )
            {
                frame.GoForward();
                args.Handled = true;
            }
        }

        /// <summary>
        /// Called after the behavior is attached to an <see cref="P:AssociatedObject"/>.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            var page = AssociatedObject;
            var window = Window.Current.CoreWindow;

            page.NavigationCacheMode = NavigationCacheMode;
            window.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
            window.PointerPressed += OnPointerPressed;
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="P:AssociatedObject"/>, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            var window = Window.Current.CoreWindow;

            window.Dispatcher.AcceleratorKeyActivated -= OnAcceleratorKeyActivated;
            window.PointerPressed -= OnPointerPressed;

            base.OnDetaching();
        }
    }
}