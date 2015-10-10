namespace More.Composition
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a <see cref="IRule{T}">rule</see> for composing <see cref="ImportParameter">import parameters</see>.
    /// </summary>
    /// <remarks>This rule can be used to apply the contract name "Decorated" to matching import <see cref="Type">types</see>.</remarks>
    public class DecoratedParameterRule : IRule<ImportParameter>
    {
        private readonly HashSet<TypeInfo> decoratedTypes = new HashSet<TypeInfo>();
        private string contractName = "Decorated";

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratedParameterRule"/> class.
        /// </summary>
        /// <param name="decoratedTypes">The <see cref="Array">array</see> of <see cref="Type">types</see> to be decorated.</param>
        public DecoratedParameterRule( params Type[] decoratedTypes )
        {
            Arg.NotNull( decoratedTypes, nameof( decoratedTypes ) );
            this.decoratedTypes.AddRange( decoratedTypes.Select( t => t.GetTypeInfo() ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratedParameterRule"/> class.
        /// </summary>
        /// <param name="decoratedTypes">The <see cref="IEnumerable{T}">sequence</see> of <see cref="Type">types</see> to be decorated.</param>
        public DecoratedParameterRule( IEnumerable<Type> decoratedTypes )
        {
            Arg.NotNull( decoratedTypes, nameof( decoratedTypes ) );
            this.decoratedTypes.AddRange( decoratedTypes.Select( t => t.GetTypeInfo() ) );
        }

        /// <summary>
        /// Gets or sets the contract name applied to decorated parameters.
        /// </summary>
        /// <value>The contract name applied to decorated parameters. The default value is "Decorated".</value>
        public string ContractName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(contractName) );
                return contractName;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                contractName = value;
            }
        }

        /// <summary>
        /// Gets the collection of decorated types.
        /// </summary>
        /// <value>A <see cref="ICollection{T}">collection</see> of decorated <see cref="TypeInfo">types</see>.</value>
        public ICollection<TypeInfo> DecoratedTypes
        {
            get
            {
                Contract.Ensures( decoratedTypes != null );
                return decoratedTypes;
            }
        }

        /// <summary>
        /// Evaluates the rule using the specified parameter and convention builder.
        /// </summary>
        /// <param name="parameter">The imported <see cref="ParameterInfo">parameter</see>.</param>
        /// <param name="conventionBuilder">The <see cref="ImportConventionBuilder">convention builder</see> for the parameter.</param>
        [CLSCompliant( false )]
        public void Evaluate( ParameterInfo parameter, ImportConventionBuilder conventionBuilder ) =>
            Evaluate( new ImportParameter( parameter, conventionBuilder ) );

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public virtual void Evaluate( ImportParameter item )
        {
            Arg.NotNull( item, nameof( item ) );

            var type = item.Parameter.ParameterType.GetTypeInfo();

            if ( type.IsGenericType )
            {
                if ( type.IsGenericTypeDefinition )
                {
                    // match generic type definitions
                    var matches = from decoratedType in decoratedTypes
                                  where decoratedType.IsGenericTypeDefinition &&
                                        decoratedType.IsAssignableFrom( type )
                                  select decoratedType;

                    if ( !matches.Any() )
                        return;
                }
                else
                {
                    // the import is generic, but not a generic type definition
                    // match the type or it's type definition against decorated types
                    var typeDef = type.GetGenericTypeDefinition().GetTypeInfo();
                    var matches = from decoratedType in decoratedTypes
                                  where decoratedType.IsAssignableFrom( type ) ||
                                        decoratedType.IsAssignableFrom( typeDef )
                                  select decoratedType;

                    if ( !matches.Any() )
                        return;
                }
            }
            else
            {
                // match assignable types
                if ( !decoratedTypes.Any( t => t.IsAssignableFrom( type ) ) )
                    return;
            }

            // there must be at least one match, so apply the decorated contract name
            item.ConventionBuilder.AsContractName( ContractName );
        }
    }
}
