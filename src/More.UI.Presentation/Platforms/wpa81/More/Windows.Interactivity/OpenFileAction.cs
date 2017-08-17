namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using global::Windows.Storage.Pickers;
    using global::Windows.UI.Xaml;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class OpenFileAction
    {
        void OpenFiles( OpenFileInteraction interaction )
        {
            Contract.Requires( interaction != null );

            var saveButton = interaction.DefaultCommand;
            var dialog = new FileOpenPicker();

            dialog.ContinuationData.AddRange( interaction.ContinuationData );
            dialog.FileTypeFilter.AddRange( interaction.FileTypeFilter.FixUpExtensions() );
            dialog.SuggestedStartLocation = SuggestedStartLocation;
            dialog.ViewMode = ViewMode;

            if ( !string.IsNullOrEmpty( SettingsIdentifier ) )
            {
                dialog.SettingsIdentifier = SettingsIdentifier;
            }

            if ( saveButton != null )
            {
                dialog.CommitButtonText = saveButton.Name;
            }

            if ( interaction.Multiselect )
            {
                dialog.PickMultipleFilesAndContinue();
            }
            else
            {
                dialog.PickSingleFileAndContinue();
            }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The <see cref="FrameworkElement"/> that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        public override object Execute( object sender, object parameter )
        {
            var interaction = GetRequestedInteraction<OpenFileInteraction>( parameter );

            if ( interaction != null )
            {
                OpenFiles( interaction );
            }

            return null;
        }
    }
}