namespace More.Windows.Controls
{   
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Represents a <see cref="INavigationService">navigation service</see> adapter for a <see cref="WebView">web view</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class WebViewNavigationAdapter : INavigationService
    {
        private sealed class WebViewNavigationCompletedEventArgsAdapter : INavigationEventArgs
        {
            private readonly WebViewNavigationCompletedEventArgs source;

            internal WebViewNavigationCompletedEventArgsAdapter( WebViewNavigationCompletedEventArgs source )
            {
                Contract.Requires( source != null );
                this.source = source;
            }

            public object SourceEventArgs
            {
                get
                {
                    return this.source;
                }
            }

            public bool IsSuccess
            {
                get
                {
                    return this.source.IsSuccess;
                }
            }

            public Uri Uri
            {
                get
                {
                    return this.source.Uri;
                }
            }
        }

        private sealed class WebViewNavigationStartingEventArgsAdapter : INavigationStartingEventArgs
        {
            private readonly WebViewNavigationStartingEventArgs source;

            internal WebViewNavigationStartingEventArgsAdapter( WebViewNavigationStartingEventArgs source )
            {
                Contract.Requires( source != null );
                this.source = source;
            }

            public object SourceEventArgs
            {
                get
                {
                    return this.source;
                }
            }

            public bool Cancel
            {
                get
                {
                    return this.source.Cancel;
                }
                set
                {
                    this.source.Cancel = value;
                }
            }

            public Uri Uri
            {
                get
                {
                    return this.source.Uri;
                }
            }
        }

        private readonly WebView webView;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewNavigationAdapter"/> class.
        /// </summary>
        /// <param name="webView">The <see cref="WebView">web view</see> to adapt to.</param>
        public WebViewNavigationAdapter( WebView webView )
        {
            Contract.Requires<ArgumentNullException>( webView != null, "webView" );

            this.webView = webView;
            this.webView.NavigationCompleted += this.OnWebViewNavigationCompleted;
            this.webView.NavigationStarting += ( s, e ) => this.OnNavigating( new WebViewNavigationStartingEventArgsAdapter( e ) );
        }

        /// <summary>
        /// Gets the underlying web view.
        /// </summary>
        /// <value>The underlying <see cref="WebView">web view</see> being adapted to.</value>
        protected WebView WebView
        {
            get
            {
                Contract.Ensures( this.webView != null );
                return this.webView;
            }
        }

        private void OnWebViewNavigationCompleted( WebView sender, WebViewNavigationCompletedEventArgs args )
        {
            Contract.Requires( args != null );

            var e = new WebViewNavigationCompletedEventArgsAdapter( args );

            if ( args.IsSuccess )
                this.OnNavigated( e );
            else
                this.OnNavigationFailed( e );
        }

        /// <summary>
        /// Raises the <see cref="E:Navigated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        protected virtual void OnNavigated( INavigationEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.Navigated;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="E:Navigating"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs"/> event data.</param>
        protected virtual void OnNavigating( INavigationStartingEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.Navigating;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="E:NavigationFailed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        protected virtual void OnNavigationFailed( INavigationEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.NavigationFailed;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="E:NavigationStopped"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        protected virtual void OnNavigationStopped( INavigationEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.NavigationStopped;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in back navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in back navigation history; false if there are no entries in back navigation history or the
        /// web view does not own its own navigation history.</value>
        public virtual bool CanGoBack
        {
            get
            {
                return this.WebView.CanGoBack;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in forward navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in forward navigation history; false if there are no entries in forward navigation history or
        /// the web view does not own its own navigation history.</value>
        public virtual bool CanGoForward
        {
            get
            {
                return this.WebView.CanGoForward;
            }
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a web view manages its own navigation history.
        /// </summary>
        public virtual void GoBack()
        {
            this.WebView.GoBack();
        }

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a web view manages its own navigation history.
        /// </summary>
        public virtual void GoForward()
        {
            this.WebView.GoForward();
        }

        bool INavigationService.Navigate( Type sourcePageType, object parameter )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Causes the service to load content that is specified by URI, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <param name="sourcePage">The <see cref="Uri">URL</see> of the content to load.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns>True if the service can navigate according to its settings; otherwise, false.</returns>
        public virtual bool Navigate( Uri sourcePage, object parameter )
        {
            if ( sourcePage == null )
                return false;

            // cannot evaluate Cancel because the event args cannot be created
            this.WebView.Navigate( sourcePage );
            return true;
        }

        int INavigationService.SetCacheSize( int cacheSize )
        {
            return 0;
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
