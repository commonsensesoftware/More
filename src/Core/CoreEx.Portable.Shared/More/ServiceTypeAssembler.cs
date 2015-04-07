namespace More
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents an object which can apply and extract a key from the <see cref="Type">type</see> defined for a service.
    /// </summary>
    public class ServiceTypeAssembler : ServiceTypeDisassembler
    {
        private static Type AssembleServiceType( Type projectedType, Stack<TypeInfo> closedTypes )
        {
            Contract.Requires( closedTypes != null );
            Contract.Requires( closedTypes.Count > 0 );
            Contract.Ensures( Contract.Result<Type>() != null );

            var iterator = closedTypes.GetEnumerator();

            iterator.MoveNext();

            var type = iterator.Current;
            var args = type.GenericTypeArguments;

            // replace original type with projected type
            args[0] = projectedType;

            // re-create the generic type
            var assembledType = type.GetGenericTypeDefinition().MakeGenericType( args );

            // walk the stack to re-create the type if it was a nested generic
            while ( iterator.MoveNext() )
            {
                type = iterator.Current;
                args = type.GenericTypeArguments;
                args[0] = assembledType;
                assembledType = type.GetGenericTypeDefinition().MakeGenericType( args );
            }

            return assembledType;
        }

        /// <summary>
        /// Applies the provided key to the specified service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to apply a key to.</param>
        /// <param name="key">The key to apply.</param>
        /// <returns>A new <see cref="Type">type</see> which represents the original <paramref name="serviceType">service type</paramref>
        /// with an associated key.</returns>
        /// <remarks>This method dynamically creates a new <see cref="Type">type</see> which has the <see cref="ServiceKeyAttribute"/> applied.
        /// Types that already have this attribute applied should not use this method.</remarks>
        public Type ApplyKey( Type serviceType, string key )
        {
            Contract.Requires<ArgumentNullException>( serviceType != null, "serviceType" );
            Contract.Ensures( Contract.Result<Type>() != null );

            // short-circuit if there's nothing to do
            if ( key == null )
                return serviceType;

            // create projected type with service key
            var closedTypes = new Stack<TypeInfo>();
            var type = DisassembleServiceType( serviceType.GetTypeInfo(), closedTypes );
            var context = new ServiceAttributeContext( key );
            var projectedType = context.MapType( type );

            // if service type isn't generic, we're done
            if ( !projectedType.IsGenericType )
                return projectedType.AsType();

            // re-create closed generic type with new projected type substituted
            return AssembleServiceType( projectedType.AsType(), closedTypes );
        }
    }
}