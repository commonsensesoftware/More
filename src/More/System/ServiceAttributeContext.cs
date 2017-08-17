namespace More
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Context;

    sealed class ServiceAttributeContext : CustomReflectionContext
    {
        readonly ServiceKeyAttribute serviceKey;

        internal ServiceAttributeContext( string key )
        {
            Contract.Requires( key != null );
            serviceKey = new ServiceKeyAttribute( key );
        }

        protected override IEnumerable<object> GetCustomAttributes( MemberInfo member, IEnumerable<object> declaredAttributes )
        {
            Contract.Assume( declaredAttributes != null );

            var attributes = declaredAttributes.ToList();
            attributes.Add( serviceKey );
            return attributes.ToArray();
        }
    }
}