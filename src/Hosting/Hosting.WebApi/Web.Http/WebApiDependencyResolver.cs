namespace More.Web.Http
{
    using Composition.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;

    /// <summary>
    /// Represents a dependency resolver for web requests backed by the Managed Extensibility Framework (MEF).
    /// </summary>
    public class WebApiDependencyResolver : IDependencyResolver
    {
        private readonly Host host;
        private bool disposed;

        /// <summary>
        /// Releases the managed and unmanaged resources used by the <see cref="WebApiDependencyResolver"/> class.
        /// </summary>
        ~WebApiDependencyResolver()
        {
            Dispose( false );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiDependencyResolver"/> class.
        /// </summary>
        /// <param name="host">The <see cref="Host">host</see> associated with the dependency scope.</param>
        [CLSCompliant( false )]
        public WebApiDependencyResolver( Host host )
        {
            Arg.NotNull( host, nameof( host ) );
            this.host = host;
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="WebApiDependencyResolver"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            disposed = true;
            // note: the host is intentionally not disposed here
        }

        /// <summary>
        /// Begins a dependency scope.
        /// </summary>
        /// <returns>A <see cref="IDependencyScope"/> object.</returns>
        public virtual IDependencyScope BeginScope() => new WebApiDependencyScope( host.CreatePerRequest() );

        /// <summary>
        /// Returns an object matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>An instance of the requested <paramref name="serviceType">service type</paramref> or null if no match is found.</returns>
        public virtual object GetService( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            if ( serviceType == typeof( IDependencyResolver ) || serviceType == typeof( WebApiDependencyResolver ) )
                return this;

            return host.GetService( serviceType );
        }

        /// <summary>
        /// Returns a sequence of objects matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services matching the requested <paramref name="serviceType">service type</paramref>.</returns>
        public virtual IEnumerable<object> GetServices( Type serviceType )
        {
            var services = new List<object>();

            if ( serviceType == typeof( IDependencyResolver ) || serviceType == typeof( WebApiDependencyResolver ) )
                services.Add( this );

            services.AddRange( host.GetServices( serviceType ) );
            return services;
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="WebApiDependencyResolver"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}