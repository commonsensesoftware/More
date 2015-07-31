namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="INotifyCommandChanged"/> interface.
    /// </summary>
    public static class INotifyCommandChangedExtensions
    {
        /// <summary>
        /// Raises the <see cref="E:ICommand.CanExecuteChanged"/> event for all commands.
        /// </summary>
        /// <param name="commands">The <see cref="IEnumerable{T}">sequence</see> <see cref="INotifyCommandChanged">commands</see> to evaluate.</param>
        [SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This is an extension method." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void RaiseCanExecuteChanged( this IEnumerable<INotifyCommandChanged> commands ) 
        {
            Arg.NotNull( commands, nameof( commands ) );

            foreach ( var command in commands )
                command.RaiseCanExecuteChanged();
        }
    }
}
