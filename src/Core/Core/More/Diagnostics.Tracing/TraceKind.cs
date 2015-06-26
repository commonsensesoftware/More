namespace More.Diagnostics.Tracing
{
    using System;

    /// <summary>
    /// Represents the possible kinds of tracing operations.
    /// </summary>
    public enum TraceKind
    {
        /// <summary>
        /// Single trace, not part of a Begin/End trace pair.
        /// </summary>
        Trace,

        /// <summary>
        /// Trace marking the beginning of some operation.
        /// </summary>
        Begin,

        /// <summary>
        /// Trace marking the end of some operation.
        /// </summary>
        End
    }
}
