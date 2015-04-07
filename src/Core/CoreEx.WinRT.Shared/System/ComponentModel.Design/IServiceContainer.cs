namespace System.ComponentModel.Design
{
    using global::System;

    /// <summary>
    /// Provides a container for services.
    /// </summary>
    public interface IServiceContainer : IServiceProvider
    {
        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        void AddService( Type serviceType, ServiceCreatorCallback callback );

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the serviceType parameter.</param>
        void AddService( Type serviceType, object serviceInstance );

        /// <summary>
        /// Adds the specified service to the service container, and optionally promotes the service to parent service containers.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        void AddService( Type serviceType, ServiceCreatorCallback callback, bool promote );

        /// <summary>
        /// Adds the specified service to the service container, and optionally promotes the service to any parent service containers.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the <paramref name="serviceType"/> parameter.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        void AddService( Type serviceType, object serviceInstance, bool promote );

        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to remove.</param>
        void RemoveService( Type serviceType );

        /// <summary>
        /// Removes the specified service type from the service container, and optionally promotes the service to parent service containers.
        /// </summary>
        /// <param name="serviceType">The type of service to remove.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        void RemoveService( Type serviceType, bool promote );
    }
}
