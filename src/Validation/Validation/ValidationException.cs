namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the exception that is thrown when a validation error occurs.
    /// </summary>
    public class ValidationException : Exception
    {
        private readonly Lazy<IValidationResult> validationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException()
        {
            this.validationResult = new Lazy<IValidationResult>( () => new ValidationResult( (string) null ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ValidationException( string message )
            : base( message )
        {
            this.validationResult = new Lazy<IValidationResult>( () => new ValidationResult( message ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The <see cref="Exception">exception</see> that caused the current exception.</param>
        public ValidationException( string message, Exception innerException )
            : base( message, innerException )
        {
            this.validationResult = new Lazy<IValidationResult>( () => new ValidationResult( message ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="validationResult">The <see cref="IValidationResult">validation result</see> associated with the exception.</param>
        /// <param name="value">The value that caused the exception.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public ValidationException( IValidationResult validationResult, object value )
            : base( validationResult.ErrorMessage )
        {
            Arg.NotNull( validationResult, "validationResult" );
            this.validationResult = new Lazy<IValidationResult>( () => validationResult );
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="errorMessage">The validation error message.</param>
        /// <param name="value">The value that caused the exception.</param>
        public ValidationException( string errorMessage, object value )
            : base( errorMessage )
        {
            this.validationResult = new Lazy<IValidationResult>( () => new ValidationResult( errorMessage ) );
            this.Value = value;
        }

        /// <summary>
        /// Gets the validation result associated with the exception.
        /// </summary>
        /// <value>The associated <see cref="IValidationResult">validation result</see>.</value>
        public IValidationResult ValidationResult
        {
            get
            {
                Contract.Ensures( Contract.Result<IValidationResult>() != null );
                return this.validationResult.Value;
            }
        }

        /// <summary>
        /// Gets the value that caused the validation exception.
        /// </summary>
        /// <value>The value that caused the validation exception or <c>null</c>.</value>
        public object Value
        {
            get;
            private set;
        }
    }
}
