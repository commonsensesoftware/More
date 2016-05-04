namespace More
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents an object which can disassemble information from the <see cref="Type">type</see> defined for a service.
    /// </summary>
    public class ServiceTypeDisassembler
    {
        private static readonly Type EnumerableOfT = typeof( IEnumerable<> );

        /// <summary>
        /// Extracts the key associated with the specified service type.
        /// </summary>
        /// <param name="serviceType">The service <see cref="Type">type</see> to extract the key from.</param>
        /// <returns>The key extracted from the specified <paramref name="serviceType">service type</paramref> or <c>null</c> if no key is associated.</returns>
        /// <remarks>The value of the key is derived from the applied <see cref="ServiceKeyAttribute"/>.</remarks>
        public virtual string ExtractKey( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );
            return serviceType.GetTypeInfo().GetCustomAttribute<ServiceKeyAttribute>( false )?.Key;
        }

        /// <summary>
        /// Creates and returns a new type that represents multiple instances of the specified service type.
        /// </summary>
        /// <param name="serviceType">The service <see cref="Type">type</see> to create a new type for.</param>
        /// <returns>A <see cref="Type">type</see> representing multiple instances of the specified <paramref name="serviceType">service type</paramref>.</returns>
        /// <remarks>If the specified <paramref name="serviceType">service type</paramref> is assignable from <see cref="IEnumerable"/>,
        /// then the original type is returned; otherwise, the returned type is assignable from <see cref="IEnumerable"/>.</remarks>
        public virtual Type ForMany( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );
            Contract.Ensures( Contract.Result<Type>() != null );
            return EnumerableOfT.MakeGenericType( serviceType );
        }

        /// <summary>
        /// Returns a value indicating whether the specified service type represents multiple instances of a service.
        /// </summary>
        /// <param name="serviceType">The service <see cref="Type">type</see> to evaluate.</param>
        /// <returns>True if the specified <paramref name="serviceType">service type</paramref> represents multiple service instances; otherwise, false.</returns>
        public virtual bool IsForMany( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            var typeInfo = serviceType.GetTypeInfo();
            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition().Equals( EnumerableOfT );
        }

        /// <summary>
        /// Returns a value indicating whether the specified service type represents multiple instances of a service.
        /// </summary>
        /// <param name="serviceType">The service <see cref="Type">type</see> to evaluate.</param>
        /// <param name="singleServiceType">The single instance service <see cref="Type">type</see>. This type will be
        /// the same as the <paramref name="serviceType">service type</paramref> when the specified type does not
        /// represent multiple instances.</param>
        /// <returns>True if the specified <paramref name="serviceType">service type</paramref> represents multiple service instances; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Optimization for callers if they also want to know the matched service type." )]
        public virtual bool IsForMany( Type serviceType, out Type singleServiceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );
            Contract.Ensures( Contract.ValueAtReturn( out singleServiceType ) != null );

            singleServiceType = serviceType;
            var typeInfo = serviceType.GetTypeInfo();

            if ( !typeInfo.IsGenericType || !typeInfo.GetGenericTypeDefinition().Equals( EnumerableOfT ) )
                return false;

            singleServiceType = typeInfo.GenericTypeArguments[0];
            return true;
        }
    }
}