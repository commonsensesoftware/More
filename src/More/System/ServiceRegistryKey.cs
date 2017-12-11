namespace More
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the key for a registered service.
    /// </summary>
    public class ServiceRegistryKey : IEquatable<ServiceRegistryKey>
    {
        readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegistryKey"/> class.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of registered service.</param>
        public ServiceRegistryKey( Type serviceType ) : this( serviceType, new ServiceTypeDisassembler().ExtractKey( serviceType ) ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegistryKey"/> class.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of registered service.</param>
        /// <param name="serviceKey">The key associated with the registered service.</param>
        public ServiceRegistryKey( Type serviceType, string serviceKey )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            type = serviceType;
            Key = serviceKey;
        }

        /// <summary>
        /// Gets the type of registered service.
        /// </summary>
        /// <value>The registered service <see cref="Type">type</see>.</value>
        public Type ServiceType
        {
            get
            {
                Contract.Ensures( type != null );
                return type;
            }
        }

        /// <summary>
        /// Gets the key associated with the registered service, if any.
        /// </summary>
        /// <value>The registered service key or <c>null</c> if the service does not have any associated key.</value>
        public string Key { get; }

        /// <summary>
        /// Returns a hash code for the current instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = ServiceType.GetHashCode();
            return Key == null ? hashCode : ( hashCode * 397 ) ^ Key.GetHashCode();
        }

        /// <summary>
        /// Returns a value indicating whether the specified object equals the current instance.
        /// </summary>
        /// <param name="obj">The other <see cref="object">object</see> to evaluate for equality.</param>
        /// <returns>True if the <paramref name="obj">object</paramref> equals the current instance; otherwise, false.</returns>
        public override bool Equals( object obj ) => Equals( obj as ServiceRegistryKey );

        /// <summary>
        /// Returns a value indicating whether the specified object equals the current instance.
        /// </summary>
        /// <param name="other">The other <see cref="ServiceRegistryKey"/> to evaluate for equality.</param>
        /// <returns>True if the <paramref name="other"/> object equals the current instance; otherwise, false.</returns>
        public bool Equals( ServiceRegistryKey other ) => other == null ? false : GetHashCode() == other.GetHashCode();

        /// <summary>
        /// Overrides the default equality operator.
        /// </summary>
        /// <param name="key1">The <see cref="ServiceRegistryKey"/> to compare.</param>
        /// <param name="key2">The <see cref="ServiceRegistryKey"/> to compare against.</param>
        /// <returns>True if the two objects are equal; otherwise, false.</returns>
        public static bool operator ==( ServiceRegistryKey key1, ServiceRegistryKey key2 ) =>
            ReferenceEquals( key1, null ) ? ReferenceEquals( key2, null ) : key1.Equals( key2 );

        /// <summary>
        /// Overrides the default inequality operator.
        /// </summary>
        /// <param name="key1">The <see cref="ServiceRegistryKey"/> to compare.</param>
        /// <param name="key2">The <see cref="ServiceRegistryKey"/> to compare against.</param>
        /// <returns>True if the two objects are not equal; otherwise, false.</returns>
        public static bool operator !=( ServiceRegistryKey key1, ServiceRegistryKey key2 ) =>
            ReferenceEquals( key1, null ) ? !ReferenceEquals( key2, null ) : !key1.Equals( key2 );
    }
}