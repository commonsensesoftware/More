namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Web.Http;
    using Web.Http;

    /// <summary>
    /// Represents an <see cref="Activity"/> that initializes the current <see cref="HttpConfiguration">HTTP configuration</see>.
    /// </summary>
    public class InitializeHttpConfiguration : Activity
    {
        /// <summary>
        /// Gets or sets the current HTTP configuration.
        /// </summary>
        /// <value>The current <see cref="HttpConfiguration">HTTP configuration</see>.</value>
        public HttpConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the HTTP configuration callback.
        /// </summary>
        /// <value>The <see cref="Action{T}">action</see> representing the configuration callback.</value>
        public Action<HttpConfiguration> Callback { get; set; }

        /// <summary>
        /// Returns a value indicating whether the activity can be executed.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> the activity can
        /// use to resolve dependent services.</param>
        /// <returns>True if the activity can be executed; otherwise, false.</returns>
        public override bool CanExecute( IServiceProvider serviceProvider ) => Configuration != null;

        /// <summary>
        /// Occurs when the activity is executed.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> the activity can
        /// use to resolve dependent services.</param>
        protected override void OnExecute( IServiceProvider serviceProvider )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );

            if ( CanExecute( serviceProvider ) && serviceProvider is Host host )
            {
                var config = Configuration;

                Callback?.Invoke( config );
                config.DependencyResolver = new WebApiDependencyResolver( host );
                config.EnsureInitialized();
            }
        }
    }
}