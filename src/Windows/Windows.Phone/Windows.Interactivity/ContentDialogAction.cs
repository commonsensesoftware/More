namespace More.Windows.Interactivity
{
    using More.Composition;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to show the <see cref="Interaction">interaction</see>
    /// from an <see cref="E:IInteractionRequest.Requested">interaction request</see> in a <see cref="ContentDialog">view</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class ContentDialogAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Represents a mediator to coordinate button clicks and provided commands.
        /// </summary>
        /// <remarks>This class uses the button click handlers instead of directly binding to the commands
        /// because the commands could perform asynchronous actions (and there's no way to know for sure).
        /// If the actions are asynchronous, a deferral is needed and there is no clear way to get or
        /// supply the deferral to the commands.</remarks>
        private sealed class CommandMediator : IDisposable
        {
            private readonly ContentDialog dialog;
            private readonly INamedCommand primary;
            private readonly INamedCommand secondary;
            private ContentDialogButtonClickDeferral deferral;

            internal CommandMediator( ContentDialog dialog, INamedCommand primary, INamedCommand secondary )
            {
                Contract.Requires( dialog != null );
                this.dialog = dialog;
                this.primary = this.WirePrimaryCommand( primary );
                this.secondary = this.WirePrimaryCommand( secondary );
            }

            private INamedCommand WirePrimaryCommand( INamedCommand command )
            {
                if ( !( this.dialog.IsPrimaryButtonEnabled = ( command != null ) ) )
                    return command;

                this.dialog.PrimaryButtonText = command.Name;
                this.dialog.PrimaryButtonClick += this.OnPrimaryClick;

                command.Executed += this.OnCommandExecuted;
                command.CanExecuteChanged += this.OnPrimaryCanExecuteChanged;

                return command;
            }

            private INamedCommand WireSecondaryCommand( INamedCommand command )
            {
                if ( !( this.dialog.IsSecondaryButtonEnabled = ( command != null ) ) )
                    return command;

                this.dialog.SecondaryButtonText = command.Name;
                this.dialog.SecondaryButtonClick += this.OnPrimaryClick;

                command.Executed += this.OnCommandExecuted;
                command.CanExecuteChanged += this.OnSecondaryCanExecuteChanged;

                return command;
            }

            private void UnwireCommands()
            {
                if ( this.dialog != null )
                {
                    this.dialog.PrimaryButtonClick -= this.OnPrimaryClick;
                    this.dialog.SecondaryButtonClick -= this.OnSecondaryClick;
                }

                if ( this.primary != null )
                {
                    this.primary.Executed -= this.OnCommandExecuted;
                    this.primary.CanExecuteChanged -= this.OnPrimaryCanExecuteChanged;
                }

                if ( this.secondary != null )
                {
                    this.secondary.Executed -= this.OnCommandExecuted;
                    this.secondary.CanExecuteChanged -= this.OnSecondaryCanExecuteChanged;
                }
            }

            private void OnPrimaryClick( object sender, ContentDialogButtonClickEventArgs e )
            {
                Contract.Requires( e != null );

                if ( this.primary == null )
                    return;

                var parameter = this.dialog.PrimaryButtonCommandParameter;
                this.deferral = e.GetDeferral();

                if ( this.secondary != null )
                    this.secondary.RaiseCanExecuteChanged();

                this.primary.RaiseCanExecuteChanged();

                if ( this.primary.CanExecute( parameter ) )
                    this.primary.Execute( parameter );
            }

            private void OnPrimaryCanExecuteChanged( object sender, EventArgs e )
            {
                var parameter = this.dialog.PrimaryButtonCommandParameter;
                var enabled = this.deferral == null && this.primary != null && this.primary.CanExecute( parameter );
                this.dialog.IsPrimaryButtonEnabled = enabled;
            }

            private void OnSecondaryClick( object sender, ContentDialogButtonClickEventArgs e )
            {
                Contract.Requires( e != null );

                if ( this.secondary == null )
                    return;

                var parameter = this.dialog.SecondaryButtonCommandParameter;

                this.deferral = e.GetDeferral();

                if ( this.primary != null )
                    this.primary.RaiseCanExecuteChanged();

                this.secondary.RaiseCanExecuteChanged();

                if ( this.secondary.CanExecute( parameter ) )
                    this.secondary.Execute( parameter );
            }

            private void OnSecondaryCanExecuteChanged( object sender, EventArgs e )
            {
                var paramter = this.dialog.SecondaryButtonCommandParameter;
                var enabled = this.deferral == null && this.secondary != null && this.secondary.CanExecute( paramter );
                this.dialog.IsSecondaryButtonEnabled = enabled;
            }

            private void OnCommandExecuted( object sender, EventArgs e )
            {
                this.deferral.Complete();
                this.UnwireCommands();
                this.dialog.Hide();
            }

            public void Dispose()
            {
                if ( this.deferral != null )
                {
                    this.deferral.Complete();
                    this.deferral = null;
                }

                this.UnwireCommands();

                if ( this.dialog != null )
                    this.dialog.Hide();

                GC.SuppressFinalize( this );
            }
        }

        /// <summary>
        /// Gets the dependency property of the view name.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ViewNameProperty =
            DependencyProperty.Register( "ViewName", typeof( string ), typeof( ContentDialogAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property of the view <see cref="Type">type</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ViewTypeProperty =
            DependencyProperty.Register( "ViewType", typeof( Type ), typeof( ContentDialogAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property for the view <see cref="Style">style</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register( "Style", typeof( Style ), typeof( ContentDialogAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property for the view <see cref="DataTemplate">content template</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register( "ContentTemplate", typeof( DataTemplate ), typeof( ContentDialogAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property for the view <see cref="DataTemplate">title template</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register( "TitleTemplate", typeof( DataTemplate ), typeof( ContentDialogAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the name of the view the action shows.
        /// </summary>
        /// <value>The view name associated with the <see cref="ContentDialog">dialog</see> that the action shows.</value>
        public string ViewName
        {
            get
            {
                return (string) this.GetValue( ViewNameProperty );
            }
            set
            {
                this.SetValue( ViewNameProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the type of dialog the action shows.
        /// </summary>
        /// <value>The view <see cref="Type">type</see> of the <see cref="ContentDialog">dialog</see> that the action shows.</value>
        public Type ViewType
        {
            get
            {
                return (Type) this.GetValue( ViewTypeProperty );
            }
            set
            {
                this.SetValue( ViewTypeProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the dialog shown by the action.
        /// </summary>
        /// <value>The <see cref="Style">style</see> applied to the <see cref="ContentDialog">dialog</see>
        /// shown by the action.</value>
        public Style Style
        {
            get
            {
                return (Style) this.GetValue( StyleProperty );
            }
            set
            {
                this.SetValue( StyleProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the content template applied to the dialog shown by the action.
        /// </summary>
        /// <value>The <see cref="DataTemplate">content template</see> applied to the
        /// <see cref="ContentDialog">dialog</see> shown by the action.</value>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate) this.GetValue( ContentTemplateProperty );
            }
            set
            {
                this.SetValue( ContentTemplateProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the title template applied to the dialog shown by the action.
        /// </summary>
        /// <value>The <see cref="DataTemplate">title template</see> applied to the
        /// <see cref="ContentDialog">dialog</see> shown by the action.</value>
        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate) this.GetValue( TitleTemplateProperty );
            }
            set
            {
                this.SetValue( TitleTemplateProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog has custom buttons.
        /// </summary>
        /// <value>True if the dialog has custom buttons; otherwise, false. The default value is <c>false</c>.</value>
        /// <remarks>If the dialog does not have custom buttons, then the provided <see cref="P:Interaction.DefaultCommand"/>
        /// will be mapped to the <see cref="P:ContentDialog.PrimaryCommand"/> and the <see cref="P:Interaction.CancelCommand"/>
        /// will be mapped to the <see cref="P:ContentDialog.SecondaryCommand"/>.  When the dialog does not have
        /// custom buttons, then no mapping is performed and any provided <see cref="P:Interaction.Commands"/> are expected
        /// to be wired via data binding in the <see cref="ContentDialog"/> content.</remarks>
        public bool HasCustomButtons
        {
            get;
            set;
        }

        /// <summary>
        /// Creates the window used by the action.
        /// </summary>
        /// <returns>A new <see cref="ContentDialog"/> object or null if <see cref="P:ViewType"/> is null.</returns>
        /// <remarks>If the <see cref="Window">window</see> specified by the <see cref="P:ViewType">window type</see>
        /// is a composed <see cref="Window">window</see>, the default implementation will first use the
        /// <see cref="P:ServiceProvider.Currrent">service provider</see> to retrieve it.  If the view is not found,
        /// then the fallback method will be <see cref="M:Activator.CreateInstance(Type)"/>.</remarks>
        protected virtual ContentDialog CreateDialog()
        {
            Contract.Ensures( Contract.Result<ContentDialog>() != null );

            ContentDialog dialog = null;

            if ( this.ViewType == null )
            {
                dialog = new ContentDialog();
            }
            else
            {
                var services = ServiceProvider.Current;
                object view = null;

                // if the view is resolved from the service provider, it is assumed to be composed
                if ( !services.TryGetService( this.ViewType, out view, this.ViewName ) )
                {
                    // use fallback method
                    view = Activator.CreateInstance( this.ViewType );

                    ICompositionService composer;

                    // try to compose the view
                    if ( services.TryGetService( out composer ) )
                        composer.Compose( view );
                }

                if ( ( dialog = view as ContentDialog ) == null )
                {
                    dialog = new ContentDialog();
                    dialog.Content = view;
                }
            }

            return dialog;
        }

        private void PrepareDialog( ContentDialog dialog, Interaction interaction )
        {
            Contract.Requires( dialog != null );
            Contract.Requires( interaction != null );

            if ( this.ContentTemplate != null )
                dialog.ContentTemplate = this.ContentTemplate;

            if ( this.TitleTemplate != null )
                dialog.TitleTemplate = this.TitleTemplate;

            if ( this.Style != null )
                dialog.Style = this.Style;

            // use interaction if there is no content
            if ( dialog.Content == null )
                dialog.Content = interaction.Content ?? interaction;

            // dialog might already have its own view model, if it doesn't
            // use the interaction as the model
            if ( dialog.DataContext == null )
                dialog.DataContext = interaction;
        }

        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The <see cref="FrameworkElement"/> that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            var interaction = GetRequestedInteraction<Interaction>( parameter );

            if ( interaction == null )
                return;

            var dialog = this.CreateDialog();
            this.PrepareDialog( dialog, interaction );

            // if custom buttons are indicated, just show the dialog
            if ( this.HasCustomButtons )
            {
                await dialog.ShowAsync();
                return;
            }

            // mediate the standard commands and show the dialog
            using ( var mediator = new CommandMediator( dialog, interaction.DefaultCommand, interaction.CancelCommand ) )
            {
                await dialog.ShowAsync();
            }
        }
    }
}
