namespace More
{
    using System;

    /// <summary>
    /// Defines the behavior of a service provider with support for key-based services.
    /// </summary>
    /// <remarks>This interface is only meant to be used as bridge for the <see cref="M:IServiceProviderExtensions.GetService(T:System.IServiceProvider,T:Type,T:String)"/>
    /// extension method because the Windows Phone runtime does not currently support <see cref="Type">type</see> delegation.  This interface should, therefore, only
    /// be implemented by service providers that require key-based services.</remarks>
    public interface IKeyedServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Returns a service of the given <paramref name="serviceType">type</paramref> with the specified key.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of object requested.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>The requested service object.</returns>
        object GetService( Type serviceType, string key );
    }
}
