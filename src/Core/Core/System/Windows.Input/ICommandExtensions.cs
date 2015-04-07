namespace System.Windows.Input
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="ICommand"/> interface.
    /// </summary>
    public static class ICommandExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the command can be executed.
        /// </summary>
        /// <param name="command">The <see cref="ICommand">command</see> to evaluate.</param>
        /// <returns>True if the command can be executed; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static bool CanExecute( this ICommand command )
        {
            Contract.Requires<ArgumentNullException>( command != null, "command" );
            return command.CanExecute( null );
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The <see cref="ICommand">command</see> to execute.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void Execute( this ICommand command )
        {
            Contract.Requires<ArgumentNullException>( command != null, "command" );
            command.Execute( null );
        }
    }
}
