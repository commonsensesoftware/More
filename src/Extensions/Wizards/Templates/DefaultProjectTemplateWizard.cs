namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using More.VisualStudio.ViewModels;
    using More.VisualStudio.Views;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the default project template wizard.
    /// </summary>
    public class DefaultProjectTemplateWizard : ProjectTemplateWizard
    {
        private void ApplyManifestExtensions( Project project )
        {
            Contract.Requires( project != null );

            // must have enabled extensions
            if ( !this.GetBoolean( "$hasExtensions$" ) )
                return;

            var writer = new ManifestExtensionWriter( this.Context );
            writer.ApplyExtensions( project, key => this.GetBoolean( key, false ) );
        }

        private void TryAddMainView( Project project )
        {
            Contract.Requires( project != null );

            var viewTemplate = this.GetString( "_viewTemplate" );

            if ( string.IsNullOrEmpty( viewTemplate ) )
                return;

            var viewName = this.GetString( "_view", "MainView" );

            this.AddFromTemplate( "Views", viewTemplate, viewName, "CSharp" );
        }

        private void TryAddSettingsFlyout( Project project )
        {
            Contract.Requires( project != null );

            var addSettings = this.GetBoolean( "$enableSettings$" );
            var settingsTemplate = this.GetString( "_settingsTemplate" );

            if ( !addSettings || string.IsNullOrEmpty( settingsTemplate ) )
                return;

            this.Context.Replacements["$viewmodel$"] = "DefaultSettingsViewModel";
            this.AddFromTemplate( "Views", settingsTemplate, "DefaultSettings", "CSharp" );
        }

        /// <summary>
        /// Occurs after a project has finished being generated.
        /// </summary>
        /// <param name="project">The generated <see cref="Project">project</see>.</param>
        public override void ProjectFinishedGenerating( Project project )
        {
            if ( project == null )
                return;

            // suppress futher user interactions
            using ( this.Context.EnterNonInteractiveScope() )
            {
                // try to add additional files from templates, if configured
                this.TryAddMainView( project );
                this.TryAddSettingsFlyout( project );
            }

            // HACK: for some reason template parameters are not replaced in *.appxmanifest files.
            // instead of relying on the template engine, this method will add the required xml.
            this.ApplyManifestExtensions( project );
        }

        /// <summary>
        /// Attempts to run the template wizard.
        /// </summary>
        /// <param name="shell">The <see cref="IVsUIShell">shell</see> associated with the wizard.</param>
        /// <returns>True if the wizard completed successfully; otherwise, false if the wizard was canceled.</returns>
        protected override bool TryRunWizard( IVsUIShell shell )
        {
            var mapper = new ProjectReplacementsMapper();
            var model = new ProjectTemplateWizardViewModel();
            var view = new NewProjectTemplateWizard( model );

            // map replacements to model
            mapper.Map( this.Context.Replacements, model );

            if ( !( view.ShowDialog( shell ) ?? false ) )
                return false;

            // map model back to replacements
            mapper.Map( model, this.Context.Replacements );

            return true;
        }
    }
}
