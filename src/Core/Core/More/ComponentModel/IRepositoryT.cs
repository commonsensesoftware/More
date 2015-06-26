namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the behavior of a repository of items.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
    [ContractClass( typeof( IRepositoryContract<> ) )]
    public interface IRepository<T> : IReadOnlyRepository<T>, INotifyPropertyChanged where T : class
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
        /// Adds a new item to the repository.
        /// </summary>
        /// <param name="item">The new item to add.</param>
        void Add( T item );

        /// <summary>
        /// Removes an item from the repository.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        void Remove( T item );

        /// <summary>
        /// Updates an existing item in the repository.
        /// </summary>
        /// <param name="item">The item to update.</param>
        void Update( T item );

        /// <summary>
        /// Discards all changes to the items within the repository, if any.
        /// </summary>
        void DiscardChanges();

        /// <summary>
        /// Saves all pending changes in the repository asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the save operation.</returns>
        Task SaveChangesAsync( CancellationToken cancellationToken );
    }
}
