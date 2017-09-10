namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Reflection;

    sealed class AssignableSpecification<T> : SpecificationBase<Type>
    {
        readonly TypeInfo type = typeof( T ).GetTypeInfo();

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
            {
                return false;
            }

            var otherType = item.GetTypeInfo();

            return otherType.IsPublic &&
                ( !otherType.IsAbstract || otherType.IsInterface ) &&
                  !otherType.IsGenericTypeDefinition &&
                   type.IsAssignableFrom( otherType );
        }
    }
}