namespace More.Web.Mvc
{
    using Composition.Hosting;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Represents a <see cref="Host">hosted</see> filter attribute provider.
    /// </summary>
    public class MvcFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        readonly Func<Host> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcFilterAttributeFilterProvider"/> class.
        /// </summary>
        /// <param name="hostFactory">The <see cref="Func{TResult}">factory method</see> used to resolve the <see cref="Host">host</see> associated with the filter provider.</param>
        [CLSCompliant( false )]
        public MvcFilterAttributeFilterProvider( Func<Host> hostFactory ) : base( cacheAttributeInstances: false )
        {
            Arg.NotNull( hostFactory, nameof( hostFactory ) );
            factory = hostFactory;
        }

        /// <summary>
        /// Returns a sequence of action filter attributes.
        /// </summary>
        /// <param name="controllerContext">The <see cref="ControllerContext">controller context</see> get attributes for.</param>
        /// <param name="actionDescriptor">The <see cref="ActionDescriptor">action descriptor</see> to get attributes for.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of <see cref="FilterAttribute">filter attributes</see>.</returns>
        protected override IEnumerable<FilterAttribute> GetActionAttributes( ControllerContext controllerContext, ActionDescriptor actionDescriptor )
        {
            Contract.Assume( controllerContext != null );
            Contract.Assume( actionDescriptor != null );

            var attributes = base.GetActionAttributes( controllerContext, actionDescriptor ).ToArray();
            attributes.ForEach( factory().Compose );

            return attributes;
        }

        /// <summary>
        /// Returns a sequence of controller filter attributes.
        /// </summary>
        /// <param name="controllerContext">The <see cref="ControllerContext">controller context</see> get attributes for.</param>
        /// <param name="actionDescriptor">The <see cref="ActionDescriptor">action descriptor</see> to get attributes for.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of <see cref="FilterAttribute">filter attributes</see>.</returns>
        protected override IEnumerable<FilterAttribute> GetControllerAttributes( ControllerContext controllerContext, ActionDescriptor actionDescriptor )
        {
            Contract.Assume( controllerContext != null );
            Contract.Assume( actionDescriptor != null );

            var attributes = base.GetControllerAttributes( controllerContext, actionDescriptor ).ToArray();
            attributes.ForEach( factory().Compose );

            return attributes;
        }
    }
}