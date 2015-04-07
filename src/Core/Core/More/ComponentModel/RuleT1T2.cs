namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.Contracts; 

    /// <summary>
    /// Represents a rule that executes a user-defined callback <see cref="Func{T1,TResult}">function</see>.
    /// </summary>
    /// <typeparam name="TInput">The <see cref="Type">type</see> to evaluate.</typeparam>
    /// <typeparam name="TOutput">The resultant <see cref="Type">type</see> of the evaluation.</typeparam>
    public sealed class Rule<TInput, TOutput> : IRule<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> evaluate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule{TInput,TOutput}"/> class.
        /// </summary>
        /// <param name="evaluate">The callback <see cref="Func{T1,TResult}">function</see> to evaluate.</param>
        public Rule( Func<TInput, TOutput> evaluate )
        {
            Contract.Requires<ArgumentNullException>( evaluate != null, "evaluate" );
            this.evaluate = evaluate;
        }

        /// <summary>
        /// Evaluates the specified item and returns the result of the evaluation.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        public TOutput Evaluate( TInput item )
        {
            return this.evaluate( item );
        }
    }
}
