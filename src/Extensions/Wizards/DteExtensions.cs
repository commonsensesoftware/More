namespace More.VisualStudio
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Diagnostics.Contracts;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    internal static class DteExtensions
    {
        internal static Project GetActiveProject( this DTE dte )
        {
            Contract.Requires( dte != null );

            // BUG: https://connect.microsoft.com/VisualStudio/feedback/details/735835/bug-dte-activesolutionprojects-property-causes-comexception-if-solution-explorer-is-not-shown-and-no-solution-loaded
            // note: this is isn't just a workaround, but is also a reasonable short-circuit
            if ( !dte.Solution.IsOpen )
                return null;

            dynamic projects = dte.ActiveSolutionProjects;

            if ( projects.Length == 0 )
                return null;

            var project = projects[0];

            return project;
        }

        internal static object GetService( this DTE dte, Type serviceType )
        {
            Contract.Requires( dte != null );
            Contract.Requires( serviceType != null );

            using ( var serviceProvider = new VisualStudioServiceProvider( (IOleServiceProvider) dte ) )
                return serviceProvider.GetService( serviceType );
        }

        internal static TService GetService<TService>( this DTE dte ) where TService : class
        {
            Contract.Requires( dte != null );
            return (TService) dte.GetService( typeof( TService ) );
        }
    }
}
