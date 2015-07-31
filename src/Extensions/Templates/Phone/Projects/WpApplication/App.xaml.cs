namespace $safeprojectname$
{
    using $safeprojectname$.Views;
    using More;
    using More.Composition;
    using More.Composition.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using global::Windows.ApplicationModel;
    using global::Windows.ApplicationModel.Activation;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Represents the current application.
    /// </summary>
    public sealed partial class App : ComposedApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();
#if DEBUG
            if ( Debugger.IsAttached )
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif
        }

        /// <summary>
        /// Creates the application host.
        /// </summary>
        /// <returns>An application <see cref="Host">host</see>.</returns>
        protected override Host CreateHost()
        {
            var host = new FrameHost<_view>();
            
            host.WithConfiguration<NavigationSettings>()
                .DependsOn<ShowShellView<FrameShellView<_view>>>()
                .Configure( ns => ns.CacheSize = 1 );

            return host;
        }
    }
}