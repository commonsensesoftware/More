namespace More.Web.Mvc
{
    using System;
    using System.Composition;
    using System.Web.Mvc;
    using static MvcModelBinderProvider;

    /// <summary>
    /// Represents the metadata about ASP.NET MVC <see cref="IModelBinder">model binder</see> exported via the Managed Extensibility Framework (MEF).
    /// </summary>
    [CLSCompliant( false )]
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true )]
    public sealed class MvcModelBinderAttribute : ExportAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MvcModelBinderAttribute"/> class.
        /// </summary>
        /// <param name="modelType">The <see cref="Type">type</see> of model the binder applies to.</param>
        public MvcModelBinderAttribute( Type modelType ) : base( GetModelBinderContractName( modelType ), typeof( IModelBinder ) ) { }

        /// <summary>
        /// Gets the type of model the binder applies to.
        /// </summary>
        /// <value>The associated model <see cref="Type">type</see>.</value>
        public Type ModelType { get; }
    }
}