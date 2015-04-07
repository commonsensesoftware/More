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
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

	/// <summary>
    /// Represents a view model for a subordinate, child window.
    /// </summary>
    public class $safeitemrootname$ :  $base$
    {
        private readonly InteractionRequest<Interaction> userFeedback = new InteractionRequest<Interaction>( "UserFeedback" );$if$ ($enableOpenFile$ == true)
        private readonly InteractionRequest<OpenFileInteraction> openFile = new InteractionRequest<OpenFileInteraction>( "OpenFile" );$endif$$if$ ($enableSaveFile$ == true)
        private readonly InteractionRequest<SaveFileInteraction> saveFile = new InteractionRequest<SaveFileInteraction>( "SaveFile" );$endif$$if$ ($enableSelectFolder$ == true)
        private readonly InteractionRequest<SelectFolderInteraction> selectFolder = new InteractionRequest<SelectFolderInteraction>( "SelectFolder" );$endif$
        private readonly ObservableKeyedCollection<string, IInteractionRequest> interactionRequests = new ObservableKeyedCollection<string, IInteractionRequest>( r => r.Id );
        private readonly ObservableKeyedCollection<string, INamedCommand> commands = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>
        public $safeitemrootname$()
            : base( "$title$" )
        {$if$ ($showTips$ == true)

            // TODO: Add or modify this interaction requests and commands to suit your needs.$endif$
            this.interactionRequests.Add( this.userFeedback );$if$ ($enableOpenFile$ == true)
            this.interactionRequests.Add( this.openFile );$endif$$if$ ($enableSaveFile$ == true)
            this.interactionRequests.Add( this.saveFile );$endif$$if$ ($enableSelectFolder$ == true)
            this.interactionRequests.Add( this.selectFolder );$endif$$if$ ($enableOpenFile$ == true)
            this.commands.Add( new NamedCommand<object>( "OpenFile", "Open File", this.OnOpenFile ) );$endif$$if$ ($enableSaveFile$ == true)
            this.commands.Add( new NamedCommand<object>( "SaveFile", "Save File", this.OnSaveFile, this.OnCanSaveFile ) );$endif$$if$ ($enableSelectFolder$ == true)
            this.commands.Add( new NamedCommand<object>( "SelectFolder", "Select Folder", this.OnSelectFolder ) );$endif$
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
        /// Gets the collection of view model interaction commands.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="INamedCommand">commands</see>.</value>
        /// <remarks>These commands do not control the behavior of a view.</remarks>
        public ObservableKeyedCollection<string, INamedCommand> InteractionCommands
        {
            get
            {
                Contract.Ensures( this.commands != null );
                return this.commands;
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
        }$if$ ($enableOpenFile$ == true)

        private void OnOpenFile( object parameter )
        {
            OpenFileInteraction interaction = null;
            interaction = new OpenFileInteraction()
            {
                Title = "Open File",
                Multiselect = false,
                FileTypeFilter =
                {
                    "Text Files (*.txt)",
                    "*.txt",
                    "All Files (*.*)",
                    "*.*"
                },
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Open", p => this.OnFilesOpened( interaction.Files ) ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            this.openFile.Request( interaction );
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
                    new NamedCommand<object>( "Save", p => this.OnFileSaved( interaction.SavedFile ) ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            this.saveFile.Request( interaction );
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
                    new NamedCommand<object>( "Select", p => this.OnFolderSelected( interaction.Folder ) ),
                    new NamedCommand<object>( "Cancel", DefaultAction.None )
                }
            };

            this.selectFolder.Request( interaction );
        }

        private void OnFolderSelected( DirectoryInfo folder )
        {
            Contract.Requires( folder != null );$endif$$if$ ($showSelectFolderTips$ == true)
            // TODO: use selected folder$endif$$if$ ($enableSelectFolder$ == true)
        }$endif$
    }
}