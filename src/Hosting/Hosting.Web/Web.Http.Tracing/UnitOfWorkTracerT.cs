namespace More.Web.Http.Tracing
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Services;
    using System.Web.Http.Tracing;

    /// <summary>
    /// Represents a unit of work that traces the operations of a decorated <see cref="IUnitOfWork{T}">unit of work</see>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of items in the unit of work.</typeparam>
    [PartNotDiscoverable]
    public class UnitOfWorkTracer<T> : ObservableObject, IUnitOfWork<T>, IDecorator<IUnitOfWork<T>> where T : class
    {
        private readonly IUnitOfWork<T> decorated;
        private readonly string operatorName;
        private readonly HttpRequestMessage request;
        private readonly ITraceWriter traceWriter;
        private string category;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkTracer{T}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork{T}">unit of work</see> to decorate.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the unit of work.</param>
        public UnitOfWorkTracer( IUnitOfWork<T> unitOfWork, HttpRequestMessage request )
            : this( unitOfWork, request, "Data" )
        {
            Contract.Requires<ArgumentNullException>( unitOfWork != null, "unitOfWork" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkTracer{T}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork{T}">unit of work</see> to decorate.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the unit of work.</param>
        /// <param name="category">The trace category.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public UnitOfWorkTracer( IUnitOfWork<T> unitOfWork, HttpRequestMessage request, string category )
        {
            Contract.Requires<ArgumentNullException>( unitOfWork != null, "unitOfWork" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( category ), "category" );

            this.operatorName = unitOfWork.GetType().Name;
            this.decorated = unitOfWork;
            this.decorated.PropertyChanged += this.OnDecoratedPropertyChanged;
            this.category = category;

            if ( ( this.request = request ) != null )
                this.traceWriter = request.GetConfiguration().Services.GetTraceWriter() ?? NullTraceWriter.Instance;
            else
                this.traceWriter = NullTraceWriter.Instance;
        }

        private void OnDecoratedPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( string.IsNullOrEmpty( e.PropertyName ) || e.PropertyName == "HasPendingChanges" )
                this.OnPropertyChanged( e );
        }

        /// <summary>
        /// Gets or sets the trace category.
        /// </summary>
        /// <value>The trace category. The default value is "Data".</value>
        protected string TraceCategory
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.category ) );
                return this.category;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( value ), "value" );
                this.category = value;
            }
        }

        /// <summary>
        /// Gets the associated trace writer.
        /// </summary>
        /// <value>The associated <see cref="ITraceWriter">trace writer</see>.</value>
        protected ITraceWriter TraceWriter
        {
            get
            {
                Contract.Ensures( this.traceWriter != null );
                return this.traceWriter;
            }
        }

        /// <summary>
        /// Gets the associated request.
        /// </summary>
        /// <value>The associated <see cref="HttpRequestMessage">request</see>.</value>
        protected HttpRequestMessage Request
        {
            get
            {
                Contract.Ensures( this.request != null );
                return this.request;
            }
        }

        /// <summary>
        /// Commits all pending units of work asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the commit operation.</returns>
        public Task CommitAsync( CancellationToken cancellationToken )
        {
            var duration = new TraceStopwatch( tr => tr.Message = SR.CommitBegin, tr => tr.Message = SR.CommitEnd );

            return this.TraceWriter.TraceBeginEndAsync(
                this.Request,
                this.TraceCategory,
                TraceLevel.Info,
                this.operatorName,
                "CommitAsync",
                duration.StartTrace,
                () => this.decorated.CommitAsync( cancellationToken ),
                duration.EndTrace,
                null );
        }

        /// <summary>
        /// Gets a value indicating whether there are any pending, uncommitted changes.
        /// </summary>
        /// <value>True if there are any pending uncommitted changes; otherwise, false.</value>
        public bool HasPendingChanges
        {
            get
            {
                return this.decorated.HasPendingChanges;
            }
        }

        /// <summary>
        /// Registers a changed item.
        /// </summary>
        /// <param name="item">The changed item to register.</param>
        public void RegisterChanged( T item )
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "RegisterChanged";
                } );

            this.decorated.RegisterChanged( item );
        }

        /// <summary>
        /// Registers a new item.
        /// </summary>
        /// <param name="item">The new item to register.</param>
        public void RegisterNew( T item )
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "RegisterNew";
                } );

            this.decorated.RegisterNew( item );
        }

        /// <summary>
        /// Registers a removed item.
        /// </summary>
        /// <param name="item">The removed item to register.</param>
        public void RegisterRemoved( T item )
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "RegisterRemoved";
                } );

            this.decorated.RegisterRemoved( item );
        }

        /// <summary>
        /// Rolls back all pending units of work.
        /// </summary>
        public void Rollback()
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "Rollback";
                } );

            this.decorated.Rollback();
        }

        /// <summary>
        /// Unregisters an item.
        /// </summary>
        /// <param name="item">The item to unregister.</param>
        public void Unregister( T item )
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "Unregister";
                } );

            this.decorated.Unregister( item );
        }

        /// <summary>
        /// Gets the underlying decorated object.
        /// </summary>
        /// <value>The decorated <see cref="IUnitOfWork{T}"/> object.</value>
        public IUnitOfWork<T> Inner
        {
            get
            {
                return this.decorated;
            }
        }
    }
}
