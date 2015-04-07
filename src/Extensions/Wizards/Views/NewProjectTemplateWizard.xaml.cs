namespace More.VisualStudio.Views
{
    using More.VisualStudio.ViewModels;
    using System;
    using System.Windows;

    /// <summary>
    /// Represents the view for a project template wizard.
    /// </summary>
    public partial class NewProjectTemplateWizard : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectTemplateWizard"/> class.
        /// </summary>
        public NewProjectTemplateWizard()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewProjectTemplateWizard"/> class.
        /// </summary>
        /// <param name="model">The <see cref="ProjectTemplateWizardViewModel">model</see> for the view.</param>
        public NewProjectTemplateWizard( ProjectTemplateWizardViewModel model )
            : this()
        {
            this.DataContext = model;
        }

        /// <summary>
        /// Gets or sets the model associated with the view.
        /// </summary>
        /// <value>The associated <see cref="ProjectTemplateWizardViewModel">model</see>.</value>
        public ProjectTemplateWizardViewModel Model
        {
            get
            {
                return this.DataContext as ProjectTemplateWizardViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
