namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a container for the results of a validation request.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.ValidationResult.</remarks>
    public class ValidationResult
    {
        private readonly IEnumerable<string> memberNames;

        /// <summary>
        /// True if validation was successful; otherwise, false.
        /// </summary>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The value is not mutable." )]
        public static readonly ValidationResult Success;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class by
        /// using an error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public ValidationResult( string errorMessage )
            : this( errorMessage, null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class by
        /// using an error message and a list of members that have validation errors.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="memberNames">The list of member names that have validation errors.</param>
        public ValidationResult( string errorMessage, IEnumerable<string> memberNames )
        {
            this.ErrorMessage = errorMessage;
            this.memberNames = memberNames ?? System.Linq.Enumerable.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class by
        /// using a <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> object.
        /// </summary>
        /// <param name="validationResult">The validation result object.</param>
        protected ValidationResult( ValidationResult validationResult )
        {
            if ( validationResult == null )
                throw new ArgumentNullException( "validationResult" );

            this.ErrorMessage = validationResult.ErrorMessage;
            this.memberNames = validationResult.memberNames;
        }

        /// <summary>
        /// Gets the collection of member names that indicate which fields have validation errors.
        /// </summary>
        /// <value>The collection of member names that indicate which fields have validation errors.</value>
        public IEnumerable<string> MemberNames
        {
            get
            {
                return this.memberNames;
            }
        }

        /// <summary>
        /// Gets or sets the error message for the validation.
        /// </summary>
        /// <value>The error message for the validation.</value>
        public string ErrorMessage
        {
            get;
            set;
        }
    }
}
