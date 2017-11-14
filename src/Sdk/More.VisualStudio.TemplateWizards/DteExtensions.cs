namespace More.VisualStudio
{
    using EnvDTE;
    using System;
    using System.Diagnostics.Contracts;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    static class DteExtensions
    {
        internal static Project GetActiveProject( this DTE dte )
        {
            Contract.Requires( dte != null );

            // BUG: https://connect.microsoft.com/VisualStudio/feedback/details/735835/bug-dte-activesolutionprojects-property-causes-comexception-if-solution-explorer-is-not-shown-and-no-solution-loaded
            if ( !dte.Solution.IsOpen )
            {
                return null;
            }

            dynamic projects = dte.ActiveSolutionProjects;

            if ( projects.Length == 0 )
            {
                return null;
            }

            var project = projects[0];

            return project;
        }

        internal static object GetService( this DTE dte, Type serviceType )
        {
            Contract.Requires( dte != null );
            Contract.Requires( serviceType != null );

            if ( dte is IOleServiceProvider oleServiceProvider )
            {
                using ( var serviceProvider = new VisualStudioServiceProvider( oleServiceProvider ) )
                {
                    return serviceProvider.GetService( serviceType );
                }
            }

            return null;
        }

        internal static TService GetService<TService>( this DTE dte ) where TService : class
        {
            Contract.Requires( dte != null );
            return (TService) dte.GetService( typeof( TService ) );
        }
    }
}