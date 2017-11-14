namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using More.VisualStudio.ViewModels;
    using More.VisualStudio.Views;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the default project template wizard.
    /// </summary>
    [CLSCompliant( false )]
    public class DefaultProjectTemplateWizard : ProjectTemplateWizard
    {
        void ApplyManifestExtensions( Project project )
        {
            Contract.Requires( project != null );

            if ( !GetBoolean( "$hasExtensions$" ) )
            {
                return;
            }

            var writer = new ManifestExtensionWriter( Context );
            writer.ApplyExtensions( project, key => GetBoolean( key, false ) );
        }

        void TryAddMainView( Project project, string language )
        {
            Contract.Requires( project != null );
            Contract.Requires( !string.IsNullOrEmpty( language ) );

            var viewTemplate = GetString( "_viewTemplate" );

            if ( string.IsNullOrEmpty( viewTemplate ) )
            {
                return;
            }

            var viewName = GetString( "_view", "MainView" );

            AddFromTemplate( "Views", viewTemplate, viewName, language );
        }

        /// <summary>
        /// Occurs after a project has finished being generated.
        /// </summary>
        /// <param name="project">The generated <see cref="Project">project</see>.</param>
        public override void ProjectFinishedGenerating( Project project )
        {
            if ( project == null )
            {
                return;
            }

            var language = project.GetTemplateLanguage();

            using ( Context.EnterNonInteractiveScope() )
            {
                TryAddMainView( project, language );
            }

            // HACK: for some reason template parameters are not replaced in *.appxmanifest files.
            // instead of relying on the template engine, this method will add the required xml.
            ApplyManifestExtensions( project );
        }

        /// <summary>
        /// Attempts to run the template wizard.
        /// </summary>
        /// <param name="shell">The <see cref="IVsUIShell">shell</see> associated with the wizard.</param>
        /// <returns>True if the wizard completed successfully; otherwise, false if the wizard was canceled.</returns>
        protected override bool TryRunWizard( IVsUIShell shell )
        {
            Arg.NotNull( shell, nameof( shell ) );

            var mapper = new ProjectReplacementsMapper();
            var model = new ProjectTemplateWizardViewModel();
            var view = new NewProjectTemplateWizard( model );

            mapper.Map( Context.Replacements, model );

            if ( !( view.ShowDialog( shell ) ?? false ) )
            {
                return false;
            }

            mapper.Map( model, Context.Replacements );

            return true;
        }
    }
}