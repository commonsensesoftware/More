namespace More
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Context;

    internal sealed class ServiceAttributeContext : CustomReflectionContext
    {
        private readonly ServiceKeyAttribute serviceKey;

        internal ServiceAttributeContext( string key )
        {
            Contract.Requires( key != null );
            this.serviceKey = new ServiceKeyAttribute( key );
        }

        protected override IEnumerable<object> GetCustomAttributes( MemberInfo member, IEnumerable<object> declaredAttributes )
        {
            Contract.Assume( member != null );
            Contract.Assume( declaredAttributes != null );

            // the ServiceKeyAttribute can only be applied once; if it's explicitly declared, use that metadata
            if ( declaredAttributes.OfType<ServiceKeyAttribute>().Any() )
                return declaredAttributes;

            var attributes = declaredAttributes.ToList();
            attributes.Add( this.serviceKey );
            return attributes.ToArray();
        }
    }
}