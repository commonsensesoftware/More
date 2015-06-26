namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;

    /// <summary>
    /// Represents a command that support change notification.
    /// </summary>
    /// <remarks>This interface supports commands that can trigger re-evaluation of the <see cref="M:ICommand.CanExecute"/> method.</remarks>
    public interface INotifyCommandChanged : ICommand
    {
        /// <summary>
        /// Raises the <see cref="E:ICommand.CanExecuteChanged"/> event.
        /// </summary>
        [SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Allows the command to be forcibly re-evaluated from an external source." )]
        void RaiseCanExecuteChanged();

        /// <summary>
        /// Occurs when the command has been executed.
        /// </summary>
        event EventHandler<EventArgs> Executed;
    }
}
