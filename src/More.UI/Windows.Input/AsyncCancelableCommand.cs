namespace More.Windows.Input
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous, cancelable command<see cref="CancelableCommand"/>.
    /// </summary>
    /// <remarks>This <see cref="INamedCommand">command</see> is a specialized variant of <see cref="NamedCommand{T}"/>
    /// which supports cancelling the execution of the command. This command is useful for scenarios such as cancelling
    /// the close of a window.</remarks>
    public class AsyncCancelableCommand : AsyncNamedCommand<CancelEventArgs>
    {
        readonly Func<CancelEventArgs, Task> executeAsyncMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCancelableCommand"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        public AsyncCancelableCommand( string name, Func<CancelEventArgs, Task> executeAsyncMethod )
            : this( null, name, executeAsyncMethod, DefaultFunc.CanExecute ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCancelableCommand"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        public AsyncCancelableCommand( string id, string name, Func<CancelEventArgs, Task> executeAsyncMethod )
            : this( id, name, executeAsyncMethod, DefaultFunc.CanExecute ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCancelableCommand"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2}"/> representing the can execute method.</param>
        public AsyncCancelableCommand( string name, Func<CancelEventArgs, Task> executeAsyncMethod, Func<CancelEventArgs, bool> canExecuteMethod )
            : this( null, name, executeAsyncMethod, canExecuteMethod ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCancelableCommand"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2}"/> representing the can execute method.</param>
        public AsyncCancelableCommand( string id, string name, Func<CancelEventArgs, Task> executeAsyncMethod, Func<CancelEventArgs, bool> canExecuteMethod )
            : base( id, name, executeAsyncMethod, canExecuteMethod ) => this.executeAsyncMethod = executeAsyncMethod;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The associated parameter with the command.</param>
        public override async void Execute( CancelEventArgs parameter )
        {
            parameter = parameter ?? new CancelEventArgs();

            await executeAsyncMethod( parameter ).ConfigureAwait( true );

            if ( !parameter.Cancel )
            {
                OnExecuted( parameter );
            }
        }
    }
}