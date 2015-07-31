namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Represents a filter attribute provider backed by the Managed Extensibility Framework (MEF).
    /// </summary>
    public class CompositionScopeFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        private readonly Func<CompositionContext> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionScopeFilterAttributeFilterProvider"/> class.
        /// </summary>
        /// <param name="contextFactory">The factory <see cref="Func{T}">method</see> used to retrieve the
        /// current <see cref="CompositionContext">context</see> used by the resolver.</param>
        [CLSCompliant( false )]
        public CompositionScopeFilterAttributeFilterProvider( Func<CompositionContext> contextFactory )
            : base( cacheAttributeInstances: false )
        {
            Arg.NotNull( contextFactory, nameof( contextFactory ) );
            factory = contextFactory;
        }

        /// <summary>
        /// Returns a sequence of action filter attributes.
        /// </summary>
        /// <param name="controllerContext">The <see cref="ControllerContext">controller context</see> get attributes for.</param>
        /// <param name="actionDescriptor">The <see cref="ActionDescriptor">action descriptor</see> to get attributes for.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of <see cref="FilterAttribute">filter attributes</see>.</returns>
        protected override IEnumerable<FilterAttribute> GetActionAttributes( ControllerContext controllerContext, ActionDescriptor actionDescriptor )
        {
            Debug.Assert( controllerContext != null );
            Debug.Assert( actionDescriptor != null );
            
            var attributes = base.GetActionAttributes( controllerContext, actionDescriptor ).ToList();
            attributes.ForEach( factory().SatisfyImports );
            
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
            Debug.Assert( controllerContext != null );
            Debug.Assert( actionDescriptor != null );
            
            var attributes = base.GetControllerAttributes( controllerContext, actionDescriptor ).ToList();
            attributes.ForEach( factory().SatisfyImports );
            
            return attributes;
        }
    }
}