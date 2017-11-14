namespace More.VisualStudio.Views
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Reflection;

    static class AssemblyNameExtensions
    {
        internal static string GetLocation( this AssemblyName assemblyName )
        {
            Contract.Requires( assemblyName != null );

            var codebase = assemblyName.CodeBase;
            Uri location;

            if ( !Uri.TryCreate( codebase, UriKind.Absolute, out location ) )
            {
                return null;
            }

            return location.LocalPath;
        }
    }
}