namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Reflection;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
#elif !WEB
    using System.Windows;
#endif

    internal sealed class PartSpecification : SpecificationBase<Type>
    {
#if WEB
        public override bool IsSatisfiedBy( Type item ) => item != null;
#else
        private static readonly TypeInfo dependencyObject = typeof( DependencyObject ).GetTypeInfo();

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
                return false;

            var ti = item.GetTypeInfo();
            return ti.IsPublic && !ti.IsAbstract && !dependencyObject.IsAssignableFrom( ti );
        }
#endif
    }
}
