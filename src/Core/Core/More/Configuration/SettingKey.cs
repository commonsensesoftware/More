namespace More.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a configuration key based on name and <see cref="DeploymentEnvironment">environment</see>.
    /// </summary>
    public struct SettingKey : IEquatable<SettingKey>
    {
        private static readonly SettingKey EmptyKey = new SettingKey( "Empty", DeploymentEnvironment.Unspecified );
        private readonly string name;
        private readonly DeploymentEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingKey"/> structure.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="environment">One of the <see cref="DeploymentEnvironment"/> values.</param>
        public SettingKey( string name, DeploymentEnvironment environment )
        {
            this.name = name;
            this.environment = environment;
        }

        /// <summary>
        /// Gets an empty configuration key.
        /// </summary>
        /// <value>A <see cref="SettingKey"/> object.</value>
        public static SettingKey Empty
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
            if ( !( obj is SettingKey ) )
                return false;

            return this.Equals( (SettingKey) obj );
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
        /// <param name="obj1">The <see cref="SettingKey"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="SettingKey"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> equals <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator ==( SettingKey obj1, SettingKey obj2 )
        {
            return obj1.Equals( obj2 );
        }

        /// <summary>
        /// Returns a value indicating whether the specified objects are not equal.
        /// </summary>
        /// <param name="obj1">The <see cref="SettingKey"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="SettingKey"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> does not equal <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator !=( SettingKey obj1, SettingKey obj2 )
        {
            return !obj1.Equals( obj2 );
        }

        /// <summary>
        /// Returns a value indicating whether the current instance equals the specified object.
        /// </summary>
        /// <param name="other">The <see cref="SettingKey"/> to compare to.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public bool Equals( SettingKey other )
        {
            return this.GetHashCode().Equals( other.GetHashCode() );
        }
    }
}