namespace More
{
    using More.Collections.Generic;
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
        private static readonly TypeInfo Enumerable = typeof( IEnumerable ).GetTypeInfo();
        private static readonly TypeInfo EnumerableOfT = typeof( IEnumerable<> ).GetTypeInfo();

        private static TypeInfo DisassembleServiceType( TypeInfo type )
        {
            Contract.Requires( type != null );
            Contract.Ensures( Contract.Result<TypeInfo>() != null );

            var closedTypes = new Stack<TypeInfo>().Adapt();
            return DisassembleServiceType( type, closedTypes );
        }

        /// <summary>
        /// Disassembles a generic service type into it's constituent types.
        /// </summary>
        /// <param name="type">The service <see cref="TypeInfo">type</see> to be disassembled.</param>
        /// <param name="closedTypes">The disassembled, closed <see cref="TypeInfo">type</see> arguments.</param>
        /// <returns>The disassembled <see cref="TypeInfo">type</see>.</returns>
        /// <remarks>If the specified <paramref name="type"/> is not generic, then the original type is returned.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        protected static TypeInfo DisassembleServiceType( TypeInfo type, IStack<TypeInfo> closedTypes )
        {
            Arg.NotNull( type, "type" );
            Arg.NotNull( closedTypes, "closedTypes" );
            Contract.Ensures( Contract.Result<TypeInfo>() != null );

            while ( type.IsGenericType )
            {
                closedTypes.Push( type );
                type = type.GenericTypeArguments[0].GetTypeInfo();
            }

            return type;
        }

        /// <summary>
        /// Extracts the key associated with the specified service type.
        /// </summary>
        /// <param name="serviceType">The service <see cref="Type">type</see> to extract the key from.</param>
        /// <returns>The key extracted from the specified <paramref name="serviceType">service type</paramref> or <c>null</c> if no key is associated.</returns>
        /// <remarks>The value of the key is derived from the applied <see cref="ServiceKeyAttribute"/>.</remarks>
        public virtual string ExtractKey( Type serviceType )
        {
            Arg.NotNull( serviceType, "serviceType" );

            // find true decorated service type and extract the key from any applied attribute
            var type = DisassembleServiceType( serviceType.GetTypeInfo() );
            var attribute = type.GetCustomAttribute<ServiceKeyAttribute>( false );
            var key = attribute == null ? null : attribute.Key;

            return key;
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
            Arg.NotNull( serviceType, "serviceType" );
            Contract.Ensures( Contract.Result<Type>() != null );

            if ( Enumerable.IsAssignableFrom( serviceType.GetTypeInfo() ) )
                return serviceType;

            return EnumerableOfT.MakeGenericType( serviceType );
        }

        /// <summary>
        /// Returns a value indicating whether the specified service type represents multiple instances of a service.
        /// </summary>
        /// <param name="serviceType">The service <see cref="Type">type</see> to evaluate.</param>
        /// <returns>True if the specified <paramref name="serviceType">service type</paramref> represents multiple service instances; otherwise, false.</returns>
        public virtual bool IsForMany( Type serviceType )
        {
            Arg.NotNull( serviceType, "serviceType" );
            return Enumerable.IsAssignableFrom( serviceType.GetTypeInfo() );
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
            Arg.NotNull( serviceType, "serviceType" );
            Contract.Ensures( Contract.ValueAtReturn( out singleServiceType ) != null );

            singleServiceType = serviceType;

            if ( !Enumerable.IsAssignableFrom( serviceType.GetTypeInfo() ) )
                return false;

            singleServiceType = DisassembleServiceType( serviceType.GetTypeInfo() ).AsType();
            return true;
        }
    }
}