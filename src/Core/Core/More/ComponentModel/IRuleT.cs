namespace More.ComponentModel
{
    using global::System;

    /// <summary>
    /// Represents a single, self-evaluating rule.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> to evaluate.</typeparam>
    public interface IRule<in T>
    {
        /// <summary>
        /// Evaluates the specified item.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        void Evaluate( T item );
    }
}
