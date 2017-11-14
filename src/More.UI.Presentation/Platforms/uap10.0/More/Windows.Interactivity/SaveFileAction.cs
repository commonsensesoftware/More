namespace More.Windows.Interactivity
{
    using Input;
    using Microsoft.Xaml.Interactivity;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using IFile = IO.IFile;
    using global::Windows.Foundation;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;

    /// <summary>
    /// Represents an <see cref="IAction">interactivity action</see> that can be used to save a file for the
    /// <see cref="SaveFileInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class SaveFileAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Gets or sets the settings identifier associated with the file save picker instance.
        /// </summary>
        /// <value>The settings identifier.</value>
        public string SettingsIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the initial location where the save file picker looks for folders to present to the user.
        /// </summary>
        /// <value>The identifier of the starting location.</value>
        public PickerLocationId SuggestedStartLocation { get; set; }

        IAsyncOperation<StorageFile> SaveFileAsync( SaveFileInteraction saveFile )
        {
            Contract.Requires( saveFile != null );
            Contract.Ensures( Contract.Result<IAsyncOperation<StorageFile>>() != null );

            var dialog = new FileSavePicker()
            {
                DefaultFileExtension = saveFile.DefaultFileExtension,
                SuggestedStartLocation = SuggestedStartLocation,
            };

            dialog.FileTypeChoices.AddRange( saveFile.FileTypeChoices.ToDictionary() );

            if ( !string.IsNullOrEmpty( saveFile.FileName ) )
            {
                dialog.SuggestedFileName = saveFile.FileName;
            }

            if ( !string.IsNullOrEmpty( SettingsIdentifier ) )
            {
                dialog.SettingsIdentifier = SettingsIdentifier;
            }

            return dialog.PickSaveFileAsync();
        }

        static void InvokeCallbackCommand( SaveFileInteraction interaction, IFile savedFile )
        {
            Contract.Requires( interaction != null );

            if ( savedFile != null )
            {
                interaction.FileName = savedFile.Name;
                interaction.SavedFile = savedFile;
                interaction.ExecuteDefaultCommand();
            }
            else
            {
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
            {
                return;
            }

            var storageFile = await SaveFileAsync( interaction );
            var savedFile = storageFile == null ? null : storageFile.AsFile();
            InvokeCallbackCommand( interaction, savedFile );
        }
    }
}