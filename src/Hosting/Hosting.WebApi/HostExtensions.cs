namespace More.Composition.Hosting
{
    using ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Web.Http;

    /// <summary>
    /// Provides ASP.NET Web API extensions methods for the <see cref="Host"/> class.
    /// </summary>
    public static class HostExtensions
    {
        private static void ConfigureWebApi( Host host, HttpConfiguration configuration, WebApiConventions conventions )
        {
            Contract.Requires( host != null );
            Contract.Requires( configuration != null );
            Contract.Requires( conventions != null );

            var initializer = new HostInitializer( host, configuration.Initializer );

            configuration.Initializer = initializer.Initialize;
            host.Configure( conventions.Configure );
        }

        /// <summary>
        /// Configures the host with the ASP.NET Web API conventions using the provided import parameter rule.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of host to configure.</typeparam>
        /// <param name="host">The <see cref="Host">host</see> to be configured.</param>
        /// <returns>The original <paramref name="host"/>.</returns>
        public static T ConfigureWebApi<T>( this T host, HttpConfiguration configuration ) where T : Host
        {
            Arg.NotNull( host, nameof( host ) );
            Arg.NotNull( configuration, nameof( configuration ) );

            ConfigureWebApi( host, configuration, new WebApiConventions() );
            return host;
        }

        /// <summary>
        /// Configures the host with the ASP.NET Web API conventions using the provided import parameter rule.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of host to configure.</typeparam>
        /// <param name="host">The <see cref="Host">host</see> to be configured.</param>
        /// <param name="importParameterRule">The <see cref="IRule{T}">rule</see> applied to imported parameters.</param>
        /// <returns>The original <paramref name="host"/>.</returns>
        public static T ConfigureWebApi<T>( this T host, HttpConfiguration configuration, IRule<ImportParameter> importParameterRule ) where T : Host
        {
            Arg.NotNull( host, nameof( host ) );
            Arg.NotNull( configuration, nameof( configuration ) );
            Arg.NotNull( importParameterRule, nameof( importParameterRule ) );

            ConfigureWebApi( host, configuration, new WebApiConventions() { ImportParameterRule = importParameterRule } );
            return host;
        }
    }
}
