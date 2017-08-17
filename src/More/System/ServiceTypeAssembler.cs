namespace More
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents an object which can apply and extract a key from the <see cref="Type">type</see> defined for a service.
    /// </summary>
    public class ServiceTypeAssembler : ServiceTypeDisassembler
    {
        /// <summary>
        /// Applies the provided key to the specified service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to apply a key to.</param>
        /// <param name="key">The key to apply.</param>
        /// <returns>A new <see cref="Type">type</see> which represents the original <paramref name="serviceType">service type</paramref>
        /// with an associated key.</returns>
        /// <remarks>This method dynamically creates a new <see cref="Type">type</see> which has the <see cref="ServiceKeyAttribute"/> applied.
        /// Types that already have this attribute applied will be perform no additional action.</remarks>
        public virtual Type ApplyKey( Type serviceType, string key )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );
            Contract.Ensures( Contract.Result<Type>() != null );

            if ( key == null )
            {
                return serviceType;
            }

            var serviceTypeInfo = serviceType.GetTypeInfo();

            if ( serviceTypeInfo.GetCustomAttribute<ServiceKeyAttribute>() != null )
            {
                return serviceType;
            }

            var context = new ServiceAttributeContext( key );
            var keyedServiceType = context.MapType( serviceTypeInfo );

            return keyedServiceType.AsType();
        }
    }
}