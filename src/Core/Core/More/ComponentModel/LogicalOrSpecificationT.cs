namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts; 

    /// <summary>
    /// Represents a specification that models a logical 'Or' expression <seealso cref="Specification{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to evaluate.</typeparam>
    public class LogicalOrSpecification<T> : SpecificationBase<T>
    {
        private readonly ISpecification<T> left;
        private readonly ISpecification<T> right;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOrSpecification{T}"/> class.
        /// </summary>
        /// <param name="left">The <see cref="ISpecification{T}">specification</see> representing the left-hand side of the unioned specification.</param>
        /// <param name="right">The <see cref="ISpecification{T}">specification</see> representing the right-hand side of the unioned specification.</param>
        public LogicalOrSpecification( ISpecification<T> left, ISpecification<T> right )
        {
            Arg.NotNull( left, "left" );
            Arg.NotNull( right, "right" );
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
                Contract.Assume( this.left != null ); 
                return this.left;
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
                Contract.Assume( this.right != null ); 
                return this.right;
            }
        }

        /// <summary>
        /// Determines whether the specified item satisfies the specification.
        /// </summary>
        /// <param name="item">The item of <typeparamref name="T"/> to evaluate.</param>
        /// <returns>True if <paramref name="item"/> satisfies the specification; otherwise, false.</returns>
        public override bool IsSatisfiedBy( T item )
        {
            return this.Left.IsSatisfiedBy( item ) || this.Right.IsSatisfiedBy( item );
        }
    }
}
