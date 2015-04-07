namespace More.Windows.Input
{
    using More.ComponentModel;
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Windows.Input;

    /// <summary>
    /// Represents an observable command using a delegate<seealso cref="AsyncCommand{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of parameter associated with the command.</typeparam>
    /// <example>This example demonstrates how to create a command that accepts a lambda expression to execute.
    /// <code lang="C#"><![CDATA[
    /// using global::System.Windows.Input;
    /// using global::System;
    /// using global::System.Windows;
    /// using global::System.Windows.Input;
    /// 
    /// var command = new Command<string>( parameter => Console.WriteLine( parameter ) );
    /// var button = new Button();
    /// button.Command = command;
    /// ]]></code>
    /// </example>
    public class Command<T> : ObservableObject, INotifyCommandChanged
    {
        private readonly Action<T> executeMethod;
        private readonly Func<T, bool> canExecuteMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        protected Command()
        {
            this.executeMethod = DefaultAction.None;
            this.canExecuteMethod = DefaultFunc.CanExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        public Command( Action<T> executeMethod )
            : this( executeMethod, DefaultFunc.CanExecute )
        {
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,TResult}"/> representing the can execute method.</param>
        public Command( Action<T> executeMethod, Func<T, bool> canExecuteMethod )
        {
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
            Contract.Requires<ArgumentNullException>( canExecuteMethod != null, "canExecuteMethod" );
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Raises the <see cref="E:Executed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnExecuted( EventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

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
        public virtual void Execute( T parameter )
        {
            this.executeMethod( parameter );
            this.OnExecuted( EventArgs.Empty );
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnCanExecuteChanged( EventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

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
