namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using More.Windows.Controls;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an activity to setup and configure application-wide navigation settings.
    /// </summary>
    public class NavigationSettings : Activity
    {
        int cacheSize = 10;

        /// <summary>
        /// Gets or sets the number of pages in the navigation history that can be cached.
        /// </summary>
        /// <value>The number of pages that can be in the navigation history.</value>
        public int CacheSize
        {
            get
            {
                Contract.Ensures( cacheSize >= 0 );
                return cacheSize;
            }
            set
            {
                Arg.GreaterThanOrEqualTo( value, 0, nameof( value ) );
                cacheSize = value;
            }
        }

        /// <summary>
        /// Occurs when the task is executed.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> associated with the task.</param>
        protected override void OnExecute( IServiceProvider serviceProvider )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );

            if ( serviceProvider.TryGetService( out INavigationService navigation ) )
            {
                navigation.SetCacheSize( CacheSize );
            }
        }
    }
}