namespace More.Configuration
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a configuration key based on name and <see cref="DeploymentEnvironment">environment</see>.
    /// </summary>
    public struct ConfigurationKey : IEquatable<ConfigurationKey>
    {
        private static readonly ConfigurationKey EmptyKey = new ConfigurationKey( "Empty", DeploymentEnvironment.Unspecified );
        private readonly string name;
        private readonly DeploymentEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationKey"/> structure.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="environment">One of the <see cref="DeploymentEnvironment"/> values.</param>
        public ConfigurationKey( string name, DeploymentEnvironment environment )
        {
            this.name = name;
            this.environment = environment;
        }

        /// <summary>
        /// Gets an empty configuration key.
        /// </summary>
        /// <value>A <see cref="ConfigurationKey"/> object.</value>
        public static ConfigurationKey Empty
        {
            get
            {
                return EmptyKey;
            }
        }

        /// <summary>
        /// Gets the name of the key.
        /// </summary>
        /// <value>The key name.</value>
        public string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.name ) );
                return this.name;
            }
        }

        /// <summary>
        /// Gets the deployment environment associated with key.
        /// </summary>
        /// <value>One of the <see cref="DeploymentEnvironment"/> values.</value>
        public DeploymentEnvironment Environment
        {
            get
            {
                return this.environment;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the current instance equals the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Object">object</see> to compare to.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public override bool Equals( object obj )
        {
            if ( !( obj is ConfigurationKey ) )
                return false;

            return this.Equals( (ConfigurationKey) obj );
        }

        /// <summary>
        /// Returns a hash code for the current instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return
                StringComparer.OrdinalIgnoreCase.GetHashCode( this.Name ) +
                EqualityComparer<DeploymentEnvironment>.Default.GetHashCode( this.Environment );
        }

        /// <summary>
        /// Returns a value indicating whether the specified objects are equal.
        /// </summary>
        /// <param name="obj1">The <see cref="ConfigurationKey"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="ConfigurationKey"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> equals <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator ==( ConfigurationKey obj1, ConfigurationKey obj2 )
        {
            return obj1.Equals( obj2 );
        }

        /// <summary>
        /// Returns a value indicating whether the specified objects are not equal.
        /// </summary>
        /// <param name="obj1">The <see cref="ConfigurationKey"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="ConfigurationKey"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> does not equal <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator !=( ConfigurationKey obj1, ConfigurationKey obj2 )
        {
            return !obj1.Equals( obj2 );
        }

        /// <summary>
        /// Returns a value indicating whether the current instance equals the specified object.
        /// </summary>
        /// <param name="other">The <see cref="ConfigurationKey"/> to compare to.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public bool Equals( ConfigurationKey other )
        {
            return this.GetHashCode().Equals( other.GetHashCode() );
        }
    }
}