namespace More.Web.Http.Tracing
{
    using More.ComponentModel;
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
        private readonly IRepository<T> decorated;
        private readonly string operatorName;
        private readonly HttpRequestMessage request;
        private readonly ITraceWriter traceWriter;
        private string category;

        private RepositoryTracer( IRepository<T> repository, Type repositoryType, HttpRequestMessage request, string category )
        {
            Contract.Requires( repository != null );
            Contract.Requires( repositoryType != null );
            Contract.Requires( !string.IsNullOrEmpty( category ) );

            this.operatorName = repositoryType.Name;
            this.decorated = repository;
            this.decorated.PropertyChanged += this.OnDecoratedPropertyChanged;
            this.category = category;

            if ( ( this.request = request ) != null )
                this.traceWriter = request.GetConfiguration().Services.GetTraceWriter() ?? NullTraceWriter.Instance;
            else
                this.traceWriter = NullTraceWriter.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTracer{T}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IReadOnlyRepository{T}">repository</see> to be decorated.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the repository.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public RepositoryTracer( IReadOnlyRepository<T> repository, HttpRequestMessage request )
            : this( new ReadOnlyRepositoryAdapter<T>( repository ), repository.GetType(), request, "Data" )
        {
            Contract.Requires<ArgumentNullException>( repository != null, "repository" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTracer{T}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IReadOnlyRepository{T}">repository</see> to be decorated.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the repository.</param>
        /// <param name="category">The trace category.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public RepositoryTracer( IReadOnlyRepository<T> repository, HttpRequestMessage request, string category )
            : this( new ReadOnlyRepositoryAdapter<T>( repository ), repository.GetType(), request, category )
        {
            Contract.Requires<ArgumentNullException>( repository != null, "repository" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( category ), "category" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTracer{T}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{T}">repository</see> to be decorated.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the repository.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public RepositoryTracer( IRepository<T> repository, HttpRequestMessage request )
            : this( repository, repository.GetType(), request, "Data" )
        {
            Contract.Requires<ArgumentNullException>( repository != null, "repository" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTracer{T}"/> class.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{T}">repository</see> to be decorated.</param>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> associated with the repository.</param>
        /// <param name="category">The trace category.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public RepositoryTracer( IRepository<T> repository, HttpRequestMessage request, string category )
            : this( repository, repository.GetType(), request, category )
        {
            Contract.Requires<ArgumentNullException>( repository != null, "repository" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( category ), "category" );
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
        /// Adds a new item to the repository.
        /// </summary>
        /// <param name="item">The new item to add.</param>
        public virtual void Add( T item )
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "Add";
                } );

            this.decorated.Add( item );
        }

        /// <summary>
        /// Discards all changes to the items within the repository, if any.
        /// </summary>
        public virtual void DiscardChanges()
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "DiscardChanges";
                } );

            this.decorated.DiscardChanges();
        }

        /// <summary>
        /// Gets a value indicating whether there are any pending, uncommitted changes.
        /// </summary>
        /// <value>True if there are any pending uncommitted changes; otherwise, false.</value>
        public virtual bool HasPendingChanges
        {
            get
            {
                return this.decorated.HasPendingChanges;
            }
        }

        /// <summary>
        /// Removes an item from the repository.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public virtual void Remove( T item )
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "Remove";
                } );

            this.decorated.Remove( item );
        }

        /// <summary>
        /// Saves all pending changes in the repository asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the save operation.</returns>
        public virtual Task SaveChangesAsync( CancellationToken cancellationToken )
        {
            var duration = new TraceStopwatch( tr => tr.Message = SR.RepositorySaveChangesAsyncBegin, tr => tr.Message = SR.RepositorySaveChangesAsyncEnd );

            return this.TraceWriter.TraceBeginEndAsync(
                this.Request,
                this.TraceCategory,
                TraceLevel.Info,
                this.operatorName,
                "SaveChangesAsync",
                duration.StartTrace,
                () => this.decorated.SaveChangesAsync( cancellationToken ),
                duration.EndTrace,
                null );
        }

        /// <summary>
        /// Updates an existing item in the repository.
        /// </summary>
        /// <param name="item">The item to update.</param>
        public virtual void Update( T item )
        {
            this.TraceWriter.Trace(
                this.Request,
                this.TraceCategory,
                TraceLevel.Debug,
                r =>
                {
                    r.Kind = TraceKind.Trace;
                    r.Operator = this.operatorName;
                    r.Operation = "Update";
                } );

            this.decorated.Update( item );
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
            var duration = new TraceStopwatch<TResult>( tr => tr.Message = SR.RepositoryGetAsyncBegin, ( tr, r ) => tr.Message = SR.RepositoryGetAsyncEnd );

            return this.TraceWriter.TraceBeginEndAsync(
                this.Request,
                this.TraceCategory,
                TraceLevel.Info,
                this.operatorName,
                "GetAsync",
                duration.StartTrace,
                () => this.decorated.GetAsync( queryShaper, cancellationToken ),
                duration.EndTrace,
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
            var duration = new TraceStopwatch<IEnumerable<T>>( tr => tr.Message = SR.RepositoryGetAsyncBegin, ( tr, r ) => tr.Message = SR.RepositoryGetAsyncEnd );

            return this.TraceWriter.TraceBeginEndAsync(
                this.Request,
                this.TraceCategory,
                TraceLevel.Info,
                this.operatorName,
                "GetAsync",
                duration.StartTrace,
                () => this.decorated.GetAsync( queryShaper, cancellationToken ),
                duration.EndTrace,
                null );
        }

        /// <summary>
        /// Gets the underlying decorated object.
        /// </summary>
        /// <value>The decorated <see cref="IRepository{T}"/> object.</value>
        public IRepository<T> Inner
        {
            get
            {
                return Decorator.GetInner( this.decorated );
            }
        }

        IReadOnlyRepository<T> IDecorator<IReadOnlyRepository<T>>.Inner
        {
            get
            {
                // note: decorated object could be read-only and adapted with another decorator
                return Decorator.GetInner<IReadOnlyRepository<T>>( this.decorated );
            }
        }
    }
}
