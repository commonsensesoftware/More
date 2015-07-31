namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a rule for validating a Uniform Resource Locator (URL).
    /// </summary>
    public class UrlRule : IRule<Property<string>, IValidationResult>
    {
        private readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlRule"/> class.
        /// </summary>
        /// <remarks>This constructor creates a rule to evaluate <see cref="F:UriKind.Absolute">absolute</see> URLs.</remarks>
        public UrlRule()
            : this( UriKind.Absolute )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlRule"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        /// <remarks>This constructor creates a rule to evaluate <see cref="F:UriKind.Absolute">absolute</see> URLs.</remarks>
        public UrlRule( string errorMessage )
            : this()
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlRule"/> class.
        /// </summary>
        /// <param name="kind">The <see cref="UriKind">kind</see> of URL to evaluate.</param>
        public UrlRule( UriKind kind )
        {
            Kind = kind;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlRule"/> class.
        /// </summary>
        /// <param name="kind">The <see cref="UriKind">kind</see> of URL to evaluate.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public UrlRule( UriKind kind, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );

            Kind = kind;
            this.errorMessage = errorMessage;

        }

        /// <summary>
        /// Gets the kind of URL this rule evaluates against.
        /// </summary>
        /// <value>One of the <see cref="UriKind"/> values. The default value is <see cref="F:UriKind.Absolute"/>.</value>
        public UriKind Kind
        {
            get;
            private set;
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<string> item )
        {
            Uri result;

            if ( item == null || item.Value == null || Uri.TryCreate( item.Value, Kind, out result ) )
                return ValidationResult.Success;

            var message = errorMessage ?? ValidationMessage.UrlInvalid.FormatDefault( item.Name );
            return new ValidationResult( message, item.Name );
        }
    }
}
