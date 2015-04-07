namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.Contracts;
    using System.Web.Mvc;

    /// <summary>
    /// Represents a dependency resolver backed by the Managed Extensibility Framework (MEF).
    /// </summary>
    public class CompositionScopeDependencyResolver : IDependencyResolver
    {
        private readonly Func<CompositionContext> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionScopeDependencyResolver"/> class.
        /// </summary>
        /// <param name="contextFactory">The factory <see cref="Func{T}">method</see> used to retrieve the
        /// current <see cref="CompositionContext">context</see> used by the resolver.</param>
        public CompositionScopeDependencyResolver( Func<CompositionContext> contextFactory )
        {
            Contract.Requires<ArgumentNullException>( contextFactory != null, "contextFactory" );
            this.factory = contextFactory;
        }

        /// <summary>
        /// Returns an object matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>An instance of the requested <paramref name="serviceType">service type</paramref> or null if no match is found.</returns>
        public object GetService( Type serviceType )
        {
            // LEGACY: IDependencyScope doesn't have a code contract
            if ( serviceType == null )
                throw new ArgumentNullException( "serviceType" );

            if ( serviceType == typeof( IDependencyResolver ) || serviceType == typeof( CompositionScopeDependencyResolver ) )
                return this;

            object export = null;

            if ( !this.factory().TryGetExport( serviceType, null, out export ) )
                export = null;

            return export;
        }

        /// <summary>
        /// Returns a sequence of objects matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services matching the requested <paramref name="serviceType">service type</paramref>.</returns>
        public IEnumerable<object> GetServices( Type serviceType )
        {
            // LEGACY: IDependencyScope doesn't have a code contract
            if ( serviceType == null )
                throw new ArgumentNullException( "serviceType" );

            var exports = new List<object>();

            if ( serviceType == typeof( IDependencyResolver ) || serviceType == typeof( CompositionScopeDependencyResolver ) )
                exports.Add( this );

            exports.AddRange( this.factory().GetExports( serviceType ) );
            return exports;
        }
    }
}