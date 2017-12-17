namespace More.VisualStudio.ViewModels
{
    using Collections.Generic;
    using ComponentModel;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using Windows.Input;

    /// <summary>
    /// Represents the view model for the Entity Framework (EF) DbContext item template wizard.
    /// </summary>
    public class DbContextItemTemplateWizardViewModel : ValidatableObject
    {
        readonly InteractionRequest<WindowCloseInteraction> close = new InteractionRequest<WindowCloseInteraction>( "CloseWindow" );
        readonly InteractionRequest<Interaction> browse = new InteractionRequest<Interaction>( "BrowseForModel" );
        readonly InteractionRequest<Interaction> addDataConnection = new InteractionRequest<Interaction>( "AddDataConnection" );
        readonly ObservableKeyedCollection<string, IInteractionRequest> interactionRequests = new ObservableKeyedCollection<string, IInteractionRequest>( r => r.Id );
        readonly ObservableKeyedCollection<string, INamedCommand> commands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        readonly ObservableKeyedCollection<string, INamedCommand> dialogCommands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        readonly RequiredTemplateOptionCollection implementedInterfaces = new RequiredTemplateOptionCollection();
        readonly ObservableCollection<TemplateOption> frameworkVersions = new ObservableCollection<TemplateOption>();
        readonly ObservableCollection<DataSource> dataSources = new ObservableCollection<DataSource>();
        string title;
        int currentStep;
        bool showTips = true;
        bool addConnectionStringParameter;
        bool saveToConfigurationFile = true;
        string saveToConfigurationCaption = SR.DefaultSaveToConfigurationCaption;
        string connectionStringName;
        Type modelType;
        AssemblyName localAssemblyName;
        TemplateOption selectedFrameworkVersion;
        DataSource selectedDataSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextItemTemplateWizardViewModel"/> class.
        /// </summary>
        public DbContextItemTemplateWizardViewModel()
        {
            Title = SR.DbContextItemTemplateWizardTitle;
            interactionRequests.Add( close );
            interactionRequests.Add( browse );
            interactionRequests.Add( addDataConnection );
            dialogCommands.Add( new NamedCommand<object>( "0", SR.BackCaption, OnGoBack, OnCanGoBack ) );
            dialogCommands.Add( new NamedCommand<object>( "1", SR.NextCaption, OnGoForward, OnCanGoForward ) );
            dialogCommands.Add( new NamedCommand<object>( "2", SR.CancelCaption, OnCancel ) );
            commands.Add( new NamedCommand<object>( "BrowseForModel", OnBrowseForModel ) );
            commands.Add( new NamedCommand<object>( "AddDataConnection", OnAddDataConnection ) );
            frameworkVersions.CollectionChanged += ( s, e ) => OnPropertyChanged( "CanSelectEntityFrameworkVersion" );

            INotifyPropertyChanged optionChanged = implementedInterfaces;
            optionChanged.PropertyChanged += OnInterfaceOptionChanged;
            Validate();
        }

        /// <summary>
        /// Gets the collection of view model interaction requests.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="IInteractionRequest">interaction requests</see>.</value>
        public ObservableKeyedCollection<string, IInteractionRequest> InteractionRequests
        {
            get
            {
                Contract.Ensures( interactionRequests != null );
                return interactionRequests;
            }
        }

        /// <summary>
        /// Gets the collection of view model commands.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="INamedCommand">commands</see>.</value>
        /// <remarks>These commands do not control the behavior of a view.</remarks>
        public ObservableKeyedCollection<string, INamedCommand> Commands
        {
            get
            {
                Contract.Ensures( commands != null );
                return commands;
            }
        }

        /// <summary>
        /// Gets the collection of view model dialog commands.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="INamedCommand">commands</see>.</value>
        /// <remarks>These commands control the behavior of a view, such as cancelling it.</remarks>
        public ObservableKeyedCollection<string, INamedCommand> DialogCommands
        {
            get
            {
                Contract.Ensures( dialogCommands != null );
                return dialogCommands;
            }
        }

        /// <summary>
        /// Gets or sets an associated title.
        /// </summary>
        /// <value>The associated title.</value>
        public string Title
        {
            get
            {
                Contract.Ensures( title != null );
                return title ?? ( title = string.Empty );
            }
            set => SetProperty( ref title, value );
        }

        /// <summary>
        /// Gets or sets the current step in the view model.
        /// </summary>
        /// <value>The zero-based current step in the view model. The default value is zero.</value>
        public int CurrentStep
        {
            get
            {
                Contract.Ensures( currentStep >= 0 );
                return currentStep;
            }
            set
            {
                Arg.GreaterThanOrEqualTo( value, 0, nameof( value ) );

                if ( SetProperty( ref currentStep, value ) )
                {
                    UpdateCommands();
                }
            }
        }

        /// <summary>
        /// Gets or sets the local assembly name used to filter types by.
        /// </summary>
        /// <value>The local <see cref="AssemblyName">assembly name</see>.</value>
        public AssemblyName LocalAssemblyName
        {
            get => localAssemblyName;
            set => SetProperty( ref localAssemblyName, value );
        }

        /// <summary>
        /// Gets or sets a value indicating whether tips are shown in generated code.
        /// </summary>
        /// <value>True if tips are shown in generated code; otherwise, false.</value>
        public bool ShowTips
        {
            get => showTips;
            set => SetProperty( ref showTips, value );
        }

        /// <summary>
        /// Gets or sets a value indicating whether generated code adds a connection string parameter.
        /// </summary>
        /// <value>True if the generated code will add a connection string parameter; otherwise, false.</value>
        public bool AddConnectionStringParameter
        {
            get => addConnectionStringParameter;
            set => SetProperty( ref addConnectionStringParameter, value );
        }

        /// <summary>
        /// Gets or sets the type of data model to use.
        /// </summary>
        /// <value>The <see cref="Type">type</see> of data model to use.</value>
        public Type ModelType
        {
            get => modelType;
            set
            {
                if ( SetProperty( ref modelType, value ) )
                {
                    UpdateCommands();
                }
            }
        }

        /// <summary>
        /// Gets the available interfaces to implement options.
        /// </summary>
        /// <value>A <see cref="ObservableCollection{T}">collection</see> of interface <see cref="TemplateOption">template options</see>.</value>
        public ObservableCollection<TemplateOption> ImplementedInterfaces
        {
            get
            {
                Contract.Ensures( implementedInterfaces != null );
                return implementedInterfaces;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a version of the Entity Framework can be selected.
        /// </summary>
        /// <value>True if an instance of the Entity Framework can be selected; otherwise, false.</value>
        /// <remarks>The must be at least two versions of the Entity Framework available for a
        /// selection to be available.</remarks>
        public bool CanSelectEntityFrameworkVersion => EntityFrameworkVersions.Count > 1;

        /// <summary>
        /// Gets or sets the selected Entity Framework version.
        /// </summary>
        /// <value>The selected Entity Framework version.</value>
        public TemplateOption SelectedEntityFrameworkVersion
        {
            get => selectedFrameworkVersion;
            set
            {
                if ( SetProperty( ref selectedFrameworkVersion, value ) )
                {
                    UpdateCommands();
                }
            }
        }

        /// <summary>
        /// Gets the available Entity Framework versions.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}">collection</see> of <see cref="TemplateOption">options</see> containing Entity Framework versions.</value>
        public ObservableCollection<TemplateOption> EntityFrameworkVersions
        {
            get
            {
                Contract.Ensures( frameworkVersions != null );
                return frameworkVersions;
            }
        }

        /// <summary>
        /// Gets or sets the selected data source.
        /// </summary>
        /// <value>The selected <see cref="DataSource">data source</see>.</value>
        public DataSource SelectedDataSource
        {
            get => selectedDataSource;
            set
            {
                if ( SetProperty( ref selectedDataSource, value ) )
                {
                    UpdateCommands();
                }
            }
        }

        /// <summary>
        /// Gets a collection of available data sources.
        /// </summary>
        /// <value>A <see cref="ObservableCollection{T}">collection</see> of interface <see cref="DataSource">data sources</see>.</value>
        public ObservableCollection<DataSource> DataSources
        {
            get
            {
                Contract.Ensures( dataSources != null );
                return dataSources;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected data source is saved to the configuration file.
        /// </summary>
        /// <value>True if the <see cref="SelectedDataSource">selected data source</see> is saved to the configuration file; otherwise, false.</value>
        public bool SaveToConfigurationFile
        {
            get => saveToConfigurationFile;
            set
            {
                if ( !SetProperty( ref saveToConfigurationFile, value ) )
                {
                    return;
                }

                ValidateProperty( ConnectionStringName, nameof( ConnectionStringName ) );
                UpdateCommands();
            }
        }

        /// <summary>
        /// Gets or sets the caption text for the save to configuration file option.
        /// </summary>
        /// <value>The caption text for the <see cref="SaveToConfigurationFile">save to configuration file</see> option.</value>
        public string SaveToConfigurationCaption
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( saveToConfigurationCaption ) );
                return saveToConfigurationCaption;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                SetProperty( ref saveToConfigurationCaption, value );
            }
        }

        /// <summary>
        /// Gets or sets the name associated with the connection string for the selected data source.
        /// </summary>
        /// <value>The name of the connection string associated with the <see cref="SelectedDataSource">selected data source</see>.</value>
        [CustomValidation( typeof( DbContextItemTemplateWizardViewModel ), nameof( ValidateConnectionStringName ) )]
        public string ConnectionStringName
        {
            get => connectionStringName;
            set
            {
                if ( SetProperty( ref connectionStringName, value ) )
                {
                    UpdateCommands();
                }
            }
        }

        /// <summary>
        /// Validates the specified connection string key using the provided validation context.
        /// </summary>
        /// <param name="value">The connection string key to validate.</param>
        /// <param name="context">The <see cref="ValidationContext">context</see> in which to perform the validation.</param>
        /// <returns>The <see cref="ValidationResult">validation result</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static ValidationResult ValidateConnectionStringName( string value, ValidationContext context )
        {
            Arg.NotNull( context, nameof( context ) );

            var model = context.ObjectInstance as DbContextItemTemplateWizardViewModel;

            if ( model == null )
            {
                return new ValidationResult( SR.InvalidObjectForValidation.FormatDefault( typeof( DbContextItemTemplateWizardViewModel ) ) );
            }

            if ( !model.SaveToConfigurationFile )
            {
                return ValidationResult.Success;
            }

            if ( string.IsNullOrEmpty( value ) )
            {
                return new ValidationResult( SR.ConnectionStringNameUnspecified, new[] { nameof( ConnectionStringName ) } );
            }

            var validator = context.GetService<IProjectItemNameValidator>();

            if ( validator != null && !validator.IsConnectionStringNameUnique( value ) )
            {
                return new ValidationResult( SR.ConnectionStringNameNonUnique.FormatDefault( value ), new[] { nameof( ConnectionStringName ) } );
            }

            return ValidationResult.Success;
        }

        void UpdateCommands()
        {
            Commands.RaiseCanExecuteChanged();
            DialogCommands.RaiseCanExecuteChanged();
        }

        void OnInterfaceOptionChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( e.PropertyName == nameof( RequiredTemplateOptionCollection.AtLeastOneItemEnabled ) || string.IsNullOrEmpty( e.PropertyName ) )
            {
                UpdateCommands();
            }
        }

        void OnBrowseForModel( object parameter )
        {
            var interaction = new Interaction()
            {
                Title = SR.ExistingModelTitle,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( SR.OKCaption, DefaultAction.None ),
                    new NamedCommand<object>( SR.CancelCaption, p => ModelType = null )
                }
            };

            browse.Request( interaction );
        }

        void OnAddDataConnection( object parameter )
        {
            var interaction = new Interaction()
            {
                Title = SR.AddDataConnectionTitle,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<DataSource>( SR.OKCaption, OnDataSourceAdded ),
                    new NamedCommand<object>( SR.CancelCaption, DefaultAction.None )
                }
            };

            addDataConnection.Request( interaction );
        }

        void OnDataSourceAdded( DataSource dataSource )
        {
            DataSources.Add( dataSource );
            SelectedDataSource = dataSource;
        }

        bool OnCanGoBack( object parameter ) => CurrentStep > 0;

        void OnGoBack( object parameter )
        {
            if ( !OnCanGoBack( parameter ) )
            {
                return;
            }

            --CurrentStep;
            dialogCommands[1].Name = SR.NextCaption;
        }

        bool OnCanGoForward( object parameter )
        {
            switch ( CurrentStep )
            {
                case 0:
                    return ModelType != null && implementedInterfaces.AtLeastOneItemEnabled;
                case 1:
                    return !SaveToConfigurationFile || SelectedDataSource != null;
                default:
                    return false;
            }
        }

        void OnGoForward( object parameter )
        {
            if ( !OnCanGoForward( parameter ) )
            {
                return;
            }

            switch ( CurrentStep )
            {
                case 0:
                    CurrentStep = 1;
                    dialogCommands[1].Name = SR.FinishCaption;
                    break;
                case 1:
                    close.Request( new WindowCloseInteraction() );
                    break;
            }
        }

        void OnCancel( object parameter ) => close.Request( new WindowCloseInteraction( true ) );
    }
}