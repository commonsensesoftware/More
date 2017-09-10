namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Composition.Hosting.Core;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <content>
    /// Provides additional implementation specific to web applications.
    /// </content>
    public partial class Host
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        /// <param name="existing">The existing <see cref="Host">host</see> to initialize from.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected Host( Host existing ) : base( existing )
        {
            Arg.NotNull( existing, nameof( existing ) );

            activityConfigurations = existing.activityConfigurations;
            activityTypes = existing.activityTypes;
            configSettingLocator = existing.configSettingLocator;
            configuration = existing.configuration;
            containerConfigurations = existing.containerConfigurations;
            conventionsHolder = existing.conventionsHolder;

            var boundaryNames = new Dictionary<string, object> { { "SharingBoundaryNames", new[] { Boundary.PerRequest } } };
            var contract = new CompositionContract( typeof( ExportFactory<CompositionContext> ), null, boundaryNames );
            var factory = (ExportFactory<CompositionContext>) existing.Container.GetExport( contract );

            container = new Lazy<CompositionContext>( () => factory.CreateExport().Value );
        }

        /// <summary>
        /// Initializes a new instance of the current host with a lifetime that aligns to the current request.
        /// </summary>
        /// <returns>A new <see cref="Host">hsot</see> instance.</returns>
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "New host must be disposed by the caller." )]
        public virtual Host CreatePerRequest()
        {
            Contract.Ensures( Contract.Result<Host>() != null );
            return new Host( this );
        }

        /// <summary>
        /// Runs the host using the specified application builder.
        /// </summary>
        /// <param name="hostedAssemblies">An <see cref="Array">array</see> of hosted, composable <see cref="Assembly">assemblies</see>.</param>
        public void Run( params Assembly[] hostedAssemblies )
        {
            Arg.NotNull( hostedAssemblies, nameof( hostedAssemblies ) );
            Run( hostedAssemblies.AsEnumerable() );
        }

        /// <summary>
        /// Runs the host using the specified application builder.
        /// </summary>
        /// <param name="hostedAssemblies">An <see cref="IEnumerable{T}">sequence</see> of hosted, composable <see cref="Assembly">assemblies</see>.</param>
        public virtual void Run( IEnumerable<Assembly> hostedAssemblies )
        {
            Arg.NotNull( hostedAssemblies, nameof( hostedAssemblies ) );

            var assemblies = new HashSet<Assembly>( hostedAssemblies ).Where( a => !Configuration.IsRegistered( a ) ).ToArray();

            Configuration.WithAssemblies( assemblies );

            if ( ServiceProvider.Current == ServiceProvider.Default )
            {
                ServiceProvider.SetCurrent( this );
            }

            Activities.Where( a => a.CanExecute( this ) ).ForEach( a => a.Execute( this ) );

            if ( UnitOfWork.Provider == UnitOfWork.DefaultProvider )
            {
                UnitOfWork.Provider = new UnitOfWorkFactoryProvider( Container.GetExports<IUnitOfWorkFactory> );
            }
        }

        static object LocateSetting( string key, Type type )
        {
            if ( typeof( ConnectionStringSettings ).IsAssignableFrom( type ) )
            {
                return ConfigurationManager.ConnectionStrings[key];
            }

            return ConfigurationManager.AppSettings[key];
        }
    }
}