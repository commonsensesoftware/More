namespace More.Web.Http.Tracing
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Web.Http.Tracing;

    /// <summary>
    /// Represents a stopwatch for the begin and end of a trace operation.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of result returned from a traced operation.</typeparam>
    public class TraceStopwatch<T> : Stopwatch
    {
        private readonly Action<TraceRecord> startTrace;
        private readonly Action<TraceRecord, T> endTrace;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceStopwatch"/> class.
        /// </summary>
        /// <param name="startTrace">The start trace <see cref="Action{T}">action</see>.</param>
        /// <param name="endTrace">The end trace <see cref="Action{T1,T2}">action</see>.</param>
        public TraceStopwatch( Action<TraceRecord> startTrace, Action<TraceRecord, T> endTrace )
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
            Arg.NotNull( traceRecord, nameof( traceRecord ) );

            Start();
            startTrace( traceRecord );
        }

        /// <summary>
        /// Performs the start trace operation.
        /// </summary>
        /// <param name="traceRecord">The <see cref="TraceRecord">trace record</see> associated with the operation.</param>
        /// <param name="result">The result of the operation.</param>
        public void EndTrace( TraceRecord traceRecord, T result )
        {
            Arg.NotNull( traceRecord, nameof( traceRecord ) );

            Stop();
            traceRecord.SetDuration( Elapsed );
            endTrace( traceRecord, result );
        }
    }
}
