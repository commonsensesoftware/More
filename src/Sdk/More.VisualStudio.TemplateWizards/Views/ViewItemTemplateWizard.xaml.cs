namespace More.VisualStudio.Views
{
    using More.VisualStudio.ViewModels;
    using More.Windows.Input;
    using System.Diagnostics.Contracts;
    using System.Windows;

    /// <summary>
    /// Represents the view for a view item template wizard.
    /// </summary>
    public partial class ViewItemTemplateWizard : Window
    {
        readonly ProjectInformation projectInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewItemTemplateWizard"/> class.
        /// </summary>
        public ViewItemTemplateWizard() => InitializeComponent();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewItemTemplateWizard"/> class.
        /// </summary>
        /// <param name="model">The <see cref="ViewItemTemplateWizardViewModel">model</see> for the view.</param>
        /// <param name="projectInformation">The source <see cref="ProjectInformation">project information</see> used for browsing existing types.</param>
        public ViewItemTemplateWizard( ViewItemTemplateWizardViewModel model, ProjectInformation projectInformation ) : this()
        {
            Model = model;
            projectInfo = projectInformation;
        }

        /// <summary>
        /// Gets or sets the model associated with the view.
        /// </summary>
        /// <value>The associated <see cref="ViewItemTemplateWizardViewModel">model</see>.</value>
        public ViewItemTemplateWizardViewModel Model
        {
            get => DataContext as ViewItemTemplateWizardViewModel;
            set
            {
                var oldValue = Model;

                if ( oldValue != null )
                {
                    oldValue.InteractionRequests["BrowseForViewModel"].Requested -= OnBrowseForViewModel;
                }

                if ( ( DataContext = value ) != null )
                {
                    value.InteractionRequests["BrowseForViewModel"].Requested += OnBrowseForViewModel;
                }
            }
        }

        async void OnBrowseForViewModel( object sender, InteractionRequestedEventArgs e )
        {
            Contract.Requires( e != null );

            var picker = new TypePicker()
            {
                Title = e.Interaction.Title,
                NameConvention = "ViewModel",
                LocalAssemblyName = Model.LocalAssemblyName,
                SourceProject = projectInfo,
                RestrictedBaseTypeNames =
                {
                    "System.Windows.DependencyObject"
                }
            };

            if ( await picker.ShowDialogAsync( this ) ?? false )
            {
                Model.ViewModelType = picker.SelectedType;
                e.Interaction.ExecuteDefaultCommand();
            }
            else
            {
                e.Interaction.ExecuteCancelCommand();
            }
        }
    }
}