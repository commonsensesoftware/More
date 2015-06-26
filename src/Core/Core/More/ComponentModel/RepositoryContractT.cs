namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Diagnostics.Contracts;
    using System.Threading;

    [ContractClassFor( typeof( Repository<> ) )]
    internal abstract class RepositoryContract<T> : Repository<T> where T : class
    {
        public override Task<IEnumerable<T>> GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( queryShaper != null, "queryShaper" );
            Contract.Ensures( Contract.Result<Task<IEnumerable<T>>>() != null );
            return null;
        }

        public override Task<TResult> GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( queryShaper != null, "queryShaper" );
            Contract.Ensures( Contract.Result<Task<TResult>>() != null );
            return null;
        }

        protected RepositoryContract( IUnitOfWork<T> unitOfWork )
            : base( unitOfWork )
        {
        }
    }
}
