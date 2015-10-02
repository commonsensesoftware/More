namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public static class HostExtensions
    {
        public static T ConfigureMvc<T>( this T host, IRule<ImportParameter> importParameterRule ) where T : Host
        {
            Arg.NotNull( host, nameof( host ) );
            Arg.NotNull( importParameterRule, nameof( importParameterRule ) );

            var conventions = new MvcConventions() { ImportParameterRule = importParameterRule };

            host.Configure( conventions.Configure );

            //DependencyResolver.SetResolver( new CompositionScopeDependencyResolver( GetContext ) );
            //FilterProviders.Providers.Remove( FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().SingleOrDefault() );
            //FilterProviders.Providers.Add( new CompositionScopeFilterAttributeFilterProvider( GetContext ) );
            //ModelBinderProviders.BinderProviders.Add( new CompositionScopeModelBinderProvider( GetContext ) );

            return host;
        }
    }
}
