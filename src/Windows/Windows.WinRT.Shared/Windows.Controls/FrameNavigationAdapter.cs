namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Represents a <see cref="INavigationService">navigation service</see> adapter for a <see cref="Frame">frame</see>.
    /// </summary>
    [CLSCompliant( false )]
    public partial class FrameNavigationAdapter : INavigationService
    {
        private readonly Frame frame;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameNavigationAdapter"/> class.
        /// </summary>
        /// <param name="frame">The <see cref="Frame">frame</see> to adapt to.</param>
        public FrameNavigationAdapter( Frame frame )
        {
            Contract.Requires<ArgumentNullException>( frame != null, "frame" );

            this.frame = frame;
            this.frame.Navigated += ( s, e ) => this.OnNavigated( new NavigationEventArgsAdapter( e ) );
            this.frame.Navigating += ( s, e ) => this.OnNavigating( new NavigatingCancelEventArgsAdapter( e ) );
            this.frame.NavigationFailed += ( s, e ) => this.OnNavigationFailed( new NavigationEventArgsAdapter( e ) );
            this.frame.NavigationStopped += ( s, e ) => this.OnNavigationStopped( new NavigationEventArgsAdapter( e ) );
        }

        /// <summary>
        /// Gets the underlying frame.
        /// </summary>
        /// <value>The underlying <see cref="Frame">frame</see> being adapted to.</value>
        protected Frame Frame
        {
            get
            {
                Contract.Ensures( this.frame != null );
                return this.frame;
            }
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
        /// frame does not own its own navigation history.</value>
        public virtual bool CanGoBack
        {
            get
            {
                return this.Frame.CanGoBack;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in forward navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in forward navigation history; false if there are no entries in forward navigation history or
        /// the frame does not own its own navigation history.</value>
        public virtual bool CanGoForward
        {
            get
            {
                return this.Frame.CanGoForward;
            }
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a frame manages its own navigation history.
        /// </summary>
        public virtual void GoBack()
        {
            this.Frame.GoBack();
        }

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a frame manages its own navigation history.
        /// </summary>
        public virtual void GoForward()
        {
            this.Frame.GoForward();
        }

        /// <summary>
        /// Causes the frame to load content that is specified by data type, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <returns>True if the frame can navigate according to its settings; otherwise, false.</returns>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        public virtual bool Navigate( Type sourcePageType, object parameter )
        {
            return this.Frame.Navigate( sourcePageType, parameter );
        }

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
            return this.Frame.Navigate( page, parameter );
        }

        /// <summary>
        /// Sets the number of pages in the navigation history that can be cached by the service.
        /// </summary>
        /// <param name="cacheSize">The new navigation history cache size.</param>
        /// <returns>The previous navigation history cache size value.</returns>
        public int SetCacheSize( int cacheSize )
        {
            var oldValue = this.Frame.CacheSize;
            this.Frame.CacheSize = cacheSize;
            return oldValue;
        }

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        public virtual event EventHandler<INavigationEventArgs> Navigated;

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        public virtual event EventHandler<INavigationStartingEventArgs> Navigating;

        /// <summary>
        /// Occurs when an error is raised while navigating to the requested content.
        /// </summary>
        public virtual event EventHandler<INavigationEventArgs> NavigationFailed;

        /// <summary>
        /// Occurs when a new navigation is requested while a current navigation is in progress.
        /// </summary>
        public virtual event EventHandler<INavigationEventArgs> NavigationStopped;
    }
}
