namespace More.Windows.Input
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;

    /// <summary>
    /// Provides extension methods for the <see cref="Interaction"/> interface.
    /// </summary>
    public static class InteractionExtensions
    {
        static bool ExecuteCommand( ICommand command )
        {
            if ( command == null || !command.CanExecute() )
            {
                return false;
            }

            command.Execute();
            return true;
        }

        /// <summary>
        /// Executes the default interaction command.
        /// </summary>
        /// <param name="interaction">The extended <see cref="Interaction">interaction</see>.</param>
        /// <returns>True if the default interaction command was invoked; otherwise, false.</returns>
        /// <remarks>The default command will not execute if it is undefined or <see cref="M:ICommand.CanExecute">cannot be executed</see>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static bool ExecuteDefaultCommand( this Interaction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            return ExecuteCommand( interaction.DefaultCommand );
        }

        /// <summary>
        /// Executes the cancel command.
        /// </summary>
        /// <param name="interaction">The extended <see cref="Interaction">interaction</see>.</param>
        /// <returns>True if the cancel command was invoked; otherwise, false.</returns>
        /// <remarks>The cancel command will not execute if it is undefined or <see cref="M:ICommand.CanExecute">cannot be executed</see>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static bool ExecuteCancelCommand( this Interaction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            return ExecuteCommand( interaction.CancelCommand );
        }
    }
}