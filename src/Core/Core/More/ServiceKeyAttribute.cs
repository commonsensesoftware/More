namespace More
{
    using global::System;

    /// <summary>
    /// Represents the metadata used to associate a key with a service.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public sealed class ServiceKeyAttribute : Attribute
    {
        private readonly string key;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceKeyAttribute"/> class.
        /// </summary>
        /// <param name="key">The key associated with a service.</param>
        public ServiceKeyAttribute( string key )
        {
            this.key = key;
        }

        /// <summary>
        /// Gets the key associated with the service.
        /// </summary>
        /// <value>The service key.</value>
        public string Key
        {
            get
            {
                return this.key;
            }
        }
    }
}
