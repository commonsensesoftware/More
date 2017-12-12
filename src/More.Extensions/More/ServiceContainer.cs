namespace More
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents the base implementation for a <see cref="IServiceContainer">service container</see>.
    /// </summary>
    public partial class ServiceContainer : IServiceContainer, IDisposable
    {
        static readonly TypeInfo DisposableType = typeof( IDisposable ).GetTypeInfo();
        readonly Dictionary<ServiceRegistryKey, ServiceEntry> registry = new Dictionary<ServiceRegistryKey, ServiceEntry>();
        readonly Lazy<IServiceContainer> parent;
        bool disposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="ServiceContainer"/> class.
        ///         /// </summary>
        ~ServiceContainer() => Dispose( false );

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainer"/> class.
        /// </summary>
        public ServiceContainer() => parent = new Lazy<IServiceContainer>( () => null );

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainer"/> class.
        /// </summary>
        /// <param name="parent">The parent <see cref="IServiceContainer">service container</see> associated with the new container.</param>
        public ServiceContainer( IServiceContainer parent ) => this.parent = new Lazy<IServiceContainer>( () => parent );

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainer"/> class.
        /// </summary>
        /// <param name="parent">The parent <see cref="IServiceProvider">service provider</see> associated with the new container.</param>
        public ServiceContainer( IServiceProvider parent ) => this.parent = new Lazy<IServiceContainer>( () => GetParentContainer( parent ) );

        static IServiceContainer GetParentContainer( IServiceProvider provider )
        {
            Contract.Requires( provider != null );

            if ( provider == null )
            {
                return null;
            }

            if ( provider.TryGetService( out IServiceContainer container ) )
            {
                return container;
            }

            return null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether services are promoted to parent containers by default.
        /// </summary>
        /// <value>True if services are promoted to parent containers by default; otherwise, false. The
        /// default value is <c>false</c>.</value>
        protected bool PromoteByDefault { get; set; }

        /// <summary>
        /// Gets the parent of the current container.
        /// </summary>
        /// <value>The parent <see cref="IServiceContainer">service container</see> or <c>null</c>
        /// if the current container is the root container.</value>
        protected IServiceContainer Parent => parent.Value;

        /// <summary>
        /// Gets a registry of services in the container.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">read-only</see> of registered service <see cref="ServiceRegistryKey">keys</see>.</value>
        /// <remarks>The default implementation returns a distinct set of registered service <see cref="ServiceRegistryKey">keys</see> for
        /// the current container and its parent containers, if any.</remarks>
        public virtual IReadOnlyList<ServiceRegistryKey> Registry
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<ServiceRegistryKey>>() != null );

                var hashSet = new HashSet<ServiceRegistryKey>( registry.Keys );
                var next = Parent as ServiceContainer;

                while ( next != null )
                {
                    hashSet.AddRange( next.Registry );
                    next = next.Parent as ServiceContainer;
                }

                return hashSet.ToArray();
            }
        }

        static bool IsDisposableService( Type serviceType )
        {
            Contract.Requires( serviceType != null );
            return DisposableType.IsAssignableFrom( serviceType.GetTypeInfo() );
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="ServiceContainer"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the objects is being disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;

            if ( !disposing )
            {
                return;
            }

            // dispose of any services where their lifetime is managed by the container
            registry.Values.Where( s => s.LifetimeIsManaged && s.IsValueCreated ).OfType<IDisposable>().ForEach( d => d.Dispose() );
            registry.Clear();
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="callback"/> is <c>null</c>.</exception>
        /// <remarks><see cref="IDisposable">Disposable</see> services created by the <paramref name="callback"/> will be automatically disposed when the container is disposed.</remarks>
        public virtual void AddService( Type serviceType, ServiceCreatorCallback callback ) => AddService( serviceType, callback, PromoteByDefault );

        /// <summary>
        /// Adds the specified service to the service container, and optionally promotes the service to parent service containers.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="callback"/> is <c>null</c>.</exception>
        /// <remarks><see cref="IDisposable">Disposable</see> services created by the <paramref name="callback"/> will be automatically disposed when the container is disposed.</remarks>
        public virtual void AddService( Type serviceType, ServiceCreatorCallback callback, bool promote )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );
            Arg.NotNull( callback, nameof( callback ) );

            var type = serviceType;
            var create = callback;
            var key = new ServiceRegistryKey( serviceType );

            registry[key] = new ServiceEntry( () => create( this, type ), IsDisposableService( serviceType ) );

            if ( promote && Parent != null )
            {
                Parent.AddService( serviceType, callback, promote );
            }
        }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">An instance of the service type to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="serviceInstance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="serviceType"/> is not assignable from <paramref name="serviceInstance"/>.</exception>
        public virtual void AddService( Type serviceType, object serviceInstance ) => AddService( serviceType, serviceInstance, PromoteByDefault );

        /// <summary>
        /// Adds the specified service to the service container, and optionally promotes the service to any parent service containers.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <param name="serviceInstance">An instance of the service type to add.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> or <paramref name="serviceInstance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="serviceType"/> is not assignable from <paramref name="serviceInstance"/>.</exception>
        public virtual void AddService( Type serviceType, object serviceInstance, bool promote )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );
            Arg.NotNull( serviceInstance, nameof( serviceInstance ) );

            if ( !serviceType.GetTypeInfo().IsAssignableFrom( serviceInstance.GetType().GetTypeInfo() ) )
            {
                throw new ArgumentOutOfRangeException( nameof( serviceInstance ) );
            }

            var service = serviceInstance;
            var key = new ServiceRegistryKey( serviceType );

            registry[key] = new ServiceEntry( service );

            if ( promote && Parent != null )
            {
                Parent.AddService( serviceType, serviceInstance, promote );
            }
        }

        /// <summary>
        /// Removes the specified service type from the service container.
        /// </summary>
        /// <param name="serviceType">The type of service to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is <c>null</c>.</exception>
        public virtual void RemoveService( Type serviceType ) => RemoveService( serviceType, PromoteByDefault );

        /// <summary>
        /// Removes the specified service type from the service container, and optionally promotes the service to parent service containers.
        /// </summary>
        /// <param name="serviceType">The type of service to remove.</param>
        /// <param name="promote">True to promote this request to any parent service containers; otherwise, false.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceType"/> is <c>null</c>.</exception>
        public virtual void RemoveService( Type serviceType, bool promote )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            var key = new ServiceRegistryKey( serviceType );
            registry.Remove( key );

            if ( promote && Parent != null )
            {
                Parent.RemoveService( serviceType, promote );
            }
        }

        /// <summary>
        /// Returns a service of the given <paramref name="serviceType">type</paramref> with the specified key.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of object requested.</param>
        /// <returns>The requested service object.</returns>
        /// <remarks>The default implementation will resolve single services from the current container as well as its
        /// parent containers, if any. If multiple services are requested, resolution is constrainted to the current container.</remarks>
        public virtual object GetService( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            var key = new ServiceRegistryKey( serviceType );

            if ( registry.TryGetValue( key, out var entry ) )
            {
                return entry.Value;
            }

            if ( typeof( IServiceContainer ).Equals( serviceType ) || typeof( IServiceProvider ).Equals( serviceType ) )
            {
                return this;
            }

            return Parent?.GetService( serviceType );
        }

        /// <summary>
        /// Creates a child container for the current container.
        /// </summary>
        /// <returns>A new, child <see cref="IServiceContainer">service container</see>.</returns>
        public virtual IServiceContainer CreateChildContainer()
        {
            Contract.Ensures( Contract.Result<IServiceContainer>() != null );
            IServiceContainer parentContainer = this;
            return new ServiceContainer( parentContainer );
        }

        /// <summary>
        /// Releases the managed and unmanaged resources used by the <see cref="ServiceContainer"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        sealed class ServiceEntry : Lazy<object>
        {
            internal ServiceEntry( object value ) : base( () => value ) { }

            internal ServiceEntry( Func<object> valueFactory, bool disposable ) : base( valueFactory )
            {
                LifetimeIsManaged = disposable;
            }

            internal bool LifetimeIsManaged { get; }
        }
    }
}