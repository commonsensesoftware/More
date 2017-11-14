namespace More.VisualStudio
{
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Diagnostics.Contracts;
    using static Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON;
    using static Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON;
    using static Microsoft.VisualStudio.Shell.Interop.OLEMSGICON;
    using static System.Runtime.InteropServices.Marshal;

    /// <summary>
    /// Provides extension methods for the <see cref="IVsUIShell"/> interface.
    /// </summary>
    [CLSCompliant( false )]
    public static class IVsUIShellExtensions
    {
        internal static void ShowMessageBox( this IVsUIShell shell, string title, string text, OLEMSGICON icon )
        {
            Contract.Requires( shell != null );
            Contract.Requires( !string.IsNullOrEmpty( title ) );
            Contract.Requires( !string.IsNullOrEmpty( text ) );

            var clsid = Guid.Empty;
            var hr = shell.ShowMessageBox( 0U, ref clsid, title, text, string.Empty, 0U, OLEMSGBUTTON_OK, OLEMSGDEFBUTTON_FIRST, icon, 0, out var result );

            ThrowExceptionForHR( hr );
        }

        /// <summary>
        /// Shows a message box using the specified title and text.
        /// </summary>
        /// <param name="shell">The extended <see cref="IVsUIShell">shell</see> object.</param>
        /// <param name="title">The message box title.</param>
        /// <param name="text">The message box text.</param>
        public static void Show( this IVsUIShell shell, string title, string text )
        {
            Arg.NotNull( shell, nameof( shell ) );
            Arg.NotNullOrEmpty( title, nameof( title ) );
            Arg.NotNullOrEmpty( text, nameof( text ) );
            shell.ShowMessageBox( title, text, OLEMSGICON_NOICON );
        }

        /// <summary>
        /// Shows an informational message box using the specified title and text.
        /// </summary>
        /// <param name="shell">The extended <see cref="IVsUIShell">shell</see> object.</param>
        /// <param name="title">The message box title.</param>
        /// <param name="text">The message box text.</param>
        public static void ShowInformation( this IVsUIShell shell, string title, string text )
        {
            Arg.NotNull( shell, nameof( shell ) );
            Arg.NotNullOrEmpty( title, nameof( title ) );
            Arg.NotNullOrEmpty( text, nameof( text ) );
            shell.ShowMessageBox( title, text, OLEMSGICON_INFO );
        }

        /// <summary>
        /// Shows an error message box using the specified title and text.
        /// </summary>
        /// <param name="shell">The extended <see cref="IVsUIShell">shell</see> object.</param>
        /// <param name="title">The message box title.</param>
        /// <param name="text">The message box text.</param>
        public static void ShowError( this IVsUIShell shell, string title, string text )
        {
            Arg.NotNull( shell, nameof( shell ) );
            Arg.NotNullOrEmpty( title, nameof( title ) );
            Arg.NotNullOrEmpty( text, nameof( text ) );
            shell.ShowMessageBox( title, text, OLEMSGICON_CRITICAL );
        }
    }
}