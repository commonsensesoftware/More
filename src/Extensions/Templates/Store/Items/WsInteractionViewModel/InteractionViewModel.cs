namespace $rootnamespace$
{
    using More;
    using More.Collections.Generic;
    using More.ComponentModel;
    using More.IO;$if$ ($enableSharing$ == true)
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
    using System.Windows.Input;
    using global::Windows.Storage;

	/// <summary>
    /// Represents a view model that supports user interactions.
    /// </summary>
    public class $safeitemrootname$ :  $base$
    {
        private readonly InteractionRequest<Interaction> userFeedback = new InteractionRequest<Interaction>( "UserFeedback" );$if$ ($enableSettings$ == true)
        private readonly InteractionRequest<Interaction> settings = new InteractionRequest<Interaction>( "Settings" );$endif$$if$ ($enableSharing$ == true)
        private readonly InteractionRequest<Interaction> share = new InteractionRequest<Interaction>( "Share" );$endif$$if$ ($enableSearch$ == true)
        private readonly InteractionRequest<Interaction> search = new InteractionRequest<Interaction>( "Search" );$endif$$if$ ($enableOpenFile$ == true)
        private readonly InteractionRequest<OpenFileInteraction> openFile = new InteractionRequest<OpenFileInteraction>( "OpenFile" );$endif$$if$ ($enableSaveFile$ == true)
        private readonly InteractionRequest<SaveFileInteraction> saveFile = new InteractionRequest<SaveFileInteraction>( "SaveFile" );$endif$$if$ ($enableSelectFolder$ == true)
        private readonly InteractionRequest<SelectFolderInteraction> selectFolder = new InteractionRequest<SelectFolderInteraction>( "SelectFolder" );$endif$$if$ ($addTitle$ == true)
        private string titleField = "$title$";$endif$

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>$if$ ($eventBrokerRequired$ == true)
        /// <param name="eventBroker">The <see cref="IEventBroker">event broker</see> used by the view model.</param>$endif$
        public $safeitemrootname$($if$ ($eventBrokerRequired$ == true) IEventBroker eventBroker $endif$)
        {$if$ ($eventBrokerRequired$ == true)
            Contract.Requires( eventBroker != null );$endif$$if$ ($enableAppSearch$ == true)

            // subscribe to application-wide search. this event is fired when the application is activated via a search.
            eventBroker.Subscribe( "Search", ( string eventName, object sender, SearchEventArgs e ) => OnSearch( e ) );$endif$$if$ ($enableAppSharing$ == true)

            // subscribe to application-wide sharing. this event is fired when the application is activated via a share operation.
            eventBroker.Subscribe( "Share", ( string eventName, object sender, ShareEventArgs e ) => OnShareReceived( e ) );
$endif$$if$ ($showTips$ == true)
            // TODO: If this class has import dependencies, they can be specified in the constructor arguments
            //       example: public $safeitemrootname$( MyService service )

            // TODO: add additional interaction requests to suit your needs$endif$
            InteractionRequests.Add( userFeedback );$if$ ($enableSettings$ == true)
            InteractionRequests.Add( settings );$endif$$if$ ($enableSharing$ == true)
            InteractionRequests.Add( share );$endif$$if$ ($enableSearch$ == true)
            InteractionRequests.Add( search );$endif$$if$ ($enableOpenFile$ == true)
            InteractionRequests.Add( openFile );$endif$$if$ ($enableSaveFile$ == true)
            InteractionRequests.Add( saveFile );$endif$$if$ ($enableSelectFolder$ == true)
            InteractionRequests.Add( selectFolder );$endif$$if$ ($enableSharing$ == true)
            Commands.Add( new NamedCommand<IDataRequest>( "Share", OnShare ) );$endif$$if$ ($enableSearch$ == true)
            Commands.Add( new NamedCommand<ISearchRequest>( "Search", OnSearch ) );
            Commands.Add( new NamedCommand<ISearchSuggestionsRequest>( "ProvideSuggestions", OnProvideSuggestions ) );
            Commands.Add( new NamedCommand<string>( "SuggestionChosen", OnSuggestionChosen ) );$endif$$if$ ($enableOpenFile$ == true)
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
        /// <param name="prompt">The user confirmation prompt.</param>
        /// <param name="acceptText">The confirmation acceptance text. The default value is "OK".</param>
        /// <param name="cancelText">The confirmation cancellation text. The default value is "Cancel".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a value indicating whether the user accepted or canceled the prompt.</returns>
        protected Task<bool> ConfirmAsync( string prompt, string acceptText = "OK", string cancelText = "Cancel" ) =>
            userFeedback.ConfirmAsync( prompt, Title, acceptText, cancelText );$if$ ($enableSettings$ == true)

        /// <summary>
        /// Requests settings be displayed to the user.
        /// </summary>
        protected void ShowSettings() => settings.Request( new Interaction() );$endif$$if$ ($enableAppSharing$ == true)

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
        protected void Share() => share.Request( new Interaction() );$endif$$if$ ($enableSearch$ == true)

        private void OnSearch( ISearchRequest searchRequest )
        {
            Contract.Requires( searchRequest != null );$endif$$if$ ($showSearchTips$ == true)
            // TODO: perform search$endif$$if$ ($enableSearch$ == true)
        }

        private void OnProvideSuggestions( ISearchSuggestionsRequest searchRequest )
        {
            Contract.Requires( searchRequest != null );$endif$$if$ ($showSearchTips$ == true)
            // TODO: provide search suggestions$endif$$if$ ($enableSearch$ == true)
        }

        private void OnSuggestionChosen( string suggestion )
        {$endif$$if$ ($showSearchTips$ == true)
            // TODO: process suggestion$endif$$if$ ($enableSearch$ == true)
        }

        /// <summary>
        /// Requests a search operation.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        protected void Search( string text ) => search.Request( new Interaction(){ Content = text } );$endif$$if$ ($enableOpenFile$ == true)

        private async void OnOpenFile( object parameter )
        {
            var fileTypeFilter = new[] { ".txt" };
            var file = await openFile.RequestSingleFileAsync( fileTypeFilter );

            if ( file == null )
                return;$endif$$if$ ($showOpenFileTips$ == true)

            // TODO: process opened file$endif$$if$ ($enableOpenFile$ == true)

        }$endif$$if$ ($enableSaveFile$ == true)

        private bool OnCanSaveFile( object parameter )
        {$endif$$if$ ($showSaveFileTips$ == true)
            // TODO: add logic when a file can be saved$endif$$if$ ($enableSaveFile$ == true)
            return true;
        }

        private async void OnSaveFile( object parameter )
        {
            var fileTypeChoices = new[] { new KeyValuePair<string, IReadOnlyList<string>>( "Text File", new[] { ".txt" } ) };

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

            if ( folder == null )
                return;$endif$$if$ ($showSelectFolderTips$ == true)

            // TODO: use selected folder$endif$$if$ ($enableSelectFolder$ == true)

        }$endif$
    }
}