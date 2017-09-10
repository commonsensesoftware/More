namespace More.Web.Http.Tracing
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Services;

    [PartNotDiscoverable]
    sealed class ReadOnlyRepositoryAdapter<T> : ObservableObject, IRepository<T>, IDecorator<IReadOnlyRepository<T>> where T : class
    {
        static readonly Task CompletedTask = Task.FromResult( false );
        readonly IReadOnlyRepository<T> repository;

        internal ReadOnlyRepositoryAdapter( IReadOnlyRepository<T> repository ) => this.repository = repository;

        public void Add( T item ) => Arg.NotNull( item, nameof( item ) );

        public void DiscardChanges() { }

        public bool HasPendingChanges => false;

        public void Remove( T item ) => Arg.NotNull( item, nameof( item ) );

        public Task SaveChangesAsync( CancellationToken cancellationToken ) => CompletedTask;

        public void Update( T item ) => Arg.NotNull( item, nameof( item ) );

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

        public IReadOnlyRepository<T> Inner => repository;
    }
}