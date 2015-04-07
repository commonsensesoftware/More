namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Reflection;
    using global::Windows.UI.Xaml;

    internal sealed class PartSpecification : SpecificationBase<Type>
    {
        private static readonly TypeInfo dependencyObject = typeof( DependencyObject ).GetTypeInfo();

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
                return false;

            var ti = item.GetTypeInfo();
            return ti.IsPublic && !ti.IsAbstract && !dependencyObject.IsAssignableFrom( ti );
        }
    }
}
