namespace More.VisualStudio.ViewModels
{
    using More.Collections.Generic;
    using More.ComponentModel;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Represents the view model for a project template wizard.
    /// </summary>
    public class ProjectTemplateWizardViewModel : ValidatableObject
    {
        private readonly InteractionRequest<WindowCloseInteraction> close = new InteractionRequest<WindowCloseInteraction>( "CloseWindow" );
        private readonly ObservableKeyedCollection<string, IInteractionRequest> interactionRequests = new ObservableKeyedCollection<string, IInteractionRequest>( r => r.Id );
        private readonly ObservableKeyedCollection<string, INamedCommand> commands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        private readonly ObservableKeyedCollection<string, INamedCommand> dialogCommands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );
        private readonly ObservableCollection<TemplateOption> interactions = new ObservableCollection<TemplateOption>();
        private readonly ObservableCollection<TemplateOption> appContracts = new ObservableCollection<TemplateOption>();
        private int currentStep;
        private bool showTips = true;
        private string title = SR.ProjectTemplateWizardTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTemplateWizardViewModel"/> class.
        /// </summary>
        public ProjectTemplateWizardViewModel()
        {
            this.interactionRequests.Add( this.close );
            this.dialogCommands.Add( new NamedCommand<object>( "0", SR.BackCaption, this.OnGoBack, this.OnCanGoBack ) );
            this.dialogCommands.Add( new NamedCommand<object>( "1", SR.FinishCaption, this.OnGoForward, this.OnCanGoForward ) );
            this.dialogCommands.Add( new NamedCommand<object>( "2", SR.CancelCaption, this.OnCancel ) );
            this.interactions.CollectionChanged += this.OnOptionsChanged;
            this.appContracts.CollectionChanged += this.OnOptionsChanged;
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

        private void UpdateCommands()
        {
            this.Commands.RaiseCanExecuteChanged();
            this.DialogCommands.RaiseCanExecuteChanged();
        }

        private void OnOptionsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if ( this.CurrentStep != 0 )
                return;

            var text = this.InteractionOptions.Any() || this.ApplicationContractOptions.Any() ? SR.NextCaption : SR.FinishCaption;
            this.dialogCommands[1].Name = text;
        }

        private bool OnCanGoBack( object parameter )
        {
            return this.CurrentStep > 0;
        }

        private void OnGoBack( object parameter )
        {
            if ( !this.OnCanGoBack( parameter ) )
                return;

            if ( --this.CurrentStep == 0 )
            {
                var text = this.InteractionOptions.Any() || this.ApplicationContractOptions.Any() ? SR.NextCaption : SR.FinishCaption;
                this.dialogCommands[1].Name = text;
            }
            else
            {
                this.dialogCommands[1].Name = SR.NextCaption;
            }
        }

        private bool OnCanGoForward( object parameter )
        {
            return this.CurrentStep < 3;
        }

        private void OnGoForward( object parameter )
        {
            if ( !this.OnCanGoForward( parameter ) )
                return;

            switch ( this.CurrentStep )
            {
                case 0: // initial
                    {
                        if ( this.InteractionOptions.Any() )
                        {
                            this.CurrentStep = 1;

                            // the next step will be the last step
                            if ( !this.ApplicationContractOptions.Any() )
                                this.dialogCommands[1].Name = SR.FinishCaption;
                        }
                        else if ( this.ApplicationContractOptions.Any() )
                        {
                            this.CurrentStep = 2;
                            this.dialogCommands[1].Name = SR.FinishCaption;
                        }
                        else
                        {
                            this.close.Request( new WindowCloseInteraction() );
                        }
                        break;
                    }
                case 1: // interaction options
                    {
                        if ( this.ApplicationContractOptions.Any() )
                        {
                            this.CurrentStep = 2;
                            this.dialogCommands[1].Name = SR.FinishCaption;
                        }
                        else
                        {
                            this.close.Request( new WindowCloseInteraction() );
                        }
                        break;
                    }
                case 2: // app contract options
                    {
                        this.close.Request( new WindowCloseInteraction() );
                        break;
                    }
            }
        }

        private void OnCancel( object parameter )
        {
            this.close.Request( new WindowCloseInteraction( true ) );
        }
    }
}
