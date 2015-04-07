namespace More.VisualStudio.Views
{
    using More.VisualStudio.ViewModels;
    using System;
    using System.Windows;

    /// <summary>
    /// Represents the view for a view model item template wizard.
    /// </summary>
    public partial class ViewModelItemTemplateWizard : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelItemTemplateWizard"/> class.
        /// </summary>
        public ViewModelItemTemplateWizard()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelItemTemplateWizard"/> class.
        /// </summary>
        /// <param name="model">The <see cref="ViewItemTemplateWizardViewModel">model</see> for the view.</param>
        public ViewModelItemTemplateWizard( ViewModelItemTemplateWizardViewModel model )
            : this()
        {
            this.DataContext = model;
        }

        /// <summary>
        /// Gets or sets the model associated with the view.
        /// </summary>
        /// <value>The associated <see cref="ViewModelItemTemplateWizardViewModel">model</see>.</value>
        public ViewModelItemTemplateWizardViewModel Model
        {
            get
            {
                return this.DataContext as ViewModelItemTemplateWizardViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
