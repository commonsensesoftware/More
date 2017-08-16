namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="ISessionStateManager"/> interface.
    /// </summary>
    public static class ISessionStateManagerExtensions
    {
        /// <summary>
        /// Restores any previously saved session state asynchronously.
        /// </summary>
        /// <param name="sessionStateManager">The extended <see cref="ISessionStateManager">session state manager</see>.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task RestoreAsync( this ISessionStateManager sessionStateManager )
        {
            Arg.NotNull( sessionStateManager, nameof( sessionStateManager ) );
            Contract.Ensures( Contract.Result<Task>() != null );
            return sessionStateManager.RestoreAsync( null );
        }
    }
}