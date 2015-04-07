namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Reflection;
    using global::Windows.UI.Xaml.Controls;


    internal sealed class UserControlSpecification : SpecificationBase<Type>
    {
        private static readonly TypeInfo page = typeof( Page ).GetTypeInfo();
        private static readonly TypeInfo userControl = typeof( UserControl ).GetTypeInfo();

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
                return false;

            // note: a Page is a UserControl, which is handled differently; therefore,
            // only match UserControl[s] that are not Page[s]
            var ti = item.GetTypeInfo();
            return ti.IsPublic &&
                  !ti.IsAbstract &&
                  !ti.IsGenericTypeDefinition &&
                   userControl.IsAssignableFrom( ti ) &&
                  !page.IsAssignableFrom( ti );
        }
    }
}
