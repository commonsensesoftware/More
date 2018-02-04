namespace More.Web.Http.Tracing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http.Tracing;

    /// <summary>
    /// Represents the well-known trace property keys for <see cref="TraceRecord">trace records</see>.
    /// </summary>
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class TracePropertyKeys
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        /// <summary>
        /// Gets the key for the duration trace record property.
        /// </summary>
        /// <value>The "More.Trace.Duration" trace record property.</value>
        public const string Duration = "More.Trace.Duration";
    }
}