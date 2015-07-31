namespace More.Composition.Hosting
{
    using System;
    using System.Composition.Convention;
    using System.Windows;
    using global::Windows.UI.Xaml.Controls;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class Host
    {
        static partial void AddWinRTSpecificConventions( ConventionBuilder builder )
        {
            var assembly = new HostAssemblySpecification();
            var settingsFlyout = new SettingsFlyoutSpecification();

            builder.ForTypesMatching( settingsFlyout.IsSatisfiedBy ).Export().Export<SettingsFlyout>().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p != null && p.Name == "Model" );
        }
    }
}
