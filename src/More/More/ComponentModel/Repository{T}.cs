namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a repository of items.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
    [ContractClass( typeof( RepositoryContract<> ) )]
    public abstract class Repository<T> : ObservableObject, IRepository<T> where T : class
    {
        readonly IUnitOfWork<T> unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork{T}">unit of work</see> used to manage changes to items in the repository.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected Repository( IUnitOfWork<T> unitOfWork )
        {
            Arg.NotNull( unitOfWork, nameof( unitOfWork ) );
            this.unitOfWork = unitOfWork;
            this.unitOfWork.PropertyChanged += OnUnitOfWorkPropertyChanged;
        }

        /// <summary>
        /// Gets the unit of work used to add and remove items to the repository.
        /// </summary>
        /// <value>The <see cref="IUnitOfWork{T}">unit of work</see> used to add and remove items.</value>
        protected IUnitOfWork<T> UnitOfWork
        {
            get
            {
                Contract.Ensures( unitOfWork != null );
                return unitOfWork;
            }
        }

        void OnUnitOfWorkPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( string.IsNullOrEmpty( e.PropertyName ) || e.PropertyName == nameof( HasPendingChanges ) )
            {
                OnPropertyChanged( e );
            }
        }

        /// <summary>
        /// Retrieves all items in the repository satisfied by the specified query asynchronously.
        /// </summary>
        /// <param name="queryShaper">The <see cref="Func{T1,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IEnumerable{T}">sequence</see> of <typeparamref name="T">items</typeparamref>.</returns>
        [SuppressMessage( "Microsoft.Contracts", "CC1055", Justification = "Validation provided by inherited type." )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        public abstract Task<IEnumerable<T>> GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken );

        /// <summary>
        /// Retrieves a query result asynchronously.
        /// </summary>
        /// <typeparam name="TResult">The <see cref="Type">type</see> of result to retrieve.</typeparam>
        /// <param name="queryShaper">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <typeparamref name="TResult">result</typeparamref> of the operation.</returns>
        [SuppressMessage( "Microsoft.Contracts", "CC1055", Justification = "Validation provided by inherited type." )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        public abstract Task<TResult> GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken );

        /// <summary>
        /// Gets a value indicating whether there are any pending, uncommitted changes.
        /// </summary>
        /// <value>True if there are any pending uncommitted changes; otherwise, false.</value>
        public virtual bool HasPendingChanges => UnitOfWork.HasPendingChanges;

        /// <summary>
        /// Adds a new item to the repository.
        /// </summary>
        /// <param name="item">The new item to add.</param>
        public virtual void Add( T item )
        {
            Arg.NotNull( item, nameof( item ) );
            UnitOfWork.RegisterNew( item );
        }

        /// <summary>
        /// Removes an item from the repository.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public virtual void Remove( T item )
        {
            Arg.NotNull( item, nameof( item ) );
            UnitOfWork.RegisterRemoved( item );
        }

        /// <summary>
        /// Updates an existing item in the repository.
        /// </summary>
        /// <param name="item">The item to update.</param>
        public virtual void Update( T item )
        {
            Arg.NotNull( item, nameof( item ) );
            UnitOfWork.RegisterChanged( item );
        }

        /// <summary>
        /// Discards all changes to the items within the repository, if any.
        /// </summary>
        public virtual void DiscardChanges() => UnitOfWork.Rollback();

        /// <summary>
        /// Saves all pending changes in the repository asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the save operation.</returns>
        public virtual Task SaveChangesAsync( CancellationToken cancellationToken ) => UnitOfWork.CommitAsync( cancellationToken );
    }
}