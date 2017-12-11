namespace More.ComponentModel.DataAnnotations
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a range-based validation rule.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of value to validate.</typeparam>
    public class RangeRule<T> : IRule<Property<T>, IValidationResult> where T : struct, IComparable<T>
    {
        readonly T maximum;
        readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeRule{T}"/> class.
        /// </summary>
        /// <param name="maximum">The maximum range value.</param>
        /// <remarks>The <see cref="Minimum"/> range value is the default value of <typeparamref name="T"/>.</remarks>
        public RangeRule( T maximum )
        {
            Arg.GreaterThanOrEqualTo( maximum, default( T ), nameof( maximum ) );
            this.maximum = maximum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeRule{T}"/> class.
        /// </summary>
        /// <param name="maximum">The maximum range value.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        /// <remarks>The <see cref="Minimum"/> range value is the default value of <typeparamref name="T"/>.</remarks>
        public RangeRule( T maximum, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            Arg.GreaterThanOrEqualTo( maximum, default( T ), nameof( maximum ) );

            this.maximum = maximum;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeRule{T}"/> class.
        /// </summary>
        /// <param name="minimum">The minimum range value.</param>
        /// <param name="maximum">The maximum range value.</param>
        public RangeRule( T minimum, T maximum )
        {
            Arg.GreaterThanOrEqualTo( maximum, minimum, nameof( maximum ) );

            Minimum = minimum;
            this.maximum = maximum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeRule{T}"/> class.
        /// </summary>
        /// <param name="minimum">The minimum range value.</param>
        /// <param name="maximum">The maximum range value.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public RangeRule( T minimum, T maximum, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            Arg.GreaterThanOrEqualTo( maximum, minimum, nameof( maximum ) );

            Minimum = minimum;
            this.maximum = maximum;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the minimum range value.
        /// </summary>
        /// <value>The minimum range value.</value>
        public T Minimum { get; }

        /// <summary>
        /// Gets the maximum range value.
        /// </summary>
        /// <value>THe maximum range value.</value>
        public T Maximum
        {
            get
            {
                Contract.Ensures( Contract.Result<T>().CompareTo( Minimum ) >= 0 );
                return maximum;
            }
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<T> item )
        {
            if ( item == null )
            {
                return ValidationResult.Success;
            }

            var value = item.Value;

            if ( value.CompareTo( Minimum ) < 0 || value.CompareTo( Maximum ) > 0 )
            {
                var message = errorMessage ?? ValidationMessage.RangeValidationError.FormatDefault( item.Name, Minimum, Maximum );
                return new ValidationResult( message, item.Name );
            }

            return ValidationResult.Success;
        }
    }
}