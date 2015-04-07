namespace More.Windows.Input
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="INotifyCommandChanged"/> interface.
    /// </summary>
    public static class INotifyCommandChangedExtensions
    {
        /// <summary>
        /// Raises the <see cref="E:ICommand.CanExecuteChanged"/> event for all commands.
        /// </summary>
        /// <param name="commands">The <see cref="IEnumerable{T}">sequence</see> <see cref="INotifyCommandChanged">commands</see> to evaluate.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void RaiseCanExecuteChanged( this IEnumerable<INotifyCommandChanged> commands ) 
        {
            Contract.Requires<ArgumentNullException>( commands != null, "commands" );

            foreach ( var command in commands )
                command.RaiseCanExecuteChanged();
        }
    }
}
