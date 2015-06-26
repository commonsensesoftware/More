namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the code contract for the <see cref="IUnitOfWork{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to perform work on.</typeparam>
    [ContractClassFor( typeof( IUnitOfWork<> ) )]
    internal abstract class IUnitOfWorkContract<T> : IUnitOfWork<T> where T : class
    {
        bool IUnitOfWork<T>.HasPendingChanges
        {
            get
            {
                return default( bool );
            }
        }

        void IUnitOfWork<T>.RegisterNew( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, "item" );
        }

        void IUnitOfWork<T>.RegisterChanged( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, "item" );
        }

        void IUnitOfWork<T>.RegisterRemoved( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, "item" );
        }

        void IUnitOfWork<T>.Unregister( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, "item" );
        }

        void IUnitOfWork<T>.Rollback()
        {
        }

        Task IUnitOfWork<T>.CommitAsync( CancellationToken cancellationToken )
        {
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
            }
            remove
            {
            }
        }
    }
}
