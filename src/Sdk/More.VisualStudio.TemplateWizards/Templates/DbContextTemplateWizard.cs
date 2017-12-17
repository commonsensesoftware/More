namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Data.Core;
    using Microsoft.VisualStudio.Data.Services;
    using Microsoft.VisualStudio.DataTools.Interop;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VSDesigner.VSDesignerPackage;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Xml.Linq;
    using ViewModels;
    using Views;
    using IVsPackageInstaller = NuGet.VisualStudio.IVsPackageInstaller;
    using IVsPackageInstallerServices = NuGet.VisualStudio.IVsPackageInstallerServices;

    /// <summary>
    /// Represents a template wizard to create an Entity Framework (EF) DbContext with interface scaffolding.
    /// </summary>
    [CLSCompliant( false )]
    public class DbContextTemplateWizard : ItemTemplateWizard
    {
        void InstallPackages( IComponentModel services, XElement packages, IDictionary<string, string> packageVersions )
        {
            Contract.Requires( services != null );
            Contract.Requires( packages != null );
            Contract.Requires( packageVersions != null );

            if ( packageVersions.Count == 0 )
            {
                return;
            }

            var extensionId = (string) packages.Attribute( "repositoryId" );
            var installer = services.GetService<IVsPackageInstaller>();
            var unzipped = false;
            var skipAssemblyReferences = false;
            var ignoreDependencies = false;

            // although it's less efficient, we install the packages one at a time to display status.
            // the mechanism to report back status is internal and can't be wired up without some
            // crafty reflection hacks.  this is a more straight forward alternative.
            foreach ( var entry in packageVersions )
            {
                var packageVersion = new Dictionary<string, string>() { [entry.Key] = entry.Value };

                DesignTimeEnvironment.StatusBar.Text = SR.PackageInstallStatus.FormatDefault( entry.Key, entry.Value );
                installer.InstallPackagesFromVSExtensionRepository( extensionId, unzipped, skipAssemblyReferences, ignoreDependencies, Project, packageVersion );
            }
        }

        void InstallEntityFrameworkPackageIfNeeded( IComponentModel services, IVsPackageInstallerServices nuget, Lazy<XElement> wizardData )
        {
            Contract.Requires( services != null );
            Contract.Requires( nuget != null );
            Contract.Requires( wizardData != null );

            var selectedKey = GetString( "_SelectedEFVersion" );
            var packages = wizardData.Value;
            var package = ( from element in packages.Elements( "package" )
                            where selectedKey == (string) element.Attribute( "key" )
                            select new
                            {
                                Id = (string) element.Attribute( "id" ),
                                Version = (string) element.Attribute( "version" )
                            } ).FirstOrDefault();

            if ( package == null || nuget.IsPackageInstalled( Project, package.Id ) )
            {
                return;
            }

            var packageVersions = new Dictionary<string, string>() { [package.Id] = package.Version };

            InstallPackages( services, packages, packageVersions );
        }

        ProjectItem GetOrCreateConfigFile()
        {
            var fileName = Project.GetConfigurationFileName();
            var comparer = StringComparer.OrdinalIgnoreCase;
            var configFile = Project.ProjectItems.Cast<ProjectItem>().FirstOrDefault( pi => comparer.Equals( pi.Name, fileName ) );

            if ( configFile != null )
            {
                return configFile;
            }

            var vb = Project.IsVisualBasic();

            // add *.config for a web application
            if ( Project.IsWebApp() )
            {
                return AddFromTemplate( "WebConfig.zip", fileName, vb ? "{349C5854-65DF-11DA-9384-00065B846F21}" : "{349C5853-65DF-11DA-9384-00065B846F21}" );
            }

            // add *.config for all other web project types
            var templateName = Project.IsWebProject() ? "WebConfig.zip" : ( vb ? "AppConfigurationInternal.zip" : "AppConfigInternal.zip" );
            return AddFromTemplate( templateName, fileName, Project.Kind );
        }

        void UpdateConfigFileIfNeeded()
        {
            var cs = GetString( "$connectionString$" );

            if ( string.IsNullOrEmpty( cs ) )
            {
                return;
            }

            var name = GetString( "$connectionStringKey$" );
            var configFile = GetOrCreateConfigFile();
            var sourceControl = DesignTimeEnvironment.SourceControl;

            if ( sourceControl.IsItemUnderSCC( configFile.Name ) && !sourceControl.IsItemCheckedOut( configFile.Name ) )
            {
                sourceControl.CheckOutItem( configFile.Name );
            }

            var path = configFile.FileNames[1];
            var xml = XDocument.Load( path );
            var configuration = xml.Root;
            var connectionStrings = configuration.Element( "connectionStrings" );

            if ( connectionStrings == null )
            {
                connectionStrings = new XElement( "connectionStrings" );
                configuration.Add( connectionStrings );
            }

            var add = connectionStrings.Elements( "add" ).FirstOrDefault( e => (string) e.Attribute( "name" ) == name );

            if ( add == null )
            {
                var providerName = GetString( "$providerName$", "System.Data.SqlClient" );
                add = new XElement( "add", new XAttribute( "name", name ), new XAttribute( "connectionString", cs ), new XAttribute( "providerName", providerName ) );
                connectionStrings.Add( add );
            }
            else
            {
                var providerName = GetString( "$providerName$", (string) add.Attribute( "providerName" ) ?? "System.Data.SqlClient" );
                add.SetAttributeValue( "connectionString", cs );
                add.SetAttributeValue( "providerName", providerName );
            }

            xml.Save( path );
        }

        /// <summary>
        /// Occurs after a project item finished being generated.
        /// </summary>
        /// <param name="projectItem">The generated <see cref="ProjectItem">project item</see>.</param>
        public override void ProjectItemFinishedGenerating( ProjectItem projectItem )
        {
            if ( projectItem == null )
            {
                return;
            }

            projectItem.Properties.Item( "CustomTool" ).Value = "DbContextGenerator";
            DesignTimeEnvironment.StatusBar.Text = SR.EvaluatingPackages;

            var services = Context.GetRequiredService<IComponentModel>();
            var nuget = services.GetService<IVsPackageInstallerServices>();
            var wizardData = new Lazy<XElement>( () => XDocument.Parse( GetString( "$packagedata$", "<packages />" ) ).Root );

            InstallEntityFrameworkPackageIfNeeded( services, nuget, wizardData );
            UpdateConfigFileIfNeeded();
        }

        DbContextReplacementsMapper CreateMapper( DbContextItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( Contract.Result<DbContextReplacementsMapper>() != null );

            var dataProviderManager = Context.GetRequiredService<IVsDataProviderManager>();
            var providerMapper = new Lazy<IDTAdoDotNetProviderMapper>( Context.GetRequiredService<IDTAdoDotNetProviderMapper> );
            var dataExplorerConnectionManager = Context.GetRequiredService<IVsDataExplorerConnectionManager>();
            Func<IVsDataConnectionManager> dataConnectionManagerFactory = Context.GetRequiredService<IVsDataConnectionManager>;

            // we honor the global connection service if available, but we don't need it
            Context.TryGetService( out IGlobalConnectionService globalConnectionService );

            return new DbContextReplacementsMapper( Project, Context, dataProviderManager, globalConnectionService, providerMapper, dataExplorerConnectionManager, dataConnectionManagerFactory );
        }

        DbContextItemTemplateWizard CreateView( DbContextItemTemplateWizardViewModel model, IVsUIShell shell )
        {
            Contract.Requires( model != null );
            Contract.Requires( shell != null );
            Contract.Ensures( Contract.Result<DbContextItemTemplateWizard>() != null );

            var projectInfo = new ProjectInformation( Project );
            var dataConnectionDialogFactory = new Lazy<IVsDataConnectionDialogFactory>( Context.GetRequiredService<IVsDataConnectionDialogFactory> );
            var dataExplorerConnectionManager = new Lazy<IVsDataExplorerConnectionManager>( Context.GetRequiredService<IVsDataExplorerConnectionManager> );

            return new DbContextItemTemplateWizard( model, projectInfo, shell, dataConnectionDialogFactory, dataExplorerConnectionManager );
        }

        /// <summary>
        /// Attempts to run the template wizard.
        /// </summary>
        /// <param name="shell">The <see cref="IVsUIShell">shell</see> associated with the wizard.</param>
        /// <returns>True if the wizard completed successfully; otherwise, false if the wizard was canceled.</returns>
        protected override bool TryRunWizard( IVsUIShell shell )
        {
            Arg.NotNull( shell, nameof( shell ) );

            var model = new DbContextItemTemplateWizardViewModel();
            var mapper = CreateMapper( model );
            var view = CreateView( model, shell );
            var statusBar = DesignTimeEnvironment.StatusBar;

            // the mapping relies on visual studio enumerating the available data sources,
            // which could take a while. we [seemingly] cannot run this in the background
            // so provide the user with some feedback to let them know we are doing work.
            statusBar.Text = SR.StatusInitializingDataSources;
            mapper.Map( Context.Replacements, model );
            statusBar.Clear();

            if ( !( view.ShowDialog( shell ) ?? false ) )
            {
                return false;
            }

            mapper.Map( model, Context.Replacements );
            return true;
        }
    }
}