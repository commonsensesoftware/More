namespace More.Web.Http
{
    using Composition.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;

    /// <summary>
    /// Represents an ASP.NET Web API <see cref="IDependencyScope">dependency scope</see>.
    /// </summary>
    public class WebApiDependencyScope : IDependencyScope
    {
        readonly Host host;
        bool disposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="WebApiDependencyScope"/> class.
        /// </summary>
        ~WebApiDependencyScope() => Dispose( false );

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiDependencyScope"/> class.
        /// </summary>
        /// <param name="host">The <see cref="Host">host</see> associated with the dependency scope.</param>
        public WebApiDependencyScope( Host host )
        {
            Arg.NotNull( host, nameof( host ) );
            this.host = host;
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="WebApiDependencyScope"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;

            if ( disposing )
            {
                host.Dispose();
            }
        }

        /// <summary>
        /// Returns an object matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>An instance of the requested <paramref name="serviceType">service type</paramref> or null if no match is found.</returns>
        public virtual object GetService( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            if ( serviceType == typeof( IDependencyScope ) || serviceType == typeof( WebApiDependencyScope ) )
            {
                return this;
            }

            return host.GetService( serviceType );
        }

        /// <summary>
        /// Returns a sequence of objects matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services matching the requested <paramref name="serviceType">service type</paramref>.</returns>
        public virtual IEnumerable<object> GetServices( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            var services = new List<object>();

            if ( serviceType == typeof( IDependencyScope ) || serviceType == typeof( WebApiDependencyScope ) )
            {
                services.Add( this );
            }

            services.AddRange( host.GetServices( serviceType ) );
            return services;
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="WebApiDependencyScope"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}