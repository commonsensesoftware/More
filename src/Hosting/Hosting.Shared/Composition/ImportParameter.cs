namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents a composed, import parameter for a component.
    /// </summary>
    public class ImportParameter
    {
        private readonly ParameterInfo parameter;
        private readonly ImportConventionBuilder conventionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportParameter"/> class.
        /// </summary>
        /// <param name="parameter">The imported <see cref="ParameterInfo">parameter</see>.</param>
        /// <param name="conventionBuilder">The <see cref="ImportConventionBuilder">convention builder</see> for the parameter.</param>
        public ImportParameter( ParameterInfo parameter, ImportConventionBuilder conventionBuilder )
        {
            Arg.NotNull( parameter, nameof( parameter ) );
            Arg.NotNull( conventionBuilder, nameof( conventionBuilder ) );

            this.parameter = parameter;
            this.conventionBuilder = conventionBuilder;
        }

        /// <summary>
        /// Gets the imported parameter.
        /// </summary>
        /// <value>The imported <see cref="ParameterInfo">parameter</see>.</value>
        public ParameterInfo Parameter
        {

            get
            {
                Contract.Ensures( parameter != null );
                return parameter;
            }
        }

        /// <summary>
        /// Gets the convention builder for the imported parameter.
        /// </summary>
        /// <value>The <see cref="ImportConventionBuilder">convention builder</see> for the parameter.</value>
        public ImportConventionBuilder ConventionBuilder
        {
            get
            {
                Contract.Ensures( conventionBuilder != null );
                return conventionBuilder;
            }
        }
    }
}
