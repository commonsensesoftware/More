namespace More.Windows.Input
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;
    using static Util;

    /// <summary>
    /// Represents an observable command using a delegate<seealso cref="AsyncCommand{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of parameter associated with the command.</typeparam>
    /// <example>This example demonstrates how to create a command that accepts a lambda expression to execute.
    /// <code lang="C#"><![CDATA[
    /// using System.Windows.Input;
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Input;
    /// 
    /// var command = new Command<string>( parameter => Console.WriteLine( parameter ) );
    /// var button = new Button();
    /// button.Command = command;
    /// ]]></code>
    /// </example>
    public class Command<T> : ObservableObject, INotifyCommandChanged
    {
        readonly Action<T> executeMethod;
        readonly Func<T, bool> canExecuteMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        protected Command()
        {
            executeMethod = DefaultAction.None;
            canExecuteMethod = DefaultFunc.CanExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        public Command( Action<T> executeMethod ) : this( executeMethod, DefaultFunc.CanExecute ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,TResult}"/> representing the can execute method.</param>
        public Command( Action<T> executeMethod, Func<T, bool> canExecuteMethod )
        {
            Arg.NotNull( executeMethod, nameof( executeMethod ) );
            Arg.NotNull( canExecuteMethod, nameof( canExecuteMethod ) );

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Raises the <see cref="E:Executed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnExecuted( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Executed?.Invoke( this, e );
        }

        /// <summary>
        /// Returns a value indicating whether the command can be executed.
        /// </summary>
        /// <param name="parameter">The associated parameter for the command to evaluate.</param>
        /// <returns>True if the command can be executed; otherwise, false.  The default implementation always returns true.</returns>
        public virtual bool CanExecute( T parameter ) => canExecuteMethod( parameter );

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The associated parameter with the command.</param>
        public virtual void Execute( T parameter )
        {
            executeMethod( parameter );
            OnExecuted( EventArgs.Empty );
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnCanExecuteChanged( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            CanExecuteChanged?.Invoke( this, e );
        }

        bool ICommand.CanExecute( object parameter ) => CanExecute( CastOrDefault<T>( parameter ) );

        /// <summary>
        /// Occurs when the ability for the command to execute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        void ICommand.Execute( object parameter ) => Execute( CastOrDefault<T>( parameter ) );

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <remarks>This causes elements bound to this command to re-evaluate the <see cref="CanExecute"/> method.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Allows the command to be forcibly re-evaluated from an external source." )]
        public virtual void RaiseCanExecuteChanged() => OnCanExecuteChanged( EventArgs.Empty );

        /// <summary>
        /// Occurs when the command has been executed.
        /// </summary>
        public event EventHandler<EventArgs> Executed;
    }
}