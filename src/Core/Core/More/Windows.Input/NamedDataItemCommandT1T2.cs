namespace More.Windows.Input
{
    using global::System;
    using global::System.Diagnostics.Contracts;
    using global::System.Threading.Tasks;

    /// <summary>
    /// Represents an observable, named command with an associated data item<seealso cref="AsyncNamedDataItemCommand{TParameter, TItem}"/>.
    /// </summary>
    /// <typeparam name="TParameter">The <see cref="Type">type</see> of parameter passed to the command.</typeparam>
    /// <typeparam name="TItem">The <see cref="Type">type</see> of item associated with the command.</typeparam>
    public class NamedDataItemCommand<TParameter, TItem> : DataItemCommand<TParameter, TItem>, INamedCommand
    {
        private string id;
        private string name;
        private string desc = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        protected NamedDataItemCommand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public NamedDataItemCommand( string name, Action<TItem, TParameter> executeMethod, TItem dataItem )
            : this( null, name, executeMethod, DefaultFunc.CanExecute, dataItem )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public NamedDataItemCommand( string id, string name, Action<TItem, TParameter> executeMethod, TItem dataItem )
            : this( id, name, executeMethod, DefaultFunc.CanExecute, dataItem )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2,TResult}"/> representing the can execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public NamedDataItemCommand( string name, Action<TItem, TParameter> executeMethod, Func<TItem, TParameter, bool> canExecuteMethod, TItem dataItem )
            : this( null, name, executeMethod, canExecuteMethod, dataItem )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
            Contract.Requires<ArgumentNullException>( canExecuteMethod != null, "canExecuteMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2,TResult}"/> representing the can execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public NamedDataItemCommand( string id, string name, Action<TItem, TParameter> executeMethod, Func<TItem, TParameter, bool> canExecuteMethod, TItem dataItem )
            : base( executeMethod, canExecuteMethod, dataItem )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
            Contract.Requires<ArgumentNullException>( canExecuteMethod != null, "canExecuteMethod" );

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
                return this.name;
            }
            set
            {
                this.SetProperty( ref this.name, value );
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
                return this.desc;
            }
            set
            {
                this.SetProperty( ref this.desc, value );
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
                return string.IsNullOrEmpty( this.id ) ? this.Name : this.id;
            }
            set
            {
                this.SetProperty( ref this.id, value );
            }
        }
    }
}
