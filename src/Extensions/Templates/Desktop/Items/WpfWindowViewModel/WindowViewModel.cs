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
    /// Represents a view model for a window.
    /// </summary>
    public class $safeitemrootname$ :  $base$
    {
        private readonly InteractionRequest<Interaction> userFeedback = new InteractionRequest<Interaction>( "UserFeedback" );
        //private readonly InteractionRequest<Window1ViewModel> showChild = new InteractionRequest<Window1ViewModel>( "ShowChildWindow" );$if$ ($enableOpenFile$ == true)
        private readonly InteractionRequest<OpenFileInteraction> openFile = new InteractionRequest<OpenFileInteraction>( "OpenFile" );$endif$$if$ ($enableSaveFile$ == true)
        private readonly InteractionRequest<SaveFileInteraction> saveFile = new InteractionRequest<SaveFileInteraction>( "SaveFile" );$endif$$if$ ($enableSelectFolder$ == true)
        private readonly InteractionRequest<SelectFolderInteraction> selectFolder = new InteractionRequest<SelectFolderInteraction>( "SelectFolder" );$endif$
        private readonly InteractionRequest<WindowCloseInteraction> close = new InteractionRequest<WindowCloseInteraction>( "CloseWindow" );
        private bool closing;
        private string titleField = "$title$";

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>
        public $safeitemrootname$()
        {$if$ ($showTips$ == true)
            // TODO: If this class has import dependencies, they can be specified in the constructor arguments
            //       example: public $safeitemrootname$( MyService service )

            // TODO: Add or modify this interaction requests and commands to suit your needs.$endif$
            InteractionRequests.Add( userFeedback );
            //InteractionRequests.Add( showChild );$if$ ($enableOpenFile$ == true)
            InteractionRequests.Add( openFile );$endif$$if$ ($enableSaveFile$ == true)
            InteractionRequests.Add( saveFile );$endif$$if$ ($enableSelectFolder$ == true)
            InteractionRequests.Add( selectFolder );$endif$
            InteractionRequests.Add( close );
            //Commands.Add( new NamedCommand<object>( "Show", OnShowChildWindow ) );$if$ ($enableOpenFile$ == true)
            Commands.Add( new NamedCommand<object>( "OpenFile", "Open File", OnOpenFile ) );$endif$$if$ ($enableSaveFile$ == true)
            Commands.Add( new NamedCommand<object>( "SaveFile", "Save File", OnSaveFile, OnCanSaveFile ) );$endif$$if$ ($enableSelectFolder$ == true)
            Commands.Add( new NamedCommand<object>( "SelectFolder", "Select Folder", OnSelectFolder ) );$endif$
            DialogCommands.Add( new NamedCommand<object>( "OK", OnAccept, OnCanAccept ) );
            DialogCommands.Add( new NamedCommand<object>( "Cancel", OnCancel ) );
        }

        //private void OnShowChildWindow( object parameter )
        //{
        //    var interaction = new Window1ViewModel()
        //    {
        //        Commands =
        //        {
        //            new NamedCommand<object>( "Yes", DefaultAction.None ),
        //            new NamedCommand<object>( "No", DefaultAction.None ),
        //        }
        //    };
        
        //    showChild.Request( interaction );
        //}

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
        /// <remarks>These commands do not control the behavior of a view.</remarks>
        public ObservableKeyedCollection<string, INamedCommand> Commands
        {
            get;
        } = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );

        /// <summary>
        /// Gets the collection of view model dialog commands.
        /// </summary>
        /// <value>A <see cref="ObservableKeyedCollection{TKey,TValue}">keyed collection</see> of
        /// <see cref="INamedCommand">commands</see>.</value>
        /// <remarks>These commands control the behavior of a view, such as cancelling it.</remarks>
        public ObservableKeyedCollection<string, INamedCommand> DialogCommands
        {
            get;
        } = new ObservableKeyedCollection<string, INamedCommand>( c => c.Id );

        private void OnAccept( object parameter )
        {$if$ ($showTips$ == true)
            // TODO: insert your logic when the view model is accepted (e.g. clicked 'OK')$endif$
            Close();
        }

        private bool OnCanAccept( object parameter )
        {$if$ ($showTips$ == true)
            // TODO: insert your logic when the view model can be accepted (e.g. 'OK' is enabled/disabled)$endif$
            return true;
        }

        private void OnCancel( object parameter )
        {
            // only process cancel request once
            if ( closing )
            {
                return;
            }$if$ ($showTips$ == true)

            // TODO: insert any custom logic which may not require user feedback in order to cancel$endif$

            // ask the user to confirm they want to cancel. note: parameter will be CancelEventArgs if WindowCloseBehavior
            // is applied and the user closed the window (e.g. clicked 'X'). with the behavior applied and bound to a
            // command, giving the view model a chance to respond to the close/cancel action.
            var cancelArgs = parameter as CancelEventArgs ?? new CancelEventArgs();
            var interaction = new Interaction()
            {
                Title = Title,
                Content = "Are you sure you want to cancel?",
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Yes", OnCancelConfirmed ),
                    new NamedCommand<object>( "No", p => cancelArgs.Cancel = true )
                }
            };

            userFeedback.Request( interaction );
        }

        private void OnCancelConfirmed( object parameter )
        {$if$ ($showTips$ == true)
            // TODO: insert your cancellation logic$endif$
            Close( true );
        }

        /// <summary>
        /// Requests the associated window be closed.
        /// </summary>
        protected void Close() => Close( false );

        /// <summary>
        /// Requests the associated window be closed.
        /// </summary>
        /// <param name="canceled">Indicates whether the orginally requested operation was canceled.</param>
        protected void Close( bool canceled )
        {
            closing = true;
            close.Request( new WindowCloseInteraction( canceled ) );
        }

        /// <summary>
        /// Requests an alert be displayed to a user.
        /// </summary>
        /// <param name="message">The alert message.</param>
        protected void Alert( string message ) => Alert( Title, message );

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