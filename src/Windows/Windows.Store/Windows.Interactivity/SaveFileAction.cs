namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using global::Windows.Foundation;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;
    using IFile = More.IO.IFile;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class SaveFileAction : System.Windows.Interactivity.TriggerAction
    {
        private IAsyncOperation<StorageFile> SaveFileAsync( SaveFileInteraction saveFile )
        {
            Contract.Requires( saveFile != null );
            Contract.Ensures( Contract.Result<IAsyncOperation<StorageFile>>() != null );

            var dialog = new FileSavePicker();

            dialog.DefaultFileExtension = saveFile.DefaultFileExtension;
            dialog.FileTypeChoices.AddRange( saveFile.FileTypeChoices );
            dialog.SuggestedStartLocation = SuggestedStartLocation;

            if ( !string.IsNullOrEmpty( saveFile.FileName ) )
                dialog.SuggestedFileName = saveFile.FileName;

            if ( !string.IsNullOrEmpty( SettingsIdentifier ) )
                dialog.SettingsIdentifier = SettingsIdentifier;

            return dialog.PickSaveFileAsync();
        }

        private static void InvokeCallbackCommand( SaveFileInteraction interaction, IFile savedFile )
        {
            Contract.Requires( interaction != null );

            if ( savedFile != null )
            {
                // set file and execute accept
                interaction.FileName = savedFile.Name;
                interaction.SavedFile = savedFile;
                interaction.ExecuteDefaultCommand();
            }
            else
            {
                // execute cancel
                interaction.ExecuteCancelCommand();
            }
        }

        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            var interaction = GetRequestedInteraction<SaveFileInteraction>( parameter );

            if ( interaction == null )
                return;

            var storageFile = await SaveFileAsync( interaction );
            var savedFile = storageFile == null ? null : storageFile.AsFile();
            InvokeCallbackCommand( interaction, savedFile );
        }
    }
}
