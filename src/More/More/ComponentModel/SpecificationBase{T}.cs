namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a single, self-evaluating rule supporting simple binary evaluation semantics.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to evaluate.</typeparam>
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificationBase{T}"/> class.
        /// </summary>
        protected SpecificationBase() { }

        /// <summary>
        /// Determines whether the specified item satisfies the specification.
        /// </summary>
        /// <param name="item">The item of <typeparamref name="T"/> to evaluate.</param>
        /// <returns>True if <paramref name="item"/> satisfies the specification; otherwise, false.</returns>
        public abstract bool IsSatisfiedBy( T item );

        /// <summary>
        /// Combines the current specification with the specified specification using logical 'And' semantics.
        /// </summary>
        /// <param name="other">The <see cref="ISpecification{T}">specification</see> to union.</param>
        /// <returns>A unioned <see cref="ISpecification{T}">specification</see> object.</returns>
        public virtual ISpecification<T> And( ISpecification<T> other )
        {
            Arg.NotNull( other, nameof( other ) );
            Contract.Ensures( Contract.Result<ISpecification<T>>() != null );
            return new LogicalAndSpecification<T>( this, other );
        }

        /// <summary>
        /// Combines the current specification with the specified specification using logical 'Or' semantics.
        /// </summary>
        /// <param name="other">The <see cref="ISpecification{T}">specification</see> to union.</param>
        /// <returns>A unioned <see cref="ISpecification{T}">specification</see> object.</returns>
        public virtual ISpecification<T> Or( ISpecification<T> other )
        {
            Arg.NotNull( other, nameof( other ) );
            Contract.Ensures( Contract.Result<ISpecification<T>>() != null );
            return new LogicalOrSpecification<T>( this, other );
        }

        /// <summary>
        /// Returns the logical complement of the specification.
        /// </summary>
        /// <returns>A <see cref="ISpecification{T}">specification</see> object.</returns>
        public virtual ISpecification<T> Not()
        {
            Contract.Ensures( Contract.Result<ISpecification<T>>() != null );
            return new LogicalNotSpecification<T>( this );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "A specification is a specialized rule. The interface is intentionally hidden." )]
        bool IRule<T, bool>.Evaluate( T item ) => IsSatisfiedBy( item );
    }
}