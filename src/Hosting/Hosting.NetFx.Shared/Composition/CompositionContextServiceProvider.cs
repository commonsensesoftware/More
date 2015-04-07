namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a <see cref="IServiceProvider">service provider</see> based on an <see cref="CompositionContext">composition context</see>.
    /// </summary>
    public class CompositionContextServiceProvider : ServiceContainer
    {
        private static readonly Type IServiceProviderType = typeof( IServiceProvider );
        private static readonly Type IServiceContainerType = typeof( IServiceContainer );
        private readonly Func<CompositionContext> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContextServiceProvider"/> class.
        /// </summary>
        /// <param name="context">The underlying <see cref="CompositionContext">composition context</see> used by the service provider.</param>
        [CLSCompliant( false )]
        public CompositionContextServiceProvider( CompositionContext context )
        {
            Contract.Requires<ArgumentNullException>( context != null, "context" );
            this.factory = () => context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContextServiceProvider"/> class.
        /// </summary>
        /// <param name="factory">The underlying <see cref="CompositionContext">composition context</see> factory used by the service provider.</param>
        public CompositionContextServiceProvider( Func<CompositionContext> factory )
        {
            Contract.Requires<ArgumentNullException>( factory != null, "factory" );
            this.factory = factory;
        }

        /// <summary>
        /// Gets the underlying composition context.
        /// </summary>
        /// <value>The underlying <see cref="CompositionContext">composition context</see>.</value>
        [CLSCompliant( false )]
        protected CompositionContext Context
        {
            get
            {
                Contract.Ensures( Contract.Result<CompositionContext>() != null );
                return this.factory();
            }
        }

        /// <summary>
        /// Gets a service of the requested type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to return.</param>
        /// <returns>The service instance corresponding to the requested
        /// <paramref name="serviceType">service type</paramref> or <c>null</c> if no match is found.</returns>
        public override object GetService( Type serviceType )
        {
            if ( serviceType == null )
                throw new ArgumentNullException( "serviceType" );

            var generator = new ServiceTypeDisassembler();
            var key = generator.ExtractKey( serviceType );
            object service = null;

            // return multiple services, if requested
            if ( generator.IsForMany( serviceType ) )
            {
                var exports = new List<object>();

                if ( key == null )
                {
                    // if no key is specified and the requested type matches an interface we implement, add ourself
                    if ( IServiceContainerType.Equals( serviceType ) || IServiceProviderType.Equals( serviceType ) )
                        exports.Add( this );
                }

                // add any matching, manually added services
                if ( ( service = base.GetService( serviceType ) ) != null )
                    exports.Add( service );

                // add matching exports
                exports.AddRange( this.Context.GetExports( serviceType, key ) );
                return exports;
            }

            // if no key is specified and the requested type matches an interface we implement, return ourself
            if ( key == null && ( IServiceContainerType.Equals( serviceType ) || IServiceProviderType.Equals( serviceType ) ) )
                return this;

            // return any matching, manually added services
            if ( ( service = base.GetService( serviceType ) ) != null )
                return service;

            // return matching export
            object export;
            this.Context.TryGetExport( serviceType, key, out export );

            return export;
        }
    }
}
