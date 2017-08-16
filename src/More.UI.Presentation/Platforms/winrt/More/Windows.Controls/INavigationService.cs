namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using global::Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Defines the behavior of a navigation service
    /// </summary>
    [CLSCompliant( false )]
    public interface INavigationService
    {
        /// <summary>
        /// Gets a value indicating whether there is at least one entry in back navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in back navigation history; false if there are no entries in back navigation history or the
        /// service does not own its own navigation history.</value>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets a value indicating whether there is at least one entry in forward navigation history.
        /// </summary>
        /// <value>True if there is at least one entry in forward navigation history; false if there are no entries in forward navigation history or
        /// the service does not own its own navigation history.</value>
        bool CanGoForward { get; }

        /// <summary>
        /// Navigates to the most recent item in back navigation history, if a service manages its own navigation history.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Navigates to the most recent item in forward navigation history, if a service manages its own navigation history.
        /// </summary>
        void GoForward();

        /// <summary>
        /// Causes the service to load content that is specified by data type, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns>True if the service can navigate according to its settings; otherwise, false.</returns>
        bool Navigate( Type sourcePageType, object parameter );

        /// <summary>
        /// Causes the service to load content that is specified by URI, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <param name="sourcePage">The <see cref="Uri">URI</see> of the content to load.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns>True if the service can navigate according to its settings; otherwise, false.</returns>
        bool Navigate( Uri sourcePage, object parameter );

        /// <summary>
        /// Sets the number of pages in the navigation history that can be cached by the service.
        /// </summary>
        /// <param name="cacheSize">The new navigation history cache size.</param>
        /// <returns>The previous navigation history cache size value.</returns>
        int SetCacheSize( int cacheSize );

        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        [SuppressMessage( "Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "WinRT events do not inherit from EventArgs." )]
        event EventHandler<INavigationEventArgs> Navigated;

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        [SuppressMessage( "Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "WinRT events do not inherit from EventArgs." )]
        event EventHandler<INavigationStartingEventArgs> Navigating;

        /// <summary>
        /// Occurs when an error is raised while navigating to the requested content.
        /// </summary>
        [SuppressMessage( "Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "WinRT events do not inherit from EventArgs." )]
        event EventHandler<INavigationEventArgs> NavigationFailed;

        /// <summary>
        /// Occurs when a new navigation is requested while a current navigation is in progress.
        /// </summary>
        [SuppressMessage( "Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "WinRT events do not inherit from EventArgs." )]
        event EventHandler<INavigationEventArgs> NavigationStopped;
    }
}