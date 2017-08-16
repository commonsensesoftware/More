namespace More.ComponentModel.DataAnnotations
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a rule for validating string length.
    /// </summary>
    public class StringLengthRule : IRule<Property<string>, IValidationResult>
    {
        readonly int minimumLength;
        readonly int maximumLength;
        readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthRule"/> class.
        /// </summary>
        /// <param name="maximumLength">The maximum length of a string allowed.</param>
        public StringLengthRule( int maximumLength )
        {
            Arg.GreaterThanOrEqualTo( maximumLength, 0, nameof( maximumLength ) );

            this.maximumLength = maximumLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthRule"/> class.
        /// </summary>
        /// <param name="maximumLength">The maximum length of a string allowed.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public StringLengthRule( int maximumLength, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            Arg.GreaterThanOrEqualTo( maximumLength, 0, nameof( maximumLength ) );

            this.maximumLength = maximumLength;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthRule"/> class.
        /// </summary>
        /// <param name="minimumLength">The minimum length of a string allowed.</param>
        /// <param name="maximumLength">The maximum length of a string allowed.</param>
        public StringLengthRule( int minimumLength, int maximumLength )
        {
            Arg.GreaterThanOrEqualTo( minimumLength, 0, nameof( minimumLength ) );
            Arg.GreaterThanOrEqualTo( maximumLength, minimumLength, nameof( maximumLength ) );

            this.minimumLength = minimumLength;
            this.maximumLength = maximumLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthRule"/> class.
        /// </summary>
        /// <param name="minimumLength">The minimum length of a string allowed.</param>
        /// <param name="maximumLength">The maximum length of a string allowed.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public StringLengthRule( int minimumLength, int maximumLength, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            Arg.GreaterThanOrEqualTo( minimumLength, 0, nameof( minimumLength ) );
            Arg.GreaterThanOrEqualTo( maximumLength, minimumLength, nameof( maximumLength ) );

            this.minimumLength = minimumLength;
            this.maximumLength = maximumLength;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the minimum length of a string allowed.
        /// </summary>
        /// <value>The minimum length of a string allowed.</value>
        public int MinimumLength
        {
            get
            {
                Contract.Ensures( minimumLength >= 0 );
                return minimumLength;
            }
        }

        /// <summary>
        /// Gets the maximum length of a string allowed.
        /// </summary>
        /// <value>The maximum length of a string allowed.</value>
        public int MaximumLength
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= MinimumLength );
                return maximumLength;
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

            var length = item.Value.Length;

            if ( length >= MinimumLength && length <= MaximumLength )
            {
                return ValidationResult.Success;
            }

            var message = default( string );

            if ( MinimumLength == 0 )
            {
                message = errorMessage ?? ValidationMessage.StringLengthValidationError.FormatDefault( item.Name, MaximumLength );
            }
            else
            {
                message = errorMessage ?? ValidationMessage.StringLengthValidationErrorIncludingMinimum.FormatDefault( item.Name, MaximumLength, MinimumLength );
            }

            return new ValidationResult( message, item.Name );
        }
    }
}