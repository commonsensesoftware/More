namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.Data.Core;
    using Microsoft.VisualStudio.Data.Services;
    using Microsoft.VisualStudio.DataTools.Interop;
    using Microsoft.VSDesigner.VSDesignerPackage;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using ViewModels;

    internal sealed class DbContextReplacementsMapper : ReplacementsMapper<DbContextItemTemplateWizardViewModel>
    {
        private sealed class MutuallyExclusiveTemplateOption : TemplateOption
        {
            private readonly TemplateOption other;

            internal MutuallyExclusiveTemplateOption( TemplateOption original, TemplateOption other )
                : base( original.Id, original.Name, original.Description )
            {
                Contract.Requires( original != null );
                Contract.Requires( other != null );

                this.other = other;
                IsEnabled = original.IsEnabled;
                IsOptional = original.IsOptional;

                if ( IsEnabled )
                    other.IsEnabled = true;

                other.IsOptional = !IsEnabled;
            }

            [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
            protected override void OnPropertyChanged( PropertyChangedEventArgs e )
            {
                Arg.NotNull( e, nameof( e ) );

                base.OnPropertyChanged( e );

                if ( !string.IsNullOrEmpty( e.PropertyName ) && e.PropertyName != nameof( IsEnabled ) )
                    return;

                if ( IsEnabled )
                    other.IsEnabled = true;

                other.IsOptional = !IsEnabled;
            }
        }

        private readonly IServiceProvider serviceProvider;
        private readonly IVsDataProviderManager dataProviderManager;
        private readonly IGlobalConnectionService globalConnectionService;
        private readonly Lazy<IDTAdoDotNetProviderMapper> providerMapper;
        private readonly IVsDataExplorerConnectionManager dataExplorerConnectionManager;
        private readonly Func<IVsDataConnectionManager> dataConnectionManagerFactory;

        internal DbContextReplacementsMapper(
            Project project,
            IServiceProvider serviceProvider,
            IVsDataProviderManager dataProviderManager,
            IGlobalConnectionService globalConnectionService,
            Lazy<IDTAdoDotNetProviderMapper> providerMapper,
            IVsDataExplorerConnectionManager dataExplorerConnectionManager,
            Func<IVsDataConnectionManager> dataConnectionManagerFactory )
            : base( project )
        {
            Contract.Requires( serviceProvider != null );
            Contract.Requires( dataProviderManager != null );
            // note: IGlobalConnectionService can be null
            Contract.Requires( providerMapper != null );
            Contract.Requires( dataExplorerConnectionManager != null );
            Contract.Requires( dataConnectionManagerFactory != null );

            this.serviceProvider = serviceProvider;
            this.dataProviderManager = dataProviderManager;
            this.globalConnectionService = globalConnectionService;
            this.providerMapper = providerMapper;
            this.dataExplorerConnectionManager = dataExplorerConnectionManager;
            this.dataConnectionManagerFactory = dataConnectionManagerFactory;
        }

        protected override IReadOnlyList<Tuple<string, Action<DbContextItemTemplateWizardViewModel, string>>> CreateReaders()
        {
            return new[]
            {
                new Tuple<string, Action<DbContextItemTemplateWizardViewModel, string>>( "_EFVersions", ReadEntityFrameworkVersions ),
                new Tuple<string, Action<DbContextItemTemplateWizardViewModel, string>>( "$showTips$", ( m, s ) => m.ShowTips = GetBoolean( s, true ) ),
                new Tuple<string, Action<DbContextItemTemplateWizardViewModel, string>>( "$compose$", ( m, s ) => m.UseComposition = GetBoolean( s ) )
            };
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        protected override IReadOnlyList<Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>> CreateWriters()
        {
            return new[]
            {
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "_SelectedEFVersion", m => m.SelectedEntityFrameworkVersion == null ? string.Empty : m.SelectedEntityFrameworkVersion.Id ),
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "$showTips$", m => m.ShowTips.ToString().ToLowerInvariant() ),
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "$compose$", m => m.UseComposition.ToString().ToLowerInvariant() ),
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "$implementedInterfaces$", BuildImplementedInterfaces ),
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "$modelNamespaceRequired$", m => ( m.ModelType.Namespace != GetReplacement( "$rootnamespace$" ) ).ToString().ToLowerInvariant() ),
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "$modelNamespace$", m => m.ModelType.Namespace ),
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "$connectionString$", m => m.SelectedDataSource == null || !m.SaveToConfigurationFile ? string.Empty : m.SelectedDataSource.ConnectionString ),
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "$connectionStringKey$", m => m.SelectedDataSource == null || !m.SaveToConfigurationFile ? GetReplacement( "$safeitemname$" ) : m.ConnectionStringName ),
                new Tuple<string, Func<DbContextItemTemplateWizardViewModel, string>>( "$providerName$", m => m.SelectedDataSource == null || !m.SaveToConfigurationFile ? string.Empty : m.SelectedDataSource.Connection.GetInvariantProviderName( providerMapper.Value ) )
            };
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        internal override void Map( IDictionary<string, string> replacements, DbContextItemTemplateWizardViewModel model )
        {
            base.Map( replacements, model );
            AddInterfaceOptions( model );
            AddDataSources( model );

            if ( Project == null )
                return;

            model.LocalAssemblyName = Project.GetQualifiedAssemblyName();
            model.SaveToConfigurationCaption = SR.SaveToConfigurationCaption.FormatDefault( Project.GetConfigurationFileName() );
        }

        private void AddInterfaceOptions( DbContextItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );

            var ids = new[] { "ImplementIReadOnlyRepositoryT", "ImplementIRepositoryT", "ImplementIUnitOfWorkT" };
            var options = StringsToTemplateOptions( ids ).ToArray();

            // if IRepository<T> is implemented, IReadOnlyRepository<T> is automatically implemented and is required
            options[1] = new MutuallyExclusiveTemplateOption( options[1], options[0] );

            model.ImplementedInterfaces.ReplaceAll( options );
        }

        private void ReadEntityFrameworkVersions( DbContextItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            var versions = GetStrings( value );
            var options = StringsToTemplateOptions( versions );

            model.EntityFrameworkVersions.ReplaceAll( options );
            model.SelectedEntityFrameworkVersion = model.EntityFrameworkVersions.FirstOrDefault();
        }

        private string BuildImplementedInterfaces( DbContextItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( Contract.Result<string>() != null );

            var culture = CultureInfo.InvariantCulture;
            var interfaces = new StringBuilder();
            var dataModel = model.ModelType.Name;

            Debug.Assert( model.ImplementedInterfaces.Count == 3, "Only 3 interfaces were expected for implementation." );

            if ( model.ImplementedInterfaces[0].IsEnabled && !model.ImplementedInterfaces[1].IsEnabled )
                interfaces.AppendFormat( culture, ", IReadOnlyRepository<{0}>", dataModel );

            if ( model.ImplementedInterfaces[1].IsEnabled )
                interfaces.AppendFormat( culture, ", IRepository<{0}>", dataModel );

            if ( model.ImplementedInterfaces[2].IsEnabled )
                interfaces.AppendFormat( culture, ", IUnitOfWork<{0}>", dataModel );

            return interfaces.ToString();
        }

        private void AddDataSources( DbContextItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );

            var keys = new HashSet<string>();
            var dataSources = GetConnections( keys ).Union( GetDataExplorerConnections( keys ) );

            model.DataSources.AddRange( dataSources );
            model.SelectedDataSource = model.DataSources.FirstOrDefault();
        }

        private IEnumerable<DataSource> GetConnections( ICollection<string> keys )
        {
            Contract.Requires( keys != null );
            Contract.Ensures( Contract.Result<IEnumerable<DataSource>>() != null );

            if ( globalConnectionService == null )
                yield break;

            var project = Project;
            DataConnection[] connections;

            try
            {
                connections = globalConnectionService.GetConnections( serviceProvider, project );
            }
            catch
            {
                yield break;
            }

            foreach ( var connection in connections )
            {
                if ( connection.Location != ConnectionLocation.SettingsFile && connection.Location != ConnectionLocation.Both )
                    continue;

                Guid provider;

                try
                {
                    provider = providerMapper.Value.MapInvariantNameToGuid( connection.ProviderName, connection.DesignTimeConnectionString, false );
                }
                catch
                {
                    continue;
                }

                if ( !dataProviderManager.HasEntityFrameworkProvider( provider, project, serviceProvider ) && !dataProviderManager.IsProjectSupported( provider, serviceProvider ) )
                    continue;

                var displayName = SR.DataConnectionDisplayName.FormatDefault( connection.Name );
                var connectionString = connection.DesignTimeConnectionString;
                var dataConnection = new Lazy<IVsDataConnection>( () => dataConnectionManagerFactory().GetConnection( provider, connectionString, false ) );

                keys.Add( connection.DesignTimeConnectionString );
                yield return new DataSource( displayName, dataConnection );
            }
        }

        private IEnumerable<DataSource> GetDataExplorerConnections( ICollection<string> keys )
        {
            Contract.Requires( dataProviderManager != null );
            Contract.Requires( keys != null );
            Contract.Ensures( Contract.Result<IEnumerable<DataSource>>() != null );

            foreach ( var connection in dataExplorerConnectionManager.Connections.Values )
            {
                var provider = connection.Provider;

                if ( dataProviderManager.HasEntityFrameworkProvider( provider, Project, serviceProvider ) && dataProviderManager.IsProjectSupported( provider, serviceProvider ) && !keys.Contains( connection.Connection.DecryptedConnectionString() ) )
                    yield return new DataSource( connection.DisplayName, connection.Connection );
            }
        }
    }
}
