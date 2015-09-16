namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="ISuspensionManager"/> interface.
    /// </summary>
    public static class ISuspensionManagerExtensions
    {
        /// <summary>
        /// Restores any previously saved session state asynchronously.
        /// </summary>
        /// <param name="suspensionManager">The extended <see cref="ISuspensionManager">suspension manager</see>.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task RestoreAsync( this ISuspensionManager suspensionManager )
        {
            Arg.NotNull( suspensionManager, nameof( suspensionManager ) );
            Contract.Ensures( Contract.Result<Task>() != null );
            return suspensionManager.RestoreAsync( null );
        }
    }
}
