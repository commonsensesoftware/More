namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;
    using static Util;

    /// <summary>
    /// Represents a command interceptor.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item associated with the intercepted command.</typeparam>
    public class CommandInterceptor<T> : INotifyCommandChanged
    {
        readonly Action<T> preAction;
        readonly Action<T> postAction;
        readonly ICommand command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInterceptor{T}"/> class.
        /// </summary>
        /// <param name="preAction">The <see cref="Action{T}"/> to perform before the command is invoked.</param>
        /// <param name="command">The <see cref="ICommand"/> to intercept.</param>
        public CommandInterceptor( Action<T> preAction, ICommand command )
            : this( preAction, DefaultAction.None, command ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInterceptor{T}"/> class.
        /// </summary>
        /// <param name="command">The <see cref="ICommand"/> to intercept.</param>
        /// <param name="postAction">The <see cref="Action{T}"/> to perform after the command is invoked.</param>
        public CommandInterceptor( ICommand command, Action<T> postAction )
            : this( DefaultAction.None, postAction, command ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInterceptor{T}"/> class.
        /// </summary>
        /// <param name="preAction">The <see cref="Action{T}"/> to perform before the command is invoked.</param>
        /// <param name="postAction">The <see cref="Action{T}"/> to perform after the command is invoked.</param>
        /// <param name="command">The <see cref="ICommand"/> to intercept.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public CommandInterceptor( Action<T> preAction, Action<T> postAction, ICommand command )
        {
            Arg.NotNull( preAction, nameof( preAction ) );
            Arg.NotNull( postAction, nameof( postAction ) );
            Arg.NotNull( command, nameof( command ) );

            this.command = command;
            this.preAction = preAction;
            this.postAction = postAction;
            this.command.CanExecuteChanged += OnSourceCanExecuteChanged;

            if ( command is INotifyCommandChanged notifyCommand )
            {
                notifyCommand.Executed += OnSourceExecuted;
            }
        }

        /// <summary>
        /// Raises the <see cref="Executed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnExecuted( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Executed?.Invoke( this, e );
        }

        /// <summary>
        /// Occurs when the execution state of the underlying command changes.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        /// <remarks>Note to inheritors: the base class has no implementation and is not required to be called.</remarks>
        protected virtual void OnCanExecuteChanged( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            CanExecuteChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Returns a value indicating whether the command can be executed.
        /// </summary>
        /// <param name="parameter">The contextual parameter for the command.</param>
        /// <returns>True if the command can be executed; otherwise, false.</returns>
        public virtual bool CanExecute( T parameter ) => command.CanExecute( parameter );

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The contextual parameter for the command.</param>
        public virtual void Execute( T parameter )
        {
            preAction( parameter );
            command.Execute( parameter );
            postAction( parameter );
        }

        bool ICommand.CanExecute( object parameter ) => CanExecute( CastOrDefault<T>( parameter ) );

        /// <summary>
        /// Occurs when the execution state of the command changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        void ICommand.Execute( object parameter ) => Execute( CastOrDefault<T>( parameter ) );

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <remarks>This causes elements bound to this command to re-evaluate the <see cref="CanExecute"/> method.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Allows the command to be forcibly re-evaluated from an external source." )]
        public virtual void RaiseCanExecuteChanged()
        {
            if ( command is INotifyCommandChanged notify )
            {
                notify.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Occurs when the command has been executed.
        /// </summary>
        public event EventHandler<EventArgs> Executed;

        void OnSourceCanExecuteChanged( object sender, EventArgs e ) => OnCanExecuteChanged( e );

        void OnSourceExecuted( object sender, EventArgs e ) => OnExecuted( e );
    }
}