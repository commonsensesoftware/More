namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the event data for a completed <see cref="IActivity">activity</see>.
    /// </summary>
    public class ActivityCompletedEventArgs : EventArgs
    {
        readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> associated with the completed activity.</param>
        public ActivityCompletedEventArgs( IServiceProvider serviceProvider )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
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
                Contract.Ensures( serviceProvider != null );
                return serviceProvider;
            }
        }
    }
}