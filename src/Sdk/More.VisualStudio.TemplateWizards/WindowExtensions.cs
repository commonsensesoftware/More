﻿namespace More.VisualStudio
{
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Interop;
    using static System.IntPtr;
    using static System.Windows.Interop.ComponentDispatcher;
    using static System.Windows.WindowStartupLocation;
    using Window = System.Windows.Window;

    /// <summary>
    /// Provides extension methods for the <see cref="Window"/> class.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Closes a window with the specified dialog result.
        /// </summary>
        /// <param name="window">The extended <see cref="Window">window</see>.</param>
        /// <param name="dialogResult">The dialog result indicating whether the user accepted, rejected,
        /// or provided no response to the dialog.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void Close( this Window window, bool? dialogResult )
        {
            Arg.NotNull( window, nameof( window ) );

            try
            {
                if ( IsThreadModal )
                {
                    window.DialogResult = dialogResult;
                }
                else
                {
                    window.Close();
                }
            }
            catch ( InvalidOperationException )
            {
                window.Close();
            }
        }

        /// <summary>
        /// Shows the specified window using the provided reference to the Visual Studio shell.
        /// </summary>
        /// <param name="window">The <see cref="Window">window</see> to show.</param>
        /// <param name="shell">The <see cref="IVsUIShell">Visual Studio shell</see> to show the modal dialog for.</param>
        /// <returns>The dialog result indicating whether the user accepted, rejected, or provided no response to the dialog.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsUIShell.EnableModeless(System.Int32)", Justification = "Nothing to do if modal mode cannot be exited." )]
        public static bool? ShowDialog( this Window window, IVsUIShell shell )
        {
            Arg.NotNull( window, nameof( window ) );

            // if the shell doesn't retrieve the dialog owner or doesn't enter modal mode, just let the dialog do it's normal thing
            if ( shell == null || shell.GetDialogOwnerHwnd( out var owner ) != 0 || shell.EnableModeless( 0 ) != 0 )
            {
                return window.ShowDialog();
            }

            window.WindowStartupLocation = CenterOwner;

            var helper = new WindowInteropHelper( window ) { Owner = owner };

            try
            {
                return window.ShowDialog();
            }
            finally
            {
                shell.EnableModeless( 1 );
                helper.Owner = Zero;
            }
        }
    }
}