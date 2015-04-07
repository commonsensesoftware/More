namespace More.Windows.Input
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Threading.Tasks;

    /// <summary>
    /// Represents an observable command with an associated data item<seealso cref="AsyncDataItemCommand{TParameter, TItem}"/>.
    /// </summary>
    /// <typeparam name="TParameter">The <see cref="Type">type</see> of parameter passed to the command.</typeparam>
    /// <typeparam name="TItem">The <see cref="Type">type</see> of item associated with the command.</typeparam>
    /// <example>This example demonstrates how to create a command that has an associated data item.  This scenario works well when an
    /// item bound to a row is clicked and the entire item must be passed to the click handler.
    /// <code lang="C#"><![CDATA[
    /// using global::System.Windows.Input;
    /// using global::System;
    /// using global::System.Windows;
    /// using global::System.Windows.Input;
    /// 
    /// var person = new Person(){ FirstName = "John", LastName = "Doe" }
    /// var command = new DataItemCommand<string, Person>( ( item, parameter ) => item.FirstName = parameter, person );
    /// var button = new Button();
    /// button.Command = command;
    /// ]]></code>
    /// </example>
    public class DataItemCommand<TParameter, TItem> : Command<TParameter>
    {
        private readonly Action<TItem, TParameter> executeMethod;
        private readonly Func<TItem, TParameter, bool> canExecuteMethod;
        private readonly IEqualityComparer<TItem> comparer = EqualityComparer<TItem>.Default;
        private TItem item;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        protected DataItemCommand()
        {
            this.canExecuteMethod = DefaultFunc.CanExecute;
            this.executeMethod = DefaultAction.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public DataItemCommand( Action<TItem, TParameter> executeMethod, TItem dataItem )
            : this( executeMethod, DefaultFunc.CanExecute, dataItem )
        {
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2,TResult}"/> representing the can execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public DataItemCommand( Action<TItem, TParameter> executeMethod, Func<TItem, TParameter, bool> canExecuteMethod, TItem dataItem )
        {
            Contract.Requires<ArgumentNullException>( executeMethod != null, "executeMethod" );
            Contract.Requires<ArgumentNullException>( canExecuteMethod != null, "canExecuteMethod" );

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            this.item = dataItem;
        }

        /// <summary>
        /// Gets the comparer used to compare items.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}">comparer</see> object.</value>
        protected virtual IEqualityComparer<TItem> Comparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<TItem>>() != null );
                return this.comparer;
            }
        }

        /// <summary>
        /// Gets or sets item associated with the command.
        /// </summary>
        /// <value>The associated item of type <typeparamref name="TItem"/>.</value>
        public TItem Item
        {
            get
            {
                return this.item;
            }
            protected set
            {
                this.SetProperty( ref this.item, value, this.Comparer );
            }
        }

        /// <summary>
        /// Returns a value indicating whether the command can be executed.
        /// </summary>
        /// <param name="parameter">The associated parameter for the command to evaluate.</param>
        /// <returns>True if the command can be executed; otherwise, false.  The default implementation always returns true.</returns>
        public override bool CanExecute( TParameter parameter )
        {
            return this.canExecuteMethod( this.Item, parameter );
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The associated parameter with the command.</param>
        public override void Execute( TParameter parameter )
        {
            this.executeMethod( this.Item, parameter );
            this.OnExecuted( EventArgs.Empty );
        }
    }
}
