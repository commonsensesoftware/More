namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="IUnitOfWork{T}"/> class.
    /// </summary>
    public static class IUnitOfWorkExtensions
    {
        /// <summary>
        /// Commits all pending units of work asynchronously.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the unit of work.</typeparam>
        /// <param name="unitOfWork">The extended <see cref="IUnitOfWork{T}">unit of work</see>.</param>
        /// <returns>A <see cref="Task">task</see> representing the commit operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task CommitAsync<T>( this IUnitOfWork<T> unitOfWork ) where T : class
        {
            Arg.NotNull( unitOfWork, nameof( unitOfWork ) );
            Contract.Ensures( Contract.Result<Task>() != null );
            return unitOfWork.CommitAsync( CancellationToken.None );
        }
    }
}
