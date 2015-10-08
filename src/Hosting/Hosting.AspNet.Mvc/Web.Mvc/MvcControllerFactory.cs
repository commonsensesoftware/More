namespace More.Web.Mvc
{
    using System;
    using System.ComponentModel.Design;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    internal sealed class MvcControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController( RequestContext requestContext, string controllerName )
        {
            if ( requestContext != null )
            {
                var container = DependencyResolver.Current.GetService<IServiceContainer>();

                // if a container is available within the current scope, register the current request
                if ( container != null )
                    container.AddService( typeof( HttpRequestBase ), requestContext.HttpContext.Request );
            }

            return base.CreateController( requestContext, controllerName );
        }
    }
}
