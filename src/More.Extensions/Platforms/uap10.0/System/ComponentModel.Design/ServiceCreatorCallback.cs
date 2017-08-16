namespace System.ComponentModel.Design
{
    using System;

    /// <summary>
    /// Provides a callback mechanism that can create an instance of a service on demand.
    /// </summary>
    /// <param name="container">The <see cref="IServiceContainer">service container</see> that requested the creation of the service.</param>
    /// <param name="serviceType">The <see cref="Type">type</see> of service to create.</param>
    /// <returns>The service specified by <paramref name="serviceType"/>, or null if the service could not be created.</returns>
    public delegate object ServiceCreatorCallback( IServiceContainer container, Type serviceType );
}