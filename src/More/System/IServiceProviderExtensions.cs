namespace System
{
    using Collections;
    using More;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceProvider"/> interface.
    /// </summary>
    public static class IServiceProviderExtensions
    {
        /// <summary>
        /// Gets the strongly-typed service object of the specified type.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service requested.</typeparam>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <returns>A service object of type <typeparamref name="TService"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static TService GetService<TService>( this IServiceProvider serviceProvider ) where TService : class
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            return (TService) serviceProvider.GetService( typeof( TService ) );
        }

        /// <summary>
        /// Gets the required, strongly-typed service object of the specified type.
        /// </summary>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of requested service.</param>
        /// <returns>The requested service object.</returns>
        /// <exception cref="NotSupportedException">There is no service of the given <paramref name="serviceType">type</paramref>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static object GetRequiredService( this IServiceProvider serviceProvider, Type serviceType )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            Arg.NotNull( serviceType, nameof( serviceType ) );
            Contract.Ensures( Contract.Result<object>() != null );

            var service = serviceProvider.GetService( serviceType );

            if ( service == null )
            {
                throw new NotSupportedException( ExceptionMessage.MissingRequiredService.FormatDefault( serviceType ) );
            }

            return service;
        }

        /// <summary>
        /// Gets the required, strongly-typed service object of the specified type.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service requested.</typeparam>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <returns>A service object of type <typeparamref name="TService"/>.</returns>
        /// <exception cref="NotSupportedException">There is no service of the given type <typeparamref name="TService"/>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static TService GetRequiredService<TService>( this IServiceProvider serviceProvider ) where TService : class
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            Contract.Ensures( Contract.Result<TService>() != null );

            var service = (TService) serviceProvider.GetService( typeof( TService ) );

            if ( service == null )
            {
                throw new NotSupportedException( ExceptionMessage.MissingRequiredService.FormatDefault( typeof( TService ) ) );
            }

            return service;
        }

        /// <summary>
        /// Attempts to resolve and return the strongly-typed service object of the specified type.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service requested.</typeparam>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="service">The requested service object of type <typeparamref name="TService"/>.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should not throw exceptions for service resolution failures." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static bool TryGetService<TService>( this IServiceProvider serviceProvider, out TService service ) where TService : class
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            service = null;

            try
            {
                service = (TService) serviceProvider.GetService( typeof( TService ) );
            }
            catch
            {
            }

            return service != null;
        }

        /// <summary>
        /// Attempts to resolve and return the service object of the specified type.
        /// </summary>
        /// <param name="serviceProvider">The extended <see cref="IServiceProvider"/> object.</param>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <param name="service">The requested service object.</param>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should not throw exceptions for service resolution failures." )]
        [SuppressMessage( "Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Provides legacy support and enables scenarios where generics cannot be used." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static bool TryGetService( this IServiceProvider serviceProvider, Type serviceType, out object service )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            Arg.NotNull( serviceType, nameof( serviceType ) );
            service = null;

            try
            {
                service = serviceProvider.GetService( serviceType );
            }
            catch
            {
            }

            return service != null;
        }

        /// <summary>
        /// Returns all services of the given type.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type">type</see> of service requested.</typeparam>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of services of the requested <typeparamref name="TService">type</typeparamref>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static IEnumerable<TService> GetServices<TService>( this IServiceProvider serviceProvider ) where TService : class
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            Contract.Ensures( Contract.Result<IEnumerable<TService>>() != null );

            var multipleServicesType = typeof( IEnumerable<TService> );
            var services = serviceProvider.GetService( multipleServicesType ) as IEnumerable;

            if ( services == null )
            {
                yield break;
            }

            foreach ( object service in services )
            {
                yield return (TService) service;
            }
        }
    }
}