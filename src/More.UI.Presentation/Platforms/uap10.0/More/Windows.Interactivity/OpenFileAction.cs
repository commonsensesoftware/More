namespace More.Windows.Interactivity
{
    using Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Windows.Storage.Pickers;
    using global::Windows.UI.Xaml;
    using IFile = IO.IFile;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class OpenFileAction
    {
        async Task<IReadOnlyList<IFile>> OpenFilesAsync( OpenFileInteraction openFile )
        {
            Contract.Requires( openFile != null );
            Contract.Ensures( Contract.Result<Task<IReadOnlyList<IFile>>>() != null );

            var saveButton = openFile.DefaultCommand;
            var dialog = new FileOpenPicker();

            dialog.FileTypeFilter.AddRange( openFile.FileTypeFilter.FixUpExtensions() );
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

            if ( openFile.Multiselect )
            {
                return ( await dialog.PickMultipleFilesAsync() ).Select( f => f.AsFile() ).ToArray();
            }

            var file = await dialog.PickSingleFileAsync();

            if ( file != null )
            {
                return new[] { file.AsFile() };
            }

            return new IFile[0];
        }

        static void InvokeCallbackCommand( OpenFileInteraction openFile, IReadOnlyList<IFile> files )
        {
            Contract.Requires( openFile != null );
            Contract.Requires( files != null );

            if ( files.Any() )
            {
                openFile.Files.ReplaceAll( files );
                openFile.ExecuteDefaultCommand();
            }
            else
            {
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
            {
                return;
            }

            var files = await OpenFilesAsync( openFile );
            InvokeCallbackCommand( openFile, files );
        }
    }
}