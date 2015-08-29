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
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Navigation;

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
            var host = new FrameHost<MainPage>();$if$ ($enableSettings$ == true)
            
            // adding an entry for the Settings contract (aka charm)
            host.WithConfiguration<ContractSettings>()
                .Configure( cs => cs.AddSetting( "Defaults", "DefaultSettings" ) );$endif$$if$ ($enableSearch$ == true)

            // enabling the Search contract (aka charm)
            host.WithConfiguration<ContractSettings>()
                .Configure( cs => cs.SearchOptions.PlaceholderText = "Enter your search" )
                .Configure( cs => cs.SearchOptions.ShowOnKeyboardInput = true );$endif$

            return host;
        }
    }
}
