namespace $safeprojectname$
{
    using $safeprojectname$.Views;
    using More;
    using More.Composition;
    using More.Composition.Hosting;
    using System;

	/// <summary>
    /// Represents the current program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point to the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using ( var host = new WindowsHost<MainWindow>() )
            {$if$ ($showTips$ == true)
                // TODO: register and/or configure tasks to execute during start up
                // the ShowShellViewTask is automatically registered. if you do not
                // have additional start up logic, then there is nothing to do here.$endif$
                host.Run( new App() );
            }
        }
    }
}