namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Web.Mvc;
    using Web.Mvc;

    /// <summary>
    /// Represents the default ASP.NET MVC hosting conventions.
    /// </summary>
    public class MvcConventions
    {
        private static readonly IRule<ImportParameter> DefaultImportParameterRule = new Rule<ImportParameter>( DefaultAction.None );
        private IRule<ImportParameter> importParameterRule = DefaultImportParameterRule;

        /// <summary>
        /// Gets or sets the import parameter used in conventions for imported parameters.
        /// </summary>
        /// <value>The <see cref="IRule{T}">rule</see> applied to imported parameters.</value>
        public IRule<ImportParameter> ImportParameterRule
        {
            get
            {
                Contract.Ensures( importParameterRule != null );
                return importParameterRule;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                importParameterRule = value;
            }
        }

        /// <summary>
        /// Applies the conventions using the provided configuration and convention builder.
        /// </summary>
        /// <param name="configuration">The current <see cref="ContainerConfiguration">container configuration</see>.</param>
        /// <param name="conventions">The <see cref="ConventionBuilder">convention builder</see> used to apply the conventions.</param>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public virtual void Apply( ContainerConfiguration configuration, ConventionBuilder conventions )
        {
            Arg.NotNull( configuration, nameof( configuration ) );
            Arg.NotNull( conventions, nameof( conventions ) );

            var assembly = typeof( MvcConventions ).Assembly;
            var decorators = new InterfaceSpecification( typeof( IDecoratorFactory<> ) );

            conventions.ForTypesMatching( decorators.IsSatisfiedBy ).ExportInterfaces( t => t.Assembly == assembly );

            var builder = conventions.ForTypesDerivedFrom<IController>().Export();

            if ( ImportParameterRule == DefaultImportParameterRule )
                return;

            var constructorRule = new ConstructorSelectionRule();

            builder.SelectConstructor( constructorRule.Evaluate, ( p, b ) => ImportParameterRule.Evaluate( new ImportParameter( p, b ) ) );
        }
    }
}
