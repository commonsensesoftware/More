namespace More.Windows.Input
{
    using More.ComponentModel;
    using System.ComponentModel;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Windows.Input;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an observable, asychronous command using a delegate<seealso cref="Command{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of parameter associated with the command.</typeparam>
    /// <example>This example demonstrates how to create a command that accepts an asynchronous lambda expression to execute.
    /// <code lang="C#"><![CDATA[
    /// using System.Windows.Input;
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Input;
    /// 
    /// var command = new Command<string>( parameter => Task.Run( () => Console.WriteLine( parameter ) ) );
    /// var button = new Button();
    /// button.Command = command;
    /// ]]></code>
    /// </example>
    public class AsyncCommand<T> : ObservableObject, INotifyCommandChanged
    {
        private readonly Func<T, Task> executeAsyncMethod;
        private readonly Func<T, bool> canExecuteMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
        /// </summary>
        protected AsyncCommand()
        {
            this.executeAsyncMethod = DefaultFunc.ExecuteAsync;
            this.canExecuteMethod = DefaultFunc.CanExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
        /// </summary>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        public AsyncCommand( Func<T, Task> executeAsyncMethod )
            : this( executeAsyncMethod, DefaultFunc.CanExecute )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
        /// </summary>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,TResult}"/> representing the can execute method.</param>
        public AsyncCommand( Func<T, Task> executeAsyncMethod, Func<T, bool> canExecuteMethod )
        {
            Arg.NotNull( executeAsyncMethod, "executeAsyncMethod" );
            Arg.NotNull( canExecuteMethod, "canExecuteMethod" );

            this.executeAsyncMethod = executeAsyncMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Raises the <see cref="E:Executed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnExecuted( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.Executed;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Returns a value indicating whether the command can be executed.
        /// </summary>
        /// <param name="parameter">The associated parameter for the command to evaluate.</param>
        /// <returns>True if the command can be executed; otherwise, false.  The default implementation always returns true.</returns>
        public virtual bool CanExecute( T parameter )
        {
            return this.canExecuteMethod( parameter );
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The associated parameter with the command.</param>
        public virtual async void Execute( T parameter )
        {
            await this.executeAsyncMethod( parameter );
            this.OnExecuted( EventArgs.Empty );
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnCanExecuteChanged( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.CanExecuteChanged;

            if ( handler != null )
                handler( this, e );
        }

        bool ICommand.CanExecute( object parameter )
        {
            return this.CanExecute( Util.CastOrDefault<T>( parameter ) );
        }

        /// <summary>
        /// Occurs when the ability for the command to execute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        void ICommand.Execute( object parameter )
        {
            this.Execute( Util.CastOrDefault<T>( parameter ) );
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <remarks>This causes elements bound to this command to re-evaluate the <see cref="CanExecute"/> method.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Allows the command to be forcibly re-evaluated from an external source." )]
        public virtual void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged( EventArgs.Empty );
        }

        /// <summary>
        /// Occurs when the command has been executed.
        /// </summary>
        public event EventHandler<EventArgs> Executed;
    }
}
