namespace More.VisualStudio.Views
{
    using More.VisualStudio.ViewModels;
    using System.Windows;

    /// <summary>
    /// Represents the view for a project template wizard.
    /// </summary>
    public partial class NewProjectTemplateWizard : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectTemplateWizard"/> class.
        /// </summary>
        public NewProjectTemplateWizard() => InitializeComponent();

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectTemplateWizard"/> class.
        /// </summary>
        /// <param name="model">The <see cref="ProjectTemplateWizardViewModel">model</see> for the view.</param>
        public NewProjectTemplateWizard( ProjectTemplateWizardViewModel model ) : this() => DataContext = model;

        /// <summary>
        /// Gets or sets the model associated with the view.
        /// </summary>
        /// <value>The associated <see cref="ProjectTemplateWizardViewModel">model</see>.</value>
        public ProjectTemplateWizardViewModel Model
        {
            get => DataContext as ProjectTemplateWizardViewModel;
            set => DataContext = value;
        }
    }
}