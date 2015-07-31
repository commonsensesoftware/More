namespace More.Web.Http.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Web.Http.Dependencies;

    /// <summary>
    /// Represents a constrained dependency scope for a specific request.
    /// </summary>
    public class RequestDependencyScope : IDependencyScope
    {
        private readonly IDependencyScope scope;
        private readonly HttpRequestMessage request;
        private readonly Dictionary<Type, Func<HttpRequestMessage, object, object>> activators = new Dictionary<Type, Func<HttpRequestMessage, object, object>>();
        private bool disposed;

        /// <summary>
        /// Releases the managed and unmanaged resources used by the <see cref="RequestDependencyScope"/> class.
        /// </summary>
        ~RequestDependencyScope()
        {
            Dispose( false );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestDependencyScope"/> class.
        /// </summary>
        /// <param name="currentScope">The current <see cref="IDependencyScope">dependency scope</see>.</param>
        /// <param name="request">The current <see cref="HttpRequestMessage">request</see>.</param>
        public RequestDependencyScope( IDependencyScope currentScope, HttpRequestMessage request )
        {
            Arg.NotNull( currentScope, nameof( currentScope ) );
            Arg.NotNull( request, nameof( request ) );

            scope = currentScope;
            this.request = request;
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="RequestDependencyScope"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            disposed = true;

            if ( disposing )
                scope.Dispose();
        }

        private static object DefaultActivator( HttpRequestMessage request, object instance )
        {
            return instance;
        }

        private Func<HttpRequestMessage, object, object> GetOrCreateActivator( Type serviceType )
        {
            Contract.Requires( serviceType != null );
            Contract.Ensures( Contract.Result<Func<HttpRequestMessage, object, object>>() != null );

            Func<HttpRequestMessage, object, object> activator;

            if ( activators.TryGetValue( serviceType, out activator ) )
                return activator;

            var factoryType = typeof( IDecoratorFactory<> ).MakeGenericType( serviceType );
            var factory = scope.GetService( factoryType );

            if ( factory == null )
            {
                activator = DefaultActivator;
            }
            else
            {
                var map = factory.GetType().GetInterfaceMap( factoryType );
                var method = map.TargetMethods.Single();
                var r = Expression.Parameter( typeof( HttpRequestMessage ), "r" );
                var o = Expression.Parameter( typeof( object ), "o" );
                var f = Expression.Constant( factory );
                var body = Expression.Call( f, method, r, Expression.Convert( o, serviceType ) );
                var l = Expression.Lambda<Func<HttpRequestMessage, object, object>>( body, r, o );

                // should produce the expression: ( r, o ) => factory.CreatePerInstanceDecorator( r, (T) o );
                Debug.WriteLine( l );
                activator = l.Compile();
            }

            activators[serviceType] = activator;
            return activator;
        }

        /// <summary>
        /// Returns an object matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>An instance of the requested <paramref name="serviceType">service type</paramref> or null if no match is found.</returns>
        public object GetService( Type serviceType )
        {
            var service = scope.GetService( serviceType );

            if ( service == null )
                return service;

            var activator = GetOrCreateActivator( serviceType );
            return activator( request, service );
        }

        /// <summary>
        /// Returns a sequence of objects matching the requested service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services matching the requested <paramref name="serviceType">service type</paramref>.</returns>
        public IEnumerable<object> GetServices( Type serviceType )
        {
            var services = scope.GetServices( serviceType );

            if ( services == null )
                yield break;

            var activator = GetOrCreateActivator( serviceType );

            foreach ( var service in services )
                yield return activator( request, service );
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="RequestDependencyScope"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
