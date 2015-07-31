namespace System.Web.Http.Tracing
{
    using More.Web.Http.Tracing;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods of the <see cref="TraceRecord"/> class.
    /// </summary>
    public static class TraceRecordExtensions
    {
        /// <summary>
        /// Sets the message associated with a trace record.
        /// </summary>
        /// <param name="traceRecord">The extended <see cref="TraceRecord"/>.</param>
        /// <param name="message">The message to set.</param>
        /// <returns>The original <see cref="TraceRecord"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static TraceRecord SetMessage( this TraceRecord traceRecord, string message )
        {
            Arg.NotNull( traceRecord, nameof( traceRecord ) );
            traceRecord.Message = message;
            return traceRecord;
        }

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

            object value;

            if ( traceRecord.Properties.TryGetValue( TracePropertyKeys.Duration, out value ) && value is TimeSpan )
                return (TimeSpan) value;

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
