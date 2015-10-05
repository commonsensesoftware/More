namespace System.Web.Http
{
    using Collections.Generic;
    using Dependencies;
    using Diagnostics.CodeAnalysis;
    using Diagnostics.Contracts;
    using Linq;

    /// <summary>
    /// Provides extension methods for the <see cref="IDependencyScope"/> interface.
    /// </summary>
    public static class IDependencyResolverExtensions
    {
        /// <summary>
        /// Gets a registered service of the specified service type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of service to resolve.</typeparam>
        /// <param name="scope">The extended <see cref="IDependencyScope">dependency scope</see>.</param>
        /// <returns>The resolved object of type <typeparamref name="T"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static T GetService<T>( this IDependencyScope scope )
        {
            Arg.NotNull( scope, nameof( scope ) );
            return (T) scope.GetService( typeof( T ) );
        }

        /// <summary>
        /// Gets one or more registered services of the specified service type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of services to resolve.</typeparam>
        /// <param name="scope">The extended <see cref="IDependencyScope">dependency scope</see>.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of resolved objects of type <typeparamref name="T"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static IEnumerable<T> GetServices<T>( this IDependencyScope scope )
        {
            Arg.NotNull( scope, nameof( scope ) );
            Contract.Ensures( Contract.Result<IEnumerable<T>>() != null );
            return scope.GetServices( typeof( T ) ).Cast<T>();
        }
    }
}
