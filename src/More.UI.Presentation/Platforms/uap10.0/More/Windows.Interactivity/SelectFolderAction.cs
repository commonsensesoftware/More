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
        IAsyncOperation<StorageFolder> SelectFolderAsync( SelectFolderInteraction selectFolder )
        {
            Contract.Requires( selectFolder != null );
            Contract.Ensures( Contract.Result<IAsyncOperation<StorageFolder>>() != null );

            var commitButton = selectFolder.DefaultCommand;
            var dialog = new FolderPicker();

            dialog.FileTypeFilter.AddRange( selectFolder.FileTypeFilter );
            dialog.SuggestedStartLocation = SuggestedStartLocation;
            dialog.ViewMode = ViewMode;

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