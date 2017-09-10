namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Reflection;
    using global::Windows.UI.Xaml.Controls;

    sealed class PageSpecification : SpecificationBase<Type>
    {
        static readonly TypeInfo page = typeof( Page ).GetTypeInfo();

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
            {
                return false;
            }

            var ti = item.GetTypeInfo();
            return ti.IsPublic && !ti.IsAbstract && !ti.IsGenericTypeDefinition && page.IsAssignableFrom( ti );
        }
    }
}