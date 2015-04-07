namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.TemplateWizard;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the base implementation for an item template wizard.
    /// </summary>
    public abstract class ItemTemplateWizard : TemplateWizard, IWizard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTemplateWizard"/> class.
        /// </summary>
        protected ItemTemplateWizard()
        {
        }

        /// <summary>
        /// Occurs before a project item is opened.
        /// </summary>
        /// <param name="projectItem">The <see cref="ProjectItem">project item</see> being opened.</param>
        public virtual void BeforeOpeningFile( ProjectItem projectItem )
        {
        }

        void IWizard.BeforeOpeningFile( ProjectItem projectItem )
        {
            try
            {
                this.BeforeOpeningFile( projectItem );
            }
            catch
            {
                // note: RunFinished is not called if an exception occurs
                this.Context.Abandon();
                throw;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Does not apply to a project item template wizard." )]
        void IWizard.ProjectFinishedGenerating( Project project )
        {
        }

        /// <summary>
        /// Occurs after a project item has finished being generated.
        /// </summary>
        /// <param name="projectItem">The generated <see cref="ProjectItem">project item</see>.</param>
        public virtual void ProjectItemFinishedGenerating( ProjectItem projectItem )
        {
        }

        void IWizard.ProjectItemFinishedGenerating( ProjectItem projectItem )
        {
            try
            {
                this.ProjectItemFinishedGenerating( projectItem );
            }
            catch
            {
                // note: RunFinished is not called if an exception occurs
                this.Context.Abandon();
                throw;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified project item should be added.
        /// </summary>
        /// <param name="filePath">The file path of the item to evaluate.</param>
        /// <returns>True if the project item should be added; otherwise, false.</returns>
        public virtual bool ShouldAddProjectItem( string filePath )
        {
            return !this.IsCanceled;
        }

        bool IWizard.ShouldAddProjectItem( string filePath )
        {
            try
            {
                return this.ShouldAddProjectItem( filePath );
            }
            catch
            {
                // note: RunFinished is not called if an exception occurs
                this.Context.Abandon();
                throw;
            }
        }
    }
}
