namespace More.Composition.Hosting
{
    using System;
    using System.Reflection;
    using More.ComponentModel;
    using global::Windows.UI.Xaml.Controls;

    sealed class SettingsFlyoutSpecification : SpecificationBase<Type>
    {
        static readonly TypeInfo settingsFlyout = typeof( SettingsFlyout ).GetTypeInfo();

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
            {
                return false;
            }

            var ti = item.GetTypeInfo();
            return ti.IsPublic && !ti.IsAbstract && !ti.IsGenericTypeDefinition && settingsFlyout.IsAssignableFrom( ti );
        }
    }
}