namespace More.Composition.Hosting
{
    using System;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class FrameHost<T>
    {
        partial void OnConfigure()
        {
            this.WithConfiguration<ContractSettings>().DependsOn<ShowShellView<FrameShellView<T>>>();
        }
    }
}
