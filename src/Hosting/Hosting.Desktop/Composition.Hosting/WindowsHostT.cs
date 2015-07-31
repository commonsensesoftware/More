namespace More.Composition.Hosting
{
    using System;
    using System.Windows;

    /// <summary>
    /// Represents an application composition host where the shell view is a window.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="Window">window</see> to initially show.</typeparam>
    public class WindowsHost<T> : Host where T : Window, IShellView
    {
        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="application">The <see cref="Application">application</see> associated with the host.</param>
        /// <example>This example demonstrates how to host a Windows Presentation Foundation (WPF) application.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        /// using System.Windows;
        /// 
        /// public class Program
        /// {
        ///     public class MainWindow : Window, IShellView
        ///     {
        ///         public MainWindow()
        ///         {
        ///             this.InitializeComponent();
        ///         }
        ///         
        ///         public virtual new void Show()
        ///         {
        ///             Application.Current.MainWindow = this;
        ///         }
        ///     }
        ///     
        ///     [STAThread]
        ///     public static void Main()
        ///     {
        ///         using ( var host = new WindowsHost<MainWindow>() )
        ///         {
        ///             host.Run( new App() );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        public override void Run( Application application )
        {
            Arg.NotNull( application, nameof( application ) );

            // setup export by convention
            Configuration.WithPart<ShowShellView<T>>();

            // automatically register tasks
            // note: use WithConfiguration to enable tolerance to the fact that the task could
            // have already been registered and re-registering would throw an exception
            WithConfiguration<ShowShellView<T>>();
            
            base.Run( application );
        }
    }
}
