namespace More.ComponentModel
{
    using global::System;

    /// <summary>
    /// Represents a single, self-evaluating rule.
    /// </summary>
    /// <typeparam name="TInput">The <see cref="Type">type</see> to evaluate.</typeparam>
    /// <typeparam name="TOutput">The <see cref="Type">type</see> result of the evaluation.</typeparam>
    public interface IRule<in TInput, out TOutput>
    {
        /// <summary>
        /// Evaluates the specified item and returns the result of the evaluation.
        /// </summary>
        /// <param name="item">The <typeparamref name="TInput">item</typeparamref> to evaluate.</param>
        /// <returns>The <typeparamref name="TOutput">result</typeparamref> of the evaluation.</returns>
        TOutput Evaluate( TInput item );
    }
}
