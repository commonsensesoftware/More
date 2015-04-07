namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Threading;
    using global::System.Threading.Tasks;

    /// <summary>
    /// Defines the behavior of a read-only repository of items.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
    [ContractClass( typeof( IReadOnlyRepositoryContract<> ) )]
    public interface IReadOnlyRepository<T> where T : class
    {

        /// <summary>
        /// Retrieves all items in the repository satisfied by the specified query asynchronously.
        /// </summary>
        /// <param name="queryShaper">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IEnumerable{T}">sequence</see>
        /// of <typeparamref name="T">items</typeparamref>.</returns>
        /// <example>The following example demonstrates how to paginate the items in a repository using an example query.
        /// <code><![CDATA[
        /// using global::System;
        /// using global::System.Collections.Generic;
        /// using global::System.Linq;
        /// using global::System.Threading;
        /// using global::System.Threading.Tasks;
        /// 
        /// public async static void Main()
        /// {
        ///     var index = 0;
        ///     var repository = new MyRepository();
        ///     var cancellationToken = new CancellationToken();
        ///     
        ///     foreach ( var item in await repository.GetAsync( q => q.Where( i => i.FirstName.StartsWith( "Jo" ) ).OrderBy( i => i.LastName ), cancellationToken );
        ///         Console.WriteLine( i => i.ToString() );
        /// }
        /// ]]>
        /// </code></example>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        Task<IEnumerable<T>> GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken );

        /// <summary>
        /// Retrieves a query result asynchronously.
        /// </summary>
        /// <typeparam name="TResult">The <see cref="Type">type</see> of result to retrieve.</typeparam>
        /// <param name="queryShaper">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <typeparamref name="TResult">result</typeparamref> of the operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        Task<TResult> GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken );
    }
}
