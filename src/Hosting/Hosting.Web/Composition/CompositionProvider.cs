namespace More.Composition
{
    using More.Collections.Generic;
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Composition.Hosting.Core;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Mvc;

    /// <summary>
    /// Provides composition services to ASP.NET MVC by integrating DependencyResolver with the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>This class is self-configuring and will be enabled by simply being present in the application's bin directory. Most applications
    /// should not need to access this class.</remarks>
    public static class CompositionProvider
    {
        private const string DefaultDependencyResolverName = "DefaultDependencyResolver";
        private const string DefaultUnitOfWorkProviderName = "EmptyFactoryProvider";
        private static readonly Type CompositionProviderKey = typeof( CompositionProvider );
        private static readonly HashSet<Assembly> assemblies = new HashSet<Assembly>( new DynamicComparer<Assembly>( ( a1, a2 ) => AssemblyName.ReferenceMatchesDefinition( a1.GetName(), a2.GetName() ) ? 0 : -1 ) );
        private static CompositionHost container;
        private static ExportFactory<CompositionContext> requestScopeFactory;
        private static ConventionBuilder conventions;

        private static bool IsInitialized
        {
            get
            {
                return requestScopeFactory != null;
            }
        }

        internal static Export<CompositionContext> Current
        {
            get
            {
                return (Export<CompositionContext>) HttpContext.Current.Items[CompositionProviderKey];
            }
            private set
            {
                HttpContext.Current.Items[CompositionProviderKey] = value;
            }
        }

        /// <summary>
        /// Gets the conventions used to compose parts by the composition provider.
        /// </summary>
        /// <value>A <see cref="ConventionBuilder"/> object.</value>
        /// <remarks>Conventions are automatically applied for types that inherit from
        /// <see cref="IController"/> or <see cref="IHttpController"/>.  These conventions
        /// can be cleared via the <see cref="M:ClearConventions"/> method.</remarks>
        [CLSCompliant( false )]
        public static ConventionBuilder Conventions
        {
            get
            {
                Contract.Ensures( conventions != null );

                if ( conventions == null )
                {
                    conventions = new ConventionBuilder();
                    conventions.ForTypesDerivedFrom<IController>().Export();
                    conventions.ForTypesDerivedFrom<IHttpController>().Export();
                }

                return conventions;
            }
        }

        /// <summary>
        /// Gets a list of assemblies containing composed parts to the composition provider.
        /// </summary>
        /// <value>A <see cref="ICollection{T}">list</see> of <see cref="Assembly">assemblies</see>.</value>
        public static ICollection<Assembly> Assemblies
        {
            get
            {
                Contract.Ensures( assemblies != null );
                return assemblies;
            }
        }

        private static CompositionContext GetContext()
        {
            Contract.Ensures( Contract.Result<CompositionContext>() != null );

            var current = Current;

            if ( current == null )
            {
                current = requestScopeFactory.CreateExport();
                Current = current;
            }

            return current.Value;
        }

        /// <summary>
        /// Sets the composition container configuration, which is used to override the default conventions.
        /// </summary>
        /// <param name="configuration">A <see cref="ContainerConfiguration">configuration</see> containing the controller types
        /// and other parts that should be used by the composition provider.</param>
        /// <exception cref="InvalidOperationException">The composition provider has already been initialized.</exception>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void SetConfiguration( ContainerConfiguration configuration )
        {
            Contract.Requires( configuration != null );

            if ( IsInitialized )
                throw new InvalidOperationException( WebExceptionMessage.CompositionProviderAlreadyInitialized );

            container = configuration.CreateContainer();

            var boundaryNames = new Dictionary<string, object> { { "SharingBoundaryNames", new[] { Boundary.HttpRequest, Boundary.DataConsistency, Boundary.UserIdentity } } };
            var factoryContract = new CompositionContract( typeof( ExportFactory<CompositionContext> ), null, boundaryNames );

            requestScopeFactory = (ExportFactory<CompositionContext>) container.GetExport( factoryContract );

            ConfigureMvc();
            ConfigureWebApi();
            ConfigureComposition();
        }

        /// <summary>
        /// Clears the default composition conventions.
        /// </summary>
        /// <remarks>This method should only be used to remove default composition conventions and must
        /// be called before any other conventions are added.</remarks>
        public static void ClearConventions()
        {
            conventions = new ConventionBuilder();
        }

        private static void ConfigureWebApi()
        {
            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new CompositionScopeHttpDependencyResolver( GetContext );
        }

        private static void ConfigureMvc()
        {
            if ( DependencyResolver.Current.GetType().Name != DefaultDependencyResolverName )
                throw new CompositionFailedException( WebExceptionMessage.DependencyResolverConflict );

            DependencyResolver.SetResolver( new CompositionScopeDependencyResolver( GetContext ) );
            FilterProviders.Providers.Remove( FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().SingleOrDefault() );
            FilterProviders.Providers.Add( new CompositionScopeFilterAttributeFilterProvider( GetContext ) );
            ModelBinderProviders.BinderProviders.Add( new CompositionScopeModelBinderProvider( GetContext ) );
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The service provider should remain until the entire application is shut down." )]
        private static void ConfigureComposition()
        {
            // set current service provider if unset
            if ( ServiceProvider.Current == ServiceProvider.Default )
                ServiceProvider.SetCurrent( () => new CompositionContextServiceProvider( GetContext ) );

            // set the default unit of work if unset
            if ( UnitOfWork.Provider == UnitOfWork.DefaultProvider )
                UnitOfWork.Provider = new UnitOfWorkFactoryProvider( GetContext().GetExports<IUnitOfWorkFactory> );
        }

        private static Assembly GuessGlobalApplicationAssembly()
        {
            var current = HttpContext.Current;

            if ( current != null )
            {
                var app = current.ApplicationInstance;

                if ( app != null )
                    return app.GetType().BaseType.Assembly;
            }

            return Assembly.GetEntryAssembly();
        }

        internal static void Initialize()
        {
            if ( IsInitialized )
                return;

            Debug.WriteLine( "Performing default CompositionProvider initialization." );

            var globalAssembly = GuessGlobalApplicationAssembly();

            if ( globalAssembly != null )
                Assemblies.Add( globalAssembly );

            var configuration = new ContainerConfiguration();
            var builder = Conventions;

            configuration.WithDefaultConventions( builder );
            configuration.WithAssemblies( Assemblies );

            SetConfiguration( configuration );
        }
    }
}