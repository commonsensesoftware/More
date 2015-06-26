namespace More.Diagnostics.Tracing
{
    using System;

    /// <summary>
    /// Represents the possible tracing levels.
    /// </summary>
    public enum TraceLevel
    {
        /// <summary>
        /// Tracing is disabled.
        /// </summary>
        Off,

        /// <summary>
        /// Trace level for debugging traces.
        /// </summary>
        Debug,

        /// <summary>
        /// Trace level for informational traces.
        /// </summary>
        Info,

        /// <summary>
        /// Trace level for warning traces.
        /// </summary>
        Warn,

        /// <summary>
        /// Trace level for error traces.
        /// </summary>
        Error,

        /// <summary>
        /// Trace level for fatal traces.
        /// </summary>
        Fatal
    }
}
