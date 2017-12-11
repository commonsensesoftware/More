namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a unit of work factory.
    /// </summary>
    public static class UnitOfWork
    {
        /// <summary>
        /// Represents an empty unit of work factory to provider used to satisfy the Null Object pattern.
        /// </summary>
        sealed class EmptyFactoryProvider : IUnitOfWorkFactoryProvider
        {
            public IEnumerable<IUnitOfWorkFactory> Factories { get; } = Enumerable.Empty<IUnitOfWorkFactory>();
        }

        /// <summary>
        /// Represents a unit of work factory that creates uncommittable units of work for any type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> to create a unit of work for.</typeparam>
        sealed class UncommittableUnitOfWorkFactory<T> : UnitOfWorkFactory where T : class
        {
            internal UncommittableUnitOfWorkFactory() => RegisterFactoryMethod( () => new UncommittableUnitOfWork<T>() );

            public override ISpecification<Type> Specification { get; } = new Specification<Type>( t => true );
        }

        /// <summary>
        /// Represents an uncommittable unit of work.
        /// </summary>
        /// <typeparam name="T">The type of objects contained in the unit of work.</typeparam>
        /// <remarks>This class is internally used to satisfy the Null Object pattern. This allows unit testing and
        /// other scenarios without requiring a unit of work to be registered.  When an attempt is made to commit
        /// work against this class is performed, an <see cref="InvalidOperationException"/> is thrown.</remarks>
        sealed class UncommittableUnitOfWork<T> : UnitOfWork<T> where T : class
        {
            protected override bool IsNew( T item ) => true;

            public override Task CommitAsync( CancellationToken cancellationToken ) =>
                throw new InvalidOperationException( ExceptionMessage.UnmappedUnitOfWorkFactory.FormatDefault( typeof( T ) ) );
        }

        static readonly IUnitOfWorkFactoryProvider defaultProvider = new EmptyFactoryProvider();
        static IUnitOfWorkFactoryProvider provider = defaultProvider;

        /// <summary>
        /// Gets default the <see cref="IUnitOfWorkFactoryProvider">unit of work factory provider</see> used for all units of work.
        /// </summary>
        /// <value>A <see cref="IUnitOfWorkFactoryProvider">unit of work factory provider</see>.</value>
        /// <remarks>This property typically only used to evaluate whether the current <see cref="Provider">provider</see> is
        /// the default provider or to reset the current <see cref="Provider">provider</see> back to its default state.</remarks>
        public static IUnitOfWorkFactoryProvider DefaultProvider
        {
            get
            {
                Contract.Ensures( defaultProvider != null );
                return defaultProvider;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IUnitOfWorkFactoryProvider">unit of work factory provider</see> used for all units of work.
        /// </summary>
        /// <value>A <see cref="IUnitOfWorkFactoryProvider">unit of work factory provider</see>.</value>
        public static IUnitOfWorkFactoryProvider Provider
        {
            get
            {
                Contract.Ensures( provider != null );
                return provider;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                provider = value;
            }
        }

        static IUnitOfWorkFactory GetFactory<TItem>() where TItem : class
        {
            Contract.Ensures( Contract.Result<IUnitOfWorkFactory>() != null );

            var type = typeof( TItem );
            var factory = default( IUnitOfWorkFactory );

            try
            {
                factory = Provider.Factories.SingleOrDefault( f => f.Specification.IsSatisfiedBy( type ) );
            }
            catch ( InvalidOperationException )
            {
                throw new InvalidOperationException( ExceptionMessage.MultipleUnitOfWorkFactory.FormatDefault( type ) );
            }

            return factory ?? new UncommittableUnitOfWorkFactory<TItem>();
        }

        /// <summary>
        /// Creates a unit of work for a given type.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to create a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <returns>A <see cref="IUnitOfWork{T}">unit of work</see>.</returns>
        public static IUnitOfWork<TItem> Create<TItem>() where TItem : class
        {
            Contract.Ensures( Contract.Result<IUnitOfWork<TItem>>() != null );
            return GetFactory<TItem>().Create<TItem>();
        }

        /// <summary>
        /// Gets the current unit of work for a given type.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to retrieve a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <returns>A <see cref="IUnitOfWork{T}">unit of work</see>.</returns>
        public static IUnitOfWork<TItem> GetCurrent<TItem>() where TItem : class
        {
            Contract.Ensures( Contract.Result<IUnitOfWork<TItem>>() != null );
            return GetFactory<TItem>().GetCurrent<TItem>();
        }

        /// <summary>
        /// Sets the current unit of work for a given type.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to set a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <param name="unitOfWork">The current <see cref="IUnitOfWork{T}">unit of work</see>.</param>
        public static void SetCurrent<TItem>( IUnitOfWork<TItem> unitOfWork ) where TItem : class
        {
            Arg.NotNull( unitOfWork, nameof( unitOfWork ) );
            GetFactory<TItem>().SetCurrent( unitOfWork );
        }

        /// <summary>
        /// Sets and returns a new unit of work for a given type.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to create a new a <see cref="IUnitOfWork{T}">unit of work</see> for.</typeparam>
        /// <returns>The new, current <see cref="IUnitOfWork{T}">unit of work</see> for the given <typeparamref name="TItem">item</typeparamref>.</returns>
        public static IUnitOfWork<TItem> NewCurrent<TItem>() where TItem : class
        {
            Contract.Ensures( Contract.Result<IUnitOfWork<TItem>>() != null );

            var factory = GetFactory<TItem>();
            var unitOfWork = factory.Create<TItem>();

            factory.SetCurrent( unitOfWork );

            return unitOfWork;
        }
    }
}