namespace More.VisualStudio.ViewModels
{
    using System;

    internal interface IProjectExecutionContext : IDisposable
    {
        string GetProviderServices( string invariantName );
    }
}
