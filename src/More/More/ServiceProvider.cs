namespace More
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// Represents an object that provides runtime services.
    /// </summary>
    public sealed class ServiceProvider : IServiceProvider
    {
        static readonly ServiceProvider defaultProvider = new ServiceProvider();
        static Lazy<IServiceProvider> current = new Lazy<IServiceProvider>( () => Default, LazyThreadSafetyMode.PublicationOnly );

        ServiceProvider() { }

        /// <summary>
        /// Returns a service of the requested type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>This method always returns itself if the requested <see cref="Type">type</see> is
        /// <see cref="IServiceProvider"/>; otherwise, it returns <see langword="null"/>.</returns>
        public object GetService( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            if ( typeof( IServiceProvider ).Equals( serviceType ) )
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// Gets the default service provider.
        /// </summary>
        /// <value>The default <see cref="IServiceProvider">service provider</see> object.</value>
        /// <remarks>This property typically only used to evaluate whether the <see cref="Current">current</see>
        /// <see cref="IServiceProvider">service provider</see> is the default <see cref="IServiceProvider">service provider</see> or to
        /// <see cref="SetCurrent(IServiceProvider)">set the current</see> <see cref="IServiceProvider">service provider</see> back to its default instance.</remarks>
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
        /// value is the <see cref="Default">default</see> <see cref="IServiceProvider">service provider</see>.</value>
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
            Arg.NotNull( serviceLocator, nameof( serviceLocator ) );
            var newCurrent = serviceLocator;
            SetCurrent( () => newCurrent );
        }

        /// <summary>
        /// Sets the current service provider.
        /// </summary>
        /// <param name="serviceProviderActivator">The <see cref="Func{T}">function</see> used to activate the new, current <see cref="IServiceProvider">service provider</see>.</param>
        public static void SetCurrent( Func<IServiceProvider> serviceProviderActivator )
        {
            Arg.NotNull( serviceProviderActivator, nameof( serviceProviderActivator ) );
            current = new Lazy<IServiceProvider>( serviceProviderActivator, LazyThreadSafetyMode.PublicationOnly );
        }
    }
}