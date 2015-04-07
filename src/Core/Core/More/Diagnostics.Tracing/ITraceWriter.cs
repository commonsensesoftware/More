namespace More.Diagnostics.Tracing
{
    using global::System;

    /// <summary>
    /// Defines the behavior of trace writer.
    /// </summary>
    public interface ITraceWriter
    {
        /// <summary>
        /// Creates and writes a new <see cref="TraceRecord"/> to the current <see cref="ITraceWriter"/>
        /// if tracing is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="category">The logical category for the trace.  Users may define their own.</param>
        /// <param name="level">The <see cref="TraceLevel"/> at which to write this trace.</param>
        /// <param name="traceAction">The action to invoke if tracing is enabled.  The caller is expected
        /// to fill in any or all of the values of the given <see cref="TraceRecord"/> in this action.</param>
        /// <remarks>
        /// The decision whether tracing is enabled for a specific category and level
        /// is an implementation detail of each individual <see cref="ITraceWriter"/>.
        /// <para>
        /// If the current <see cref="ITraceWriter"/> decides tracing is enabled for the given 
        /// category and level, it will construct a new <see cref="TraceRecord"/> and invoke 
        /// the caller's <paramref name="traceAction"/> to allow the caller to fill in additional
        /// information.
        /// </para>
        /// <para>
        /// If the current <see cref="ITraceWriter"/> decides tracing is not enabled for the given
        /// category and level, no <see cref="TraceRecord"/> will be created,
        /// and the <paramref name="traceAction"/> will not be called.
        /// </para> 
        /// </remarks>
        void Trace( string category, TraceLevel level, Action<TraceRecord> traceAction );
    }
}
