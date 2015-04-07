namespace More.Configuration
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service to locate configuration-based connection strings. This type uses a configuration
    /// file based connection string configuration via the <see cref="ConfigurationManager.ConnectionStrings"/>
    /// collection, matching by key name.
    /// </summary>
    /// <example>This example demonstrates retrieving a connection string for the <see cref="P:DefaultEnvironment">default deployment environment</see>.
    /// <code><![CDATA[
    /// public void ExampleMethod( IConfigurationSettingLocator<ConnectionStringSettings> locator )
    /// {
    ///   var connectionString = locator.LocateAsync( "My Key" ).Result;
    /// 
    ///   /* Would match this connection string in the config file
    ///    * <connectionStrings>
    ///    *   <add name="My Key" connectionString="{connection string}"/>
    ///    * </connectionStrings>
    ///    */
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <example>This example demonstrates retrieving a connection string for the specified <see cref="DeploymentEnvironment">deployment environment</see>.
    /// <code><![CDATA[
    /// public void ExampleMethod( IConfigurationSettingLocator<ConnectionStringSettings> locator )
    /// {
    ///   var connectionString = locator.LocateAsync( "My Key", DeploymentEnvironment.Production ).Result;
    /// 
    ///   /* Would match this connection string in the config file
    ///    * <connectionStrings>
    ///    *   <add name="My Key" connectionString="{connection string}"/>
    ///    * </connectionStrings>
    ///    */
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public partial class ConfigurationFileConnectionStringLocator : IConfigurationSettingLocator<ConnectionStringSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileConnectionStringLocator"/> class.
        /// </summary>
        public ConfigurationFileConnectionStringLocator()
            : this( null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileConnectionStringLocator"/> class.
        /// </summary>
        /// <param name="nextLocator">The next <see cref="IConfigurationSettingLocator{T}"/> in the chain of responsibility. This value may be null.</param>
        public ConfigurationFileConnectionStringLocator( IConfigurationSettingLocator<ConnectionStringSettings> nextLocator )
        {
            this.DefaultEnvironment = DeploymentEnvironment.Unspecified;
            this.NextLocator = nextLocator;
        }

        /// <summary>
        /// Gets or sets the default deployment environment used by the locator.
        /// </summary>
        /// <value>One of the <see cref="DeploymentEnvironment"/> values.</value>
        public DeploymentEnvironment DefaultEnvironment
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the next locator for the current instance.
        /// </summary>
        /// <value>An <see cref="IConfigurationSettingLocator{T}"/> object or null if this locator is the last locator in the chain.</value>
        public IConfigurationSettingLocator<ConnectionStringSettings> NextLocator
        {
            get;
            protected set;
        }

        /// <summary>
        /// Clears any connection strings cached by the locator.
        /// </summary>
        public virtual void ClearCache()
        {
            ConfigurationManager.RefreshSection( "connectionStrings" );

            if ( this.NextLocator != null )
                this.NextLocator.ClearCache();
        }

        /// <summary>
        /// Locates a connection string with the specified key and environment.
        /// </summary>
        /// <param name="key">The key for the connection string to locate.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="ConnectionStringSettings">connection string</see> or null if no match is found.</returns>
        public virtual Task<ConnectionStringSettings> LocateAsync( string key )
        {
            return this.LocateAsync( key, this.DefaultEnvironment );
        }

        /// <summary>
        /// Locates a connection string with the specified key and environment.
        /// </summary>
        /// <remarks>
        /// This method simply proxies access to the <see cref="ConnectionStringSettingsCollection"/> indexer.
        /// If the returned instance for the supplied <paramref name="key"/> value is null and a the 
        /// <see cref="NextLocator"/> is not null, it will be queried for resolution.
        /// </remarks>
        /// <param name="key">The key for the connection string to locate.</param>
        /// <param name="environment">One of the <see cref="DeploymentEnvironment"/> values.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="ConnectionStringSettings">connection string</see> or null if no match is found.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This is validated by a code contract" )]
        public virtual Task<ConnectionStringSettings> LocateAsync( string key, DeploymentEnvironment environment )
        {
            var connectionString = ConfigurationManager.ConnectionStrings[key];

            if ( connectionString != null || this.NextLocator == null )
            {
                var source = new TaskCompletionSource<ConnectionStringSettings>();
                source.SetResult( connectionString );
                return source.Task;
            }

            return this.NextLocator.LocateAsync( key, environment );
        }
    }
}