namespace More.Composition.Hosting
{
    using Collections.Generic;
    using ComponentModel;
    using ComponentModel.DataAnnotations;
    using IO;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Design;
    using System.Composition;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using ContainerConfigurationAction = System.Action<System.Composition.Hosting.ContainerConfiguration, System.Composition.Convention.ConventionBuilder>;
    using ServiceContainer = More.ServiceContainer;

    /// <summary>
    /// Represents an application composition host.
    /// </summary>
    [SuppressMessage( "Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Service containers and locators may have high coupling." )]
    public partial class Host : ServiceContainer, ICompositionService, IDisposable
    {
        private static readonly Type[] ExportedInterfaces = new[] { typeof( IServiceContainer ), typeof( ICompositionService ), typeof( IServiceProvider ) };
        private static readonly Type[] ExportedTypes = new[] { typeof( CompositionContext ), typeof( CompositionHost ) };
        private readonly ContainerConfiguration configuration = new ContainerConfiguration();
        private readonly Lazy<List<ContainerConfigurationAction>> containerConfigurations = new Lazy<List<ContainerConfigurationAction>>( () => new List<ContainerConfigurationAction>() );
        private readonly List<Type> activityTypes = new List<Type>();
        private readonly Dictionary<Type, IActivityConfiguration> activityConfigurations = new Dictionary<Type, IActivityConfiguration>();
        private readonly Lazy<CompositionContext> container;
        private readonly Lazy<ConventionBuilder> conventionsHolder;
        private readonly Func<string, object> configSettingLocator;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        public Host()
            : this( LocateSetting )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        /// <param name="configurationSettingLocator">The user-defined <see cref="Func{T,TResult}">function</see> used to resolve composable configuration settings.</param>
        public Host( Func<string, object> configurationSettingLocator )
        {
            Arg.NotNull( configurationSettingLocator, nameof( configurationSettingLocator ) );

            container = new Lazy<CompositionContext>( CreateContainer );
            conventionsHolder = new Lazy<ConventionBuilder>( GetConventions );
            configSettingLocator = configurationSettingLocator;
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
                Contract.Ensures( configuration != null );
                return configuration;
            }
        }

        /// <summary>
        /// Gets the composition container for the host.
        /// </summary>
        /// <value>A <see cref="CompositionContext"/> object.</value>
        [CLSCompliant( false )]
        protected CompositionContext Container
        {
            get
            {
                Contract.Ensures( Contract.Result<CompositionContext>() != null );
                CheckDisposed();
                return container.Value;
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
                CheckDisposed();
                return QueryActivities();
            }
        }

        private void CheckDisposed()
        {
            if ( disposed )
                throw new ObjectDisposedException( GetType().Name );
        }

        partial void AddPlatformSpecificDefaultServices();

        /// <summary>
        /// Creates the underlying container.
        /// </summary>
        /// <returns>The constructed <see cref="CompositionContext">container</see>.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is maintainable method. Service containers and locators may have high coupling." )]
        protected virtual CompositionContext CreateContainer()
        {
            Contract.Ensures( Contract.Result<CompositionContext>() != null );

            var conventions = conventionsHolder.Value;
            var config = Configuration;
            var part = new PartSpecification();
            var core = typeof( ServiceProvider ).GetTypeInfo().Assembly;
#if !WEB
            var ui = typeof( IShellView ).GetTypeInfo().Assembly;
            var presentation = typeof( ShellViewBase ).GetTypeInfo().Assembly;
            var presentationTypes = presentation.ExportedTypes.Where( part.IsSatisfiedBy );
#endif
            var host = typeof( Host ).GetTypeInfo().Assembly;
            var hostTypes = host.ExportedTypes.Where( part.IsSatisfiedBy );

            config.WithAssembly( core, conventions );
#if !WEB
            config.WithAssembly( ui, conventions );
            config.WithParts( presentationTypes, conventions );
#endif
            config.WithParts( hostTypes, conventions );
            config.WithDefaultConventions( conventions );
            config.WithProvider( new HostExportDescriptorProvider( this, nameof( Host ) ) );
            config.WithProvider( new ConfigurationExportProvider( configSettingLocator, nameof( Host ) ) );

            // run custom configurations, if any
            if ( containerConfigurations.IsValueCreated )
                containerConfigurations.Value.ForEach( a => a( config, conventions ) );

            var newContainer = config.CreateContainer();

            // register default services directly after the underlying container is created
            // optimization: call base implementation because this object will never be composed
            base.AddService( typeof( IFileSystem ), ( sc, t ) => new FileSystem() );
            base.AddService( typeof( IValidator ), ( sc, t ) => new ValidatorAdapter() );
            AddPlatformSpecificDefaultServices();

            return newContainer;
        }

        private IReadOnlyList<IActivity> QueryActivities()
        {
            Contract.Ensures( Contract.Result<IReadOnlyList<IActivity>>() != null );

            var composedTasks = GetComposedActivities();
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
            var activityExports = Container.GetExports<ExportFactory<IActivity, ExportMetadata>>().ToArray();

            foreach ( var activityType in activityTypes )
            {
                var activity = GetExportedActivity( activityExports, activityType );
                IActivityConfiguration config;
                Tuple<IActivity, IEnumerable<Type>> tuple;

                // look up configuration
                if ( activityConfigurations.TryGetValue( activityType, out config ) )
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
            if ( disposed )
                return;

            disposed = true;
            base.Dispose( disposing );

            if ( !disposing )
                return;

            activityTypes.Clear();
            activityConfigurations.Clear();

            if ( container.IsValueCreated )
                ( container.Value as IDisposable )?.Dispose();
        }

        static partial void AddUISpecificConventions( ConventionBuilder builder );

        static partial void AddPlatformSpecificConventions( ConventionBuilder builder );

        /// <summary>
        /// Gets the attributed model conventions used by the host.
        /// </summary>
        /// <returns>A <see cref="ConventionBuilder">convention builder</see>.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is a factory method and should not be a property." )]
        protected virtual ConventionBuilder GetConventions()
        {
            Contract.Ensures( Contract.Result<ConventionBuilder>() != null );

            var assembly = new PublicKeyTokenSpecification( typeof( Host ) );
            var unitOfWork = new InterfaceSpecification( typeof( IUnitOfWork<> ) );
            var unitOfWorkFactory = new InterfaceSpecification( typeof( IUnitOfWorkFactory ) );
            var repository = new InterfaceSpecification( typeof( IReadOnlyRepository<> ) );
            var dataAccess = repository.Or( unitOfWork );
            var builder = new SettingsConventionBuilder();

            builder.ForTypesDerivedFrom<IActivity>().Export( b => b.AddMetadata( "ExportedType", t => t ) ).Export<IActivity>();
#if WEB
            builder.ForTypesMatching<IUnitOfWorkFactory>( unitOfWorkFactory.IsSatisfiedBy ).Export<IUnitOfWorkFactory>().Shared( Boundary.PerRequest );
            builder.ForTypesMatching( dataAccess.IsSatisfiedBy ).ExportInterfaces( assembly.IsSatisfiedBy ).Shared( Boundary.PerRequest );
#else
            builder.ForTypesMatching<IUnitOfWorkFactory>( unitOfWorkFactory.IsSatisfiedBy ).Export<IUnitOfWorkFactory>().Shared();
            builder.ForTypesMatching( dataAccess.IsSatisfiedBy ).ExportInterfaces( assembly.IsSatisfiedBy );
#endif
            AddUISpecificConventions( builder );
            AddPlatformSpecificConventions( builder );

            return builder;
        }

        /// <summary>
        /// Configures the underlying container when the host starts.
        /// </summary>
        /// <param name="configurationAction">The <see cref="ContainerConfigurationAction">configuration</see> to add.</param>
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
        ///         using ( var host = new Host() )
        ///         {
        ///             host.Configure( ( config, conventions ) => config.WithAppDomain( conventions ) );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [CLSCompliant( false )]
        public virtual void Configure( ContainerConfigurationAction configurationAction )
        {
            Arg.NotNull( configurationAction, nameof( configurationAction ) );
            containerConfigurations.Value.Add( configurationAction );
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
            CheckDisposed();

            var activityType = typeof( TActivity );

            if ( activityTypes.Contains( activityType ) )
                throw new InvalidOperationException( ExceptionMessage.ActivityCannotBeRegisteredMoreThanOnce.FormatDefault( activityType ) );

            activityTypes.Add( activityType );

            IActivityConfiguration config;

            if ( activityConfigurations.TryGetValue( activityType, out config ) )
                return (ActivityConfiguration<TActivity>) config;

            var newConfiguration = new ActivityConfiguration<TActivity>();
            activityConfigurations.Add( activityType, newConfiguration );

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
            CheckDisposed();

            var activityType = typeof( TActivity );

            if ( !activityTypes.Contains( activityType ) )
                activityTypes.Add( activityType );

            IActivityConfiguration config;

            if ( activityConfigurations.TryGetValue( activityType, out config ) )
                return (ActivityConfiguration<TActivity>) config;

            var newConfiguration = new ActivityConfiguration<TActivity>();
            activityConfigurations.Add( activityType, newConfiguration );

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
            CheckDisposed();

            var @new = callback;
            ServiceCreatorCallback composedCallback = ( c, t ) =>
            {
                var instance = @new( c, t );
                Container.SatisfyImports( instance );
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
            Arg.NotNull( serviceType, nameof( serviceType ) );
            CheckDisposed();

            var generator = new ServiceTypeDisassembler();
            var key = generator.ExtractKey( serviceType );
#if WINDOWS_PHONE_APP
            return GetService( serviceType, key );
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
                        exports.Add( Container );
                }

                // add any matching, manually added services
                if ( ( service = base.GetService( innerServiceType ) ) != null )
                    exports.Add( service );

                // add matching exports
                exports.AddRange( Container.GetExports( innerServiceType, key ) );
                return exports;
            }

            // if no key is specified and the requested type matches an interface we implement, return ourself
            if ( key == null )
            {
                if ( ExportedInterfaces.Contains( serviceType ) )
                    return this;
                else if ( ExportedTypes.Contains( serviceType ) )
                    return Container;
            }

            // return any matching, manually added services
            if ( ( service = base.GetService( serviceType ) ) != null )
                return service;

            // return matching export
            object export;
            Container.SafeTryGetExport( serviceType, key, out export );

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
                Container.SatisfyImports( instance, conventionsHolder.Value );
        }
    }
}
