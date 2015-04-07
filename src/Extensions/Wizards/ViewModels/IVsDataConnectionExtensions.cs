namespace More.VisualStudio.ViewModels
{
    using Microsoft.VisualStudio.Data.Services;
    using Microsoft.VisualStudio.DataTools.Interop;
    using System;
    using System.Diagnostics.Contracts;

    internal static class IVsDataConnectionExtensions
    {
        internal static string GetInvariantProviderName( this IVsDataConnection connection, IDTAdoDotNetProviderMapper mapper )
        {
            Contract.Requires( connection != null );
            Contract.Requires( mapper != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            var invariantProviderName = mapper.MapGuidToInvariantName( connection.Provider );
            return string.IsNullOrWhiteSpace( invariantProviderName ) ? "System.Data.SqlClient" : invariantProviderName;
        }

        internal static string DecryptedConnectionString( this IVsDataConnection connection )
        {
            Contract.Requires( connection != null );
            return DataProtection.DecryptString( connection.EncryptedConnectionString );
        }
    }
}
