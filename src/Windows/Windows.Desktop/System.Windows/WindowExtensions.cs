namespace System.Windows
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using System.Windows.Interop;

    /// <summary>
    /// Provides extension methods for the <see cref="Window"/> class.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Removes the window icon.
        /// </summary>
        /// <param name="window">The <see cref="Window">window</see> to remove the icon from.</param>
        public static void RemoveIcon( this Window window )
        {
            Contract.Requires<ArgumentNullException>( window != null, "window" );

            var helper = new WindowInteropHelper( window );
            var hwnd = helper.Handle;

            // change the extended window style to not show a window icon
            var extendedStyle = NativeMethods.GetWindowLong( hwnd, NativeMethods.ExtendedStyle );

            if ( NativeMethods.SetWindowLong( hwnd, NativeMethods.ExtendedStyle, extendedStyle | NativeMethods.DialogModalFrame ) == 0 )
            {
                var hresult = Marshal.GetLastWin32Error();
                Marshal.ThrowExceptionForHR( hresult );
            }

            // update the non-client area to reflect the changes
            var flags = NativeMethods.NoMove | NativeMethods.NoSize | NativeMethods.NoZOrder | NativeMethods.FrameChanged;
            if ( !NativeMethods.SetWindowPos( hwnd, IntPtr.Zero, 0, 0, 0, 0, flags ) )
            {
                var hresult = Marshal.GetLastWin32Error();
                Marshal.ThrowExceptionForHR( hresult );
            }
        }

        /// <summary>
        /// Gets the client size of the window.
        /// </summary>
        /// <param name="window">The <see cref="Window">window</see> to get the client size of.</param>
        /// <returns>The client area <see cref="Size">size</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This method only applies to windows." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Size GetClientSize( this Window window )
        {
            Contract.Requires<ArgumentNullException>( window != null, "window" );

            var width = window.ActualWidth - ( SystemParameters.ResizeFrameVerticalBorderWidth * 2d );
            var height = window.ActualHeight - ( ( SystemParameters.ResizeFrameHorizontalBorderHeight * 2d ) + SystemParameters.WindowCaptionHeight );

            return new Size( width, height );
        }

        /// <summary>
        /// Sets the enabled state of the window close button.
        /// </summary>
        /// <param name="window">The <see cref="Window">window</see> to set the close button state of.</param>
        /// <param name="value">Indicates whether the close button is enabled.</param>
        public static void SetCloseButtonEnabled( this Window window, bool value )
        {
            Contract.Requires<ArgumentNullException>( window != null, "window" );

            var helper = new WindowInteropHelper( window );
            var hwnd = helper.Handle;
            var menu = NativeMethods.GetSystemMenu( hwnd, false );

            if ( menu == IntPtr.Zero )
                return;

            var flags = value ? NativeMethods.EnabledFlag : NativeMethods.GrayedFlag;
            NativeMethods.EnableMenuItem( menu, NativeMethods.CloseButton, flags );
        }
    }
}
