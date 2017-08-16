namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a rule for validating a credit card.
    /// </summary>
    public class CreditCardRule : IRule<Property<string>, IValidationResult>
    {
        readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardRule"/> class.
        /// </summary>
        public CreditCardRule() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardRule"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public CreditCardRule( string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            this.errorMessage = errorMessage;
        }

        static bool ValidCheckSum( string number )
        {
            Contract.Requires( number != null );

            var checksum = 0;
            var flag = false;

            for ( var i = number.Length - 1; i >= 0; i-- )
            {
                var ch = number[i];

                if ( ch == '-' || ch == ' ' )
                {
                    continue;
                }

                if ( ch < '0' || ch > '9' )
                {
                    return false;
                }

                var digit = ( ch - '0' ) * ( flag ? 2 : 1 );

                flag = !flag;

                while ( digit > 0 )
                {
                    checksum += digit % 10;
                    digit /= 10;
                }
            }

            return ( checksum % 10 ) == 0;
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<string> item )
        {
            if ( item?.Value == null || ValidCheckSum( item.Value ) )
            {
                return ValidationResult.Success;
            }

            var message = errorMessage ?? ValidationMessage.CreditCardInvalid.FormatDefault( item.Name );
            return new ValidationResult( message, item.Name );
        }
    }
}