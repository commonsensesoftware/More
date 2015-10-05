namespace System.Web.Http
{
    using Collections.Generic;
    using Diagnostics.CodeAnalysis;
    using Diagnostics.Contracts;
    using Linq;
    using Mvc;

    /// <summary>
    /// Provides extension methods for the <see cref="IDependencyResolver"/> interface.
    /// </summary>
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Gets a registered service of the specified service type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of service to resolve.</typeparam>
        /// <param name="resolver">The extended <see cref="IDependencyResolver">dependency resolver</see>.</param>
        /// <returns>The resolved object of type <typeparamref name="T"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static T GetService<T>( this IDependencyResolver resolver )
        {
            Arg.NotNull( resolver, nameof( resolver ) );
            return (T) resolver.GetService( typeof( T ) );
        }

        /// <summary>
        /// Gets one or more registered services of the specified service type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of services to resolve.</typeparam>
        /// <param name="resolver">The extended <see cref="IDependencyResolver">dependency resolver</see>.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of resolved objects of type <typeparamref name="T"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static IEnumerable<T> GetServices<T>( this IDependencyResolver resolver )
        {
            Arg.NotNull( resolver, nameof( resolver ) );
            Contract.Ensures( Contract.Result<IEnumerable<T>>() != null );
            return resolver.GetServices( typeof( T ) ).Cast<T>();
        }
    }
}
