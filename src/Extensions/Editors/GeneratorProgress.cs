namespace More.VisualStudio.Editors
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the progress for a generator.
    /// </summary>
    public sealed class GeneratorProgress
    {
        private readonly int total;
        private int completed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorProgress"/> class.
        /// </summary>
        /// <param name="completed">The amount of progress complete; typically in steps.</param>
        /// <param name="total">The total number of steps to complete.</param>
        public GeneratorProgress( int completed, int total )
        {
            Arg.GreaterThanOrEqualTo( total, 0, nameof( total ) );
            Arg.InRange( completed, 0, total, nameof( completed ) );

            this.completed = completed;
            this.total = total;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorProgress"/> class.
        /// </summary>
        /// <param name="error">The <see cref="GeneratorError">error</see> that occurred.</param>
        public GeneratorProgress( GeneratorError error )
        {
            Arg.NotNull( error, nameof( error ) );
            Error = error;
        }

        /// <summary>
        /// Gets the amount of progress complete.
        /// </summary>
        /// <value>The amount of progress complete; typically in steps. The default value is 0.</value>
        /// <remarks>This property is always greater than or equal to the <see cref="P:Total">total</see>.</remarks>
        public int Completed
        {
            get
            {
                Contract.Ensures( completed >= 0 );
                Contract.Ensures( completed <= Total );
                return completed;
            }
        }

        /// <summary>
        /// Gets the total number of steps to complete.
        /// </summary>
        /// <value>The total number of steps to complete.</value>
        public int Total
        {
            get
            {
                return total;
            }
        }

        /// <summary>
        /// Gets the error that has occurred.
        /// </summary>
        /// <value>The <see cref="GeneratorError">error</see> that has occurred.  The default value is <c>null</c>.</value>
        /// <remarks>If the property is not <c>null</c>, then an error is not being reported.</remarks>
        public GeneratorError Error
        {
            get;
            private set;
        }
    }
}
