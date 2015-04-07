namespace More.ComponentModel
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    /// <summary>
    /// Defines the behavior for a unit of work.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to perform work on.</typeparam>
    [ContractClass( typeof( IUnitOfWorkContract<> ) )]
    public interface IUnitOfWork<T> : INotifyPropertyChanged where T : class
    {
       /// <summary>
        /// Gets a value indicating whether there are any pending, uncommitted changes.
        /// </summary>
        /// <value>True if there are any pending uncommitted changes; otherwise, false.</value>
        bool HasPendingChanges
        {
            get;
        }

        /// <summary>
        /// Registers a new item.
        /// </summary>
        /// <param name="item">The new item to register.</param>
        void RegisterNew( T item );

        /// <summary>
        /// Registers a changed item.
        /// </summary>
        /// <param name="item">The changed item to register.</param>
        void RegisterChanged( T item );

        /// <summary>
        /// Registers a removed item.
        /// </summary>
        /// <param name="item">The removed item to register.</param>
        void RegisterRemoved( T item );

        /// <summary>
        /// Unregisters an item.
        /// </summary>
        /// <param name="item">The item to unregister.</param>
        void Unregister( T item );

        /// <summary>
        /// Rolls back all pending units of work.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Commits all pending units of work asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the commit operation.</returns>
        Task CommitAsync( CancellationToken cancellationToken );
    }
}
