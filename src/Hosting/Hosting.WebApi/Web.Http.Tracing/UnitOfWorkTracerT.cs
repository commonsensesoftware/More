namespace More.Web.Http.Tracing
{
    using ComponentModel;
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
    using Stopwatch = System.Diagnostics.Stopwatch;

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
            Arg.NotNull( unitOfWork, nameof( unitOfWork ) );
            Arg.NotNullOrEmpty( category, nameof( category ) );

            operatorName = unitOfWork.GetType().Name;
            decorated = unitOfWork;
            decorated.PropertyChanged += OnDecoratedPropertyChanged;
            this.category = category;

            if ( ( this.request = request ) != null )
                traceWriter = request.GetConfiguration()?.Services.GetTraceWriter() ?? NullTraceWriter.Instance;
            else
                traceWriter = NullTraceWriter.Instance;
        }

        private void OnDecoratedPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( string.IsNullOrEmpty( e.PropertyName ) || e.PropertyName == nameof( HasPendingChanges ) )
                OnPropertyChanged( e );
        }

        /// <summary>
        /// Gets or sets the trace category.
        /// </summary>
        /// <value>The trace category. The default value is "Data".</value>
        protected string TraceCategory
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( category ) );
                return category;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                category = value;
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
                Contract.Ensures( traceWriter != null );
                return traceWriter;
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
                Contract.Ensures( request != null );
                return request;
            }
        }

        /// <summary>
        /// Commits all pending units of work asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the commit operation.</returns>
        public Task CommitAsync( CancellationToken cancellationToken )
        {
            var stopwatch = new Stopwatch();

            return TraceWriter.TraceBeginEndAsync(
                Request,
                TraceCategory,
                TraceLevel.Info,
                operatorName,
                nameof( CommitAsync ),
                tr =>
                {
                    tr.Message = SR.CommitBegin;
                    stopwatch.Start();
                },
                () => decorated.CommitAsync( cancellationToken ),
                tr =>
                {
                    stopwatch.Stop();
                    tr.SetDuration( stopwatch.Elapsed );
                    tr.Message = SR.CommitEnd;
                },
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
                return decorated.HasPendingChanges;
            }
        }

        /// <summary>
        /// Registers a changed item.
        /// </summary>
        /// <param name="item">The changed item to register.</param>
        public void RegisterChanged( T item )
        {
            Arg.NotNull( item, nameof( item ) );

            TraceWriter.Trace(
                Request,
                TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( RegisterChanged );
                } );

            decorated.RegisterChanged( item );
        }

        /// <summary>
        /// Registers a new item.
        /// </summary>
        /// <param name="item">The new item to register.</param>
        public void RegisterNew( T item )
        {
            Arg.NotNull( item, nameof( item ) );

            TraceWriter.Trace(
                Request,
                TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( RegisterNew );
                } );

            decorated.RegisterNew( item );
        }

        /// <summary>
        /// Registers a removed item.
        /// </summary>
        /// <param name="item">The removed item to register.</param>
        public void RegisterRemoved( T item )
        {
            Arg.NotNull( item, nameof( item ) );

            TraceWriter.Trace(
                Request,
                TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( RegisterRemoved );
                } );

            decorated.RegisterRemoved( item );
        }

        /// <summary>
        /// Rolls back all pending units of work.
        /// </summary>
        public void Rollback()
        {
            TraceWriter.Trace(
                Request,
                TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( Rollback );
                } );

            decorated.Rollback();
        }

        /// <summary>
        /// Unregisters an item.
        /// </summary>
        /// <param name="item">The item to unregister.</param>
        public void Unregister( T item )
        {
            Arg.NotNull( item, nameof( item ) );

            TraceWriter.Trace(
                Request,
                TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( Unregister );
                } );

            decorated.Unregister( item );
        }

        /// <summary>
        /// Gets the underlying decorated object.
        /// </summary>
        /// <value>The decorated <see cref="IUnitOfWork{T}"/> object.</value>
        public IUnitOfWork<T> Inner
        {
            get
            {
                return decorated;
            }
        }
    }
}
