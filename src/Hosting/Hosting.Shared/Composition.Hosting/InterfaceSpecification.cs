namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    internal sealed class InterfaceSpecification : SpecificationBase<Type>
    {
        private readonly Type type;

        internal InterfaceSpecification( Type type )
        {
            Contract.Requires( type != null );
            this.type = type;
        }

        private static bool ImplementsInterface( Type sourceType, Type interfaceType )
        {
            Contract.Requires( sourceType != null );
            Contract.Requires( interfaceType != null );

            var t1 = sourceType.GetTypeInfo();

            if ( !t1.IsPublic || t1.IsAbstract || t1.IsInterface )
                return false;

            var t2 = interfaceType.GetTypeInfo();
            var interfaces = from pureVirtualClass in t1.ImplementedInterfaces.Select( t => t.GetTypeInfo() )
                             where pureVirtualClass.Assembly.Equals( t2.Assembly )
                             let typeDef = pureVirtualClass.IsGenericType ? pureVirtualClass.GetGenericTypeDefinition().GetTypeInfo() : pureVirtualClass
                             where t2.Equals( typeDef ) || t2.IsAssignableFrom( typeDef )
                             select pureVirtualClass;
            var match = interfaces.Any();

            return match;
        }

        public override bool IsSatisfiedBy( Type item )
        {
            return item == null ? false : ImplementsInterface( item, this.type );
        }
    }
}
