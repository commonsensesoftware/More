namespace System.Windows
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using System.Windows.Interop;

    /// <summary>
    /// Provides helper methods for the <see cref="Window"/> class.
    /// </summary>
    static class NativeMethods
    {
        internal const int ExtendedStyle = -20;
        internal const int DialogModalFrame = 0x0001;
        internal const uint NoSize = 0x0001U;
        internal const uint NoMove = 0x0002U;
        internal const uint NoZOrder = 0x0004U;
        internal const uint FrameChanged = 0x0020U;
        internal const uint EnabledFlag = 0x0U;
        internal const uint GrayedFlag = 0x01U;
        internal const uint CloseButton = 0xF060U;

        [DllImport( "user32.dll" )]
        internal static extern int GetWindowLong( IntPtr hwnd, int index );

        [DllImport( "user32.dll", SetLastError = true )]
        internal static extern int SetWindowLong( IntPtr hwnd, int index, int newStyle );

        [DllImport( "user32.dll", SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        internal static extern bool SetWindowPos( IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags );

        [DllImport( "user32.dll" )]
        internal static extern IntPtr GetSystemMenu( IntPtr hwnd, [MarshalAs( UnmanagedType.Bool )] bool revert );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        internal static extern bool EnableMenuItem( IntPtr menuHandle, uint menuItemID, uint enableFlags );
    }
}