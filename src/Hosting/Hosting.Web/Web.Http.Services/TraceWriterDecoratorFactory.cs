namespace More.Web.Http.Services
{
    using More.Web.Http.Tracing;
    using System.Net.Http;
    using System.Web.Http.Tracing;

    /// <summary>
    /// Represents a Per-Request decorator factory for a <see cref="ITraceWriter">trace writer</see>.
    /// </summary>
    /// <remarks>Creates <see cref="ITraceWriter">trace writer</see> with an associated <see cref="HttpRequestMessage">request</see>
    /// for scenarios where a component may not know or have access to the current request pipeline.</remarks>
    public class TraceWriterDecoratorFactory : IDecoratorFactory<ITraceWriter>
    {
        /// <summary>
        /// Creates and returns a decorator for the specified request and instance.
        /// </summary>
        /// <param name="request">The current <see cref="HttpRequestMessage">request</see> to create a decorator for.</param>
        /// <param name="instance">The <see cref="ITraceWriter">trace writer</see> to be decorated.</param>
        /// <returns>An new <see cref="ITraceWriter">trace writer</see> that decorates the original <paramref name="instance"/>.</returns>
        public ITraceWriter CreatePerRequestDecorator( HttpRequestMessage request, ITraceWriter instance )
        {
            return new PerRequestTraceWriter( instance, request );
        }
    }
}
