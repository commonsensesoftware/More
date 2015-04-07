namespace More.VisualStudio
{
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides extension methods for the <see cref="IVsUIShell"/> interface.
    /// </summary>
    public static class IVsUIShellExtensions
    {
        internal static void ShowMessageBox( this IVsUIShell shell, string title, string text, OLEMSGICON icon )
        {
            Contract.Requires( shell != null );
            Contract.Requires( !string.IsNullOrEmpty( title ) );
            Contract.Requires( !string.IsNullOrEmpty( text ) );

            var clsid = Guid.Empty;
            var button = OLEMSGBUTTON.OLEMSGBUTTON_OK;
            var defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
            int result;
            var hr = shell.ShowMessageBox( 0U, ref clsid, title, text, string.Empty, 0U, button, defaultButton, icon, 0, out result );

            Marshal.ThrowExceptionForHR( hr );
        }

        /// <summary>
        /// Shows a message box using the specified title and text.
        /// </summary>
        /// <param name="shell">The extended <see cref="IVsUIShell">shell</see> object.</param>
        /// <param name="title">The message box title.</param>
        /// <param name="text">The message box text.</param>
        public static void Show( this IVsUIShell shell, string title, string text )
        {
            Contract.Requires<ArgumentNullException>( shell != null, "shell" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( title ), "title" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( text ), "text" );
            shell.ShowMessageBox( title, text, OLEMSGICON.OLEMSGICON_NOICON );
        }

        /// <summary>
        /// Shows an informational message box using the specified title and text.
        /// </summary>
        /// <param name="shell">The extended <see cref="IVsUIShell">shell</see> object.</param>
        /// <param name="title">The message box title.</param>
        /// <param name="text">The message box text.</param>
        public static void ShowInformation( this IVsUIShell shell, string title, string text )
        {
            Contract.Requires<ArgumentNullException>( shell != null, "shell" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( title ), "title" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( text ), "text" );
            shell.ShowMessageBox( title, text, OLEMSGICON.OLEMSGICON_INFO );
        }

        /// <summary>
        /// Shows an error message box using the specified title and text.
        /// </summary>
        /// <param name="shell">The extended <see cref="IVsUIShell">shell</see> object.</param>
        /// <param name="title">The message box title.</param>
        /// <param name="text">The message box text.</param>
        public static void ShowError( this IVsUIShell shell, string title, string text )
        {
            Contract.Requires<ArgumentNullException>( shell != null, "shell" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( title ), "title" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( text ), "text" );
            shell.ShowMessageBox( title, text, OLEMSGICON.OLEMSGICON_CRITICAL );
        }
    }
}
