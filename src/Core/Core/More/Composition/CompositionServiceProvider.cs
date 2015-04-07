namespace More.Composition
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Reflection;

    /// <summary>
    /// Represents a <see cref="IServiceProvider">service provider</see> based on composite resolution functions.
    /// </summary>
    public class CompositionServiceProvider : IServiceProvider
    {
        private static readonly TypeInfo IEnumerableOfT = typeof( IEnumerable<> ).GetTypeInfo();
        private readonly Func<Type, string, object> resolve;
        private readonly Func<Type, string, IEnumerable<object>> resolveAll;

         /// <summary>
        /// Initializes a new instance of the <see cref="CompositionServiceProvider"/> class.
        /// </summary>
        /// <param name="resolve">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve a single service of a particular <see cref="Type">type</see>.</param>
        public CompositionServiceProvider( Func<Type, string, object> resolve )
        {
            Contract.Requires<ArgumentNullException>( resolve != null, "resolve" );

            this.resolve = resolve;
            this.resolveAll = this.DefaultResolveAll;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionServiceProvider"/> class.
        /// </summary>
        /// <param name="resolve">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve a single service of a particular <see cref="Type">type</see>.</param>
        /// <param name="resolveAll">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve all services of a particular <see cref="Type">type</see>.</param>
        public CompositionServiceProvider( Func<Type, string, object> resolve, Func<Type, string, IEnumerable<object>> resolveAll )
        {
            Contract.Requires<ArgumentNullException>( resolve != null, "resolve" );
            Contract.Requires<ArgumentNullException>( resolveAll != null, "resolveAll" );

            this.resolve = resolve;
            this.resolveAll = resolveAll;
        }

        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should not throw exceptions for service resolution failures." )]
        private IEnumerable<object> DefaultResolveAll( Type serviceType, string key )
        {
            Contract.Requires( serviceType != null );

            try
            {
                // return sequence with a single element
                return new[] { this.resolve( serviceType, key ) };
            }
            catch
            {
                // return empty sequence if resolution fails
                return Enumerable.Empty<object>();
            }
        }

        private static bool IsIEnumerableOfT( Type type, out Type elementType )
        {
            Contract.Requires( type != null );

            var typeInfo = type.GetTypeInfo();

            if ( !typeInfo.IsGenericType && !IEnumerableOfT.IsAssignableFrom( type.GetGenericTypeDefinition().GetTypeInfo() ) )
                elementType = null;
            else
                elementType = typeInfo.GenericTypeParameters[0];

            return elementType != null;
        }

        /// <summary>
        /// Gets the key associated with the specified service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to get the key for.</param>
        /// <returns>The key associated with the <paramref name="serviceType">service type</paramref> or <c>null</c>
        /// if the service does not have an associated key.</returns>
        protected static string GetServiceKey( Type serviceType )
        {
            Contract.Requires<ArgumentNullException>( serviceType != null, "serviceType" );
            var attribute = serviceType.GetTypeInfo().GetCustomAttribute<ServiceKeyAttribute>( false );
            return attribute == null ? null : attribute.Key;
        }

        /// <summary>
        /// Gets a service of the requested type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to return.</param>
        /// <returns>The service instance corresponding to the requested
        /// <paramref name="serviceType">service type</paramref> or null if no match is found.</returns>
        public virtual object GetService( Type serviceType )
        {
            if ( serviceType == null )
                throw new ArgumentNullException( "serviceType" );

            var key = GetServiceKey( serviceType );
            Type elementType;

            if ( IsIEnumerableOfT( serviceType, out elementType ) )
                return this.resolveAll( elementType, key );

            return this.resolve( serviceType, key );
        }
    }
}
