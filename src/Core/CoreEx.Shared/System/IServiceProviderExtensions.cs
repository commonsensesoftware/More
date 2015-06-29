namespace System
{
    using More;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceProvider"/> interface.
    /// </summary>
    public static partial class IServiceProviderExtensions
    {
        private static object GetServiceWithKey( IServiceProvider serviceProvider, Type serviceType, string key )
        {
#if WINDOWS_PHONE_APP
            var keyedServiceProvider = serviceProvider as IKeyedServiceProvider;
            return keyedServiceProvider == null ? null : keyedServiceProvider.GetService( serviceType, key );
#else
            var generator = new ServiceTypeAssembler();
            var projectedType = generator.ApplyKey( serviceType, key );
            return serviceProvider.GetService( projectedType );
#endif
        }

        private static IEnumerable<TService> GetServicesWithKey<TService>( IServiceProvider serviceProvider, Type serviceType, string key )
        {
#if WINDOWS_PHONE_APP
            var keyedServiceProvider = serviceProvider as IKeyedServiceProvider;

            if ( keyedServiceProvider == null )
                return Enumerable.Empty<TService>();

            var generator = new ServiceTypeDisassembler();
            var multipleServicesType = generator.ForMany( serviceType );
            return ( (IEnumerable<TService>) keyedServiceProvider.GetService( multipleServicesType, key ) ) ?? Enumerable.Empty<TService>();
#else
            var generator = new ServiceTypeAssembler();
            var projectedType = generator.ApplyKey( serviceType, key );
            var multipleServicesType = generator.ForMany( projectedType );
            return ( (IEnumerable<TService>) serviceProvider.GetService( multipleServicesType ) ) ?? Enumerable.Empty<TService>();
#endif
        }

        /// <summary>
        /// Returns a service of the given <paramref name="serviceType">type</paramref> with the specified key.
        /// </summary>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of object requested.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>The requested service object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static object GetService( this IServiceProvider serviceProvider, Type serviceType, string key )
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            Arg.NotNull( serviceType, "serviceType" );
            return GetServiceWithKey( serviceProvider, serviceType, key );
        }

        /// <summary>
        /// Returns all services of the given <paramref name="serviceType">type</paramref>.
        /// </summary>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of object requested.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services of the requested <paramref name="serviceType">type</paramref>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static IEnumerable<object> GetServices( this IServiceProvider serviceProvider, Type serviceType )
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            Arg.NotNull( serviceType, "serviceType" );
            Contract.Ensures( Contract.Result<IEnumerable<object>>() != null );

            var generator = new ServiceTypeDisassembler();
            var multipleServicesType = generator.ForMany( serviceType );
            return ( (IEnumerable<object>) serviceProvider.GetService( multipleServicesType ) ) ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Returns all services of the given <paramref name="serviceType">type</paramref> and key.
        /// </summary>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of object requested.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services of the requested <paramref name="serviceType">type</paramref>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static IEnumerable<object> GetServices( this IServiceProvider serviceProvider, Type serviceType, string key )
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            Arg.NotNull( serviceType, "serviceType" );
            Contract.Ensures( Contract.Result<IEnumerable<object>>() != null );
            return GetServicesWithKey<object>( serviceProvider, serviceType, key );
        }

        /// <summary>
        /// Gets the strongly-typed service object of the specified type and key.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service requested.</typeparam>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>A service object of type <typeparamref name="TService"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static TService GetService<TService>( this IServiceProvider serviceProvider, string key ) where TService : class
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            return (TService) serviceProvider.GetService( typeof( TService ), key );
        }

        /// <summary>
        /// Gets the required, strongly-typed service object of the specified type and key.
        /// </summary>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of requested service.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>The requested service object.</returns>
        /// <exception cref="NotSupportedException">There is no service of the given <paramref name="serviceType">type</paramref>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static object GetRequiredService( this IServiceProvider serviceProvider, Type serviceType, string key )
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            Arg.NotNull( serviceType, "serviceType" );
            Contract.Ensures( Contract.Result<object>() != null );

            var service = serviceProvider.GetService( serviceType, key );

            if ( service == null )
                throw new NotSupportedException( ExceptionMessage.MissingRequiredKeyedService.FormatDefault( serviceType, key ) );

            return service;
        }

        /// <summary>
        /// Gets the required, strongly-typed service object of the specified type and key.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service requested.</typeparam>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>A service object of type <typeparamref name="TService"/>.</returns>
        /// <exception cref="NotSupportedException">There is no service of the given type <typeparamref name="TService"/>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static TService GetRequiredService<TService>( this IServiceProvider serviceProvider, string key ) where TService : class
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            Contract.Ensures( Contract.Result<TService>() != null );

            var service = (TService) serviceProvider.GetService( typeof( TService ), key );

            if ( service == null )
                throw new NotSupportedException( ExceptionMessage.MissingRequiredKeyedService.FormatDefault( typeof( TService ), key ) );

            return service;
        }

        /// <summary>
        /// Attempts to resolve and return the strongly-typed service object of the specified type and key.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service requested.</typeparam>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="service">The requested service object of type <typeparamref name="TService"/>.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Required for the TryGetXXX style." )]
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should not throw exceptions for service resolution failures." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static bool TryGetService<TService>( this IServiceProvider serviceProvider, out TService service, string key ) where TService : class
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            service = null;

            try
            {
                service = (TService) serviceProvider.GetService( typeof( TService ), key );
            }
            catch
            {
            }

            return service != null;
        }

        /// <summary>
        /// Attempts to resolve and return the service object of the specified type and key.
        /// </summary>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <param name="service">The requested service object.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Required for the TryGetXXX style." )]
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should not throw exceptions for service resolution failures." )]
        [SuppressMessage( "Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Provides legacy support and enables scenarios where generics cannot be used." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static bool TryGetService( this IServiceProvider serviceProvider, Type serviceType, out object service, string key )
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            Arg.NotNull( serviceType, "serviceType" );
            service = null;

            try
            {
                service = serviceProvider.GetService( serviceType, key );
            }
            catch
            {
            }

            return service != null;
        }

        /// <summary>
        /// Returns all services of the given type and key.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service requested.</typeparam>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="key">The key the object was registered with.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services of the requested <typeparamref name="TService">type</typeparamref>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static IEnumerable<TService> GetServices<TService>( this IServiceProvider serviceProvider, string key ) where TService : class
        {
            Arg.NotNull( serviceProvider, "serviceProvider" );
            Contract.Ensures( Contract.Result<IEnumerable<TService>>() != null );
            return GetServicesWithKey<TService>( serviceProvider, typeof( TService ), key );
        }
    }
}
