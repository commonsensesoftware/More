namespace More
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a <see cref="IServiceProvider">service provider</see> that can adapt to another
    /// service locator or dependency injection interface.
    /// </summary>
    public class ServiceProviderAdapter : IServiceProvider
    {
        private readonly Lazy<ServiceTypeDisassembler> disassembler = new Lazy<ServiceTypeDisassembler>( () => new ServiceTypeDisassembler() );
        private readonly Func<Type, string, object> resolve;
        private readonly Func<Type, string, IEnumerable<object>> resolveAll;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/> class.
        /// </summary>
        /// <param name="resolve">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve a single service of a particular <see cref="Type">type</see>.</param>
        public ServiceProviderAdapter( Func<Type, string, object> resolve )
        {
            Arg.NotNull( resolve, nameof( resolve ) );

            this.resolve = resolve;
            resolveAll = DefaultResolveAll;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/> class.
        /// </summary>
        /// <param name="resolve">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve a single service of a particular <see cref="Type">type</see>.</param>
        /// <param name="resolveAll">The <see cref="Func{T1,T2,TResult}">function</see> used to resolve all services of a particular <see cref="Type">type</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        public ServiceProviderAdapter( Func<Type, string, object> resolve, Func<Type, string, IEnumerable<object>> resolveAll )
        {
            Arg.NotNull( resolve, nameof( resolve ) );
            Arg.NotNull( resolveAll, nameof( resolveAll ) );

            this.resolve = resolve;
            this.resolveAll = resolveAll;
        }

        private ServiceTypeDisassembler Disassembler
        {
            get
            {
                Contract.Ensures( Contract.Result<ServiceTypeDisassembler>() != null );
                return disassembler.Value;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should not throw exceptions for service resolution failures." )]
        private IEnumerable<object> DefaultResolveAll( Type serviceType, string key )
        {
            Contract.Requires( serviceType != null );

            var services = new List<object>();

            if ( key == null )
            {
                // add ourself when requested
                if ( typeof( IServiceProvider ) == serviceType )
                    services.Add( this );
            }

            try
            {
                // return sequence with a single element
                var service = resolve( serviceType, key );

                if ( service != null )
                    services.Add( service );
            }
            catch
            {
                // do nothing if resolution fails
            }

            return services;
        }

        /// <summary>
        /// Gets a service of the requested type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service to return.</param>
        /// <returns>The service instance corresponding to the requested
        /// <paramref name="serviceType">service type</paramref> or null if no match is found.</returns>
        public virtual object GetService( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );

            var key = Disassembler.ExtractKey( serviceType );
            Type innerServiceType;

            // return multiple services, if requested
            if ( Disassembler.IsForMany( serviceType, out innerServiceType ) )
                return resolveAll( innerServiceType, key );

            if ( key == null )
            {
                // return ourself when requested
                if ( typeof( IServiceProvider ) == serviceType )
                    return this;
            }

            return resolve( serviceType, key );
        }
    }
}
