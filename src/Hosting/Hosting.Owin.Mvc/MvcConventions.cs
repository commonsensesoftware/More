﻿namespace More.Composition.Hosting
{
    using ComponentModel;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using System.Web.Mvc;

    internal sealed class MvcConventions
    {
        private static readonly IRule<ImportParameter> DefaultImportParameterRule = new Rule<ImportParameter>( DefaultAction.None );

        internal IRule<ImportParameter> ImportParameterRule
        {
            get;
            set;
        } = DefaultImportParameterRule;

        internal void Configure( ContainerConfiguration configuration, ConventionBuilder conventions )
        {
            Contract.Requires( configuration != null );
            Contract.Requires( conventions != null );

            // export mvc controllers
            var builder = conventions.ForTypesDerivedFrom<IController>().Export();

            // if the default parameter rule is applied, then we're done
            if ( ImportParameterRule == DefaultImportParameterRule )
                return;

            var constructorRule = new ConstructorSelectionRule();

            builder.SelectConstructor( constructorRule.Evaluate, ( p, b ) => ImportParameterRule.Evaluate( new ImportParameter( p, b ) ) );
        }
    }
}
