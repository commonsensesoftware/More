namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Text.RegularExpressions;
    using static System.Text.RegularExpressions.RegexOptions;

    /// <summary>
    /// Represents a rule for validating a telephone number.
    /// </summary>
    public class PhoneRule : IRule<Property<string>, IValidationResult>
    {
        const string Pattern = @"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$";
        static readonly Regex regex = new Regex( Pattern, ExplicitCapture | IgnoreCase );
        readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneRule"/> class.
        /// </summary>
        public PhoneRule() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneRule"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public PhoneRule( string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<string> item )
        {
            if ( item?.Value == null || regex.IsMatch( item.Value ) )
            {
                return ValidationResult.Success;
            }

            var message = errorMessage ?? ValidationMessage.PhoneInvalid.FormatDefault( item.Name );
            return new ValidationResult( message, item.Name );
        }
    }
}