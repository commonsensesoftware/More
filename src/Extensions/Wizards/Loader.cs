namespace More.VisualStudio
{
    using Microsoft.VisualStudio.Shell.Interop;
    using More.VisualStudio.Views;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Represents a loader utility to from an asynchronous task while displaying feedback to the user.
    /// </summary>
    internal static class Loader
    {
        internal static Task<TResult> LoadAsync<TResult>( Func<Window, IProgress<Window>, Task<TResult>> load, Window owner, string status )
        {
            Contract.Requires( load != null );
            Contract.Ensures( Contract.Result<Task<TResult>>() != null );

            var startupLocation = owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
            var window = new ProgressWindow( status )
            {
                Owner = owner,
                WindowStartupLocation = startupLocation
            };
            var progress = new Progress<Window>( w => w.Close() );
            var task = load( window, progress );

            window.ShowDialog();

            return task;
        }

        internal static Task<TResult> LoadAsync<TArg, TResult>( Func<TArg, Window, IProgress<Window>, Task<TResult>> load, IVsUIShell owner, string status, TArg arg )
        {
            Contract.Requires( load != null );
            Contract.Ensures( Contract.Result<Task>() != null );

            var window = new ProgressWindow( status )
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            var progress = new Progress<Window>( w => w.Close() );
            var task = load( arg, window, progress );

            window.ShowDialog( owner );

            return task;
        }
    }
}
