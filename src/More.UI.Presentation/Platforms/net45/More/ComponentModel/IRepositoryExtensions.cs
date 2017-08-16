namespace More.ComponentModel
{
    using More.Collections.Generic;
    using More.Windows.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Windows.Data;

    /// <summary>
    /// Provides extension methods for the <see cref="IReadOnlyRepository{T}"/> and <see cref="IRepository{T}"/> interfaces.
    /// </summary>
    public static class IRepositoryExtensions
    {
        /// <summary>
        /// Retrieves and pages all items in the repository.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the repository.</typeparam>
        /// <param name="repository">The extended <see cref="IReadOnlyRepository{T}">repository</see>.</param>
        /// <param name="pagingArgs">The <see cref="PagingArguments">paging arguments</see> for the query.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="PagedCollection{T}">paged collection</see>
        /// of <typeparamref name="T">items</typeparamref>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static Task<PagedCollection<T>> PaginateAsync<T>( this IReadOnlyRepository<T> repository, PagingArguments pagingArgs ) where T : class
        {
            Arg.NotNull( repository, nameof( repository ) );
            Arg.NotNull( pagingArgs, nameof( pagingArgs ) );
            Contract.Ensures( Contract.Result<Task<PagedCollection<T>>>() != null );

            var sortDescriptions = pagingArgs.SortDescriptions;
            var pageIndex = pagingArgs.PageIndex;
            var pageSize = pagingArgs.PageSize;
            return repository.PaginateAsync( q => q.ApplySortDescriptions( sortDescriptions ), pageIndex, pageSize );
        }
    }
}