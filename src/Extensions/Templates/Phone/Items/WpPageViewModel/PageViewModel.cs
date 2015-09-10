namespace $rootnamespace$
{
    using More;
    using More.Collections.Generic;
    using More.ComponentModel;
    using More.IO;
    using More.Windows;$if$ ($enableSharing$ == true)
    using More.Windows.Data;$endif$
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Windows.ApplicationModel.Activation;
    using Windows.Storage;

	/// <summary>
    /// Represents a view model for a page.
    /// </summary>
    public class $safeitemrootname$ :  $base$
    {
        private readonly InteractionRequest<Interaction> userFeedback = new InteractionRequest<Interaction>( "UserFeedback" );
        private readonly InteractionRequest<NavigateInteraction> navigate = new InteractionRequest<NavigateInteraction>( "Navigate" );$if$ ($enableOpenFile$ == true)
        private readonly InteractionRequest<OpenFileInteraction> openFile;$endif$$if$ ($enableSaveFile$ == true)
        private readonly InteractionRequest<SaveFileInteraction> saveFile;$endif$$if$ ($enableSelectFolder$ == true)
        private readonly InteractionRequest<SelectFolderInteraction> selectFolder;$endif$$if$ ($enableSharing$ == true)
        private readonly InteractionRequest<Interaction> share = new InteractionRequest<Interaction>( "Share" );$endif$
        private string titleField = "$title$";

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>$if$ ($eventBrokerRequired$ == true)
        /// <param name="eventBroker">The <see cref="IEventBroker">event broker</see> used by the view model.</param>$endif$
        public $safeitemrootname$($ctorParameters$)
        {$if$ ($eventBrokerRequired$ == true)
            Contract.Requires( eventBroker != null );$endif$$if$ ($enableAppSharing$ == true)

            // subscribe to application-wide sharing. this event is fired when the application is activated via a share operation.
            eventBroker.Subscribe( "Share", ( string eventName, object sender, ShareEventArgs e ) => OnShareReceived( e ) );
$endif$$if$ ($showTips$ == true)
            // TODO: If this class has import dependencies, they can be specified in the constructor arguments
            //       example: public $safeitemrootname$( MyService service )

            // TODO: Add or modify this interaction requests and commands to suit your needs$endif$$if$ ($enableOpenFile$ == true)
            openFile = new OpenFileInteractionRequest( "OpenFile", OnFilesOpened );$endif$$if$ ($enableSaveFile$ == true)
            saveFile = new SaveFileInteractionRequest( "SaveFile", OnFileSaved );$endif$$if$ ($enableSelectFolder$ == true)
            selectFolder = new SelectFolderInteractionRequest( "SelectFolder", OnFolderSelected );$endif$
            InteractionRequests.Add( userFeedback );
            InteractionRequests.Add( navigate );$if$ ($enableOpenFile$ == true)
            InteractionRequests.Add( openFile );$endif$$if$ ($enableSaveFile$ == true)
            InteractionRequests.Add( saveFile );$endif$$if$ ($enableSelectFolder$ == true)
            InteractionRequests.Add( selectFolder );$endif$$if$ ($enableSharing$ == true)
            InteractionRequests.Add( share );$endif$$if$ ($enableOpenFile$ == true)
            Commands.Add( new NamedCommand<object>( "OpenFile", "Open File", OnOpenFile ) );$endif$$if$ ($enableSaveFile$ == true)
            Commands.Add( new NamedCommand<object>( "SaveFile", "Save File", OnSaveFile ) );$endif$$if$ ($enableSelectFolder$ == true)
            Commands.Add( new NamedCommand<object>( "SelectFolder", "Select Folder", OnSelectFolder ) );$endif$$if$ ($enableSharing$ == true)
            Commands.Add( new NamedCommand<IDataRequest>( "Share", OnShare ) );$endif$
            //Commands.Add( new NamedCommand<object>( "Navigate", p => Navigate( "Page1" ) ) );
        }

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
        }

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
        public ObservableKeyedCollection<string, INamedCommand> Commands
        {
            get;
        } = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );

        /// <summary>
        /// Requests an alert be displayed to a user asynchronously.
        /// </summary>
        /// <param name="message">The alert message.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        protected Task AlertAsync( string message ) => userFeedback.AlertAsync( Title, message );

        /// <summary>
        /// Requests a user confirmation asynchronously.
        /// </summary>
        /// <param name="prompt">The user confirmation prompt.</param>
        /// <param name="acceptText">The confirmation acceptance text. The default value is "OK".</param>
        /// <param name="cancelText">The confirmation cancellation text. The default value is "Cancel".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a value indicating whether the user accepted or canceled the prompt.</returns>
        protected Task<bool> ConfirmAsync( string prompt, string acceptText = "OK", string cancelText = "Cancel" ) =>
            userFeedback.ConfirmAsync( prompt, Title, acceptText, cancelText );

        /// <summary>
        /// Requests a navigation operation to the specified page.
        /// </summary>
        /// <param name="pageName">The name of the page to navigate to.</param>
        protected void Navigate( string pageName ) => Navigate( new NavigateInteraction() { Url = new Uri( pageName, UriKind.Relative ) } );

        /// <summary>
        /// Requests a navigation operation using the specified interaction.
        /// </summary>
        /// <param name="interaction">The <see cref="NavigateInteraction">navigate interaction</see> requested.</param>
        protected void Navigate( NavigateInteraction interaction ) => navigate.Request( interaction );$if$ ($enableAppSharing$ == true)

        private void OnShareReceived( IShareOperation share )
        {
            Contract.Requires( share != null );$endif$$if$ ($showAppSharingTips$ == true)
            // TODO: handle receipt of share operation data$endif$$if$ ($enableAppSharing$ == true)
        }$endif$$if$ ($enableSharing$ == true)

        private void OnShare( IDataRequest dataRequest )
        {
            Contract.Requires( dataRequest != null );$endif$$if$ ($showSharingTips$ == true)
            // TODO: provide the requested share data$endif$$if$ ($enableSharing$ == true)
        }

        /// <summary>
        /// Requests a share operation.
        /// </summary>
        protected void Share() => share.Request( new Interaction() );$endif$$if$ ($enableOpenFile$ == true)

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