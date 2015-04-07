namespace More.Composition
{
    using System;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Web.Mvc;

    /// <summary>
    /// Represents a model binder provider backed by the Managed Extensibility Framework (MEF).
    /// </summary>
    public class CompositionScopeModelBinderProvider : IModelBinderProvider
    {
        private const string ModelBinderContractNameSuffix = "++ModelBinder";
        private readonly Func<CompositionContext> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionScopeModelBinderProvider"/> class.
        /// </summary>
        /// <param name="contextFactory">The factory <see cref="Func{T}">method</see> used to retrieve the
        /// current <see cref="CompositionContext">context</see> used by the resolver.</param>
        public CompositionScopeModelBinderProvider( Func<CompositionContext> contextFactory )
        {
            Contract.Requires<ArgumentNullException>( contextFactory != null, "contextFactory" );
            this.factory = contextFactory;
        }

        /// <summary>
        /// Returns the model binder contract name for the specified model type.
        /// </summary>
        /// <param name="modelType">The <see cref="Type">type</see> of model to get the model binder contract name for.</param>
        /// <returns>The corresponding model binder contract name.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static string GetModelBinderContractName( Type modelType )
        {
            Contract.Requires<ArgumentNullException>( modelType != null, "modelType" );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
            return modelType.AssemblyQualifiedName + ModelBinderContractNameSuffix;
        }

        /// <summary>
        /// Returns the model binder for the specified model type.
        /// </summary>
        /// <param name="modelType">The <see cref="Type">type</see> of model to get the model binder for.</param>
        /// <returns>The corresponding <see cref="IModelBinder">model binder</see>.</returns>
        public IModelBinder GetBinder( Type modelType )
        {
            // LEGACY: IModelBinderProvider doesn't have a code contract
            if ( modelType == null )
                throw new ArgumentNullException( "modelType" );

            var contractName = GetModelBinderContractName( modelType );
            IModelBinder export = null;

            if ( !this.factory().TryGetExport( contractName, out export ) )
                export = null;

            return export;
        }
    }
}