namespace More.ComponentModel.DataAnnotations
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a validation rule for a required value.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of value to validate.</typeparam>
    public class RequiredRule<T> : IRule<Property<T>, IValidationResult>
    {
        private readonly Func<Property<T>, IValidationResult> evaluate;
        private readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredRule{T}"/> class.
        /// </summary>
        public RequiredRule()
        {
            // optimize evaluation strings and non-strings durng rule construction
            if ( typeof( string ).Equals( typeof( T ) ) )
                this.evaluate = this.EvaluateString;
            else
                this.evaluate = EvaluateDefault;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredRule{T}"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public RequiredRule( string errorMessage )
            : this()
        {
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty strings are allowed.
        /// </summary>
        /// <value>True if empty strings are allowed; otherwise, false. The default value is <c>false</c>.</value>
        public bool AllowEmptyStrings
        {
            get;
            set;
        }

        private IValidationResult EvaluateDefault( Property<T> property )
        {
            if ( property == null || property.Value == null )
            {
                var message = this.errorMessage ?? ValidationMessage.RequiredValidationError.FormatDefault( property.Name );
                return new ValidationResult( message, property.Name );
            }

            return ValidationResult.Success;
        }

        private IValidationResult EvaluateString( Property<T> property )
        {
            string message;

            if ( property == null )
            {
                message = this.errorMessage ?? ValidationMessage.RequiredValidationError.FormatDefault( property.Name );
                return new ValidationResult( message, property.Name );
            }

            var str = property.Value as string;

            if ( str == null || ( str.Length == 0 && !this.AllowEmptyStrings ) )
            {
                message = this.errorMessage ?? ValidationMessage.RequiredValidationError.FormatDefault( property.Name );
                return new ValidationResult( message, property.Name );
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<T> item )
        {
            return this.evaluate( item );
        }
    }
}
