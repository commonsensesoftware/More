namespace More.Composition.Hosting
{
    using More.Collections.Generic;
    using More.ComponentModel;
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Composition;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using ServiceContainer = More.ServiceContainer;

    /// <summary>
    /// Represents an application composition host.
    /// </summary>
    [SuppressMessage( "Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Service containers and locators may have high coupling." )]
    public partial class Host : ServiceContainer, ICompositionService, IDisposable
    {
        private static readonly Type[] ExportedInterfaces = new[] { typeof( IServiceContainer ), typeof( ICompositionService ), typeof( IServiceProvider ) };
        private static readonly Type[] ExportedTypes = new[] { typeof( CompositionContext ), typeof( CompositionHost ) };
        private readonly ContainerConfiguration configuration;
        private readonly List<Type> activityTypes = new List<Type>();
        private readonly Dictionary<Type, IActivityConfiguration> configurations = new Dictionary<Type, IActivityConfiguration>();
        private readonly Lazy<CompositionHost> container;
        private readonly Lazy<ConventionBuilder> conventionsHolder;
        private readonly Func<string, object> configSettingLocator;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        public Host()
            : this( new ContainerConfiguration(), LocateSetting )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        /// <param name="configurationSettingLocator">The user-defined <see cref="Func{T,TResult}">function</see> used to resolve composable configuration settings.</param>
        public Host( Func<string, object> configurationSettingLocator )
            : this( new ContainerConfiguration(), configurationSettingLocator )
        {
            Contract.Requires<ArgumentNullException>( configurationSettingLocator != null, "configurationSettingLocator" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="ContainerConfiguration">configuration</see> to initialize the host with.</param>
        [CLSCompliant( false )]
        public Host( ContainerConfiguration configuration )
            : this( configuration, LocateSetting )
        {
            Contract.Requires<ArgumentNullException>( configuration != null, "configuration" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="ContainerConfiguration">configuration</see> to initialize the host with.</param>
        /// <param name="configurationSettingLocator">The user-defined <see cref="Func{T,TResult}">function</see> used to resolve composable configuration settings.</param>
        [CLSCompliant( false )]
        public Host( ContainerConfiguration configuration, Func<string, object> configurationSettingLocator )
        {
            Contract.Requires<ArgumentNullException>( configuration != null, "configuration" );
            Contract.Requires<ArgumentNullException>( configurationSettingLocator != null, "configurationSettingLocator" );

            this.configuration = configuration;
            this.container = new Lazy<CompositionHost>( this.CreateContainer );
            this.conventionsHolder = new Lazy<ConventionBuilder>( this.GetConventions );
            this.configSettingLocator = configurationSettingLocator;
        }

        /// <summary>
        /// Gets the configuration for the host container.
        /// </summary>
        /// <value>The host <see cref="ContainerConfiguration">container configuration</see>.</value>
        [CLSCompliant( false )]
        protected ContainerConfiguration Configuration
        {
            get
            {
                Contract.Ensures( this.configuration != null );
                return this.configuration;
            }
        }

        /// <summary>
        /// Gets the composition container for the host.
        /// </summary>
        /// <value>A <see cref="CompositionHost"/> object.</value>
        [CLSCompliant( false )]
        protected CompositionHost Container
        {
            get
            {
                Contract.Ensures( Contract.Result<CompositionHost>() != null );
                this.CheckDisposed();
                return this.container.Value;
            }
        }

        /// <summary>
        /// Gets a read-only list of activities, which are resolved, configured, and composed in dependent order.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IActivity">activities</see>.</value>
        protected virtual IReadOnlyList<IActivity> Activities
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<IActivity>>() != null );
                this.CheckDisposed();
                return this.QueryActivities();
            }
        }

        private void CheckDisposed()
        {
            if ( this.disposed )
                throw new ObjectDisposedException( this.GetType().Name );
        }

        private IReadOnlyList<IActivity> QueryActivities()
        {
            Contract.Ensures( Contract.Result<IReadOnlyList<IActivity>>() != null );

            var composedTasks = this.GetComposedActivities();
            var correlatedTasks = CorrelateActivityDependencies( composedTasks ).ToArray();

            return correlatedTasks;
        }

        private static IActivity GetExportedActivity( IEnumerable<ExportFactory<IActivity, ExportMetadata>> activityExports, Type activityType )
        {
            Contract.Requires( activityExports != null );
            Contract.Requires( activityType != null );
            Contract.Ensures( Contract.Result<IActivity>() != null );

            var matches = activityExports.Where( a => a.Metadata.ExportedType.Equals( activityType ) ).ToArray();

            switch ( matches.Length )
            {
                case 0:
                    throw new HostException( ExceptionMessage.MissingStartupActivity.FormatDefault( activityType ) );
                case 1:
                    return matches[0].CreateExport().Value;
                default:
                    throw new HostException( ExceptionMessage.MultipleStartupActivitiesExported.FormatDefault( activityType ) );
            }
        }

        private KeyedCollection<Type, Tuple<IActivity, IEnumerable<Type>>> GetComposedActivities()
        {
            Contract.Ensures( Contract.Result<KeyedCollection<Type, Tuple<IActivity, IEnumerable<Type>>>>() != null );

            var composedActivities = new ObservableKeyedCollection<Type, Tuple<IActivity, IEnumerable<Type>>>( t => t.Item1.GetType() );
            var activityExports = this.Container.GetExports<ExportFactory<IActivity, ExportMetadata>>().ToArray();

            foreach ( var activityType in this.activityTypes )
            {
                var activity = GetExportedActivity( activityExports, activityType );
                IActivityConfiguration config;
                Tuple<IActivity, IEnumerable<Type>> tuple;

                // look up configuration
                if ( this.configurations.TryGetValue( activityType, out config ) )
                {
                    // apply activity configuration
                    config.Configure( activity );

                    // create pairing of activity and dependencies
                    tuple = new Tuple<IActivity, IEnumerable<Type>>( activity, config.Dependencies.Union( activity.DependsOn() ).ToArray() );
                }
                else
                {
                    // no configuration so only consider dependencies from attribution
                    tuple = new Tuple<IActivity, IEnumerable<Type>>( activity, activity.DependsOn().ToArray() );
                }

                composedActivities.Add( tuple );
            }

            return composedActivities;
        }

        private static IEnumerable<IActivity> CorrelateActivityDependencies( KeyedCollection<Type, Tuple<IActivity, IEnumerable<Type>>> activities )
        {
            Contract.Requires( activities != null );
            Contract.Ensures( Contract.Result<IEnumerable<IActivity>>() != null );

            // auto-correlate dependencies
            foreach ( var activity in activities )
            {
                var current = activity.Item1;
                var dependencies = activity.Item2;

                // add matching dependent activity
                foreach ( var dependency in dependencies )
                {
                    // the specified activity wasn't found
                    if ( !activities.Contains( dependency ) )
                        throw new HostException( ExceptionMessage.MissingDependentActivity.FormatDefault( current.Name, current.GetType(), dependency ) );

                    current.Dependencies.Add( activities[dependency].Item1 );
                }

                yield return current;
            }
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="Host"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        protected override void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;
            base.Dispose( disposing );

            if ( !disposing )
                return;

            this.activityTypes.Clear();
            this.configurations.Clear();

            if ( this.container.IsValueCreated )
                this.container.Value.Dispose();
        }

        partial void AddUISpecificConventions( ConventionBuilder builder );

        partial void AddPlatformSpecificConventions( ConventionBuilder builder );

        /// <summary>
        /// Gets the attributed model conventions used by the host.
        /// </summary>
        /// <returns>A <see cref="ConventionBuilder">convention builder</see>.</returns>
        [CLSCompliant( false )]
        protected virtual ConventionBuilder GetConventions()
        {
            Contract.Ensures( Contract.Result<ConventionBuilder>() != null );

            var assembly = new HostAssemblySpecification();
            var unitOfWork = new InterfaceSpecification( typeof( IUnitOfWork<> ) );
            var unitOfWorkFactory = new InterfaceSpecification( typeof( IUnitOfWorkFactory ) );
            var repository = new InterfaceSpecification( typeof( IReadOnlyRepository<> ) );
            var dataAccess = repository.Or( unitOfWork );
            var builder = new ConventionBuilder();

            // build default conventions
            builder.ForTypesDerivedFrom<IActivity>().Export( b => b.AddMetadata( "ExportedType", t => t ) ).Export<IActivity>();
            builder.ForTypesMatching<IUnitOfWorkFactory>( unitOfWorkFactory.IsSatisfiedBy ).Export<IUnitOfWorkFactory>().Shared();
            builder.ForTypesMatching( dataAccess.IsSatisfiedBy ).ExportInterfaces( assembly.IsSatisfiedBy );
            this.AddUISpecificConventions( builder );
            this.AddPlatformSpecificConventions( builder );

            return builder;
        }

        /// <summary>
        /// Registers an activity to execute when the host starts.
        /// </summary>
        /// <typeparam name="TActivity">The <see cref="Type">type</see> of <see cref="IActivity">activity</see> to register.</typeparam>
        /// <returns>A <see cref="ActivityConfiguration{T}">activity configuration</see> object.</returns>
        /// <example>This example illustrates how to register startup activities.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System.ComponentModel;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        /// using System;
        /// 
        /// public class Program
        /// {
        ///     [STAThread]
        ///     public static void Main()
        ///     {
        ///         var configRoot = new Uri( "myconfig.xml", UriKind.Relative );
        ///         
        ///         using ( var host = new Host() )
        ///         {
        ///             host.Register<LoadConfiguration>();
        ///             host.Register<CreateSecurityPrincipal>();
        ///             host.Register<ShowShellView>();
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        public ActivityConfiguration<TActivity> Register<TActivity>() where TActivity : IActivity
        {
            Contract.Ensures( Contract.Result<ActivityConfiguration<TActivity>>() != null );
            this.CheckDisposed();

            var activityType = typeof( TActivity );

            if ( this.activityTypes.Contains( activityType ) )
                throw new InvalidOperationException( ExceptionMessage.ActivityCannotBeRegisteredMoreThanOnce.FormatDefault( activityType ) );

            this.activityTypes.Add( activityType );

            IActivityConfiguration config;

            if ( this.configurations.TryGetValue( activityType, out config ) )
                return (ActivityConfiguration<TActivity>) config;

            var newConfiguration = new ActivityConfiguration<TActivity>();
            this.configurations.Add( activityType, newConfiguration );

            return newConfiguration;
        }

        /// <summary>
        /// Configures an activity registered for execution when the host starts.
        /// </summary>
        /// <typeparam name="TActivity">The <see cref="Type">type</see> of <see cref="IActivity">activity</see> to configure.</typeparam>
        /// <returns>A <see cref="ActivityConfiguration{T}">activity configuration</see> object.</returns>
        /// <remarks>If the specified <typeparamref name="TActivity">activity</typeparamref> has not be registered, it will
        /// be automatically registered when the <see cref="ActivityConfiguration{T}">configuration</see> is created.</remarks>
        /// <example>This example illustrates how to register and configure a startup activity.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        /// 
        /// public class Program
        /// {
        ///     [STAThread]
        ///     public static void Main()
        ///     {
        ///         var configRoot = new Uri( "myconfig.xml", UriKind.Relative );
        ///         
        ///         using ( var host = new Host() )
        ///         {
        ///             // register startup activity
        ///             host.Register<LoadConfiguration>();
        ///             
        ///             // get configuration and define new configuration operation
        ///             host.WithConfiguration<LoadConfiguration>().Configure( lc => lc.ConfigurationRoot = configRoot );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        public ActivityConfiguration<TActivity> WithConfiguration<TActivity>() where TActivity : IActivity
        {
            Contract.Ensures( Contract.Result<ActivityConfiguration<TActivity>>() != null );
            this.CheckDisposed();

            var activityType = typeof( TActivity );

            if ( !this.activityTypes.Contains( activityType ) )
                this.activityTypes.Add( activityType );

            IActivityConfiguration config;

            if ( this.configurations.TryGetValue( activityType, out config ) )
                return (ActivityConfiguration<TActivity>) config;

            var newConfiguration = new ActivityConfiguration<TActivity>();
            this.configurations.Add( activityType, newConfiguration );

            return newConfiguration;
        }

        /// <summary>
        /// Adds the specified service to the service container, and optionally promotes the service to parent service containers.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to add.</param>
        /// <param name="callback">A <see cref="ServiceCreatorCallback">callback</see> object that is used to create the service.
        /// This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        public override void AddService( Type serviceType, ServiceCreatorCallback callback, bool promote )
        {
            this.CheckDisposed();

            var @new = callback;
            ServiceCreatorCallback composedCallback = ( c, t ) =>
            {
                var instance = @new( c, t );
                this.Container.SatisfyImports( instance );
                return instance;
            };

            base.AddService( serviceType, composedCallback, promote );
        }

        /// <summary>
        /// Gets a service of the requested type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to return.</param>
        /// <returns>The service instance corresponding to the requested <paramref name="serviceType">service type</paramref>
        /// or <c>null</c> if no match is found.</returns>
        public override object GetService( Type serviceType )
        {
            this.CheckDisposed();

            if ( serviceType == null )
                throw new ArgumentNullException( "serviceType" );

            var generator = new ServiceTypeDisassembler();
            var key = generator.ExtractKey( serviceType );
#if WINDOWS_PHONE_APP
            return this.GetService( serviceType, key );
#else
            Type innerServiceType;
            object service = null;

            // return multiple services, if requested
            if ( generator.IsForMany( serviceType, out innerServiceType ) )
            {
                var exports = new List<object>();

                if ( key == null )
                {
                    // if no key is specified and the requested type matches an interface we implement, add ourself
                    if ( ExportedInterfaces.Contains( innerServiceType ) )
                        exports.Add( this );
                    else if ( ExportedTypes.Contains( innerServiceType ) )
                        exports.Add( this.Container );
                }

                // add any matching, manually added services
                if ( ( service = base.GetService( innerServiceType ) ) != null )
                    exports.Add( service );

                // add matching exports
                exports.AddRange( this.Container.GetExports( innerServiceType, key ) );
                return exports;
            }

            // if no key is specified and the requested type matches an interface we implement, return ourself
            if ( key == null )
            {
                if ( ExportedInterfaces.Contains( serviceType ) )
                    return this;
                else if ( ExportedTypes.Contains( serviceType ) )
                    return this.Container;
            }

            // return any matching, manually added services
            if ( ( service = base.GetService( serviceType ) ) != null )
                return service;

            // return matching export
            object export;
            this.Container.TryGetExport( serviceType, key, out export );

            return export;
#endif
        }

        /// <summary>
        /// Composes the specified object instance.
        /// </summary>
        /// <param name="instance">The object to compose.</param>
        public virtual void Compose( object instance )
        {
            if ( instance != null )
                this.Container.SatisfyImports( instance, this.conventionsHolder.Value );
        }
    }
}
