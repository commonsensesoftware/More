namespace More.Web.Mvc
{
    using Composition.Hosting;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Web.Mvc;

    /// <summary>
    /// Represents a <see cref="Host">hosted</see> model binder provider.
    /// </summary>
    public class MvcModelBinderProvider : IModelBinderProvider
    {
        private const string ModelBinderContractNameSuffix = "++ModelBinder";
        private readonly Func<Host> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcModelBinderProvider"/> class.
        /// </summary>
        /// <param name="hostFactory">The <see cref="Func{TResult}">factory method</see> used to resolve the <see cref="Host">host</see> associated with the filter provider.</param>
        [CLSCompliant( false )]
        public MvcModelBinderProvider( Func<Host> hostFactory )
        {
            Arg.NotNull( hostFactory, nameof( hostFactory ) );
            factory = hostFactory;
        }

        /// <summary>
        /// Returns the model binder contract name for the specified model type.
        /// </summary>
        /// <param name="modelType">The <see cref="Type">type</see> of model to get the model binder contract name for.</param>
        /// <returns>The corresponding model binder contract name.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static string GetModelBinderContractName( Type modelType )
        {
            Arg.NotNull( modelType, nameof( modelType ) );
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
            Arg.NotNull( modelType, nameof( modelType ) );
            var contractName = GetModelBinderContractName( modelType );
            return factory().GetService<IModelBinder>( contractName );
        }
    }
}