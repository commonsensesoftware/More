namespace $rootnamespace$
{
    using More;
    using More.Collections.Generic;
    using More.ComponentModel;
    using More.IO;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

	/// <summary>
    /// Represents a view model that supports user interactions.
    /// </summary>
    public class $safeitemrootname$ :  $base$
    {
        private readonly InteractionRequest<Interaction> userFeedback = new InteractionRequest<Interaction>( "UserFeedback" );$if$ ($enableTextInput$ == true)
        private readonly InteractionRequest<TextInputInteraction> textInput = new InteractionRequest<TextInputInteraction>( "TextInput" );$endif$$if$ ($enableOpenFile$ == true)
        private readonly InteractionRequest<OpenFileInteraction> openFile;$endif$$if$ ($enableSaveFile$ == true)
        private readonly InteractionRequest<SaveFileInteraction> saveFile;$endif$$if$ ($enableSelectFolder$ == true)
        private readonly InteractionRequest<SelectFolderInteraction> selectFolder;$endif$$if$ ($addTitle$ == true)
        private string titleField = "$title$";$endif$

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>
        public $safeitemrootname$()
        {$if$ ($showTips$ == true)
            // TODO: If this class has import dependencies, they can be specified in the constructor arguments
            //       example: public $safeitemrootname$( MyService service )

            // TODO: add additional interaction requests to suit your needs$endif$$if$ ($enableOpenFile$ == true)
            openFile = new OpenFileInteractionRequest( "OpenFile", OnFilesOpened );$endif$$if$ ($enableSaveFile$ == true)
            saveFile = new SaveFileInteractionRequest( "SaveFile", OnFileSaved );$endif$$if$ ($enableSelectFolder$ == true)
            selectFolder = new SelectFolderInteractionRequest( "SelectFolder", OnFolderSelected );$endif$
            InteractionRequests.Add( userFeedback );$if$ ($enableTextInput$ == true)
            InteractionRequests.Add( textInput );$endif$$if$ ($enableOpenFile$ == true)
            InteractionRequests.Add( openFile );$endif$$if$ ($enableSaveFile$ == true)
            InteractionRequests.Add( saveFile );$endif$$if$ ($enableSelectFolder$ == true)
            InteractionRequests.Add( selectFolder );$endif$$if$ ($enableOpenFile$ == true)
            Commands.Add( new NamedCommand<object>( "OpenFile", "Open File", OnOpenFile ) );$endif$$if$ ($enableSaveFile$ == true)
            Commands.Add( new NamedCommand<object>( "SaveFile", "Save File", OnSaveFile, OnCanSaveFile ) );$endif$$if$ ($enableSelectFolder$ == true)
            Commands.Add( new NamedCommand<object>( "SelectFolder", "Select Folder", OnSelectFolder ) );$endif$
        }$if$ ($addTitle$ == true)

        /// <summary>
        /// Gets or sets the title of the view model.
        /// </summary>
        /// <value>The view model title.</value>
        public string Title
        {
            get
            {
                return titleField;
            }
            set
            {
                SetProperty( ref titleField, value );
            }
        }$endif$

        /// <summary>
        /// Gets the collection of view model interaction requests.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="IInteractionRequest">interaction requests</see>.</value>
        public ObservableKeyedCollection<string, IInteractionRequest> InteractionRequests
        {
            get;
        } = new ObservableKeyedCollection<string, IInteractionRequest>( r => r.Id );

        /// <summary>
        /// Gets the collection of view model commands.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="INamedCommand">commands</see>.</value>
        /// <remarks>These commands do not control the behavior of a view.</remarks>
        public ObservableKeyedCollection<string, INamedCommand> Commands
        {
            get;
        } = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );$if$ ($addTitle$ == true)

        /// <summary>
        /// Requests an alert be displayed to a user asynchronously.
        /// </summary>
        /// <param name="message">The alert message.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        protected Task AlertAsync( string message ) => userFeedback.AlertAsync( Title, message );$endif$

        /// <summary>
        /// Requests a user confirmation asynchronously.
        /// </summary>
        /// <param name="prompt">The user confirmation prompt.</param>$if$ ($addTitle$ == false)
        /// <param name="title">The confirmation title.</param>$endif$
        /// <param name="acceptText">The confirmation acceptance text. The default value is "OK".</param>
        /// <param name="cancelText">The confirmation cancellation text. The default value is "Cancel".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a value indicating whether the user accepted or canceled the prompt.</returns>
        protected Task<bool> ConfirmAsync( string prompt, $if$ ($addTitle$ == false)string title, $endif$string acceptText = "OK", string cancelText = "Cancel" ) =>
            userFeedback.ConfirmAsync( prompt, $if$ ($addTitle$ == true)Title$endif$$if$ ($addTitle$ == false)title$endif$, acceptText, cancelText );$if$ ($enableTextInput$ == true)

        /// <summary>
        /// Requests input from a user asynchronously.
        /// </summary>
        /// <param name="prompt">The prompt provided to the user.</param>
        /// <param name="defaultResponse">The default user response. The default value is an empty string.</param>
        /// <param name="title">The title of the prompt. The default value is the current <see cref="P:Title"/>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the response. If the user canceled the operation, the response value is <c>null</c>.</returns>
        protected Task<string> GetInputAsync( string prompt, string defaultResponse = "", string title = "" ) =>
            textInput.RequestAsync( title, prompt, defaultResponse );$endif$$if$ ($enableOpenFile$ == true)

        private async void OnOpenFile( object parameter )
        {
            var fileTypeFilter = new[] { new FileType( "Text Files", ".txt" ) };
            var files = await openFile.RequestAsync( fileTypeFilter, false );
            OnFilesOpened( files );
        }

        private void OnFilesOpened( IList<IFile> files )
        {
            Contract.Requires( files != null );$endif$$if$ ($showOpenFileTips$ == true)
            // TODO: process opened files$endif$$if$ ($enableOpenFile$ == true)
        }$endif$$if$ ($enableSaveFile$ == true)

        private bool OnCanSaveFile( object parameter )
        {$endif$$if$ ($showSaveFileTips$ == true)
            // TODO: add logic when a file can be saved$endif$$if$ ($enableSaveFile$ == true)
            return true;
        }

        private async void OnSaveFile( object parameter )
        {
            var fileTypeChoices = new[] { new FileType( "Text Files", ".txt" ) };
            var file = await saveFile.RequestAsync( fileTypeChoices );
            OnFileSaved( file );
        }

        private async void OnFileSaved( IFile file )
        {
            if ( file == null )
                return;
    
            using ( var stream = await file.OpenReadWriteAsync() )
            {
                using ( var writer = new StreamWriter( stream ) )
                {$endif$$if$ ($showSaveFileTips$ == true)
                    // TODO: save contents to created file$endif$$if$ ($enableSaveFile$ == true)

                    await writer.FlushAsync();
                }
            }
        }$endif$$if$ ($enableSelectFolder$ == true)

        private async void OnSelectFolder( object parameter )
        {
            var folder = await selectFolder.RequestAsync();
            OnFolderSelected( folder );
        }

        private void OnFolderSelected( IFolder folder )
        {
            if ( folder == null )
                return;$endif$$if$ ($showSelectFolderTips$ == true)

            // TODO: use selected folder$endif$$if$ ($enableSelectFolder$ == true)
        }$endif$
    }
}