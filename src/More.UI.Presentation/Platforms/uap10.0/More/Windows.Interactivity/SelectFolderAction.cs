namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using IFolder = More.IO.IFolder;
    using global::Windows.Foundation;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to select a folder for the
    /// <see cref="SelectFolderInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class SelectFolderAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Gets or sets the settings identifier associated with the file open picker instance.
        /// </summary>
        /// <value>The settings identifier.</value>
        public string SettingsIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the initial location where the folder picker looks for folders to present to the user.
        /// </summary>
        /// <value>The identifier of the starting location.</value>
        public PickerLocationId SuggestedStartLocation { get; set; }

        /// <summary>
        /// Gets or sets the view mode that the file open picker uses to display items.
        /// </summary>
        /// <value>One of the <see cref="ViewMode"/> values. The default value is <see cref="F:PickerViewMode.List"/>.</value>
        public PickerViewMode ViewMode { get; set; }

        IAsyncOperation<StorageFolder> SelectFolderAsync( SelectFolderInteraction selectFolder )
        {
            Contract.Requires( selectFolder != null );
            Contract.Ensures( Contract.Result<IAsyncOperation<StorageFolder>>() != null );

            var commitButton = selectFolder.DefaultCommand;
            var dialog = new FolderPicker
            {
                SuggestedStartLocation = SuggestedStartLocation,
                ViewMode = ViewMode,
            };

            dialog.FileTypeFilter.AddRange( selectFolder.FileTypeFilter );

            if ( dialog.FileTypeFilter.Count == 0 )
            {
                dialog.FileTypeFilter.Add( "*" );
            }

            if ( !string.IsNullOrEmpty( SettingsIdentifier ) )
            {
                dialog.SettingsIdentifier = SettingsIdentifier;
            }

            if ( commitButton != null )
            {
                dialog.CommitButtonText = commitButton.Name;
            }

            return dialog.PickSingleFolderAsync();
        }

        static void InvokeCallbackCommand( SelectFolderInteraction interaction, IFolder selectedFolder )
        {
            Contract.Requires( interaction != null );

            if ( selectedFolder == null )
            {
                interaction.ExecuteCancelCommand();
            }
            else
            {
                interaction.Folder = selectedFolder;
                interaction.ExecuteDefaultCommand();
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
            var interaction = GetRequestedInteraction<SelectFolderInteraction>( parameter );

            if ( interaction == null )
            {
                return;
            }

            var storageFolder = await SelectFolderAsync( interaction );
            var selectedFolder = storageFolder == null ? null : storageFolder.AsFolder();
            InvokeCallbackCommand( interaction, selectedFolder );
        }
    }
}