namespace More.Windows.Input
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Threading.Tasks;

    /// <summary>
    /// Represents a named command<seealso cref="AsyncNamedCommand{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of parameter associated with the command.</typeparam>
    public class NamedCommand<T> : Command<T>, INamedCommand
    {
        private string id;
        private string name;
        private string desc = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCommand{T}"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        public NamedCommand( string name, Action<T> executeMethod )
            : this( null, name, executeMethod, DefaultFunc.CanExecute )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCommand{T}"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        public NamedCommand( string id, string name, Action<T> executeMethod )
            : this( id, name, executeMethod, DefaultFunc.CanExecute )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCommand{T}"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2}"/> representing the can execute method.</param>
        public NamedCommand( string name, Action<T> executeMethod, Func<T, bool> canExecuteMethod )
            : this( null, name, executeMethod, canExecuteMethod )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
            Contract.Requires<ArgumentNullException>( canExecuteMethod != null, "canExecuteMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCommand{T}"/> class.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <param name="name">The command name.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2}"/> representing the can execute method.</param>
        public NamedCommand( string id, string name, Action<T> executeMethod, Func<T, bool> canExecuteMethod )
            : base( executeMethod, canExecuteMethod )
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
                if ( string.IsNullOrEmpty( this.id ) )
                    return this.Name;

                return this.id;
            }
            set
            {
                this.SetProperty( ref this.id, value );
            }
        }
    }
}
