namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous, named command<seealso cref="NamedCommand{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of parameter associated with the command.</typeparam>
    public class AsyncNamedCommand<T> : AsyncCommand<T>, INamedCommand
    {
        string id;
        string name;
        string desc = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedCommand{T}"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        public AsyncNamedCommand( string name, Func<T, Task> executeAsyncMethod )
            : this( null, name, executeAsyncMethod, DefaultFunc.CanExecute ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedCommand{T}"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        public AsyncNamedCommand( string id, string name, Func<T, Task> executeAsyncMethod )
            : this( id, name, executeAsyncMethod, DefaultFunc.CanExecute ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedCommand{T}"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2}"/> representing the can execute method.</param>
        public AsyncNamedCommand( string name, Func<T, Task> executeAsyncMethod, Func<T, bool> canExecuteMethod )
            : this( null, name, executeAsyncMethod, canExecuteMethod ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedCommand{T}"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2}"/> representing the can execute method.</param>
        public AsyncNamedCommand( string id, string name, Func<T, Task> executeAsyncMethod, Func<T, bool> canExecuteMethod )
            : base( executeAsyncMethod, canExecuteMethod )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );

            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        /// <value>The command name.</value>
        public string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( name ) );
                return name;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                SetProperty( ref name, value );
            }
        }

        /// <summary>
        /// Gets or sets the command description.
        /// </summary>
        /// <value>The command description.</value>
        public virtual string Description
        {
            get
            {
                Contract.Ensures( desc != null );
                return desc;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                SetProperty( ref desc, value );
            }
        }

        /// <summary>
        /// Gets or sets the identifier associated with the command.
        /// </summary>
        /// <value>The command identifier. If this property is unset, the default
        /// value is the <see cref="Name">name</see> of the command.</value>
        public string Id
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return string.IsNullOrEmpty( id ) ? Name : id;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                SetProperty( ref id, value );
            }
        }
    }
}