namespace More.Composition.Hosting
{
    using Microsoft.Owin;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the OWIN middleware that connects the application host to the request pipeline.
    /// </summary>
    public class HostMiddleware : OwinMiddleware
    {
        private readonly Host host;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next <see cref="OwinMiddleware">middleware</see> in the pipeline.</param>
        /// <param name="host">The application <see cref="Host">host</see>.</param>
        public HostMiddleware( OwinMiddleware next, Host host )
            : base( next )
        {
            Arg.NotNull( host, nameof( host ) );
            this.host = host;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext">context</see> in which the middleware is being executed.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        public override async Task Invoke( IOwinContext context )
        {
            Contract.Assume( context != null );

            using ( var currentHost = host.CreatePerRequest() )
            {
                context.Set( "More.Host", currentHost );
                await Next?.Invoke( context );
            }
        }
    }
}
