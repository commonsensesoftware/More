namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Reflection;

    internal sealed class AssignableSpecification<T> : SpecificationBase<Type>
    {
        private readonly TypeInfo type = typeof( T ).GetTypeInfo();

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
                return false;

            var otherType = item.GetTypeInfo();
            return otherType.IsPublic &&
                  ( !otherType.IsAbstract || otherType.IsInterface ) &&
                  !otherType.IsGenericTypeDefinition &&
                   this.type.IsAssignableFrom( otherType );
        }
    }
}
