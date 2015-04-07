namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts; 

    /// <summary>
    /// Represents a collection of symmetric <see cref="IRule{TInput,TOutput}">rules</see> that can be used
    /// as a single, symmetric composite <see cref="IRule{TInput,TOutput}">rule</see>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> to evaluate.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This type represents a composite rule that is a collection of other rules." )]
    public class CompositeRule<T> : CompositeRule<T, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeRule{T}"/> class.
        /// </summary>
        public CompositeRule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeRule{T}"/> class.
        /// </summary>
        /// <param name="rules">The initial <see cref="IEnumerator{T}">rules</see> in the set.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        public CompositeRule( IEnumerable<IRule<T, T>> rules )
            : base( rules )
        {
            Contract.Requires<ArgumentNullException>( rules != null, "rules" );
        }

        /// <summary>
        /// Evaluates the specified item and returns the result of the evaluation.
        /// </summary>
        /// <param name="item">The <typeparamref name="T">item</typeparamref> to evaluate.</param>
        /// <returns>The <typeparamref name="T">result</typeparamref> of the evaluation.</returns>
        public override T Evaluate( T item )
        {
            this.NestedRules.ForEach( rule => item = rule.Evaluate( item ) );
            return item;
        }
    }
}
