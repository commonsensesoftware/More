namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts; 

    /// <summary>
    /// Represents a rule that executes a user-defined callback <see cref="Action{T}">method</see>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> to evaluate.</typeparam>
    public sealed class Rule<T> : IRule<T>
    {
        private readonly Action<T> evaluate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule{T}"/> class.
        /// </summary>
        /// <param name="evaluate">The callback <see cref="Action{T}">method</see> to evaluate.</param>
        public Rule( Action<T> evaluate )
        {
            Arg.NotNull( evaluate, "evaluate" );
            this.evaluate = evaluate;
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        public void Evaluate( T item )
        {
            this.evaluate( item );
        }
    }
}
