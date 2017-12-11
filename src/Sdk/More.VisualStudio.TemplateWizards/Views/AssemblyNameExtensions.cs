namespace More.VisualStudio.Views
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    static class AssemblyNameExtensions
    {
        internal static string GetLocation( this AssemblyName assemblyName )
        {
            Contract.Requires( assemblyName != null );

            var codebase = assemblyName.CodeBase;

            if ( !Uri.TryCreate( codebase, UriKind.Absolute, out var location ) )
            {
                return null;
            }

            return location.LocalPath;
        }
    }
}