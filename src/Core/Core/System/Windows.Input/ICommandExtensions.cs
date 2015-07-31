namespace System.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

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
            Arg.NotNull( command, nameof( command ) );
            return command.CanExecute( null );
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The <see cref="ICommand">command</see> to execute.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void Execute( this ICommand command )
        {
            Arg.NotNull( command, nameof( command ) );
            command.Execute( null );
        }
    }
}
