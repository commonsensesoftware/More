namespace More.Web.Http.Tracing
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Services;
    using System.Web.Http.Tracing;
    using static System.Web.Http.Tracing.TraceKind;
    using static System.Web.Http.Tracing.TraceLevel;
    using Stopwatch = System.Diagnostics.Stopwatch;

    /// <summary>
    /// Represents a repository that traces the operations of a decorated repository<seealso cref="IReadOnlyRepository{T}"/><seealso cref="IRepository{T}"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of items in the repository.</typeparam>
    [PartNotDiscoverable]
    public class RepositoryTracer<T> :
        ObservableObject,
        IRepository<T>,
        IDecorator<IRepository<T>>,
        IDecorator<IReadOnlyRepository<T>> where T : class
    {
        readonly IRepository<T> decorated;
        readonly string operatorName;
        readonly HttpRequestMessage request;
        readonly ITraceWriter traceWriter;
        string category;

        RepositoryTracer( IRepository<T> repository, Type repositoryType, HttpRequestMessage request, string category )
        {
            Arg.NotNull( repository, nameof( repository ) );
            Arg.NotNullOrEmpty( category, nameof( category ) );
            Contract.Requires( repositoryType != null );

            operatorName = repositoryType.Name;
            decorated = repository;
            decorated.PropertyChanged += OnDecoratedPropertyChanged;
            this.category = category;

            if ( ( this.request = request ) != null )
            {
                traceWriter = request.GetConfiguration()?.Services.GetTraceWriter() ?? NullTraceWriter.Instance;
            }
            else
            {
                traceWriter = NullTraceWriter.Instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTracer{T}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IReadOnlyRepository{T}">repository</see> to be decorated.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the repository.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public RepositoryTracer( IReadOnlyRepository<T> repository, HttpRequestMessage request )
            : this( new ReadOnlyRepositoryAdapter<T>( repository ), repository?.GetType() ?? typeof( IReadOnlyRepository<T> ), request, "Data" ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTracer{T}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IReadOnlyRepository{T}">repository</see> to be decorated.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the repository.</param>
        /// <param name="category">The trace category.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public RepositoryTracer( IReadOnlyRepository<T> repository, HttpRequestMessage request, string category )
            : this( new ReadOnlyRepositoryAdapter<T>( repository ), repository?.GetType() ?? typeof( IReadOnlyRepository<T> ), request, category ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTracer{T}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{T}">repository</see> to be decorated.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the repository.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public RepositoryTracer( IRepository<T> repository, HttpRequestMessage request )
            : this( repository, repository?.GetType() ?? typeof( IRepository<T> ), request, "Data" ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTracer{T}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{T}">repository</see> to be decorated.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the repository.</param>
        /// <param name="category">The trace category.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public RepositoryTracer( IRepository<T> repository, HttpRequestMessage request, string category )
            : this( repository, repository?.GetType() ?? typeof( IRepository<T> ), request, category ) { }

        void OnDecoratedPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( string.IsNullOrEmpty( e.PropertyName ) || e.PropertyName == nameof( HasPendingChanges ) )
            {
                OnPropertyChanged( e );
            }
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
        /// Adds a new item to the repository.
        /// </summary>
        /// <param name="item">The new item to add.</param>
        public virtual void Add( T item )
        {
            Arg.NotNull( item, nameof( item ) );

            TraceWriter.Trace(
                Request,
                TraceCategory,
                Debug,
                r =>
                {
                    r.Kind = Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( Add );
                } );

            decorated.Add( item );
        }

        /// <summary>
        /// Discards all changes to the items within the repository, if any.
        /// </summary>
        public virtual void DiscardChanges()
        {
            TraceWriter.Trace(
                Request,
                TraceCategory,
                Debug,
                r =>
                {
                    r.Kind = Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( DiscardChanges );
                } );

            decorated.DiscardChanges();
        }

        /// <summary>
        /// Gets a value indicating whether there are any pending, uncommitted changes.
        /// </summary>
        /// <value>True if there are any pending uncommitted changes; otherwise, false.</value>
        public virtual bool HasPendingChanges => decorated.HasPendingChanges;

        /// <summary>
        /// Removes an item from the repository.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public virtual void Remove( T item )
        {
            Arg.NotNull( item, nameof( item ) );

            TraceWriter.Trace(
                Request,
                TraceCategory,
                Debug,
                r =>
                {
                    r.Kind = Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( Remove );
                } );

            decorated.Remove( item );
        }

        /// <summary>
        /// Saves all pending changes in the repository asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the save operation.</returns>
        public virtual Task SaveChangesAsync( CancellationToken cancellationToken )
        {
            var stopwatch = new Stopwatch();

            return TraceWriter.TraceBeginEndAsync(
                Request,
                TraceCategory,
                Info,
                operatorName,
                nameof( SaveChangesAsync ),
                tr =>
                {
                    tr.Message = SR.RepositorySaveChangesAsyncBegin;
                    stopwatch.Start();
                },
                () => decorated.SaveChangesAsync( cancellationToken ),
                tr =>
                {
                    stopwatch.Stop();
                    tr.SetDuration( stopwatch.Elapsed );
                    tr.Message = SR.RepositorySaveChangesAsyncEnd;
                },
                null );
        }

        /// <summary>
        /// Updates an existing item in the repository.
        /// </summary>
        /// <param name="item">The item to update.</param>
        public virtual void Update( T item )
        {
            Arg.NotNull( item, nameof( item ) );

            TraceWriter.Trace(
                Request,
                TraceCategory,
                Debug,
                r =>
                {
                    r.Kind = Trace;
                    r.Operator = operatorName;
                    r.Operation = nameof( Update );
                } );

            decorated.Update( item );
        }

        /// <summary>
        /// Retrieves a query result asynchronously.
        /// </summary>
        /// <typeparam name="TResult">The <see cref="Type">type</see> of result to retrieve.</typeparam>
        /// <param name="queryShaper">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the <typeparamref name="TResult">result</typeparamref> of the operation.</returns>
        public virtual Task<TResult> GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken )
        {
            Arg.NotNull( queryShaper, nameof( queryShaper ) );

            var stopwatch = new Stopwatch();

            return TraceWriter.TraceBeginEndAsync(
                Request,
                TraceCategory,
                Info,
                operatorName,
                nameof( GetAsync ),
                tr =>
                {
                    tr.Message = SR.RepositoryGetAsyncBegin;
                    stopwatch.Start();
                },
                () => decorated.GetAsync( queryShaper, cancellationToken ),
                ( tr, r ) =>
                {
                    stopwatch.Stop();
                    tr.SetDuration( stopwatch.Elapsed );
                    tr.Message = SR.RepositoryGetAsyncEnd;
                },
                null );
        }

        /// <summary>
        /// Retrieves all items in the repository satisfied by the specified query.
        /// </summary>
        /// <param name="queryShaper">The <see cref="Func{T,TResult}">function</see> that shapes the <see cref="IQueryable{T}">query</see> to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retrieved <see cref="IEnumerable{T}">sequence</see> of <typeparamref name="T">items</typeparamref>.</returns>
        public virtual Task<IEnumerable<T>> GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken )
        {
            Arg.NotNull( queryShaper, nameof( queryShaper ) );

            var stopwatch = new Stopwatch();

            return TraceWriter.TraceBeginEndAsync(
                Request,
                TraceCategory,
                Info,
                operatorName,
                nameof( GetAsync ),
                tr =>
                {
                    tr.Message = SR.RepositoryGetAsyncBegin;
                    stopwatch.Start();
                },
                () => decorated.GetAsync( queryShaper, cancellationToken ),
                ( tr, r ) =>
                {
                    stopwatch.Stop();
                    tr.SetDuration( stopwatch.Elapsed );
                    tr.Message = SR.RepositoryGetAsyncEnd;
                },
                null );
        }

        /// <summary>
        /// Gets the underlying decorated object.
        /// </summary>
        /// <value>The decorated <see cref="IRepository{T}"/> object.</value>
        public IRepository<T> Inner => Decorator.GetInner( decorated );

        IReadOnlyRepository<T> IDecorator<IReadOnlyRepository<T>>.Inner => Decorator.GetInner<IReadOnlyRepository<T>>( decorated );
    }
}