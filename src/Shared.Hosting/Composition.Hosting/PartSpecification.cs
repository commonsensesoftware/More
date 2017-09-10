namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Reflection;
#if UAP10_0
    using global::Windows.UI.Xaml;
#elif !ASPNET
    using System.Windows;
#endif

    sealed class PartSpecification : SpecificationBase<Type>
    {
#if ASPNET
        public override bool IsSatisfiedBy( Type item ) => item != null;
#else
        private static readonly TypeInfo dependencyObject = typeof( DependencyObject ).GetTypeInfo();

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
            {
                return false;
            }

            var ti = item.GetTypeInfo();
            return ti.IsPublic && !ti.IsAbstract && !dependencyObject.IsAssignableFrom( ti );
        }
#endif
    }
}