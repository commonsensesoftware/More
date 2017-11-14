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
    using System.Text;
    using System.Threading.Tasks;
    using VSLangProj;
    using VSLangProj80;
    using VsWebSite;
    using static System.Reflection.AssemblyContentType;
    using static System.Runtime.InteropServices.Marshal;
    using static System.String;
    using static System.StringSplitOptions;

    static class ProjectExtensions
    {
        const string CSharp = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        const string VisualBasic = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        const string WindowsPhoneApp81 = "{76F1466A-8B6D-4E39-A767-685A06062A39}";
        const string WindowsStoreApp81 = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
        const string Website = "{E24C65DC-7377-472b-9ABA-BC803B73C61A}";
        const string WebApplication = "{349C5851-65DF-11DA-9384-00065B846F21}";
        const string Portable = "{786C830F-07A1-408B-BD7F-6EE04809D6DB}";

        internal static TValue GetProperty<TValue>( this Project project, string propertyName )
        {
            Contract.Requires( project != null );
            Contract.Requires( !IsNullOrEmpty( propertyName ) );

            var property = project.Properties.OfType<Property>().SingleOrDefault( p => p.Name == propertyName );

            if ( property == null )
            {
                return default( TValue );
            }

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

            var solution = project.GetService<IVsSolution>();
            var hr = solution.GetProjectOfUniqueName( project.UniqueName, out var hierarchy );

            if ( hr != 0 )
            {
                ThrowExceptionForHR( hr );
            }

            return hierarchy;
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved non-generic service location in the future." )]
        internal static object GetService( this Project project, Type serviceType ) => project.DTE.GetService( serviceType );

        internal static TService GetService<TService>( this Project project ) where TService : class => (TService) project.DTE.GetService( typeof( TService ) );

        internal static string GetRootNamespace( this Project project ) => project.GetProperty<string>( "RootNamespace" ) ?? Empty;

        static async Task<AssemblyName> GetAssemblyNameAsync( this Project visualStudioProject )
        {
            Contract.Requires( visualStudioProject != null );
            Contract.Ensures( Contract.Result<Task<AssemblyName>>() != null );

            var path = visualStudioProject.FullName;
            var assemblyName = default( AssemblyName );

            using ( var workspace = MSBuildWorkspace.Create() )
            {
                var project = await workspace.OpenProjectAsync( path ).ConfigureAwait( false );
                var compilation = await project.GetCompilationAsync().ConfigureAwait( false );
                assemblyName = new AssemblyName( compilation.Assembly.Identity.GetDisplayName() );
            }

            var builder = new UriBuilder() { Scheme = Uri.UriSchemeFile, Host = Empty, Path = path };

            assemblyName.CodeBase = builder.Uri.ToString();
            assemblyName.ContentType = visualStudioProject.IsWinRTApp() ? WindowsRuntime : Default;

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

            if ( project.Object is VSProject2 project2 )
            {
                foreach ( var reference in project2.References.Cast<Reference>() )
                {
                    yield return new KeyValuePair<string, Version>( reference.Name, new Version( reference.Version ) );
                }
            }

            var site = project.Object as VSWebSite;

            if ( site == null )
            {
                yield break;
            }

            foreach ( var reference in site.References.Cast<AssemblyReference>() )
            {
                var name = default( AssemblyName );

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

        static IReadOnlyList<string> ProjectTypeGuids( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( Contract.Result<IReadOnlyList<string>>() != null );

            var hierarchy = project.GetHierarchy();

            if ( hierarchy is IVsAggregatableProject aggregatableProject )
            {
                var hr = aggregatableProject.GetAggregateProjectTypeGuids( out var projectTypeGuids );

                if ( hr != 0 )
                {
                    ThrowExceptionForHR( hr );
                }

                if ( !IsNullOrEmpty( projectTypeGuids ) )
                {
                    return projectTypeGuids.Split( new[] { ';' }, RemoveEmptyEntries ).Select( s => s.Trim() ).ToArray();
                }
            }

            return new string[0];
        }

        static bool IsProjectType( this Project project, params string[] projectTypeGuids )
        {
            Contract.Requires( project != null );
            Contract.Requires( projectTypeGuids != null );

            var actualProjectTypes = project.ProjectTypeGuids();
            var expectedProjectTypes = new HashSet<string>( projectTypeGuids, StringComparer.OrdinalIgnoreCase );
            var match = actualProjectTypes.Count == 0 ? expectedProjectTypes.Contains( project.Kind ) : actualProjectTypes.Any( expectedProjectTypes.Contains );
            return match;
        }

        internal static bool IsWindowsStoreApp( this Project project ) => project.IsProjectType( WindowsStoreApp81 );

        internal static bool IsWindowsPhoneApp( this Project project ) => project.IsProjectType( WindowsPhoneApp81 );

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved for WinRT-specific project behaviors. Reconsider for removal at a later date." )]
        internal static bool IsWinRTApp( this Project project ) => project.IsProjectType( WindowsStoreApp81, WindowsPhoneApp81 );

        internal static bool IsCSharp( this Project project ) => project.IsProjectType( CSharp );

        internal static bool IsVisualBasic( this Project project ) => project.IsProjectType( VisualBasic );

        internal static bool IsWebProject( this Project project ) => project.IsProjectType( Website, WebApplication );

        internal static bool IsWebApp( this Project project ) => project.IsProjectType( WebApplication );

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved for PCL-specific project behaviors. Reconsider for removal at a later date." )]
        internal static bool IsPortableLibrary( this Project project ) => project.IsProjectType( Portable );

        internal static string GetTemplateLanguage( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );

            var language = new StringBuilder();

            if ( project.IsCSharp() )
            {
                language.Append( "CSharp" );
            }
            else if ( project.IsVisualBasic() )
            {
                language.Append( "VisualBasic" );
            }

            if ( language.Length == 0 )
            {
                return "Unknown";
            }

            if ( project.IsWindowsStoreApp() )
            {
                language.Append( "\\WinRT-Managed" );
            }
            else if ( project.IsWindowsPhoneApp() )
            {
                language.Append( "\\WindowsPhoneApp-Managed" );
            }

            return language.ToString();
        }

        internal static string GetConfigurationFileName( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );
            return project.IsWebProject() ? "web.config" : "app.config";
        }

        internal static string GetTargetDirectory( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );

            var config = project.ConfigurationManager.ActiveConfiguration;
            var projectDir = (string) project.Properties.Item( "FullPath" ).Value;
            var outputPath = (string) config.Properties.Item( "OutputPath" ).Value;

            return Path.Combine( projectDir, outputPath );
        }

        internal static string GetTargetPath( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );

            var config = project.ConfigurationManager.ActiveConfiguration;
            var projectDir = (string) project.Properties.Item( "FullPath" ).Value;
            var outputPath = (string) config.Properties.Item( "OutputPath" ).Value;
            var assemblyName = (string) project.Properties.Item( "OutputFileName" ).Value;

            return Path.Combine( projectDir, outputPath, assemblyName );
        }

        internal static string GetIntermediateDirectory( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );

            var config = project.ConfigurationManager.ActiveConfiguration;
            var projectDir = (string) project.Properties.Item( "FullPath" ).Value;
            var intermediatePath = (string) config.Properties.Item( "IntermediatePath" ).Value;

            return Path.Combine( projectDir, intermediatePath );
        }
    }
}