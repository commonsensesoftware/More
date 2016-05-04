namespace More.Composition.Hosting
{
    using More.Windows;
    using System;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class Host : IKeyedServiceProvider
    {
        partial void AddWinRTDefaultServices() => base.AddService( typeof( IContinuationManager ), ( sc, t ) => new ContinuationManager() );

        /// <summary>
        /// Gets a service of the requested type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to return.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>The service instance corresponding to the requested <paramref name="serviceType">service type</paramref>
        /// or <c>null</c> if no match is found.</returns>
        object IKeyedServiceProvider.GetService( Type serviceType, string key ) => GetService( serviceType, key );
    }
}
