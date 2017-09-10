namespace More.Web.Http.Tracing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http.Tracing;

    /// <summary>
    /// Represents the well-known trace property keys for <see cref="TraceRecord">trace records</see>.
    /// </summary>
    [SuppressMessage( "Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Intentional so the type can be extended via inheritance for convenience." )]
    public class TracePropertyKeys
    {
        /// <summary>
        /// Gets the key for the duration trace record property.
        /// </summary>
        /// <value>The "More.Trace.Duration" trace record property.</value>
        public const string Duration = "More.Trace.Duration";
    }
}