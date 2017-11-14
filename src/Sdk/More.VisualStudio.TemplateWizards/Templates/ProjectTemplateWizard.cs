namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.TemplateWizard;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using static ProjectTemplateCancelBehavior;
    using static System.IO.Directory;

    /// <summary>
    /// Represents the base implementation for a project template wizard.
    /// </summary>
    [CLSCompliant( false )]
    public abstract class ProjectTemplateWizard : TemplateWizard, IWizard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTemplateWizard"/> class.
        /// </summary>
        protected ProjectTemplateWizard() => CancelBehavior = Cancel;

        /// <summary>
        /// Gets or sets the cancel behavior for the wizard.
        /// </summary>
        /// <value>One of the <see cref="ProjectTemplateCancelBehavior"/> values. The default value is
        /// <see cref="Cancel"/>.</value>
        protected ProjectTemplateCancelBehavior CancelBehavior { get; set; }

        /// <summary>
        /// Occurs before a project item is opened.
        /// </summary>
        /// <param name="projectItem">The <see cref="ProjectItem">project item</see> being opened.</param>
        /// <remarks>This method is called before opening any item in the project template that has the attribute
        /// "OpenInEditor" set to true.</remarks>
        public virtual void BeforeOpeningFile( ProjectItem projectItem ) { }

        void IWizard.BeforeOpeningFile( ProjectItem projectItem )
        {
            try
            {
                BeforeOpeningFile( projectItem );
            }
            catch
            {
                // note: RunFinished is not called if an exception occurs
                Context.Abandon();
                throw;
            }
        }

        /// <summary>
        /// Occurs after a project has finished being generated.
        /// </summary>
        /// <param name="project">The generated <see cref="Project">project</see>.</param>
        public virtual void ProjectFinishedGenerating( Project project ) { }

        void IWizard.ProjectFinishedGenerating( Project project )
        {
            try
            {
                ProjectFinishedGenerating( project );
            }
            catch
            {
                // note: RunFinished is not called if an exception occurs
                Context.Abandon();
                throw;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Does not apply to a project template wizard." )]
        void IWizard.ProjectItemFinishedGenerating( ProjectItem projectItem ) { }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Does not apply to a project template wizard." )]
        bool IWizard.ShouldAddProjectItem( string filePath ) => true;

        /// <summary>
        /// Occurs when the wizard has been canceled.
        /// </summary>
        protected override void OnCanceled()
        {
            base.OnCanceled();

            try
            {
                if ( Exists( ProjectDirectory ) )
                {
                    Delete( ProjectDirectory );
                }

                if ( Exists( SolutionDirectory ) )
                {
                    Delete( SolutionDirectory );
                }
            }
            catch ( IOException )
            {
            }

            // visual studio's behavior is influenced by which exception is thrown. use the enumeration
            // to map to an exception. the exception message is irrelevant in this context.
            switch ( CancelBehavior )
            {
                case BackOut:
                    throw new WizardBackoutException();
                case Cancel:
                    throw new WizardCancelledException();
            }
        }
    }
}