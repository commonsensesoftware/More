namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;
    using global::Windows.UI.Xaml;
    using IFile = More.IO.IFile;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class OpenFileAction
    {
        private async Task<IReadOnlyList<IFile>> OpenFilesAsync( OpenFileInteraction openFile )
        {
            Contract.Requires( openFile != null );
            Contract.Ensures( Contract.Result<Task<IReadOnlyList<IFile>>>() != null );

            var saveButton = openFile.DefaultCommand;
            var dialog = new FileOpenPicker();

            dialog.FileTypeFilter.AddRange( openFile.FileTypeFilter );
            dialog.SuggestedStartLocation = this.SuggestedStartLocation;
            dialog.ViewMode = this.ViewMode;

            if ( !string.IsNullOrEmpty( this.SettingsIdentifier ) )
                dialog.SettingsIdentifier = this.SettingsIdentifier;

            if ( saveButton != null )
                dialog.CommitButtonText = saveButton.Name;

            if ( openFile.Multiselect )
                return ( await dialog.PickMultipleFilesAsync() ).Select( f => f.AsFile() ).ToArray();

            var file = await dialog.PickSingleFileAsync();

            if ( file != null )
                return new[] { file.AsFile() };

            return new IFile[0];
        }

        private static void InvokeCallbackCommand( OpenFileInteraction openFile, IReadOnlyList<IFile> files )
        {
            Contract.Requires( openFile != null );
            Contract.Requires( files != null );

            if ( files.Any() )
            {
                // set file and execute accept
                openFile.Files.ReplaceAll( files );
                openFile.ExecuteDefaultCommand();
            }
            else
            {
                // execute cancel
                openFile.ExecuteCancelCommand();
            }
        }

        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The <see cref="FrameworkElement"/> that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            var openFile = GetRequestedInteraction<OpenFileInteraction>( parameter );

            if ( openFile == null )
                return;

            var files = await this.OpenFilesAsync( openFile );
            InvokeCallbackCommand( openFile, files );
        }
    }
}
