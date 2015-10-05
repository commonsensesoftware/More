namespace More.Composition.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// Represents a <see cref="Host">hosted</see> dependency resolver.
    /// </summary>
    public class MvcDependencyResolver : IDependencyResolver
    {
        private readonly Func<Host> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcDependencyResolver"/> class.
        /// </summary>
        /// <param name="hostFactory">The <see cref="Func{TResult}">factory method</see> used to resolve the <see cref="Host">host</see> associated with the filter provider.</param>
        [CLSCompliant( false )]
        public MvcDependencyResolver( Func<Host> hostFactory )
        {
            Arg.NotNull( hostFactory, nameof( hostFactory ) );
            factory = hostFactory;
        }

        /// <summary>
        /// Returns an object matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>An instance of the requested <paramref name="serviceType">service type</paramref> or null if no match is found.</returns>
        public object GetService( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            if ( serviceType == typeof( IDependencyResolver ) || serviceType == typeof( MvcDependencyResolver ) )
                return this;

            return factory().GetService( serviceType );
        }

        /// <summary>
        /// Returns a sequence of objects matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services matching the requested <paramref name="serviceType">service type</paramref>.</returns>
        public IEnumerable<object> GetServices( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            var services = new List<object>();

            if ( serviceType == typeof( IDependencyResolver ) || serviceType == typeof( MvcDependencyResolver ) )
                services.Add( this );

            services.AddRange( factory().GetServices( serviceType ) );
            return services;
        }
    }
}