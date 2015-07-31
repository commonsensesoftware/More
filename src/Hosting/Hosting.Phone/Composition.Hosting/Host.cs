namespace More.Composition.Hosting
{
    using More.Windows;
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Linq;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class Host : IKeyedServiceProvider
    {
        static partial void AddWinRTSpecificConventions( ConventionBuilder builder )
        {
            builder.ForType<ContinuationManager>().Export<IContinuationManager>().Shared();
        }

        private object GetService( Type serviceType, string key )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );
            CheckDisposed();

            var generator = new ServiceTypeDisassembler();
            object service = null;

            // return multiple services, if requested
            if ( generator.IsForMany( serviceType ) )
            {
                var exports = new List<object>();

                if ( key == null )
                {
                    // if no key is specified and the requested type matches an interface we implement, add ourself
                    if ( ExportedInterfaces.Contains( serviceType ) )
                        exports.Add( this );
                    else if ( ExportedTypes.Contains( serviceType ) )
                        exports.Add( Container );
                }

                // add any matching, manually added services
                if ( ( service = base.GetService( serviceType ) ) != null )
                    exports.Add( service );

                // add matching exports
                exports.AddRange( Container.GetExports( serviceType, key ) );
                return exports;
            }

            // if no key is specified and the requested type matches an interface we implement, return ourself
            if ( key == null )
            {
                if ( ExportedInterfaces.Contains( serviceType ) )
                    return this;
                else if ( ExportedTypes.Contains( serviceType ) )
                    return Container;
            }

            // return any matching, manually added services
            if ( ( service = base.GetService( serviceType ) ) != null )
                return service;

            // return matching export
            object export;
            Container.TryGetExport( serviceType, key, out export );

            return export;
        }

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
