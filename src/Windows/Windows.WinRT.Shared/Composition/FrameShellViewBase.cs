namespace More.Composition
{
    using More.Windows.Controls;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel.Activation;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Represents the base implemention for a <see cref="IShellView">shell view</see> using a <see cref="Frame">frame</see>.
    /// </summary>
    [CLSCompliant( false )]
    public partial class FrameShellViewBase : IShellView, INavigationService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Frame shellFrame = new Frame();

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameShellViewBase"/> class.
        /// </summary>
        public FrameShellViewBase()
            : this( More.ServiceProvider.Current )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameShellViewBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> associated with the shell view.</param>
        public FrameShellViewBase( IServiceProvider serviceProvider )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );

            this.serviceProvider = serviceProvider;
            shellFrame.Navigated += ( s, e ) => OnNavigated( new NavigationEventArgsAdapter( e ) );
            shellFrame.Navigating += ( s, e ) => OnNavigating( new NavigatingCancelEventArgsAdapter( e ) );
            shellFrame.NavigationFailed += ( s, e ) => OnNavigationFailed( new NavigationEventArgsAdapter( e ) );
            shellFrame.NavigationStopped += ( s, e ) => OnNavigationStopped( new NavigationEventArgsAdapter( e ) );
            SelfRegisterAsNavigationService();
        }

        private void SelfRegisterAsNavigationService()
        {
            // note: we register this way instead of letting MEF do the conventions because there may be many shell views.
            // this approach registers the navigation service manually after the shell view is actually created, which
            // should result in a single shell view.
            IServiceContainer container;

            if ( serviceProvider.TryGetService( out container ) )
                container.ReplaceService<INavigationService>( this, true );
        }

        /// <summary>
        /// Gets the underlying frame.
        /// </summary>
        /// <value>The underlying <see cref="Frame">frame</see> being adapted to.</value>
        protected Frame Frame
        {
            get
            {
                Contract.Ensures( shellFrame != null );
                return shellFrame;
            }
        }

        /// <summary>
        /// Gets the service provider associated with the view.
        /// </summary>
        /// <value>A <see cref="IServiceProvider">service provider</see> object.</value>
        protected IServiceProvider ServiceProvider
        {
            get
            {
                Contract.Ensures( serviceProvider != null );
                return serviceProvider;
            }
        }

        /// <summary>
        /// Gets or sets the the start page type.
        /// </summary>
        /// <value>The start page <see cref="Type">type</see>.</value>
        public Type StartPage
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void OnNavigated( INavigationEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );

            var args = e.SourceEventArgs as NavigationEventArgs;

            if ( args == null )
                goto RaiseEventHandler;

            // exit if there's nothing to compose
            if ( args.Content == null )
                goto RaiseEventHandler;

            ICompositionService composer;

            if ( ServiceProvider.TryGetService( out composer ) )
                composer.Compose( args.Content );

        RaiseEventHandler:
            Navigated?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="E:Navigating"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnNavigating( INavigationStartingEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Navigating?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="E:NavigationFailed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnNavigationFailed( INavigationEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            NavigationFailed?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="E:NavigationStopped"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnNavigationStopped( INavigationEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            NavigationStopped?.Invoke( this, e );
        }

        /// <summary>
        /// Occurs when the application should load state from a previously suspended application.
        /// </summary>
        /// <param name="applicationState">The <see cref="IApplicationState">application state</see> information to load from.</param>
        /// <remarks>Note to inheritors: The base implementation does not perform any action.</remarks>
        protected virtual void OnLoadState( IApplicationState applicationState )
        {
            Arg.NotNull( applicationState, nameof( applicationState ) );
        }

        /// <summary>
        /// Gets or sets localization/globalization language information that applies to the shell view.
        /// </summary>
        /// <value>The language information for the shell view.  This property can be null.</value>
        public string Language
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the direction that text and other user interface (UI) elements flow within the shell view.
        /// </summary>
        /// <value>The direction that text and other UI elements flow within the shell view. This property can be null.</value>
        public string FlowDirection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in back navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in back navigation history; false if there are no entries in back navigation history or the
        /// frame does not own its own navigation history.</value>
        public bool CanGoBack
        {
            get
            {
                return Frame.CanGoBack;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in forward navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in forward navigation history; false if there are no entries in forward navigation history or
        /// the frame does not own its own navigation history.</value>
        public bool CanGoForward
        {
            get
            {
                return Frame.CanGoForward;
            }
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a frame manages its own navigation history.
        /// </summary>
        public void GoBack() => Frame.GoBack();

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a frame manages its own navigation history.
        /// </summary>
        public void GoForward() => Frame.GoForward();

        /// <summary>
        /// Occurs when the application is navigating to the initial start page.
        /// </summary>
        /// <param name="applicationState">The <see cref="IApplicationState">application state</see> information used to navigate to the initial start page.</param>
        /// <remarks>If the <see cref="P:StartPage">start page</see> is null, the base implementation will not perform any action.</remarks>
        protected virtual void OnNavigateToStartPage( IApplicationState applicationState )
        {
            Arg.NotNull( applicationState, nameof( applicationState ) );

            if ( StartPage == null )
                return;

            object parameter = applicationState == null ? null : applicationState.Activation.Arguments;
            Frame.Navigate( StartPage, parameter );
        }

        private void ConfigureRootElement( FrameworkElement element )
        {
            Contract.Requires( element != null );

            if ( !string.IsNullOrEmpty( Language ) )
                element.Language = Language;

            if ( !string.IsNullOrEmpty( FlowDirection ) )
                element.FlowDirection = (FlowDirection) Enum.Parse( typeof( FlowDirection ), FlowDirection, false );
        }

        partial void BeforeFirstNavigation( Frame frame );

        /// <summary>
        /// Shows the view as the root visual.
        /// </summary>
        public virtual void Show()
        {
            IApplicationState state = null;

            ServiceProvider.TryGetService<IApplicationState>( out state );

            // do not repeat app initialization when the window already has content
            if ( Window.Current.Content == null )
            {
                if ( state != null )
                {
                    // load state from previously suspended application
                    if ( state.Activation.PreviousExecutionState == ApplicationExecutionState.Terminated )
                        OnLoadState( state );
                }

                ConfigureRootElement( Frame );
                Window.Current.Content = Frame;
            }

            // when the navigation stack isn't restored, navigate to the first page and configure
            // the new page by passing required information as a navigation parameter
            if ( Frame.Content == null )
            {
                BeforeFirstNavigation( Frame );
                OnNavigateToStartPage( state );
            }

            // ensure that the window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Causes the frame to load content that is specified by data type, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <returns>True if the frame can navigate according to its settings; otherwise, false.</returns>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        public bool Navigate( Type sourcePageType, object parameter ) => Frame.Navigate( sourcePageType, parameter );

        /// <summary>
        /// Causes the service to load content that is specified by URI, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <param name="sourcePage">The <see cref="Uri">URI</see> of the content to load. Source page <see cref="Type">types</see> can be expressed
        /// using a relative <see cref="Uri">URI</see> (ex: "MainPage" or "/MainPage") or using the Uniform Resource Name (URN) scheme (ex: "urn:MainPage").</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns>True if the service can navigate according to its settings; otherwise, false.</returns>
        public virtual bool Navigate( Uri sourcePage, object parameter )
        {
            Type page;

            // resolve page from specified uri
            if ( !PageUriHelper.TryGetPageFromUri( sourcePage, out page ) )
                return false;

            // let navigation service do what it normally does
            return Frame.Navigate( page, parameter );
        }

        /// <summary>
        /// Sets the number of pages in the navigation history that can be cached by the service.
        /// </summary>
        /// <param name="cacheSize">The new navigation history cache size.</param>
        /// <returns>The previous navigation history cache size value.</returns>
        public int SetCacheSize( int cacheSize )
        {
            var oldValue = Frame.CacheSize;
            Frame.CacheSize = cacheSize;
            return oldValue;
        }

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        public event EventHandler<INavigationEventArgs> Navigated;

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        public event EventHandler<INavigationStartingEventArgs> Navigating;

        /// <summary>
        /// Occurs when an error is raised while navigating to the requested content.
        /// </summary>
        public event EventHandler<INavigationEventArgs> NavigationFailed;

        /// <summary>
        /// Occurs when a new navigation is requested while a current navigation is in progress.
        /// </summary>
        public event EventHandler<INavigationEventArgs> NavigationStopped;
    }
}
