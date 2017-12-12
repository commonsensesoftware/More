namespace More.Windows.Controls
{
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a <see cref="INavigationService">navigation service</see> adapter for a <see cref="WebView">web view</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class WebViewNavigationAdapter : INavigationService
    {
        readonly WebView webView;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewNavigationAdapter"/> class.
        /// </summary>
        /// <param name="webView">The <see cref="WebView">web view</see> to adapt to.</param>
        public WebViewNavigationAdapter( WebView webView )
        {
            Arg.NotNull( webView, nameof( webView ) );

            this.webView = webView;
            this.webView.NavigationCompleted += OnWebViewNavigationCompleted;
            this.webView.NavigationStarting += OnSourceNavigationStarting;
        }

        /// <summary>
        /// Gets the underlying web view.
        /// </summary>
        /// <value>The underlying <see cref="WebView">web view</see> being adapted to.</value>
        protected WebView WebView
        {
            get
            {
                Contract.Ensures( webView != null );
                return webView;
            }
        }

        void OnWebViewNavigationCompleted( WebView sender, WebViewNavigationCompletedEventArgs args )
        {
            Contract.Requires( args != null );

            var e = new WebViewNavigationCompletedEventArgsAdapter( args );

            if ( args.IsSuccess )
            {
                OnNavigated( e );
            }
            else
            {
                OnNavigationFailed( e );
            }
        }

        /// <summary>
        /// Raises the <see cref="Navigated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnNavigated( INavigationEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Navigated?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="Navigating"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnNavigating( INavigationStartingEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Navigating?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="NavigationFailed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnNavigationFailed( INavigationEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            NavigationFailed?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="NavigationStopped"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnNavigationStopped( INavigationEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            NavigationStopped?.Invoke( this, e );
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in back navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in back navigation history; false if there are no entries in back navigation history or the
        /// web view does not own its own navigation history.</value>
        public virtual bool CanGoBack => WebView.CanGoBack;

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in forward navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in forward navigation history; false if there are no entries in forward navigation history or
        /// the web view does not own its own navigation history.</value>
        public virtual bool CanGoForward => WebView.CanGoForward;

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a web view manages its own navigation history.
        /// </summary>
        public virtual void GoBack() => WebView.GoBack();

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a web view manages its own navigation history.
        /// </summary>
        public virtual void GoForward() => WebView.GoForward();

        bool INavigationService.Navigate( Type sourcePageType, object parameter ) => throw new NotSupportedException();

        /// <summary>
        /// Causes the service to load content that is specified by URI, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <param name="sourcePage">The <see cref="Uri">URL</see> of the content to load.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns>True if the service can navigate according to its settings; otherwise, false.</returns>
        public virtual bool Navigate( Uri sourcePage, object parameter )
        {
            if ( sourcePage == null )
            {
                return false;
            }

            // cannot evaluate Cancel because the event args cannot be created
            WebView.Navigate( sourcePage );
            return true;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden because this adapter does not support cache sizes." )]
        int INavigationService.SetCacheSize( int cacheSize ) => 0;

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

        void OnSourceNavigationStarting( WebView sender, WebViewNavigationStartingEventArgs args ) => OnNavigating( new WebViewNavigationStartingEventArgsAdapter( args ) );

        sealed class WebViewNavigationCompletedEventArgsAdapter : INavigationEventArgs
        {
            readonly WebViewNavigationCompletedEventArgs source;

            internal WebViewNavigationCompletedEventArgsAdapter( WebViewNavigationCompletedEventArgs source ) => this.source = source;

            public object SourceEventArgs => source;

            public bool IsSuccess => source.IsSuccess;

            public Uri Uri => source.Uri;
        }

        sealed class WebViewNavigationStartingEventArgsAdapter : INavigationStartingEventArgs
        {
            readonly WebViewNavigationStartingEventArgs source;

            internal WebViewNavigationStartingEventArgsAdapter( WebViewNavigationStartingEventArgs source ) => this.source = source;

            public object SourceEventArgs => source;

            public bool Cancel
            {
                get => source.Cancel;
                set => source.Cancel = value;
            }

            public Uri Uri => source.Uri;
        }
    }
}