namespace More
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an object that provides runtime services.
    /// </summary>
    public sealed class ServiceProvider : IServiceProvider
    {
        private static readonly object syncRoot = new object();
        private static readonly ServiceProvider defaultProvider = new ServiceProvider();
        private static Lazy<IServiceProvider> current = new Lazy<IServiceProvider>( () => Default );

        private ServiceProvider()
        {
        }

        /// <summary>
        /// Returns a service of the requested type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>This method always returns itself if the requested <see cref="Type">type</see> is
        /// <see cref="IServiceProvider"/>; otherwise, it returns <see langword="null"/>.</returns>
        public object GetService( Type serviceType )
        {
            // LEGACY: IServiceProvider does not have a code contract
            if ( serviceType == null )
                throw new ArgumentNullException( "serviceType" );

            if ( typeof( IServiceProvider ).Equals( serviceType ) )
                return this;

            return null;
        }

        /// <summary>
        /// Gets the default service provider.
        /// </summary>
        /// <value>The default <see cref="IServiceProvider">service provider</see> object.</value>
        /// <remarks>This property typically only used to evaluate whether the <see cref="P:Current">current</see>
        /// <see cref="IServiceProvider">service provider</see> is the default <see cref="IServiceProvider">service provider</see> or to
        /// <see cref="M:SetCurrent">set the current</see> <see cref="IServiceProvider">service provider</see> back to its default instance.</remarks>
        public static IServiceProvider Default
        {
            get
            {
                Contract.Ensures( defaultProvider != null );
                return defaultProvider;
            }
        }

        /// <summary>
        /// Gets the current service provider.
        /// </summary>
        /// <value>The current <see cref="IServiceProvider">service provider</see> object. The default
        /// value is the <see cref="P:Default">default</see> <see cref="IServiceProvider">service provider</see>.</value>
        public static IServiceProvider Current
        {
            get
            {
                Contract.Ensures( current != null );
                return current.Value;
            }
        }

        /// <summary>
        /// Sets the current service provider.
        /// </summary>
        /// <param name="serviceLocator">The <see cref="IServiceProvider">service provider</see> to make the current service provider.</param>
        public static void SetCurrent( IServiceProvider serviceLocator )
        {
            Contract.Requires<ArgumentNullException>( serviceLocator != null, "serviceLocator" );
            var newCurrent = serviceLocator;
            SetCurrent( () => newCurrent );
        }

        /// <summary>
        /// Sets the current service provider.
        /// </summary>
        /// <param name="serviceProviderActivator">The <see cref="Func{T}">function</see> used to activate the new, current <see cref="IServiceProvider">service provider</see>.</param>
        public static void SetCurrent( Func<IServiceProvider> serviceProviderActivator )
        {
            Contract.Requires<ArgumentNullException>( serviceProviderActivator != null, "serviceProviderActivator" );

            lock ( syncRoot )
                current = new Lazy<IServiceProvider>( serviceProviderActivator );
        }
    }
}
