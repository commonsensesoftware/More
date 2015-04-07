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
        private int cacheSize = 10;

        /// <summary>
        /// Gets or sets the number of pages in the navigation history that can be cached.
        /// </summary>
        /// <value>The number of pages that can be in the navigation history.</value>
        public int CacheSize
        {
            get
            {
                Contract.Ensures( this.cacheSize >= 0 );
                return this.cacheSize;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>( value >= 0, "value" );
                this.cacheSize = value;
            }
        }

        /// <summary>
        /// Occurs when the task is executed.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> associated with the task.</param>
        protected override void OnExecute( IServiceProvider serviceProvider )
        {
            INavigationService navigation;

            if ( serviceProvider.TryGetService( out navigation ) )
                navigation.SetCacheSize( this.CacheSize );
        }
    }
}
