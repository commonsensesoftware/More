namespace More.Web.Http.Tracing
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Services;

    [PartNotDiscoverable]
    internal sealed class ReadOnlyRepositoryAdapter<T> : ObservableObject, IRepository<T>, IDecorator<IReadOnlyRepository<T>> where T : class
    {
        private static readonly Task CompletedTask = Task.FromResult( false );
        private readonly IReadOnlyRepository<T> repository;

        internal ReadOnlyRepositoryAdapter( IReadOnlyRepository<T> repository )
        {
            Contract.Requires( repository != null );
            this.repository = repository;
        }

        public void Add( T item ) => Arg.NotNull( item, nameof( item ) );

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

        public void Remove( T item ) => Arg.NotNull( item, nameof( item ) );

        public Task SaveChangesAsync( CancellationToken cancellationToken ) => CompletedTask;

        public void Update( T item ) =>Arg.NotNull( item, nameof( item ) );

        public Task<TResult> GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken )
        {
            Arg.NotNull( queryShaper, nameof( queryShaper ) );
            return repository.GetAsync( queryShaper, cancellationToken );
        }

        public Task<IEnumerable<T>> GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken )
        {
            Arg.NotNull( queryShaper, nameof( queryShaper ) );
            return repository.GetAsync( queryShaper, cancellationToken );
        }

        public IReadOnlyRepository<T> Inner
        {
            get
            {
                return repository;
            }
        }
    }
}
