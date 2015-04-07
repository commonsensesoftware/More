namespace More.VisualStudio
{
    using EnvDTE;
    using Microsoft.CodeAnalysis.MSBuild;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using VSLangProj;
    using VSLangProj80;
    using VsWebSite;

    internal static class ProjectExtensions
    {
        private const string CSharp = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        private const string VisualBasic = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        private const string WindowsPhoneApp81 = "{76F1466A-8B6D-4E39-A767-685A06062A39}";
        private const string WindowsStoreApp81 = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
        private const string Website = "{E24C65DC-7377-472b-9ABA-BC803B73C61A}";
        private const string WebApplication = "{349C5851-65DF-11DA-9384-00065B846F21}";
        private const string Portable = "{786C830F-07A1-408B-BD7F-6EE04809D6DB}";

        internal static TValue GetProperty<TValue>( this Project project, string propertyName )
        {
            Contract.Requires( project != null );
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );

            var property = project.Properties.OfType<Property>().SingleOrDefault( p => p.Name == propertyName );

            if ( property == null )
                return default( TValue );

            try
            {
                return (TValue) property.Value;
            }
            catch ( COMException )
            {
                return default( TValue );
            }
        }

        internal static IVsHierarchy GetHierarchy( this Project project )
        {
            Contract.Requires( project != null );

            IVsHierarchy hierarchy;
            var solution = project.GetService<IVsSolution>();
            var hr = solution.GetProjectOfUniqueName( project.UniqueName, out hierarchy );

            if ( hr != 0 )
                Marshal.ThrowExceptionForHR( hr );

            return hierarchy;
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved non-generic service location in the future." )]
        internal static object GetService( this Project project, Type serviceType )
        {
            Contract.Requires( project != null );
            Contract.Requires( serviceType != null );
            return project.DTE.GetService( serviceType );
        }

        internal static TService GetService<TService>( this Project project ) where TService : class
        {
            Contract.Requires( project != null );
            return (TService) project.DTE.GetService( typeof( TService ) );
        }

        internal static string GetRootNamespace( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( Contract.Result<string>() != null );
            return project.GetProperty<string>( "RootNamespace" ) ?? string.Empty;
        }

        private static async Task<AssemblyName> GetAssemblyNameAsync( this Project visualStudioProject )
        {
            Contract.Requires( visualStudioProject != null );
            Contract.Ensures( Contract.Result<Task<AssemblyName>>() != null );

            var path = visualStudioProject.FullName;
            AssemblyName assemblyName;

            using ( var workspace = MSBuildWorkspace.Create() )
            {
                var project = await workspace.OpenProjectAsync( path ).ConfigureAwait( false );
                var compilation = await project.GetCompilationAsync().ConfigureAwait( false );
                assemblyName = new AssemblyName( compilation.Assembly.Identity.GetDisplayName() );
            }

            var builder = new UriBuilder()
            {
                Scheme = Uri.UriSchemeFile,
                Host = string.Empty,
                Path = path
            };

            assemblyName.CodeBase = builder.Uri.ToString();
            assemblyName.ContentType = visualStudioProject.IsWinRTApp() ? AssemblyContentType.WindowsRuntime : AssemblyContentType.Default;

            return assemblyName;
        }

        internal static AssemblyName GetQualifiedAssemblyName( this Project project )
        {
            Contract.Requires( project != null );

            var task = project.GetAssemblyNameAsync();
            var assemblyName = task.Result;

            return assemblyName;
        }

        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
        internal static IEnumerable<KeyValuePair<string, Version>> GetReferences( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( Contract.Result<IEnumerable<KeyValuePair<string, Version>>>() != null );

            var project2 = project.Object as VSProject2;

            if ( project2 != null )
            {
                foreach ( var reference in project2.References.Cast<Reference>() )
                    yield return new KeyValuePair<string, Version>( reference.Name, new Version( reference.Version ) );
            }

            var site = project.Object as VSWebSite;

            if ( site == null )
                yield break;

            foreach ( var reference in site.References.Cast<AssemblyReference>() )
            {
                AssemblyName name;

                try
                {
                    name = new AssemblyName( reference.StrongName );
                }
                catch
                {
                    continue;
                }

                yield return new KeyValuePair<string, Version>( name.Name, name.Version );
            }
        }

        private static IReadOnlyList<string> ProjectTypeGuids( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( Contract.Result<IReadOnlyList<string>>() != null );

            var projectTypeGuids = string.Empty;
            var hierarchy = project.GetHierarchy();
            var aggregatableProject = hierarchy as IVsAggregatableProject;

            if ( aggregatableProject == null )
                return new string[0];

            var hr = aggregatableProject.GetAggregateProjectTypeGuids( out projectTypeGuids );

            if ( hr != 0 )
                Marshal.ThrowExceptionForHR( hr );

            if ( string.IsNullOrEmpty( projectTypeGuids ) )
                return new string[0];

            return projectTypeGuids.Split( new[] { ';' }, StringSplitOptions.RemoveEmptyEntries ).Select( s => s.Trim() ).ToArray();
        }

        private static bool IsProjectType( this Project project, params string[] projectTypeGuids )
        {
            Contract.Requires( project != null );
            Contract.Requires( projectTypeGuids != null );

            var actualProjectTypes = project.ProjectTypeGuids();
            var expectedProjectTypes = new HashSet<string>( projectTypeGuids, StringComparer.OrdinalIgnoreCase );
            var match = actualProjectTypes.Count == 0 ? expectedProjectTypes.Contains( project.Kind ) : actualProjectTypes.Any( expectedProjectTypes.Contains );
            return match;
        }

        internal static bool IsWindowsPhoneApp( this Project project )
        {
            Contract.Requires( project != null );
            return project.IsProjectType( WindowsPhoneApp81 );
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved for WinRT-specific project behaviors. Reconsider for removal at a later date." )]
        internal static bool IsWinRTApp( this Project project )
        {
            Contract.Requires( project != null );
            return project.IsProjectType( WindowsStoreApp81, WindowsPhoneApp81 );
        }

        internal static bool IsVisualBasic( this Project project )
        {
            Contract.Requires( project != null );
            return project.IsProjectType( VisualBasic );
        }

        internal static bool IsWebProject( this Project project )
        {
            Contract.Requires( project != null );
            return project.IsProjectType( Website, WebApplication );
        }

        internal static bool IsWebApp( this Project project )
        {
            Contract.Requires( project != null );
            return project.IsProjectType( WebApplication );
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved for PCL-specific project behaviors. Reconsider for removal at a later date." )]
        internal static bool IsPortableLibrary( this Project project )
        {
            Contract.Requires( project != null );
            return project.IsProjectType( Portable );
        }

        internal static string GetConfigurationFileName( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
            return project.IsWebProject() ? "web.config" : "app.config";
        }

        internal static string GetTargetDirectory( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            var config = project.ConfigurationManager.ActiveConfiguration;
            var projectDir = (string) project.Properties.Item( "FullPath" ).Value;
            var outputPath = (string) config.Properties.Item( "OutputPath" ).Value;

            return Path.Combine( projectDir, outputPath );
        }

        internal static string GetTargetPath( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            var config = project.ConfigurationManager.ActiveConfiguration;
            var projectDir = (string) project.Properties.Item( "FullPath" ).Value;
            var outputPath = (string) config.Properties.Item( "OutputPath" ).Value;
            var assemblyName = (string) project.Properties.Item( "OutputFileName" ).Value;

            return Path.Combine( projectDir, outputPath, assemblyName );
        }

        internal static string GetIntermediateDirectory( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            var config = project.ConfigurationManager.ActiveConfiguration;
            var projectDir = (string) project.Properties.Item( "FullPath" ).Value;
            var intermediatePath = (string) config.Properties.Item( "IntermediatePath" ).Value;

            return Path.Combine( projectDir, intermediatePath );
        }
    }
}
