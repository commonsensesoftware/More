﻿namespace More.ComponentModel.DataAnnotations
{
    using More.ComponentModel;
    using System;

    /// <summary>
    /// Represents a validation rule for a required value.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of value to validate.</typeparam>
    public class RequiredRule<T> : IRule<Property<T>, IValidationResult>
    {
        readonly Func<Property<T>, IValidationResult> evaluate;
        readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredRule{T}"/> class.
        /// </summary>
        public RequiredRule()
        {
            // optimize evaluation of strings and non-strings
            if ( typeof( string ).Equals( typeof( T ) ) )
            {
                evaluate = EvaluateString;
            }
            else
            {
                evaluate = EvaluateDefault;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredRule{T}"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public RequiredRule( string errorMessage ) : this()
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty strings are allowed.
        /// </summary>
        /// <value>True if empty strings are allowed; otherwise, false. The default value is <c>false</c>.</value>
        public bool AllowEmptyStrings { get; set; }

        IValidationResult EvaluateDefault( Property<T> property )
        {
            if ( property == null || property.Value == null )
            {
                var message = errorMessage ?? ValidationMessage.RequiredValidationError.FormatDefault( property.Name );
                return new ValidationResult( message, property.Name );
            }

            return ValidationResult.Success;
        }

        private IValidationResult EvaluateString( Property<T> property )
        {
            var message = default( string );

            if ( property == null )
            {
                message = errorMessage ?? ValidationMessage.RequiredValidationError.FormatDefault( property.Name );
                return new ValidationResult( message, property.Name );
            }

            var str = property.Value as string;

            if ( str == null || ( str.Length == 0 && !AllowEmptyStrings ) )
            {
                message = errorMessage ?? ValidationMessage.RequiredValidationError.FormatDefault( property.Name );
                return new ValidationResult( message, property.Name );
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<T> item ) => evaluate( item );
    }
}