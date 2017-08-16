namespace More.ComponentModel.DataAnnotations
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.Contracts;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a string validation rule using a regular expression.
    /// </summary>
    public class RegularExpressionRule : IRule<Property<string>, IValidationResult>
    {
        readonly string pattern;
        readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegularExpressionRule"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        public RegularExpressionRule( string pattern )
        {
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );

            this.pattern = pattern;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegularExpressionRule"/> class.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public RegularExpressionRule( string pattern, string errorMessage )
        {
            Arg.NotNullOrEmpty( pattern, nameof( pattern ) );
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );

            this.pattern = pattern;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the validation regular expression pattern.
        /// </summary>
        /// <value>The pattern to match.</value>
        public string Pattern
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( pattern ) );
                return pattern;
            }
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<string> item )
        {
            if ( item?.Value == null )
            {
                return ValidationResult.Success;
            }

            if ( Regex.IsMatch( item.Value, Pattern ) )
            {
                return ValidationResult.Success;
            }

            var message = errorMessage ?? ValidationMessage.RegexValidationError.FormatDefault( item.Name, Pattern );
            return new ValidationResult( message, item.Name );
        }
    }
}