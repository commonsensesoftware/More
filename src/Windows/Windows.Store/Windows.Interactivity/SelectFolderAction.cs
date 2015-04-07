namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using global::Windows.Foundation;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;
    using global::Windows.UI.Xaml;
    using IFolder = More.IO.IFolder;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class SelectFolderAction : System.Windows.Interactivity.TriggerAction
    {
        private IAsyncOperation<StorageFolder> SelectFolderAsync( SelectFolderInteraction selectFolder )
        {
            Contract.Requires( selectFolder != null );
            Contract.Ensures( Contract.Result<IAsyncOperation<StorageFolder>>() != null );

            var commitButton = selectFolder.DefaultCommand;
            var dialog = new FolderPicker();

            dialog.FileTypeFilter.AddRange( selectFolder.FileTypeFilter );
            dialog.SuggestedStartLocation = this.SuggestedStartLocation;
            dialog.ViewMode = this.ViewMode;

            if ( dialog.FileTypeFilter.Count == 0 )
                dialog.FileTypeFilter.Add( "*" );

            if ( !string.IsNullOrEmpty( this.SettingsIdentifier ) )
                dialog.SettingsIdentifier = this.SettingsIdentifier;

            if ( commitButton != null )
                dialog.CommitButtonText = commitButton.Name;

            return dialog.PickSingleFolderAsync();
        }

        private static void InvokeCallbackCommand( SelectFolderInteraction interaction, IFolder selectedFolder )
        {
            Contract.Requires( interaction != null );

            if ( selectedFolder == null )
            {
                // execute cancel
                interaction.ExecuteCancelCommand();
            }
            else
            {
                // set selected folder and execute accept
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
                return;

            var storageFolder = await this.SelectFolderAsync( interaction );
            var selectedFolder = storageFolder == null ? null : storageFolder.AsFolder();
            InvokeCallbackCommand( interaction, selectedFolder );
        }
    }
}
