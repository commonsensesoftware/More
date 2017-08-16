namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the base implementation for a unit of work factory.
    /// </summary>
    public abstract class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        readonly object syncRoot = new object();
        readonly ISpecification<Type> specification;
        readonly Dictionary<Type, Delegate> factories = new Dictionary<Type, Delegate>();
        readonly Dictionary<Type, object> unitsOfWork = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactory"/> class.
        /// </summary>
        protected UnitOfWorkFactory() => specification = new Specification<Type>( type => factories.ContainsKey( type ) );

        /// <summary>
        /// Registers a factory method for a type of unit of work.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to register a unit of work factory for.</typeparam>
        /// <param name="factory">A <see cref="Func{T}">function</see> representing the factory method to create units of work.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        protected void RegisterFactoryMethod<TItem>( Func<IUnitOfWork<TItem>> factory ) where TItem : class
        {
            Arg.NotNull( factory, nameof( factory ) );
            factories[typeof( TItem )] = factory;
        }

        /// <summary>
        /// Gets the specification associated with the factory.
        /// </summary>
        /// <value>A <see cref="ISpecification{T}"/> object.</value>
        public virtual ISpecification<Type> Specification
        {
            get
            {
                Contract.Ensures( Contract.Result<ISpecification<Type>>() != null );
                return specification;
            }
        }

        /// <summary>
        /// Returns a new unit of work.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to create a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <returns>A <see cref="IUnitOfWork{T}">unit of work</see>.</returns>
        public virtual IUnitOfWork<TItem> Create<TItem>() where TItem : class
        {
            if ( !Specification.IsSatisfiedBy( typeof( TItem ) ) )
            {
                throw new InvalidOperationException();
            }

            Contract.Ensures( Contract.Result<IUnitOfWork<TItem>>() != null );

            var type = typeof( TItem );
            var factory = factories[type];
            var createUnitOfWork = (Func<IUnitOfWork<TItem>>) factory;

            return createUnitOfWork();
        }

        /// <summary>
        /// Returns the current unit of work.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to retrieve a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <returns>A <see cref="IUnitOfWork{T}">unit of work</see>.</returns>
        [SuppressMessage( "Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "False positive. The variable 'current' is only cast once in each code path." )]
        public virtual IUnitOfWork<TItem> GetCurrent<TItem>() where TItem : class
        {
            if ( !Specification.IsSatisfiedBy( typeof( TItem ) ) )
            {
                throw new InvalidOperationException();
            }

            Contract.Ensures( Contract.Result<IUnitOfWork<TItem>>() != null );

            var type = typeof( TItem );

            if ( unitsOfWork.TryGetValue( type, out var current ) )
            {
                return (IUnitOfWork<TItem>) current;
            }

            IUnitOfWork<TItem> newItem;

            lock ( syncRoot )
            {
                if ( unitsOfWork.TryGetValue( type, out current ) )
                {
                    return (IUnitOfWork<TItem>) current;
                }

                unitsOfWork[type] = newItem = Create<TItem>();
            }

            return newItem;
        }

        /// <summary>
        /// Sets the current unit of work for a given type.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to set a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <param name="unitOfWork">The current <see cref="IUnitOfWork{T}">unit of work</see>.</param>
        public void SetCurrent<TItem>( IUnitOfWork<TItem> unitOfWork ) where TItem : class
        {
            Arg.NotNull( unitOfWork, nameof( unitOfWork ) );

            if ( !Specification.IsSatisfiedBy( typeof( TItem ) ) )
            {
                throw new InvalidOperationException();
            }

            Contract.EndContractBlock();

            lock ( syncRoot )
            {
                unitsOfWork[typeof( TItem )] = unitOfWork;
            }
        }
    }
}