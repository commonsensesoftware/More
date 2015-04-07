namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    [ContractClassFor( typeof( IReadOnlyRepository<> ) )]
    internal abstract class IReadOnlyRepositoryContract<T> : IReadOnlyRepository<T> where T : class
    {
        Task<IEnumerable<T>> IReadOnlyRepository<T>.GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( queryShaper != null, "queryShaper" );
            Contract.Ensures( Contract.Result<Task<IEnumerable<T>>>() != null );
            return null;
        }

        Task<TResult> IReadOnlyRepository<T>.GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( queryShaper != null, "queryShaper" );
            Contract.Ensures( Contract.Result<Task<TResult>>() != null );
            return null;
        }
    }
}
