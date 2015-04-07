namespace More.Web.Http.Tracing
{
    using System;
    using System.Composition;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Web.Http.Tracing;

    /// <summary>
    /// Represents a <see cref="ITraceWriter">trace writer</see> that implements the Null-Object pattern.
    /// </summary>
    [PartNotDiscoverable]
    public sealed class NullTraceWriter : ITraceWriter
    {
        private static readonly NullTraceWriter instance = new NullTraceWriter();

        private NullTraceWriter()
        {
        }

        /// <summary>
        /// Gets an instance of the <see cref="ITraceWriter"/>.
        /// </summary>
        /// <value>A thread-safe, singleton instance of the <see cref="ITraceWriter"/>.</value>
        public static ITraceWriter Instance
        {
            get
            {
                Contract.Ensures( instance != null );
                return instance;
            }
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
        }
    }
}
