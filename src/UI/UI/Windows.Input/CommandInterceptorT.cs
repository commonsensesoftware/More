namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Windows.Input;

    /// <summary>
    /// Represents a command interceptor.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item associated with the intercepted command.</typeparam>
    public class CommandInterceptor<T> : INotifyCommandChanged
    {
        private readonly Action<T> preAction;
        private readonly Action<T> postAction;
        private readonly ICommand command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInterceptor{T}"/> class.
        /// </summary>
        /// <param name="preAction">The <see cref="Action{T}"/> to perform before the command is invoked.</param>
        /// <param name="command">The <see cref="ICommand"/> to intercept.</param>
        public CommandInterceptor( Action<T> preAction, ICommand command )
            : this( preAction, DefaultAction.None, command )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInterceptor{T}"/> class.
        /// </summary>
        /// <param name="command">The <see cref="ICommand"/> to intercept.</param>
        /// <param name="postAction">The <see cref="Action{T}"/> to perform after the command is invoked.</param>
        public CommandInterceptor( ICommand command, Action<T> postAction )
            : this( DefaultAction.None, postAction, command )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInterceptor{T}"/> class.
        /// </summary>
        /// <param name="preAction">The <see cref="Action{T}"/> to perform before the command is invoked.</param>
        /// <param name="postAction">The <see cref="Action{T}"/> to perform after the command is invoked.</param>
        /// <param name="command">The <see cref="ICommand"/> to intercept.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public CommandInterceptor( Action<T> preAction, Action<T> postAction, ICommand command )
        {
            Arg.NotNull( preAction, "preAction" );
            Arg.NotNull( postAction, "postAction" );
            Arg.NotNull( command, "command" );
            
            this.command = command;
            this.preAction = preAction;
            this.postAction = postAction;
            this.command.CanExecuteChanged += ( s, e ) => this.OnCanExecuteChanged( e );

            var notifyCommand = command as INotifyCommandChanged;

            if ( notifyCommand != null )
                notifyCommand.Executed += ( s, e ) => this.OnExecuted( e );
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
        /// Occurs when the execution state of the underlying command changes.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        /// <remarks>Note to inheritors: the base class has no implementation and is not required to be called.</remarks>
        protected virtual void OnCanExecuteChanged( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.CanExecuteChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Returns a value indicating whether the command can be executed.
        /// </summary>
        /// <param name="parameter">The contextual parameter for the command.</param>
        /// <returns>True if the command can be executed; otherwise, false.</returns>
        public virtual bool CanExecute( T parameter )
        {
            return this.command.CanExecute( parameter );
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The contextual parameter for the command.</param>
        public virtual void Execute( T parameter )
        {
            this.preAction( parameter );
            this.command.Execute( parameter );
            this.postAction( parameter );
        }

        bool ICommand.CanExecute( object parameter )
        {
            return this.CanExecute( Util.CastOrDefault<T>( parameter ) );
        }

        /// <summary>
        /// Occurs when the execution state of the command changes.
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
            var notify = this.command as INotifyCommandChanged;

            if ( notify != null )
                notify.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Occurs when the command has been executed.
        /// </summary>
        public event EventHandler<EventArgs> Executed;
    }
}
