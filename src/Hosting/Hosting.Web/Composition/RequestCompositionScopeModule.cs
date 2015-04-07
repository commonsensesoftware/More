namespace More.Composition
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    /// <summary>
    /// Provides lifetime management for the <see cref="CompositionProvider"/> class.
    /// </summary>
    /// <remarks>This module is automatically injected into the ASP.NET request processing pipeline at startup and should not be called by user code.</remarks>
    public sealed class RequestCompositionScopeModule : IHttpModule, IDisposable
    {
        private static volatile bool started;

        /// <summary>
        /// Performs intialization work before the application starts.
        /// </summary>
        /// <remarks>This method is automatically called at startup and should not be called by user code.</remarks>
        public static void Start()
        {
            if ( started )
                return;

            started = true;
            DynamicModuleUtility.RegisterModule( typeof( RequestCompositionScopeModule ) );
        }

        private static void OnEndRequest( object sender, EventArgs e )
        {
            var scope = CompositionProvider.Current;

            if ( scope != null )
                scope.Dispose();
        }

        /// <summary>
        /// Release resources used by the module.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="context">The <see cref="HttpApplication">application</see> in which the module is running.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The ASP.NET runtime will never pass null here." )]
        public void Init( HttpApplication context )
        {
            Debug.Assert( context != null );

            context.EndRequest += OnEndRequest;
            CompositionProvider.Initialize();
        }
    }
}