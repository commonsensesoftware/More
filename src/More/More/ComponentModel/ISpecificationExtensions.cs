namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="ISpecification{T}"/> interface.
    /// </summary>
    public static class ISpecificationExtensions
    {
        /// <summary>
        /// Combines the current specification with the specified specification using logical 'And' semantics.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item to evaluate.</typeparam>
        /// <param name="specification">The left-hand side <see cref="ISpecification{T}">specification</see>.</param>
        /// <param name="other">The <see cref="ISpecification{T}">specification</see> to union.</param>
        /// <returns>A unioned <see cref="ISpecification{T}">specification</see> object.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "Represents a logical AND operation." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static ISpecification<T> And<T>( this ISpecification<T> specification, Func<T, bool> other )
        {
            Arg.NotNull( specification, nameof( specification ) );
            Arg.NotNull( other, nameof( other ) );
            Contract.Ensures( Contract.Result<ISpecification<T>>() != null );

            return specification.And( new Specification<T>( other ) );
        }

        /// <summary>
        /// Combines the current specification with the specified specification using logical 'Or' semantics.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item to evaluate.</typeparam>
        /// <param name="specification">The left-hand side <see cref="ISpecification{T}">specification</see>.</param>
        /// <param name="other">The <see cref="ISpecification{T}">specification</see> to union.</param>
        /// <returns>A unioned <see cref="ISpecification{T}">specification</see> object.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Or", Justification = "Represents a logical OR operation." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static ISpecification<T> Or<T>( this ISpecification<T> specification, Func<T, bool> other )
        {
            Arg.NotNull( specification, nameof( specification ) );
            Arg.NotNull( other, nameof( other ) );
            Contract.Ensures( Contract.Result<ISpecification<T>>() != null );

            return specification.Or( new Specification<T>( other ) );
        }
    }
}