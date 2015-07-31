namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Composition.Convention;

    /// <content>
    /// Provides additional implementation specific to Windows applications.
    /// </content>
    public partial class Host
    {
        static partial void AddUISpecificConventions( ConventionBuilder builder )
        {
            var viewModel = new ViewModelSpecification();

            builder.ForTypesDerivedFrom<IShellView>().Export().Export<IShellView>().Shared();
            builder.ForTypesMatching( viewModel.IsSatisfiedBy ).Export();
            builder.ForType<EventBroker>().Export<IEventBroker>().Shared();
        }
    }
}
