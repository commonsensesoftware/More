namespace More.VisualStudio.ViewModels
{
    using System;

    interface IProjectExecutionContext : IDisposable
    {
        string GetProviderServices( string invariantName );
    }
}