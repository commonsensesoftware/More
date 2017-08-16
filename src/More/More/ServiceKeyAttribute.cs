namespace More
{
    using System;

    /// <summary>
    /// Represents the metadata used to associate a key with a service.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public sealed class ServiceKeyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceKeyAttribute"/> class.
        /// </summary>
        /// <param name="key">The key associated with a service.</param>
        public ServiceKeyAttribute( string key ) => Key = key;

        /// <summary>
        /// Gets the key associated with the service.
        /// </summary>
        /// <value>The service key.</value>
        public string Key { get; }
    }
}