namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Data.Core;
    using Microsoft.VisualStudio.Data.Services;
    using Microsoft.VisualStudio.DataTools.Interop;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VSDesigner.VSDesignerPackage;
    using More.VisualStudio.ViewModels;
    using More.VisualStudio.Views;
    using NuGet.VisualStudio;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Window = System.Windows.Window;

    /// <summary>
    /// Represents a template wizard to create an Entity Framework (EF) DbContext with interface scaffolding.
    /// </summary>
    public class DbContextTemplateWizard : ItemTemplateWizard
    {
        private void InstallPackages( IComponentModel services, XElement packages, IDictionary<string, string> packageVersions )
        {
            Contract.Requires( services != null );
            Contract.Requires( packages != null );
            Contract.Requires( packageVersions != null );

            if ( packageVersions.Count == 0 )
                return;

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
                var packageVersion = new Dictionary<string, string>()
                {
                    { entry.Key, entry.Value }
                };

                // provide user feedback
                this.DesignTimeEnvironment.StatusBar.Text = SR.PackageInstallStatus.FormatDefault( entry.Key, entry.Value );

                // install the package from the vsix location
                installer.InstallPackagesFromVSExtensionRepository( extensionId, unzipped, skipAssemblyReferences, ignoreDependencies, this.Project, packageVersion );
            }
        }

        private void InstallCompositionPackagesIfNeeded( IComponentModel services, IVsPackageInstallerServices nuget, Lazy<XElement> wizardData )
        {
            Contract.Requires( services != null );
            Contract.Requires( nuget != null );
            Contract.Requires( wizardData != null );

            // ensure composition is enabled
            if ( !this.GetBoolean( "$compose$" ) )
                return;

            var packages = wizardData.Value;
            var packageIds = new[] { "Microsoft.Composition", "More.Composition" };
            var packageVersions = new Dictionary<string, string>();

            // build collection of required packages and versions
            foreach ( var packageId in packageIds )
            {
                if ( nuget.IsPackageInstalled( this.Project, packageId ) )
                    continue;

                var packageVersion = ( from element in packages.Elements( "package" )
                                       let id = (string) element.Attribute( "id" )
                                       where id == packageId
                                       select (string) element.Attribute( "version" ) ).FirstOrDefault();

                if ( !string.IsNullOrEmpty( packageVersion ) )
                    packageVersions[packageId] = packageVersion;
            }

            this.InstallPackages( services, packages, packageVersions );
        }

        private void InstallEntityFrameworkPackageIfNeeded( IComponentModel services, IVsPackageInstallerServices nuget, Lazy<XElement> wizardData )
        {
            Contract.Requires( services != null );
            Contract.Requires( nuget != null );
            Contract.Requires( wizardData != null );

            // determine whether the package is already installed
            if ( nuget.IsPackageInstalled( this.Project, "EntityFramework" ) )
                return;

            var packages = wizardData.Value;
            var selectedId = this.GetString( "_SelectedEFVersion" );
            var packageVersion = ( from element in packages.Elements( "package" )
                                   let id = (string) element.Attribute( "id" )
                                   where id == selectedId
                                   select (string) element.Attribute( "version" ) ).FirstOrDefault();

            // package version unknown (should only happen if there's an error in the wizard or the vstemplate)
            if ( string.IsNullOrEmpty( packageVersion ) )
                return;

            var packageVersions = new Dictionary<string, string>()
            {
                { "EntityFramework", packageVersion }
            };

            this.InstallPackages( services, packages, packageVersions );
        }

        private ProjectItem GetOrCreateConfigFile()
        {
            var fileName = this.Project.GetConfigurationFileName();
            var comparer = StringComparer.OrdinalIgnoreCase;
            var configFile = this.Project.ProjectItems.Cast<ProjectItem>().FirstOrDefault( pi => comparer.Equals( pi.Name, fileName ) );

            // if a *.config file already exists, there's nothing to do
            if ( configFile != null )
                return configFile;

            var vb = this.Project.IsVisualBasic();

            // add *.config for a web application
            if ( this.Project.IsWebApp() )
                return this.AddFromTemplate( "WebConfig.zip", fileName, vb ? "{349C5854-65DF-11DA-9384-00065B846F21}" : "{349C5853-65DF-11DA-9384-00065B846F21}" );

            // add *.config for all other web project types
            var templateName = this.Project.IsWebProject() ? "WebConfig.zip" : ( vb ? "AppConfigurationInternal.zip" : "AppConfigInternal.zip" );
            return this.AddFromTemplate( templateName, fileName, this.Project.Kind );
        }

        private void UpdateConfigFileIfNeeded()
        {
            var cs = this.GetString( "$connectionString$" );

            if ( string.IsNullOrEmpty( cs ) )
                return;

            var name = this.GetString( "$connectionStringKey$" );
            var providerName = this.GetString( "$providerName$", "System.Data.SqlClient" );
            var configFile = this.GetOrCreateConfigFile();
            var sourceControl = this.DesignTimeEnvironment.SourceControl;

            // check-out file if necessary
            if ( sourceControl.IsItemUnderSCC( configFile.Name ) && !sourceControl.IsItemCheckedOut( configFile.Name ) )
                sourceControl.CheckOutItem( configFile.Name );

            // add <add/> element to <connectionStrings/>
            var path = configFile.FileNames[1];
            var xml = XDocument.Load( path );
            var configuration = xml.Root;
            var connectionStrings = configuration.Element( "connectionStrings" );
            var add = new XElement( "add", new XAttribute( "name", name ), new XAttribute( "connectionString", cs ), new XAttribute( "providerName", providerName ) );

            // add <connectionStrings/> element as necessary
            if ( connectionStrings == null )
            {
                connectionStrings = new XElement( "connectionStrings" );
                configuration.Add( connectionStrings );
            }

            // save the changes
            connectionStrings.Add( add );
            xml.Save( path );
        }

        /// <summary>
        /// Occurs after a project item finished being generated.
        /// </summary>
        /// <param name="projectItem">The generated <see cref="ProjectItem">project item</see>.</param>
        public override void ProjectItemFinishedGenerating( ProjectItem projectItem )
        {
            if ( projectItem == null )
                return;

            projectItem.Properties.Item( "CustomTool" ).Value = "DbContextGenerator";
            this.DesignTimeEnvironment.StatusBar.Text = SR.EvaluatingPackages;

            var services = this.Context.GetRequiredService<IComponentModel>();
            var nuget = services.GetService<IVsPackageInstallerServices>();
            var wizardData = new Lazy<XElement>( () => XDocument.Parse( this.GetString( "$packagedata$", "<packages />" ) ).Root );

            this.InstallCompositionPackagesIfNeeded( services, nuget, wizardData );
            this.InstallEntityFrameworkPackageIfNeeded( services, nuget, wizardData );
            this.UpdateConfigFileIfNeeded();
        }

        private DbContextReplacementsMapper CreateMapper( DbContextItemTemplateWizardViewModel model, Window window, IProgress<Window> progress )
        {
            Contract.Requires( model != null );
            Contract.Requires( window != null );
            Contract.Requires( progress != null );
            Contract.Ensures( Contract.Result<DbContextReplacementsMapper>() != null );

            DbContextReplacementsMapper mapper;

            try
            {
                var dataProviderManager = this.Context.GetRequiredService<IVsDataProviderManager>();
                var providerMapper = new Lazy<IDTAdoDotNetProviderMapper>( this.Context.GetRequiredService<IDTAdoDotNetProviderMapper> );
                var dataExplorerConnectionManager = this.Context.GetRequiredService<IVsDataExplorerConnectionManager>();
                Func<IVsDataConnectionManager> dataConnectionManagerFactory = this.Context.GetRequiredService<IVsDataConnectionManager>;
                IGlobalConnectionService globalConnectionService;

                this.Context.TryGetService( out globalConnectionService );

                // create the mapper and perform the mapping now (in the background)
                mapper = new DbContextReplacementsMapper( this.Project, this.Context, dataProviderManager, globalConnectionService, providerMapper, dataExplorerConnectionManager, dataConnectionManagerFactory );
                mapper.Map( this.Context.Replacements, model );
            }
            finally
            {
                // note: this is effectively a callback to close the specified window
                progress.Report( window );
            }

            return mapper;
        }

        private Task<DbContextReplacementsMapper> CreateMapperAsync( DbContextItemTemplateWizardViewModel model, Window window, IProgress<Window> progress )
        {
            Contract.Requires( model != null );
            Contract.Requires( window != null );
            Contract.Requires( progress != null );
            Contract.Ensures( Contract.Result<Task<DbContextReplacementsMapper>>() != null );
            
            // mapping the data connections from visual studio can be expensive, so create and run the initial mapping in the background
            return Task.Run( () => this.CreateMapper( model, window, progress ) );
        }

        private DbContextItemTemplateWizard CreateView( DbContextItemTemplateWizardViewModel model, IVsUIShell shell )
        {
            Contract.Requires( model != null );
            Contract.Requires( shell != null );
            Contract.Ensures( Contract.Result<DbContextItemTemplateWizard>() != null );

            var projectInfo = new ProjectInformation( this.Project );
            var dataConnectionDialogFactory = new Lazy<IVsDataConnectionDialogFactory>( this.Context.GetRequiredService<IVsDataConnectionDialogFactory> );
            var dataExplorerConnectionManager = new Lazy<IVsDataExplorerConnectionManager>( this.Context.GetRequiredService<IVsDataExplorerConnectionManager> );

            return new DbContextItemTemplateWizard( model, projectInfo, shell, dataConnectionDialogFactory, dataExplorerConnectionManager );
        }

        /// <summary>
        /// Attempts to run the template wizard.
        /// </summary>
        /// <param name="shell">The <see cref="IVsUIShell">shell</see> associated with the wizard.</param>
        /// <returns>True if the wizard completed successfully; otherwise, false if the wizard was canceled.</returns>
        protected override bool TryRunWizard( IVsUIShell shell )
        {
            var model = new DbContextItemTemplateWizardViewModel();
            var mapper = Loader.LoadAsync( this.CreateMapperAsync, shell, SR.StatusInitializing, model ).Result;
            var view = this.CreateView( model, shell );

            if ( !( view.ShowDialog( shell ) ?? false ) )
                return false;

            // map model back to replacements
            mapper.Map( model, this.Context.Replacements );
            return true;
        }
    }
}
