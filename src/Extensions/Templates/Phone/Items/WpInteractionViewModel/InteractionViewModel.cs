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
    using global::Windows.ApplicationModel.Activation;
    using global::Windows.Storage;

	/// <summary>
    /// Represents a view model that supports user interactions.
    /// </summary>
    public class $safeitemrootname$ :  $base$
    {
        private readonly InteractionRequest<Interaction> userFeedback = new InteractionRequest<Interaction>( "UserFeedback" );$if$ ($enableOpenFile$ == true)
        private readonly InteractionRequest<OpenFileInteraction> openFile;$endif$$if$ ($enableSaveFile$ == true)
        private readonly InteractionRequest<SaveFileInteraction> saveFile;$endif$$if$ ($enableSelectFolder$ == true)
        private readonly InteractionRequest<SelectFolderInteraction> selectFolder;$endif$$if$ ($enableSharing$ == true)
        private readonly InteractionRequest<Interaction> share = new InteractionRequest<Interaction>( "Share" );$endif$
        private readonly ObservableKeyedCollection<string, IInteractionRequest> interactionRequests = new ObservableKeyedCollection<string, IInteractionRequest>( r => r.Id );

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>$if$ ($continuationRequired$ == true)
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used by the view model.</param>$endif$$if$ ($eventBrokerRequired$ == true)
        /// <param name="eventBroker">The <see cref="IEventBroker">event broker</see> used by the view model.</param>$endif$
        public $safeitemrootname$($ctorParameters$)
        {$if$ ($continuationRequired$ == true)
            Contract.Requires( continuationManager != null );$endif$$if$ ($eventBrokerRequired$ == true)
            Contract.Requires( eventBroker != null );$endif$$if$ ($enableAppSharing$ == true)

            // subscribe to application-wide sharing. this event is fired when the application is activated via a share operation.
            eventBroker.Subscribe( "Share", ( string eventName, object sender, ShareEventArgs e ) => this.OnShareReceived( e ) );
$endif$$if$ ($showTips$ == true)
            // TODO: If this class has import dependencies, they can be specified in the constructor arguments
            //       example: public $safeitemrootname$( MyService service )

            // TODO: add additional interaction requests to suit your needs$endif$$if$ ($enableOpenFile$ == true)
            this.openFile = continuationManager.CreateInteractionRequest<OpenFileInteraction, IFileOpenPickerContinuationEventArgs>( "OpenFile", this.OnFilesOpened );$endif$$if$ ($enableSaveFile$ == true)
            this.saveFile = continuationManager.CreateInteractionRequest<SaveFileInteraction, IFileSavePickerContinuationEventArgs>( "SaveFile", this.OnFileSaved );$endif$$if$ ($enableSelectFolder$ == true)
            this.selectFolder = continuationManager.CreateInteractionRequest<SelectFolderInteraction, IFolderPickerContinuationEventArgs>( "SelectFolder", this.OnFolderSelected );$endif$
            this.interactionRequests.Add( this.userFeedback );$if$ ($enableOpenFile$ == true)
            this.interactionRequests.Add( this.openFile );$endif$$if$ ($enableSaveFile$ == true)
            this.interactionRequests.Add( this.saveFile );$endif$$if$ ($enableSelectFolder$ == true)
            this.interactionRequests.Add( this.selectFolder );$endif$$if$ ($enableSharing$ == true)
            this.interactionRequests.Add( this.share );$endif$$if$ ($enableOpenFile$ == true)
            this.commands.Add( new NamedCommand<object>( "OpenFile", "Open File", this.OnOpenFile ) );$endif$$if$ ($enableSaveFile$ == true)
            this.commands.Add( new NamedCommand<object>( "SaveFile", "Save File", this.OnSaveFile, this.OnCanSaveFile ) );$endif$$if$ ($enableSelectFolder$ == true)
            this.commands.Add( new NamedCommand<object>( "SelectFolder", "Select Folder", this.OnSelectFolder ) );$endif$$if$ ($enableSharing$ == true)
            this.commands.Add( new NamedCommand<IDataRequest>( "Share", this.OnShare ) );$endif$
        }

        /// <summary>
        /// Gets the collection of view model interaction requests.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="IInteractionRequest">interaction requests</see>.</value>
        public ObservableKeyedCollection<string, IInteractionRequest> InteractionRequests
        {
            get
            {
                Contract.Ensures( this.interactionRequests != null );
                return this.interactionRequests;
            }
        }

        /// <summary>
        /// Requests an alert be displayed to a user.
        /// </summary>
        /// <param name="message">The alert message.</param>
        protected void Alert( string message )
        {
            Contract.Requires( message != null );
            this.Alert( this.Title, message );
        }

        /// <summary>
        /// Requests an alert be displayed to a user.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        protected void Alert( string title, string message )
        {
            Contract.Requires( !string.IsNullOrEmpty( title ) );
            Contract.Requires( message != null );
            this.userFeedback.Request( new Interaction( title, message ) );
        }

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

            this.userFeedback.Request( interaction );
        }$if$ ($enableAppSharing$ == true)

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
        protected void Share()
        {
            this.share.Request( new Interaction() );
        }$endif$$if$ ($enableOpenFile$ == true)

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
                    new NamedCommand<object>( "Open", DefaultAction.None ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            this.openFile.Request( interaction );
        }

        private void OnFilesOpened( IFileOpenPickerContinuationEventArgs e )
        {
            Contract.Requires( e != null );$endif$$if$ ($showOpenFileTips$ == true)
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
                    new NamedCommand<object>( "Save", DefaultAction.None ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            this.saveFile.Request( interaction );
        }

        private void OnFileSaved( IFileSavePickerContinuationEventArgs e )
        {
            Contract.Requires( e != null );$endif$$if$ ($showSaveFileTips$ == true)
            // TODO: save contents to created file$endif$$if$ ($enableSaveFile$ == true)
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
                    new NamedCommand<object>( "Select", DefaultAction.None ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            this.selectFolder.Request( interaction );
        }

        private void OnFolderSelected( IFolderPickerContinuationEventArgs e )
        {
            Contract.Requires( e != null );$endif$$if$ ($showSelectFolderTips$ == true)
            // TODO: use selected folder$endif$$if$ ($enableSelectFolder$ == true)
        }$endif$
    }
}