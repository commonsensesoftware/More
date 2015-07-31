namespace More.Web.Http.Tracing
{
    using System;
    using System.Composition;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Tracing;

    /// <summary>
    /// Represents a trace writer using the Per-Request model.
    /// </summary>
    public sealed class PerRequestTraceWriter : ITraceWriter
    {
        private readonly ITraceWriter traceWriter;
        private readonly HttpRequestMessage currentRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerRequestTraceWriter"/> class.
        /// </summary>
        /// <param name="request">The current <see cref="HttpRequestMessage">request</see> associated with the trace writer.</param>
        /// <remarks>The underlying <see cref="ITraceWriter">trace writer</see> is resolved from the currently
        /// <see cref="P:HttpConfiguration.Services">configured services</see> for the <paramref name="request"/>. If the
        /// current <paramref name="request"/> does not have an associated <see cref="ITraceWriter">trace writer</see>,
        /// then the <see cref="NullTraceWriter"/> is used.</remarks>
        public PerRequestTraceWriter( HttpRequestMessage request )
        {
            Arg.NotNull( request, nameof( request ) );
            traceWriter = request.GetConfiguration().Services.GetTraceWriter() ?? NullTraceWriter.Instance;
            currentRequest = request;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerRequestTraceWriter"/> class.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter">trace writer</see> to decorate.</param>
        /// <param name="request">The current <see cref="HttpRequestMessage">request</see> associated with the trace writer.</param>
        public PerRequestTraceWriter( ITraceWriter traceWriter, HttpRequestMessage request )
        {
            Arg.NotNull( traceWriter, nameof( traceWriter ) );
            this.traceWriter = traceWriter;
            currentRequest = request;
        }

        /// <summary>
        /// Traces the specified information if permitted.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to associate with this trace.</param>
        /// <param name="category">A string specifying an arbitrary category name for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel">level</see> of the trace.</param>
        /// <param name="traceAction">An <see cref="Action{T}">action</see> to call for the detailed trace
        /// information if this method determines the trace request will be honored.  The action is expected
        /// to fill in the given <see cref="TraceRecord">trace record</see> with any information it wishes.</param>
        /// <remarks>The implementation for this instance performs no operation.</remarks>
        public void Trace( HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction )
        {
            traceWriter.Trace( request ?? currentRequest, category, level, traceAction );
        }
    }
}
