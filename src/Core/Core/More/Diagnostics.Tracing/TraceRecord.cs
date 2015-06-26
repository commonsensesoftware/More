namespace More.Diagnostics.Tracing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the trace entry used to record traces.
    /// </summary>
    [DebuggerDisplay( "Category: {Category}, Operation: {Operation}, Level: {Level}, Kind: {Kind}" )]
    public class TraceRecord
    {
        private Lazy<Dictionary<object, object>> properties = new Lazy<Dictionary<object, object>>( () => new Dictionary<object, object>() );

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceRecord"/> class.
        /// </summary>
        /// <param name="category">The trace category.</param>
        /// <param name="level">The <see cref="TraceLevel">trace level</see>.</param>
        public TraceRecord( string category, TraceLevel level )
            : this( Guid.NewGuid(), category, level )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceRecord"/> class.
        /// </summary>
        /// <param name="correlationId">The <see cref="Guid">GUID</see> representing the correlation identifier.</param>
        /// <param name="category">The trace category.</param>
        /// <param name="level">The <see cref="TraceLevel">trace level</see>.</param>
        public TraceRecord( Guid correlationId, string category, TraceLevel level )
        {
            this.CorrelationId = correlationId;
            this.Timestamp = DateTime.UtcNow;
            this.Category = category;
            this.Level = level;
        }

        /// <summary>
        /// Gets the correlation correlation identifier.
        /// </summary>
        /// <value>A <see cref="Guid">GUID</see> representing the correlation identifier.</value>
        public Guid CorrelationId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the tracing category.
        /// </summary>
        /// <value>The tracing category.</value>
        public string Category
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The <see cref="Exception">exception</see> being traced.</value>
        public Exception Exception
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the kind of trace.
        /// </summary>
        /// <value>One of the <see cref="TraceKind"/> values.</value>
        public TraceKind Kind
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tracing level.
        /// </summary>
        /// <value>One of the <see cref="TraceLevel"/> values.</value>
        public TraceLevel Level
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The trace message.</value>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logical operation name being performed.
        /// </summary>
        /// <value>The name of the operation being performed.</value>
        public string Operation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logical name of the object performing the operation.
        /// </summary>
        /// <value>The name of the object performing the operation.</value>
        public string Operator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an optional user-defined property bag.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}">dictionary</see> of key/value pairs.</value>
        public IDictionary<object, object> Properties
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<object, object>>() != null );
                return this.properties.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> of this trace (via <see cref="DateTime.UtcNow"/>)
        /// </summary>
        /// <value>The <see cref="DateTime">timestamp</see> of the trace in Universal Coordinated Time (UTC).</value>
        public DateTime Timestamp
        {
            get;
            private set;
        }
    }
}