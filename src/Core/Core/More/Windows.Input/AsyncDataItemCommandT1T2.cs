namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an observable, asychronous command with an associated data item<seealso cref="DataItemCommand{TParameter,TItem}"/>.
    /// </summary>
    /// <typeparam name="TParameter">The <see cref="Type">type</see> of parameter passed to the command.</typeparam>
    /// <typeparam name="TItem">The <see cref="Type">type</see> of item associated with the command.</typeparam>
    public class AsyncDataItemCommand<TParameter, TItem> : AsyncCommand<TParameter>
    {
        private readonly Func<TItem, TParameter, Task> executeAsyncMethod;
        private readonly Func<TItem, TParameter, bool> canExecuteMethod;
        private readonly IEqualityComparer<TItem> comparer = EqualityComparer<TItem>.Default;
        private TItem item;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        protected AsyncDataItemCommand()
        {
            this.canExecuteMethod = DefaultFunc.CanExecute;
            this.executeAsyncMethod = DefaultFunc.ExecuteAsync;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public AsyncDataItemCommand( Func<TItem, TParameter, Task> executeAsyncMethod, TItem dataItem )
            : this( executeAsyncMethod, DefaultFunc.CanExecute, dataItem )
        {
            Arg.NotNull( executeAsyncMethod, "executeAsyncMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDataItemCommand{TParameter,TItem}"/> class.
        /// </summary>
        /// <param name="executeAsyncMethod">The <see cref="Func{T,TResult}"/> representing the asynchronous execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,T2,TResult}"/> representing the can execute method.</param>
        /// <param name="dataItem">The item of type <typeparamref name="TItem"/> associated with the command.</param>
        public AsyncDataItemCommand( Func<TItem, TParameter, Task> executeAsyncMethod, Func<TItem, TParameter, bool> canExecuteMethod, TItem dataItem )
        {
            Arg.NotNull( executeAsyncMethod, "executeAsyncMethod" );
            Arg.NotNull( canExecuteMethod, "canExecuteMethod" );

            this.executeAsyncMethod = executeAsyncMethod;
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
        public override async void Execute( TParameter parameter )
        {
            await this.executeAsyncMethod( this.Item, parameter );
            this.OnExecuted( EventArgs.Empty );
        }
    }
}
