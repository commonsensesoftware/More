namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IRepository<> ) )]
    internal abstract class IRepositoryContract<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> IReadOnlyRepository<T>.GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken )
        {
            return null;
        }

        Task<TResult> IReadOnlyRepository<T>.GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken )
        {
            return null;
        }

        bool IRepository<T>.HasPendingChanges
        {
            get
            {
                return default( bool );
            }
        }

        void IRepository<T>.Add( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, "item" );
        }

        void IRepository<T>.Remove( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, "item" );
        }

        void IRepository<T>.Update( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, "item" );
        }

        void IRepository<T>.DiscardChanges()
        {
        }

        Task IRepository<T>.SaveChangesAsync( CancellationToken cancellationToken )
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
