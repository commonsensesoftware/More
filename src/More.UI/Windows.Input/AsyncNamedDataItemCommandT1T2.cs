namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous, observable, named command with an associated data item<seealso cref="AsyncNamedDataItemCommand{TParameter, TItem}"/>.
    /// </summary>
    /// <typeparam name="TParameter">The <see cref="Type">type</see> of parameter passed to the command.</typeparam>
    /// <typeparam name="TItem">The <see cref="Type">type</see> of item associated with the command.</typeparam>
    public class AsyncNamedDataItemCommand<TParameter, TItem> : AsyncDataItemCommand<TParameter, TItem>, INamedCommand
    {
        string id;
        string name;
        string desc = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        protected AsyncNamedDataItemCommand() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T1,T2,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public AsyncNamedDataItemCommand( string name, Func<TItem, TParameter, Task> executeAsyncMethod, TItem dataItem )
            : this( null, name, executeAsyncMethod, DefaultFunc.CanExecute, dataItem ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T1,T2,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public AsyncNamedDataItemCommand( string id, string name, Func<TItem, TParameter, Task> executeAsyncMethod, TItem dataItem )
            : this( id, name, executeAsyncMethod, DefaultFunc.CanExecute, dataItem ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T1,T2,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2,TResult}"/> representing the can execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public AsyncNamedDataItemCommand( string name, Func<TItem, TParameter, Task> executeAsyncMethod, Func<TItem, TParameter, bool> canExecuteMethod, TItem dataItem )
            : this( null, name, executeAsyncMethod, canExecuteMethod, dataItem ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncNamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeAsyncMethod">The <see cref="Func{T1,T2,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2,TResult}"/> representing the can execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public AsyncNamedDataItemCommand( string id, string name, Func<TItem, TParameter, Task> executeAsyncMethod, Func<TItem, TParameter, bool> canExecuteMethod, TItem dataItem )
            : base( executeAsyncMethod, canExecuteMethod, dataItem )
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
        /// value is the <see cref="P:Name">name</see> of the command.</value>
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