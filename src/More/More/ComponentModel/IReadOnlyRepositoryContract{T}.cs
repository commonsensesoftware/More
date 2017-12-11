namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( IReadOnlyRepository<> ) )]
    abstract class IReadOnlyRepositoryContract<T> : IReadOnlyRepository<T> where T : class
    {
        Task<IEnumerable<T>> IReadOnlyRepository<T>.GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( queryShaper != null, nameof( queryShaper ) );
            Contract.Ensures( Contract.Result<Task<IEnumerable<T>>>() != null );
            return null;
        }

        Task<TResult> IReadOnlyRepository<T>.GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( queryShaper != null, nameof( queryShaper ) );
            Contract.Ensures( Contract.Result<Task<TResult>>() != null );
            return null;
        }
    }
}