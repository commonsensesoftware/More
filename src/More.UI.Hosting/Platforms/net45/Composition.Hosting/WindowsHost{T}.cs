namespace More.Composition.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// Represents an application composition host where the shell view is a window.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="Window">window</see> to initially show.</typeparam>
    public class WindowsHost<T> : Host where T : Window, IShellView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsHost{T}"/> class.
        /// </summary>
        public WindowsHost() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsHost{T}"/> class.
        /// </summary>
        /// <param name="configurationSettingLocator">The user-defined <see cref="Func{T1,T2,TResult}">function</see> used to resolve composable configuration settings.</param>
        public WindowsHost( Func<string, Type, object> configurationSettingLocator ) : base( configurationSettingLocator ) { }

        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="application">The <see cref="Application">application</see> associated with the host.</param>
        /// <param name="hostedAssemblies">An <see cref="IEnumerable{T}">sequence</see> of hosted, composable <see cref="Assembly">assemblies</see>.</param>
        /// <remarks>The <see cref="Assembly">assembly</see> the <paramref name="application"/> is defined in is always added by default.</remarks>
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
        public override void Run( Application application, IEnumerable<Assembly> hostedAssemblies )
        {
            Arg.NotNull( application, nameof( application ) );
            Arg.NotNull( hostedAssemblies, nameof( hostedAssemblies ) );

            Configuration.WithPart<ShowShellView<T>>();
            WithConfiguration<ShowShellView<T>>();
            base.Run( application, hostedAssemblies );
        }
    }
}