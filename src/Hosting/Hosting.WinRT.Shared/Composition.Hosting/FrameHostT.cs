namespace More.Composition.Hosting
{
    using System;
    using System.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <summary>
    /// Represents an application composition host where the shell view is a page frame.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="Page"/> to show as the initial start page.</typeparam>
    [CLSCompliant( false )]
    public partial class FrameHost<T> : Host where T : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameHost{T}"/> class.
        /// </summary>
        public FrameHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameHost{T}"/> class.
        /// </summary>
        /// <param name="configurationSettingLocator">The user-defined <see cref="Func{T,TResult}">function</see> used to resolve composable configuration settings.</param>
        public FrameHost( Func<string, object> configurationSettingLocator )
            : base( configurationSettingLocator )
        {
        }

        partial void OnConfigure();

        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="application">The <see cref="Application">application</see> associated with the host.</param>
        /// <param name="language">The language code for localization used by the application.  This parameter can be null.</param>
        /// <param name="flowDirection">The flow direction of text within the application. This parameter can be null.</param>
        /// <example>This example demonstrates how to host a navigation application.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        /// using System.Reflection;
        /// using global::Windows.ApplicationModel;
        /// using global::Windows.ApplicationModel.Activation;
        /// using global::Windows.UI.Xaml;
        /// 
        /// public class MainPage : Page
        /// {
        /// }
        ///     
        /// public sealed class App : Application
        /// {
        ///     public App()
        ///     {
        ///         this.InitializeComponent();
        ///         var host = new FrameHost<MainPage>();
        ///         host.Run( this, "en-US", "LeftToRight" ); 
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        public virtual void Run( Application application, string language, string flowDirection )
        {
            Arg.NotNull( application, nameof( application ) );

            // setup export by convention
            Configuration.WithPart<FrameShellView<T>>();
            Configuration.WithPart<ShowShellView<FrameShellView<T>>>();

            // automatically register activities
            // note: use WithConfiguration to enable tolerance to the fact that the activity
            // could have already been registered and re-registering would throw an exception
            var taskConfig = WithConfiguration<ShowShellView<FrameShellView<T>>>();

            // configure language
            if ( !string.IsNullOrEmpty( language ) )
                taskConfig.Configure( t => t.Language = language );

            // configure flow direction
            if ( !string.IsNullOrEmpty( flowDirection ) )
                taskConfig.Configure( t => t.FlowDirection = flowDirection );

            OnConfigure();
            base.Run( application );
        }

        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="application">The <see cref="Application">application</see> associated with the host.</param>
        public sealed override void Run( Application application )
        {
            Arg.NotNull( application, nameof( application ) );
            Run( application, null, null );
        }
    }
}
