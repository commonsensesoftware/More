namespace More.Windows.Interactivity
{
    using global::Windows.Storage.Pickers;
    using global::Windows.UI.Xaml;
    using Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using IFile = IO.IFile;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction">interactivity action</see> that can be used to open a file for the
    /// <see cref="OpenFileInteraction">interaction</see> received from an <see cref="IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class OpenFileAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Gets or sets the settings identifier associated with the file open picker instance.
        /// </summary>
        /// <value>The settings identifier.</value>
        public string SettingsIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the initial location where the open file picker looks for folders to present to the user.
        /// </summary>
        /// <value>The identifier of the starting location.</value>
        public PickerLocationId SuggestedStartLocation { get; set; }

        /// <summary>
        /// Gets or sets the view mode that the file open picker uses to display items.
        /// </summary>
        /// <value>One of the <see cref="ViewMode"/> values. The default value is <see cref="PickerViewMode.List"/>.</value>
        public PickerViewMode ViewMode { get; set; }

        async Task<IReadOnlyList<IFile>> OpenFilesAsync( OpenFileInteraction openFile )
        {
            Contract.Requires( openFile != null );
            Contract.Ensures( Contract.Result<Task<IReadOnlyList<IFile>>>() != null );

            var saveButton = openFile.DefaultCommand;
            var dialog = new FileOpenPicker()
            {
                SuggestedStartLocation = SuggestedStartLocation,
                ViewMode = ViewMode,
            };

            dialog.FileTypeFilter.AddRange( openFile.FileTypeFilter.FixUpExtensions() );

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

            return Array.Empty<IFile>();
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

            var files = await OpenFilesAsync( openFile ).ConfigureAwait( true );
            InvokeCallbackCommand( openFile, files );
        }
    }
}