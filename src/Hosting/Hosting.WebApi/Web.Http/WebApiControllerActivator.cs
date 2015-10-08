namespace More.Web.Http
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;

    internal sealed class WebApiControllerActivator : IHttpControllerActivator
    {
        private readonly IHttpControllerActivator decorated;

        internal WebApiControllerActivator( IHttpControllerActivator decorated )
        {
            Contract.Requires( decorated != null );
            this.decorated = decorated;
        }

        /// <summary>
        /// Creates the <see cref="T:System.Web.Http.Controllers.IHttpController" /> specified by <paramref name="controllerType" /> using the given <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>An instance of type <paramref name="controllerType" />.</returns>
        public IHttpController Create( HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType )
        {
            if ( request != null )
            {
                var container = request.GetDependencyScope().GetService<IServiceContainer>();

                // if a container is available within the current scope, register the current request
                if ( container != null )
                    container.AddService( typeof( HttpRequestMessage ), request );

                var context = container.GetService<System.Composition.CompositionContext>();
                var type = Type.GetType( "More.ComponentModel.IReadOnlyRepository`1[[DataAccess.Person, DataAccess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], More, Version=1.1.0.0, Culture=neutral, PublicKeyToken=5e67f9a3da787917", true );
                try
                {
                    var export = context.GetExport( type, "Decorated" );
                }
                catch ( Exception ex )
                {
                }
            }

            return decorated.Create( request, controllerDescriptor, controllerType );
        }
    }
}
