namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a validation result.
    /// </summary>
    [DebuggerDisplay( "{ErrorMessage}" )]
    public class ValidationResult : IValidationResult
    {
        private readonly IEnumerable<string> memberNames;
        private string errorMessage;

        /// <summary>
        /// Gets the <see cref="ValidationResult"/> for success.
        /// </summary>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The value is immutable" )]
        public static readonly ValidationResult Success = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errorMessage">The validation error message.</param>
        /// <param name="memberNames">An <see cref="Array">array</see> invalid member names.</param>
        public ValidationResult( string errorMessage, params string[] memberNames )
        {
            Arg.NotNull( memberNames, nameof( memberNames ) );
            this.errorMessage = errorMessage;
            this.memberNames = memberNames;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errorMessage">The validation error message.</param>
        /// <param name="memberNames">A <see cref="IEnumerable{T}">sequence</see> invalid member names.</param>
        public ValidationResult( string errorMessage, IEnumerable<string> memberNames )
        {
            Arg.NotNull( memberNames, nameof( memberNames ) );
            this.errorMessage = errorMessage;
            this.memberNames = memberNames;
        }

        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        /// <value>The validation error message.</value>
        public virtual string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
            }
        }

        /// <summary>
        /// Gets the names of the members with validation errors.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}">sequence</see> of member names.</value>
        public IEnumerable<string> MemberNames
        {
            get
            {
                return memberNames;
            }
        }
    }
}
