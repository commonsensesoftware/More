namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using global::Windows.Foundation;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;
    using global::Windows.UI.Xaml;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class SaveFileAction
    {
        private void SaveFile( SaveFileInteraction saveFile )
        {
            Contract.Requires( saveFile != null );

            var dialog = new FileSavePicker();

            dialog.ContinuationData.AddRange( saveFile.ContinuationData );
            dialog.DefaultFileExtension = saveFile.DefaultFileExtension;
            dialog.FileTypeChoices.AddRange( saveFile.FileTypeChoices );
            dialog.SuggestedStartLocation = this.SuggestedStartLocation;

            if ( !string.IsNullOrEmpty( saveFile.FileName ) )
                dialog.SuggestedFileName = saveFile.FileName;

            if ( !string.IsNullOrEmpty( this.SettingsIdentifier ) )
                dialog.SettingsIdentifier = this.SettingsIdentifier;

            dialog.PickSaveFileAndContinue();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>The result of the action. The default implementation always executes asynchronously and returns null.</returns>
        public override object Execute( object sender, object parameter )
        {
            var saveFile = GetRequestedInteraction<SaveFileInteraction>( parameter );

            if ( saveFile != null )
                this.SaveFile( saveFile );

            return null;
        }
    }
}
