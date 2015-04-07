namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the event data for a completed <see cref="IActivity">activity</see>.
    /// </summary>
    public class ActivityCompletedEventArgs : EventArgs
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> associated with the completed activity.</param>
        public ActivityCompletedEventArgs( IServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null, "serviceProvider" );
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the service provider associated with the completed activity.
        /// </summary>
        /// <value>A <see cref="IServiceProvider">service provider</see>.</value>
        public IServiceProvider ServiceProvider
        {
            get
            {
                Contract.Ensures( this.serviceProvider != null );
                return this.serviceProvider;
            }
        }
    }
}
