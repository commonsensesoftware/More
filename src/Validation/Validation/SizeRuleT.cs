namespace More.ComponentModel.DataAnnotations
{
    using More.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a rule which validates the size or count of a <see cref="IEnumerable">sequence</see>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="IEnumerable">sequence</see>.</typeparam>
    public class SizeRule<T> : IRule<Property<T>, IValidationResult> where T : IEnumerable
    {
        private readonly int minimumCount;
        private readonly int maximumCount;
        private readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeRule{T}"/> class.
        /// </summary>
        /// <param name="minimumCount">The minimum count.</param>
        /// <remarks>The <see cref="P:Minimum"/> range value is the default value of <typeparamref name="T"/>.</remarks>
        public SizeRule( int minimumCount )
        {
            Arg.GreaterThanOrEqualTo( minimumCount, 0, "minimumCount" );

            this.minimumCount = minimumCount;
            this.maximumCount = int.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeRule{T}"/> class.
        /// </summary>
        /// <param name="minimumCount">The minimum count.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        /// <remarks>The <see cref="P:Minimum"/> range value is the default value of <typeparamref name="T"/>.</remarks>
        public SizeRule( int minimumCount, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Arg.GreaterThanOrEqualTo( minimumCount, 0, "minimumCount" );

            this.minimumCount = minimumCount;
            this.maximumCount = int.MaxValue;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeRule{T}"/> class.
        /// </summary>
        /// <param name="minimumCount">The minimum count.</param>
        /// <param name="maximumCount">The maximum count.</param>
        public SizeRule( int minimumCount, int maximumCount )
        {
            Arg.GreaterThanOrEqualTo( minimumCount, 0, "minimumCount" );
            Arg.GreaterThanOrEqualTo( maximumCount, minimumCount, "maximumCount" );

            this.minimumCount = minimumCount;
            this.maximumCount = maximumCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeRule{T}"/> class.
        /// </summary>
        /// <param name="minimumCount">The minimum count.</param>
        /// <param name="maximumCount">The maximum count.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public SizeRule( int minimumCount, int maximumCount, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, "errorMessage" );
            Arg.GreaterThanOrEqualTo( minimumCount, 0, "minimumCount" );
            Arg.GreaterThanOrEqualTo( maximumCount, minimumCount, "maximumCount" );

            this.minimumCount = minimumCount;
            this.maximumCount = maximumCount;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the minimum count.
        /// </summary>
        /// <value>The minimum count.</value>
        public int MinimumCount
        {
            get
            {
                Contract.Ensures( this.minimumCount >= 0 );
                return this.minimumCount;
            }
        }

        /// <summary>
        /// Gets the maximum count.
        /// </summary>
        /// <value>THe maximum count.</value>
        public int MaximumCount
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= this.MinimumCount );
                return this.maximumCount;
            }
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<T> item )
        {
            if ( item == null || item.Value == null )
                return ValidationResult.Success;

            var count = item.Value.Count();
            string message;

            if ( count < this.MinimumCount )
            {
                if ( this.MaximumCount == int.MaxValue )
                    message = this.errorMessage ?? ValidationMessage.CountValidationError.FormatDefault( item.Name, this.MinimumCount );
                else
                    message = this.errorMessage ?? ValidationMessage.CountValidationErrorIncludingMaximum.FormatDefault( item.Name, this.MinimumCount, this.MaximumCount );

                return new ValidationResult( message, item.Name );
            }
            else if ( count > this.MaximumCount )
            {
                message = this.errorMessage ?? ValidationMessage.CountValidationErrorIncludingMaximum.FormatDefault( item.Name, this.MinimumCount, this.MaximumCount );
                return new ValidationResult( message, item.Name );
            }

            return ValidationResult.Success;
        }
    }
}
