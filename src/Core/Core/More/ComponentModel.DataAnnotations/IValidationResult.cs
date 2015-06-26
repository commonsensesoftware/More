namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a container for the results of a validation request.
    /// </summary>
     [ContractClass( typeof( IValidationResultContract ) )]
    public interface IValidationResult
    {
        /// <summary>
        /// Gets or sets the error message for the validation.
        /// </summary>
        /// <value>The error message for the validation.</value>
        string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of member names that indicate which fields have validation errors.
        /// </summary>
        /// <value>The collection of member names that indicate which fields have validation errors.</value>
        IEnumerable<string> MemberNames
        {
            get;
        }
    }
}
