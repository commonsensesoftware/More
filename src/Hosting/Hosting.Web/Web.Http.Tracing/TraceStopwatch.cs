namespace More.Web.Http.Tracing
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Web.Http.Tracing;

    /// <summary>
    /// Represents a stopwatch for the begin and end of a trace operation.
    /// </summary>
    public class TraceStopwatch : Stopwatch
    {
        private readonly Action<TraceRecord> startTrace;
        private readonly Action<TraceRecord> endTrace;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceStopwatch"/> class.
        /// </summary>
        /// <param name="startTrace">The start trace <see cref="Action{T}">action</see>.</param>
        /// <param name="endTrace">The end trace <see cref="Action{T}">action</see>.</param>
        public TraceStopwatch( Action<TraceRecord> startTrace, Action<TraceRecord> endTrace )
        {
            this.startTrace = startTrace ?? DefaultAction.None;
            this.endTrace = endTrace ?? DefaultAction.None;
        }

        /// <summary>
        /// Performs the start trace operation.
        /// </summary>
        /// <param name="traceRecord">The <see cref="TraceRecord">trace record</see> associated with the operation.</param>
        public void StartTrace( TraceRecord traceRecord )
        {
            Contract.Requires<ArgumentNullException>( traceRecord != null, "traceRecord" );

            this.Start();
            this.startTrace( traceRecord );
        }

        /// <summary>
        /// Performs the start trace operation.
        /// </summary>
        /// <param name="traceRecord">The <see cref="TraceRecord">trace record</see> associated with the operation.</param>
        public void EndTrace( TraceRecord traceRecord )
        {
            Contract.Requires<ArgumentNullException>( traceRecord != null, "traceRecord" );

            this.Stop();
            traceRecord.SetDuration( this.Elapsed );
            this.endTrace( traceRecord );
        }
    }
}
