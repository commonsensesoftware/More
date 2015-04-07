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
        private void OpenFiles( OpenFileInteraction interaction )
        {
            Contract.Requires( interaction != null );

            var saveButton = interaction.DefaultCommand;
            var dialog = new FileOpenPicker();

            dialog.ContinuationData.AddRange( interaction.ContinuationData );
            dialog.FileTypeFilter.AddRange( interaction.FileTypeFilter );
            dialog.SuggestedStartLocation = this.SuggestedStartLocation;
            dialog.ViewMode = this.ViewMode;

            if ( !string.IsNullOrEmpty( this.SettingsIdentifier ) )
                dialog.SettingsIdentifier = this.SettingsIdentifier;

            if ( saveButton != null )
                dialog.CommitButtonText = saveButton.Name;

            if ( interaction.Multiselect )
                dialog.PickMultipleFilesAndContinue();
            else
                dialog.PickSingleFileAndContinue();
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
                this.OpenFiles( interaction );

            return null;
        }
    }
}
