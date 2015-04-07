namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior for a unit of work factory.
    /// </summary>
    [ContractClass( typeof( IUnitOfWorkFactoryContract ) )]
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// Gets the specification associated with the factory.
        /// </summary>
        /// <value>A <see cref="ISpecification{T}"/> object.</value>
        ISpecification<Type> Specification
        {
            get;
        }

        /// <summary>
        /// Returns a new unit of work.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to create a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <returns>A <see cref="IUnitOfWork{T}">unit of work</see>.</returns>
        IUnitOfWork<TItem> Create<TItem>() where TItem : class;

        /// <summary>
        /// Gets the current unit of work for a given type.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to retrieve a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <returns>A <see cref="IUnitOfWork{T}">unit of work</see>.</returns>
        IUnitOfWork<TItem> GetCurrent<TItem>() where TItem : class;

        /// <summary>
        /// Sets the current unit of work for a given type.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to set a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <param name="unitOfWork">The current <see cref="IUnitOfWork{T}">unit of work</see>.</param>
        void SetCurrent<TItem>( IUnitOfWork<TItem> unitOfWork ) where TItem : class;
    }
}
