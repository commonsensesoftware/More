namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using More.VisualStudio.ViewModels;
    using More.VisualStudio.Views;
    using System;

    /// <summary>
    /// Represents a template wizard to create a view model using the Model-View-ViewModel (MVVM) pattern.
    /// </summary>
    public class ViewModelTemplateWizard : ItemTemplateWizard
    {
        /// <summary>
        /// Occurs after a project item finished being generated.
        /// </summary>
        /// <param name="projectItem">The generated <see cref="ProjectItem">project item</see>.</param>
        public override void ProjectItemFinishedGenerating( ProjectItem projectItem )
        {
            if ( projectItem == null )
                return;

            var writer = new ManifestExtensionWriter( Context );
            writer.ApplyExtensions( projectItem.ContainingProject, key => GetBoolean( key, false ) );
        }

        /// <summary>
        /// Attempts to run the template wizard.
        /// </summary>
        /// <param name="shell">The <see cref="IVsUIShell">shell</see> associated with the wizard.</param>
        /// <returns>True if the wizard completed successfully; otherwise, false if the wizard was canceled.</returns>
        protected override bool TryRunWizard( IVsUIShell shell )
        {
            Arg.NotNull( shell, nameof( shell ) );

            var mapper = new ViewModelReplacementsMapper( Project );
            var model = new ViewModelItemTemplateWizardViewModel();

            // map replacements to model
            mapper.Map( Context.Replacements, model );

            // only show the dialog if the context is interactive
            if ( Context.IsInteractive )
            {
                var view = new ViewModelItemTemplateWizard( model );

                // show the wizard
                if ( !( view.ShowDialog( shell ) ?? false ) )
                    return false;
            }

            // map model back to replacements
            mapper.Map( model, Context.Replacements );
            return true;
        }
    }
}
