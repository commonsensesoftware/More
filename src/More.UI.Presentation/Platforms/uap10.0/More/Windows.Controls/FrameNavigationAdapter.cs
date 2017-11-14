namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Represents a <see cref="INavigationService">navigation service</see> adapter for a <see cref="Frame">frame</see>.
    /// </summary>
    [CLSCompliant( false )]
    public partial class FrameNavigationAdapter : INavigationService
    {
        readonly Frame frame;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameNavigationAdapter"/> class.
        /// </summary>
        /// <param name="frame">The <see cref="Frame">frame</see> to adapt to.</param>
        public FrameNavigationAdapter( Frame frame )
        {
            Arg.NotNull( frame, nameof( frame ) );

            this.frame = frame;
            this.frame.Navigated += ( s, e ) => OnNavigated( new NavigationEventArgsAdapter( e ) );
            this.frame.Navigating += ( s, e ) => OnNavigating( new NavigatingCancelEventArgsAdapter( e ) );
            this.frame.NavigationFailed += ( s, e ) => OnNavigationFailed( new NavigationEventArgsAdapter( e ) );
            this.frame.NavigationStopped += ( s, e ) => OnNavigationStopped( new NavigationEventArgsAdapter( e ) );
        }

        /// <summary>
        /// Gets the underlying frame.
        /// </summary>
        /// <value>The underlying <see cref="Frame">frame</see> being adapted to.</value>
        protected Frame Frame
        {
            get
            {
                Contract.Ensures( frame != null );
                return frame;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Navigated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="INavigationEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnNavigated( INavigationEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
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
        /// Gets a value indicating whether there is at least one entry in back navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in back navigation history; false if there are no entries in back navigation history or the
        /// frame does not own its own navigation history.</value>
        public virtual bool CanGoBack => Frame.CanGoBack;

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in forward navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in forward navigation history; false if there are no entries in forward navigation history or
        /// the frame does not own its own navigation history.</value>
        public virtual bool CanGoForward => Frame.CanGoForward;

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a frame manages its own navigation history.
        /// </summary>
        public virtual void GoBack() => Frame.GoBack();

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a frame manages its own navigation history.
        /// </summary>
        public virtual void GoForward() => Frame.GoForward();

        /// <summary>
        /// Causes the frame to load content that is specified by data type, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <returns>True if the frame can navigate according to its settings; otherwise, false.</returns>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        public virtual bool Navigate( Type sourcePageType, object parameter ) => Frame.Navigate( sourcePageType, parameter );

        /// <summary>
        /// Causes the service to load content that is specified by URI, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <param name="sourcePage">The <see cref="Uri">URI</see> of the content to load. Source page <see cref="Type">types</see> can be expressed
        /// using a relative <see cref="Uri">URI</see> (ex: "MainPage" or "/MainPage") or using the Uniform Resource Name (URN) scheme (ex: "urn:MainPage").</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns>True if the service can navigate according to its settings; otherwise, false.</returns>
        public virtual bool Navigate( Uri sourcePage, object parameter )
        {
            if ( !PageUriHelper.TryGetPageFromUri( sourcePage, out var page ) )
            {
                return false;
            }

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