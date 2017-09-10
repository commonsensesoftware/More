namespace System.Web.Http.Tracing
{
    using Diagnostics.CodeAnalysis;
    using More.Web.Http.Tracing;
    using System;

    /// <summary>
    /// Provides extension methods of the <see cref="TraceRecord"/> class.
    /// </summary>
    public static class TraceRecordExtensions
    {
        /// <summary>
        /// Gets the duration associated with the trace record.
        /// </summary>
        /// <param name="traceRecord">The extended <see cref="TraceRecord"/>.</param>
        /// <returns>The <see cref="Nullable{T}">nullable</see> <see cref="TimeSpan"/> associated with the
        /// <paramref name="traceRecord">trace record</paramref>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static TimeSpan? GetDuration( this TraceRecord traceRecord )
        {
            Arg.NotNull( traceRecord, nameof( traceRecord ) );

            if ( traceRecord.Properties.TryGetValue( TracePropertyKeys.Duration, out var value ) && value is TimeSpan timespan )
            {
                return timespan;
            }

            return null;
        }

        /// <summary>
        /// Sets the duration associated with a trace record.
        /// </summary>
        /// <param name="traceRecord">The extended <see cref="TraceRecord"/>.</param>
        /// <param name="duration">The <see cref="TimeSpan">duration</see> of the associated operation.</param>
        /// <returns>The original <see cref="TraceRecord"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static TraceRecord SetDuration( this TraceRecord traceRecord, TimeSpan duration )
        {
            Arg.NotNull( traceRecord, nameof( traceRecord ) );
            Arg.GreaterThanOrEqualTo( duration, TimeSpan.Zero, nameof( duration ) );
            traceRecord.Properties[TracePropertyKeys.Duration] = duration;
            return traceRecord;
        }
    }
}