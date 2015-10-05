namespace More.Composition.Hosting
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Http;

    internal sealed class HostInitializer
    {
        private readonly Host host;
        private readonly Action<HttpConfiguration> defaultInitializer;

        internal HostInitializer( Host host, Action<HttpConfiguration> defaultInitializer )
        {
            Contract.Requires( host != null );
            this.host = host;
            this.defaultInitializer = defaultInitializer;
        }

        internal void Initialize( HttpConfiguration configuration )
        {
            Contract.Assume( configuration != null );

            configuration.DependencyResolver = new WebApiDependencyResolver( host );
            defaultInitializer?.Invoke( configuration );
        }
    }
}
