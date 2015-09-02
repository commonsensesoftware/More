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
        /// Requests an alert be displayed to a user.
        /// </summary>
        /// <param name="message">The alert message.</param>
        protected void Alert( string message ) => Alert( Title, message );$endif$

        /// <summary>
        /// Requests an alert be displayed to a user.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        protected void Alert( string title, string message ) => userFeedback.Request( new Interaction( title, message ) );

        /// <summary>
        /// Requests a user confirmation.
        /// </summary>
        /// <param name="prompt">The user confirmation prompt.</param>
        /// <param name="title">The prompt title.</param>
        /// <param name="confirmed">The <see cref="Action{T}">action</see> performed when the request is confirmed.</param>
        /// <param name="acceptText">The confirmation acceptance text. The default value is "OK".</param>
        /// <param name="cancelText">The confirmation cancellation text. The default value is "Cancel".</param>
        protected void Confirm(
            string prompt,
            string title,
            Action<object> confirmed,
            string acceptText = "OK",
            string cancelText = "Cancel" )
        {
            Contract.Requires( !string.IsNullOrEmpty( prompt ) );
            Contract.Requires( !string.IsNullOrEmpty( title ) );
            Contract.Requires( confirmed != null );
            Contract.Requires( !string.IsNullOrEmpty( acceptText ) );
            Contract.Requires( !string.IsNullOrEmpty( cancelText ) );

            var interaction = new Interaction()
            {
                Title = title,
                Content = prompt,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( acceptText, confirmed ),
                    new NamedCommand<object>( cancelText, DefaultAction.None )
                }
            };

            userFeedback.Request( interaction );
        }$if$ ($enableSettings$ == true)

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

        private void OnOpenFile( object parameter )
        {
            OpenFileInteraction interaction = null;
            interaction = new OpenFileInteraction()
            {
                Title = "Open File",
                Multiselect = false,
                FileTypeFilter =
                {
                    ".txt"
                },
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Open", p => OnFilesOpened( interaction.Files ) ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            openFile.Request( interaction );
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

        private void OnSaveFile( object parameter )
        {
            SaveFileInteraction interaction = null;
            interaction = new SaveFileInteraction()
            {
                Title = "Save File",
                DefaultFileExtension = ".txt",
                FileTypeChoices =
                {
                    { "Text File", new []{ ".txt" } }
                },
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Save", p => OnFileSaved( interaction.SavedFile ) ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            saveFile.Request( interaction );
        }

        private async void OnFileSaved( IFile file )
        {
            Contract.Requires( file != null );

            using ( var stream = await file.OpenReadWriteAsync() )
            {$endif$$if$ ($showSaveFileTips$ == true)
                // TODO: save contents to created file$endif$$if$ ($enableSaveFile$ == true)
            }
        }$endif$$if$ ($enableSelectFolder$ == true)

        private void OnSelectFolder( object parameter )
        {
            SelectFolderInteraction interaction = null;
            interaction = new SelectFolderInteraction()
            {
                Title = "Select Folder",
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Select", p => OnFolderSelected( interaction.Folder ) ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            selectFolder.Request( interaction );
        }

        private void OnFolderSelected( IFolder folder )
        {
            Contract.Requires( folder != null );$endif$$if$ ($showSelectFolderTips$ == true)
            // TODO: use selected folder$endif$$if$ ($enableSelectFolder$ == true)
        }$endif$
    }
}