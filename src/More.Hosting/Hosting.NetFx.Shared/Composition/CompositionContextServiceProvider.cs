namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Composition;
    using System.Diagnostics.Contracts;

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
            Arg.NotNull( context, nameof( context ) );
            factory = () => context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContextServiceProvider"/> class.
        /// </summary>
        /// <param name="factory">The underlying <see cref="CompositionContext">composition context</see> factory used by the service provider.</param>
        [CLSCompliant( false )]
        public CompositionContextServiceProvider( Func<CompositionContext> factory )
        {
            Arg.NotNull( factory, nameof( factory ) );
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
                return factory();
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
            Arg.NotNull( serviceType, nameof( serviceType ) );

            var generator = new ServiceTypeDisassembler();
            var key = generator.ExtractKey( serviceType );
            Type innerServiceType;

            if ( generator.IsForMany( serviceType, out innerServiceType ) )
            {
                var exports = new List<object>();

                if ( key == null )
                {
                    if ( IServiceContainerType.Equals( innerServiceType ) || IServiceProviderType.Equals( innerServiceType ) )
                        exports.Add( this );
                }

                var services = base.GetService( serviceType ) as IEnumerable<object>;

                if ( services != null )
                    exports.AddRange( services );

                exports.AddRange( Context.GetExports( innerServiceType, key ) );
                return exports;
            }

            if ( key == null && ( IServiceContainerType.Equals( serviceType ) || IServiceProviderType.Equals( serviceType ) ) )
                return this;

            var service = base.GetService( serviceType );

            if ( service != null )
                return service;

            object export;
            Context.SafeTryGetExport( serviceType, key, out export );

            return export;
        }
    }
}
