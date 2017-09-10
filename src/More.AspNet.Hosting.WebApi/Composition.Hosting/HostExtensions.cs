namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Http;

    /// <summary>
    /// Provides ASP.NET Web API extensions methods for the <see cref="Host"/> class.
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Configures the host with the ASP.NET Web API conventions using the provided import parameter rule.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of host to configure.</typeparam>
        /// <param name="host">The <see cref="Host">host</see> to be configured.</param>
        /// <param name="configuration">The hosted <see cref="HttpConfiguration">HTTP configuration</see>.</param>
        /// <returns>The original <paramref name="host"/>.</returns>
        public static T ConfigureWebApi<T>( this T host, HttpConfiguration configuration ) where T : Host
        {
            Arg.NotNull( host, nameof( host ) );
            Arg.NotNull( configuration, nameof( configuration ) );

            ConfigureWebApi( host, configuration, DefaultAction.None, new WebApiConventions() );
            return host;
        }

        /// <summary>
        /// Configures the host with the ASP.NET Web API conventions using the provided import parameter rule.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of host to configure.</typeparam>
        /// <param name="host">The <see cref="Host">host</see> to be configured.</param>
        /// <param name="configuration">The hosted <see cref="HttpConfiguration">HTTP configuration</see>.</param>
        /// <param name="configurationCallback">The configuration <see cref="Action{T}">action</see> to perform before the host runs.</param>
        /// <returns>The original <paramref name="host"/>.</returns>
        public static T ConfigureWebApi<T>( this T host, HttpConfiguration configuration, Action<HttpConfiguration> configurationCallback ) where T : Host
        {
            Arg.NotNull( host, nameof( host ) );
            Arg.NotNull( configuration, nameof( configuration ) );

            ConfigureWebApi( host, configuration, configurationCallback, new WebApiConventions() );
            return host;
        }

        /// <summary>
        /// Configures the host with the ASP.NET Web API conventions using the provided import parameter rule.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of host to configure.</typeparam>
        /// <param name="host">The <see cref="Host">host</see> to be configured.</param>
        /// <param name="configuration">The hosted <see cref="HttpConfiguration">HTTP configuration</see>.</param>
        /// <param name="importParameterRule">The <see cref="IRule{T}">rule</see> applied to imported parameters.</param>
        /// <returns>The original <paramref name="host"/>.</returns>
        public static T ConfigureWebApi<T>( this T host, HttpConfiguration configuration, IRule<ImportParameter> importParameterRule ) where T : Host
        {
            Arg.NotNull( host, nameof( host ) );
            Arg.NotNull( configuration, nameof( configuration ) );
            Arg.NotNull( importParameterRule, nameof( importParameterRule ) );

            var conventions = new WebApiConventions() { ImportParameterRule = importParameterRule };
            ConfigureWebApi( host, configuration, DefaultAction.None, conventions );
            return host;
        }

        /// <summary>
        /// Configures the host with the ASP.NET Web API conventions using the provided import parameter rule.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of host to configure.</typeparam>
        /// <param name="host">The <see cref="Host">host</see> to be configured.</param>
        /// <param name="configuration">The hosted <see cref="HttpConfiguration">HTTP configuration</see>.</param>
        /// <param name="configurationCallback">The configuration <see cref="Action{T}">action</see> to perform before the host runs.</param>
        /// <param name="importParameterRule">The <see cref="IRule{T}">rule</see> applied to imported parameters.</param>
        /// <returns>The original <paramref name="host"/>.</returns>
        public static T ConfigureWebApi<T>( this T host, HttpConfiguration configuration, Action<HttpConfiguration> configurationCallback, IRule<ImportParameter> importParameterRule ) where T : Host
        {
            Arg.NotNull( host, nameof( host ) );
            Arg.NotNull( configuration, nameof( configuration ) );
            Arg.NotNull( configurationCallback, nameof( configurationCallback ) );
            Arg.NotNull( importParameterRule, nameof( importParameterRule ) );

            var conventions = new WebApiConventions() { ImportParameterRule = importParameterRule };
            ConfigureWebApi( host, configuration, configurationCallback, conventions );
            return host;
        }

        static void ConfigureWebApi( Host host, HttpConfiguration configuration, Action<HttpConfiguration> configurationCallback, WebApiConventions conventions )
        {
            Contract.Requires( host != null );
            Contract.Requires( configuration != null );
            Contract.Requires( configurationCallback != null );
            Contract.Requires( conventions != null );

            var config = configuration;
            var callback = configurationCallback;

            host.Configure( conventions.Apply );
            host.Configure( ( c, b ) => c.WithPart<InitializeHttpConfiguration>() );
            host.WithConfiguration<InitializeHttpConfiguration>()
                .Configure(
                    activity =>
                    {
                        activity.Configuration = config;
                        activity.Callback = callback;
                    } );
        }
    }
}