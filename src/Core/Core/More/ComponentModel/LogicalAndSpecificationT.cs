namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts; 

    /// <summary>
    /// Represents a specification that models a logical 'And' expression <seealso cref="Specification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to evaluate.</typeparam>
    public class LogicalAndSpecification<T> : SpecificationBase<T>
    {
        private readonly ISpecification<T> left;
        private readonly ISpecification<T> right;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalAndSpecification{T}"/> class.
        /// </summary>
        /// <param name="left">The <see cref="ISpecification{T}">specification</see> representing the left-hand side of the unioned specification.</param>
        /// <param name="right">The <see cref="ISpecification{T}">specification</see> representing the right-hand side of the unioned specification.</param>
        public LogicalAndSpecification( ISpecification<T> left, ISpecification<T> right )
        {
            Arg.NotNull( left, nameof( left ) );
            Arg.NotNull( right, nameof( right ) );
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Gets the left-hand side of the unioned specification.
        /// </summary>
        /// <value>An <see cref="ISpecification{T}">specification</see> object.</value>
        protected ISpecification<T> Left
        {
            get
            {
                Contract.Ensures( Contract.Result<ISpecification<T>>() != null );
                Contract.Assume( left != null ); 
                return left;
            }
        }

        /// <summary>
        /// Gets the right-hand side of the unioned specification.
        /// </summary>
        /// <value>An <see cref="ISpecification{T}">specification</see> object.</value>
        protected ISpecification<T> Right
        {
            get
            {
                Contract.Ensures( Contract.Result<ISpecification<T>>() != null );
                Contract.Assume( right != null ); 
                return right;
            }
        }

        /// <summary>
        /// Determines whether the specified item satisfies the specification.
        /// </summary>
        /// <param name="item">The item of <typeparamref name="T"/> to evaluate.</param>
        /// <returns>True if <paramref name="item"/> satisfies the specification; otherwise, false.</returns>
        public override bool IsSatisfiedBy( T item )
        {
            return Left.IsSatisfiedBy( item ) && Right.IsSatisfiedBy( item );
        }
    }
}
