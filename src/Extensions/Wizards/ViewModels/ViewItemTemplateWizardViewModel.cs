namespace More.VisualStudio.ViewModels
{
    using More.Collections.Generic;
    using More.ComponentModel;
    using More.Windows.Input;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents the view model for the view item template wizard.
    /// </summary>
    public class ViewItemTemplateWizardViewModel : ValidatableObject
    {
        private readonly InteractionRequest<WindowCloseInteraction> close = new InteractionRequest<WindowCloseInteraction>( "CloseWindow" );
        private readonly InteractionRequest<Interaction> browse = new InteractionRequest<Interaction>( "BrowseForViewModel" );
        private readonly ObservableKeyedCollection<string, IInteractionRequest> interactionRequests = new ObservableKeyedCollection<string, IInteractionRequest>( r => r.Id );
        private readonly ObservableKeyedCollection<string, INamedCommand> commands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        private readonly ObservableKeyedCollection<string, INamedCommand> dialogCommands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        private readonly ObservableCollection<TemplateOption> viewOptions = new ObservableCollection<TemplateOption>();
        private readonly ObservableCollection<TemplateOption> interactions = new ObservableCollection<TemplateOption>();
        private readonly ObservableCollection<TemplateOption> appContracts = new ObservableCollection<TemplateOption>();
        private int currentStep;
        private bool topLevel;
        private bool topLevelSupported;
        private bool showTips = true;
        private int interfaceOption;
        private int viewModelOption = 1;
        private string title = SR.ViewItemTemplateWizardTitle;
        private string viewModelName;
        private Type viewModelType;
        private AssemblyName localAssemblyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewItemTemplateWizardViewModel"/> class.
        /// </summary>
        public ViewItemTemplateWizardViewModel()
        {
            this.interactionRequests.Add( this.close );
            this.interactionRequests.Add( this.browse );
            this.dialogCommands.Add( new NamedCommand<object>( "0", SR.BackCaption, this.OnGoBack, this.OnCanGoBack ) );
            this.dialogCommands.Add( new NamedCommand<object>( "1", SR.NextCaption, this.OnGoForward, this.OnCanGoForward ) );
            this.dialogCommands.Add( new NamedCommand<object>( "2", SR.CancelCaption, this.OnCancel ) );
            this.commands.Add( new NamedCommand<object>( "BrowseForViewModel", this.OnBrowseForViewModel, this.OnCanBrowseForViewModel ) );
            this.InitializeValidationErrors();
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
                Contract.Ensures( this.interactionRequests != null );
                return this.interactionRequests;
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
                Contract.Ensures( this.commands != null );
                return this.commands;
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
                Contract.Ensures( this.dialogCommands != null );
                return this.dialogCommands;
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
                Contract.Ensures( this.title != null );
                return this.title ?? ( this.title = string.Empty );
            }
            set
            {
                this.SetProperty( ref this.title, value );
            }
        }

        /// <summary>
        /// Gets or sets the current step in the view model.
        /// </summary>
        /// <value>The zero-based current step in the view model. The default value is zero.</value>
        public int CurrentStep
        {
            get
            {
                Contract.Ensures( this.currentStep >= 0 );
                return this.currentStep;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>( value >= 0, "value" );

                if ( this.SetProperty( ref this.currentStep, value ) )
                    this.UpdateCommands();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tips are shown in generated code.
        /// </summary>
        /// <value>True if tips are shown in generated code; otherwise, false.</value>
        public bool ShowTips
        {
            get
            {
                return this.showTips;
            }
            set
            {
                this.SetProperty( ref this.showTips, value );
            }
        }

        /// <summary>
        /// Gets a value indicating whether a view model is required.
        /// </summary>
        /// <value>True if a view model is required; otherwise, false.</value>
        public bool RequiresViewModel
        {
            get
            {
                return this.ViewInterfaceOption > 0;
            }
        }

        /// <summary>
        /// Gets or sets the local assembly name used to filter types by.
        /// </summary>
        /// <value>The local <see cref="AssemblyName">assembly name</see>.</value>
        public AssemblyName LocalAssemblyName
        {
            get
            {
                return this.localAssemblyName;
            }
            set
            {
                this.SetProperty( ref this.localAssemblyName, value );
            }
        }

        /// <summary>
        /// Gets or sets the view interface implementation option.
        /// </summary>
        /// <value>The zero-based view model generation option.</value>
        /// <remarks>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Option</term>
        ///         <description>Meaning</description>
        ///     </listheader>
        ///     <item>
        ///         <term>0</term>
        ///         <description>Indicates a basic view without a specified view model.</description>
        ///     </item>
        ///     <item>
        ///         <term>1</term>
        ///         <description>Indicates the view exposes a view model</description>
        ///     </item>
        ///     <item>
        ///         <term>2</term>
        ///         <description>Indicates the view exposes a view model that can be attached at runtime.</description>
        ///     </item>
        ///     <item>
        ///         <term>3</term>
        ///         <description>Indicates the view supports dialog semantics and exposes a view model that can be attached at runtime.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public int ViewInterfaceOption
        {
            get
            {
                Contract.Ensures( this.interfaceOption >= 0 );
                return this.interfaceOption;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>( value >= 0, "value" );

                if ( !this.SetProperty( ref this.interfaceOption, value ) )
                    return;

                // if an interface is implemented, then ensure a view model is used
                if ( value > 0 && this.ViewModelOption == 0 )
                    this.ViewModelOption = 1;

                this.OnPropertyChanged( "RequiresViewModel" );
            }
        }

        /// <summary>
        /// Gets the view template options.
        /// </summary>
        /// <value>A <see cref="ObservableCollection{T}">collection</see> of view <see cref="TemplateOption">template options</see>.</value>
        public ObservableCollection<TemplateOption> ViewOptions
        {
            get
            {
                Contract.Ensures( this.viewOptions != null );
                return this.viewOptions;
            }
        }

        /// <summary>
        /// Gets or sets the view model option.
        /// </summary>
        /// <value>The zero-based view model generation option.  The default value is zero.</value>
        /// <remarks>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Option</term>
        ///         <description>Meaning</description>
        ///     </listheader>
        ///     <item>
        ///         <term>0</term>
        ///         <description>Indicates that no view model will be generated</description>
        ///     </item>
        ///     <item>
        ///         <term>1</term>
        ///         <description>Indicates a new view model will be generated from an item template</description>
        ///     </item>
        ///     <item>
        ///         <term>2</term>
        ///         <description>Indicates an existing view model will be used</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public int ViewModelOption
        {
            get
            {
                Contract.Ensures( this.viewModelOption >= 0 );
                return this.viewModelOption;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>( value >= 0, "value" );

                if ( !this.SetProperty( ref this.viewModelOption, value ) )
                    return;

                this.ViewModelName = null;
                this.ViewModelType = null;
                this.UpdateCommands();
            }
        }

        /// <summary>
        /// Gets or sets the name of the generated view model.
        /// </summary>
        /// <value>The name of the generated view model.</value>
        /// <remarks>The value of this property will depend on the <see cref="P:ViewModelOption"/>.  This property
        /// is only used in code generation when a new view model is created.  In all other scenarios, the value
        /// of this property is for information purposes.</remarks>
        [CustomValidation( typeof( ViewItemTemplateWizardViewModel ), "ValidateViewModelName" )]
        public string ViewModelName
        {
            get
            {
                return this.viewModelName;
            }
            set
            {
                if ( this.SetProperty( ref this.viewModelName, value, NullStringEqualityComparer.Default ) )
                    this.UpdateCommands();
            }
        }

        /// <summary>
        /// Gets or sets the type of view model to use.
        /// </summary>
        /// <value>The <see cref="Type">type</see> of view model to use.</value>
        /// <remarks>The value of this property will depend on the <see cref="P:ViewModelOption"/>.  This property
        /// is only used in code generation when an existing view model is used.</remarks>
        public Type ViewModelType
        {
            get
            {
                return this.viewModelType;
            }
            set
            {
                if ( !this.SetProperty( ref this.viewModelType, value ) )
                    return;

                this.ViewModelName = value == null ? null : value.Name;
                this.UpdateCommands();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view is a top-level view.
        /// </summary>
        /// <value>True if the view is a top-level view; otherwise, false.</value>
        public bool IsTopLevel
        {
            get
            {
                return this.topLevel;
            }
            set
            {
                this.SetProperty( ref this.topLevel, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the a top-level view is supported by a template.
        /// </summary>
        /// <value>True if a top-level view is supported by a template; otherwise, false.</value>
        public bool IsTopLevelSupported
        {
            get
            {
                return this.topLevelSupported;
            }
            set
            {
                this.SetProperty( ref this.topLevelSupported, value );
            }
        }

        /// <summary>
        /// Gets the available user interactions for the view model.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}">collection</see> of <see cref="TemplateOption">options</see> containing available user interactions.</value>
        public ObservableCollection<TemplateOption> InteractionOptions
        {
            get
            {
                Contract.Ensures( this.interactions != null );
                return this.interactions;
            }
        }

        /// <summary>
        /// Gets the available Windows contracts for the view model.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}">collection</see> of <see cref="TemplateOption">options</see> containing available Windows contracts.</value>
        /// <remarks>This property only applies to templates for platforms that support Windows contracts (aka "charms").</remarks>
        public ObservableCollection<TemplateOption> ApplicationContractOptions
        {
            get
            {
                Contract.Ensures( this.appContracts != null );
                return this.appContracts;
            }
        }

        /// <summary>
        /// Validates the specified view model name using the provided validation context.
        /// </summary>
        /// <param name="value">The view model name to validate.</param>
        /// <param name="context">The <see cref="ValidationContext">context</see> in which to perform the validation.</param>
        /// <returns>The <see cref="ValidationResult">validation result</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static ValidationResult ValidateViewModelName( string value, ValidationContext context )
        {
            Contract.Requires<ArgumentNullException>( context != null, "context" );

            var model = context.ObjectInstance as ViewItemTemplateWizardViewModel;

            if ( model == null )
                return new ValidationResult( SR.InvalidObjectForValidation.FormatDefault( typeof( ViewItemTemplateWizardViewModel ) ) );

            switch ( model.ViewModelOption )
            {
                case 1:
                    {
                        // must have the name of the new view model
                        if ( string.IsNullOrEmpty( value ) )
                            return new ValidationResult( SR.ViewModelNameUnspecified, new[] { "ViewModelName" } );

                        var validator = context.GetService<IProjectItemNameValidator>();

                        // the specified view model name must be unique
                        if ( validator != null && !validator.IsItemNameUnique( value ) )
                            return new ValidationResult( SR.ViewModelNameNonUnique.FormatDefault( value ), new[] { "ViewModelName" } );

                        break;
                    }
                case 2:
                    {
                        // must have a selected view model
                        if ( string.IsNullOrEmpty( value ) )
                            return new ValidationResult( SR.ExistingViewModelUnspecified, new[] { "ViewModelName" } );

                        break;
                    }
            }

            return ValidationResult.Success;
        }

        private void UpdateCommands()
        {
            this.Commands.RaiseCanExecuteChanged();
            this.DialogCommands.RaiseCanExecuteChanged();
        }

        private bool OnCanGoBack( object parameter )
        {
            return this.CurrentStep > 0;
        }

        private void OnGoBack( object parameter )
        {
            if ( !this.OnCanGoBack( parameter ) )
                return;

            --this.CurrentStep;
            this.dialogCommands[1].Name = SR.NextCaption;
        }

        private bool OnCanGoForward( object parameter )
        {
            switch ( this.CurrentStep )
            {
                case 0: // initial
                case 2: // interaction options
                case 3: // app contract options
                    {
                        // can always go next from steps without validation
                        return true;
                    }
                case 1: // view model options
                    {
                        switch ( this.ViewModelOption )
                        {
                            case 0:
                                // no validation when there's no view model
                                return true;
                            case 1:
                                // must have a valid view model name
                                return this.IsPropertyValid( this.ViewModelName, "ViewModelName" );
                            case 2:
                                // must have a selected view model
                                return this.viewModelType != null;
                            default:
                                // unknown option
                                return false;
                        }
                    }
                default:
                    {
                        // unknown step
                        return false;
                    }
            }
        }

        private void OnGoForward( object parameter )
        {
            if ( !this.OnCanGoForward( parameter ) )
                return;

            switch ( this.CurrentStep )
            {
                case 0: // initial
                    {
                        this.CurrentStep = 1;

                        // the next step will be the last step
                        if ( !this.InteractionOptions.Any() && !this.ApplicationContractOptions.Any() )
                            this.dialogCommands[1].Name = SR.FinishCaption;

                        break;
                    }
                case 1: // view model options
                    {
                        if ( this.InteractionOptions.Any() )
                        {
                            this.CurrentStep = 2;

                            // the next step will be the last step
                            if ( !this.ApplicationContractOptions.Any() )
                                this.dialogCommands[1].Name = SR.FinishCaption;
                        }
                        else if ( this.ApplicationContractOptions.Any() )
                        {
                            this.CurrentStep = 3;
                            this.dialogCommands[1].Name = SR.FinishCaption;
                        }
                        else
                        {
                            this.close.Request( new WindowCloseInteraction() );
                        }
                        break;
                    }
                case 2: // interaction options
                    {
                        if ( this.ApplicationContractOptions.Any() )
                        {
                            this.CurrentStep = 3;
                            this.dialogCommands[1].Name = SR.FinishCaption;
                        }
                        else
                        {
                            this.close.Request( new WindowCloseInteraction() );
                        }
                        break;
                    }
                case 3: // app contract options
                    {
                        this.close.Request( new WindowCloseInteraction() );
                        break;
                    }
            }
        }

        private bool OnCanBrowseForViewModel( object parameter )
        {
            return this.CurrentStep == 1 && this.ViewModelOption == 2;
        }

        private void OnBrowseForViewModel( object parameter )
        {
            if ( !this.OnCanBrowseForViewModel( parameter ) )
                return;

            var interaction = new Interaction()
            {
                Title = SR.ExistingViewModelTitle,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( SR.OKCaption, DefaultAction.None ),
                    new NamedCommand<object>( SR.CancelCaption, p => this.ViewModelType = null )
                }
            };

            // request an ui interaction to browse for a type
            this.browse.Request( interaction );
        }

        private void OnCancel( object parameter )
        {
            this.close.Request( new WindowCloseInteraction( true ) );
        }
    }
}
