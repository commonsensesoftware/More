namespace More.Web.Http.Tracing
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Services;

    internal sealed class ReadOnlyRepositoryAdapter<T> : ObservableObject, IRepository<T>, IDecorator<IReadOnlyRepository<T>> where T : class
    {
        private readonly IReadOnlyRepository<T> repository;

        internal ReadOnlyRepositoryAdapter( IReadOnlyRepository<T> repository )
        {
            Contract.Requires( repository != null );
            this.repository = repository;
        }

        public void Add( T item )
        {
        }

        public void DiscardChanges()
        {
        }

        public bool HasPendingChanges
        {
            get
            {
                return false;
            }
        }

        public void Remove( T item )
        {
        }

        public Task SaveChangesAsync( CancellationToken cancellationToken )
        {
            return Task.Run( (Action) DefaultAction.None );
        }

        public void Update( T item )
        {
        }

        public Task<TResult> GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken )
        {
            return this.repository.GetAsync( queryShaper, cancellationToken );
        }

        public Task<IEnumerable<T>> GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken )
        {
            return this.repository.GetAsync( queryShaper, cancellationToken );
        }

        public IReadOnlyRepository<T> Inner
        {
            get
            {
                return this.repository;
            }
        }
    }
}
