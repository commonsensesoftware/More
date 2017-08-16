namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Text.RegularExpressions;
    using static System.Text.RegularExpressions.RegexOptions;

    /// <summary>
    /// Represents a rule for validating an email address.
    /// </summary>
    public class EmailRule : IRule<Property<string>, IValidationResult>
    {
        const string Pattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
        static readonly Regex regex = new Regex( Pattern, ExplicitCapture | IgnoreCase );
        readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailRule"/> class.
        /// </summary>
        public EmailRule() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailRule"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public EmailRule( string errorMessage )
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

            var message = errorMessage ?? ValidationMessage.EmailInvalid.FormatDefault( item.Name );
            return new ValidationResult( message, item.Name );
        }
    }
}