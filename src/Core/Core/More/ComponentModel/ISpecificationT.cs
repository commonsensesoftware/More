namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a single, self-evaluating rule supporting simple binary evaluation semantics using the specification pattern.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to evaluate.</typeparam>
    [ContractClass( typeof( ISpecificationContract<> ) )]
    public interface ISpecification<T> : IRule<T, bool>
    {
        /// <summary>
        /// Determines whether the specified item satisfies the specification.
        /// </summary>
        /// <param name="item">The item of <typeparamref name="T"/> to evaluate.</param>
        /// <returns>True if <paramref name="item"/> satisfies the specification; otherwise, false.</returns>
        [Pure]
        bool IsSatisfiedBy( T item );

        /// <summary>
        /// Combines the current specification with the specified specification using logical 'And' semantics.
        /// </summary>
        /// <param name="other">The <see cref="ISpecification{T}">specification</see> to union.</param>
        /// <returns>A unioned <see cref="ISpecification{T}">specification</see> object.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "Represents a logical AND operation." )]
        ISpecification<T> And( ISpecification<T> other );

        /// <summary>
        /// Combines the current specification with the specified specification using logical 'Or' semantics.
        /// </summary>
        /// <param name="other">The <see cref="ISpecification{T}">specification</see> to union.</param>
        /// <returns>A unioned <see cref="ISpecification{T}">specification</see> object.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Or", Justification = "Represents a logical OR operation." )]
        ISpecification<T> Or( ISpecification<T> other );

        /// <summary>
        /// Returns the logical compliment of the specification.
        /// </summary>
        /// <returns>A <see cref="ISpecification{T}">specification</see> object.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Not", Justification = "Represents a logical compliment operation." )]
        ISpecification<T> Not();
    }
}
