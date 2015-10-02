namespace More.Composition.Hosting
{
    using ComponentModel;
    using Owin;
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting.Core;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <content>
    /// Provides additional implementation specific to web applications.
    /// </content>
    public partial class Host
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        /// <param name="existing">The existing <see cref="Host">host</see> to initialize from.</param>
        protected Host( Host existing )
        {
            Arg.NotNull( existing, nameof( existing ) );

            configuration = existing.configuration;
            containerConfigurations = existing.containerConfigurations;
            activityTypes = existing.activityTypes;
            activityConfigurations = existing.activityConfigurations;
            conventionsHolder = existing.conventionsHolder;
            configSettingLocator = existing.configSettingLocator;

            var boundaryNames = new Dictionary<string, object> { { "SharingBoundaryNames", new[] { Boundary.PerRequest } } };
            var contract = new CompositionContract( typeof( ExportFactory<CompositionContext> ), null, boundaryNames );
            var factory = (ExportFactory<CompositionContext>) existing.Container.GetExport( contract );

            container = new Lazy<CompositionContext>( () => factory.CreateExport().Value );
        }

        /// <summary>
        /// Initializes a new instance of the current host with a lifetime that aligns to the current request.
        /// </summary>
        /// <returns>A new <see cref="Host">hsot</see> instance.</returns>
        public virtual Host CreatePerRequest()
        {
            Contract.Ensures( Contract.Result<Host>() != null );
            return new Host( this );
        }

        private static object LocateSetting( string key )
        {
            object value = ConfigurationManager.AppSettings[key];
            return value ?? ConfigurationManager.ConnectionStrings[key];
        }

        /// <summary>
        /// Runs the host using the specified application builder.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder">application builder</see> the host will run with.</param>
        /// <remarks>The default implementation will register itself as <see cref="HostMiddleware">middleware</see> with the application.</remarks>
        public virtual void Run( IAppBuilder builder )
        {
            Arg.NotNull( builder, nameof( builder ) );

            // set current service provider if unset
            if ( ServiceProvider.Current == ServiceProvider.Default )
                ServiceProvider.SetCurrent( this );

            // build up and execute the startup activities
            foreach ( var activity in Activities.Where( a => a.CanExecute( this ) ) )
                activity.Execute( this );

            // set the default unit of work if unset
            if ( UnitOfWork.Provider == UnitOfWork.DefaultProvider )
                UnitOfWork.Provider = new UnitOfWorkFactoryProvider( Container.GetExports<IUnitOfWorkFactory> );

            // register ourself as middleware
            builder.Use<HostMiddleware>( this );
        }
    }
}
