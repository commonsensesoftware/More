namespace More.Windows.Interactivity
{
    using Microsoft.Win32;
    using More.IO;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Interop;
    using static System.Environment;
    using static System.Linq.Expressions.Expression;
    using static System.Reflection.BindingFlags;
    using static System.Windows.Window;
    using Expression = System.Linq.Expressions.Expression;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to select a folder for the
    /// <see cref="SelectFolderInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class SelectFolderAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        const int WinXP = 5;
        const string LegacyFolderBrowserDialogTypeName = "System.Windows.Forms.FolderBrowserDialog, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        const string LegacyDialogResultTypeName = "System.Windows.Forms.DialogResult, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        const string LegacyNativeWindowTypeName = "System.Windows.Forms.NativeWindow, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        const string LegacyWin32WindowTypeName = "System.Windows.Forms.IWin32Window, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        static readonly Lazy<Func<SelectFolderAction, SelectFolderInteraction, DirectoryInfo>> legacySelectFolder = new Lazy<Func<SelectFolderAction, SelectFolderInteraction, DirectoryInfo>>( NewLegacySelectFolderFunc );

        /// <summary>
        /// Gets or sets the root folder where the browsing starts from.
        /// </summary>
        /// <value>One of the <see cref="T:System.Environment+SpecialFolder"/> values. The default value is
        /// <see cref="F:System.Environment+SpecialFolder.Desktop"/>.</value>
        public SpecialFolder RootFolder { get; set; }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="args">The <see cref="InteractionRequestedEventArgs"/> event data provided by the corresponding trigger.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Invoke( InteractionRequestedEventArgs args )
        {
            Arg.NotNull( args, nameof( args ) );

            var selectFolder = args.Interaction as SelectFolderInteraction;

            if ( selectFolder == null )
            {
                return;
            }

            var folder = default( DirectoryInfo );

            if ( OSVersion.Version.Major > WinXP )
            {
                folder = SelectFolder( selectFolder );
            }
            else
            {
                folder = legacySelectFolder.Value( this, selectFolder );
            }

            InvokeCallbackCommand( selectFolder, folder );
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="parameter">The parameter supplied from the corresponding trigger.</param>
        /// <remarks>This method is not meant to be called directly by your code.</remarks>
        protected sealed override void Invoke( object parameter )
        {
            if ( parameter is InteractionRequestedEventArgs args )
            {
                Invoke( args );
            }
        }

        DirectoryInfo SelectFolder( SelectFolderInteraction selectFolder )
        {
            Contract.Requires( selectFolder != null );

            var dialog = new FolderBrowserDialog()
            {
                Title = selectFolder.Title,
                RootFolder = RootFolder
            };

            if ( selectFolder.Folder != null )
            {
                dialog.SelectedPath = selectFolder.Folder.Path;
            }

            var owner = GetWindow( AssociatedObject );
            var result = dialog.ShowDialog( owner ) ?? false;

            if ( result )
            {
                return new DirectoryInfo( dialog.SelectedPath );
            }

            return null;
        }

        static void InvokeCallbackCommand( SelectFolderInteraction selectedFolder, DirectoryInfo folder )
        {
            Contract.Requires( selectedFolder != null );

            if ( folder == null )
            {
                selectedFolder.ExecuteCancelCommand();
            }
            else
            {
                selectedFolder.Folder = folder.AsFolder();
                selectedFolder.ExecuteDefaultCommand();
            }
        }

        static Func<SelectFolderAction, SelectFolderInteraction, DirectoryInfo> NewLegacySelectFolderFunc()
        {
            Contract.Ensures( Contract.Result<Func<SelectFolderAction, SelectFolderInteraction, DirectoryInfo>>() != null );

            // INFO: the expressions below create the following function:

            /* ( SelectFolderAction action, SelectFolderInteraction selectFolder ) =>
             * {
             *  FolderBrowserDialog dialog;
             *  DialogResult dialogResult;
             *  NativeWindow owner;
             *  Window window;
             *  WindowInteropHelper helper;
             *  string selectedPath;
             *  DirectoryInfo selectedFolder;
             * 
             *  dialog = new FolderBrowserDialog() { Description = selectFolder.Title, RootFolder = action.RootFolder };
             *  window = Window.GetWindow( action.AssociatedObject );
             *  
             *  if ( window != null )
             *  {
             *      owner = new NativeWindow();
             *      helper = new WindowInteropHelper( window );
             *      owner.AssignHandle( helper.Handle );
             *  }
             * 
             *  try
             *  {
             *      dialogResult = dialog.ShowDialog( owner );
             *      
             *      if ( dialogResult = DialogResult.OK )
             *      {
             *          selectedPath = dialog.SelectedPath;
             *      }
             *  }
             *  finally
             *  {
             *      dialog.Dispose();
             *      
             *      if ( owner != null )
             *      {
             *          owner.ReleaseHandle();
             *      }
             *  }
             *  
             *  if ( !string.IsNullOrEmpty( selectedPath ) )
             *  {
             *      selectedFolder = new DirectoryInfo( selectedPath );
             *  }
             *  
             *  return selectedFolder;
             * }
             */

            var action = Parameter( typeof( SelectFolderAction ), "action" );
            var selectFolder = Parameter( typeof( SelectFolderInteraction ), "selectFolder" );
            var dialogType = Type.GetType( LegacyFolderBrowserDialogTypeName, true, false );
            var dialog = Variable( dialogType, "dialog" );
            var showDialog = dialogType.GetMethod( "ShowDialog", new[] { Type.GetType( LegacyWin32WindowTypeName, true, false ) } );
            var dialogResult = Variable( showDialog.ReturnType, "dialogResult" );
            var nativeWindowType = Type.GetType( LegacyNativeWindowTypeName, true, false );
            var owner = Variable( nativeWindowType, "owner" );
            var newDialog = MemberInit(
                                New( dialogType ),
                                Bind( dialogType.GetProperty( "Description" ), Property( selectFolder, nameof( SelectFolderInteraction.Title ) ) ),
                                Bind( dialogType.GetProperty( "RootFolder" ), Property( action, nameof( RootFolder ) ) ) );
            var window = Variable( typeof( Window ), "window" );
            var helper = Variable( typeof( WindowInteropHelper ), "helper" );
            var selectedPath = Variable( typeof( string ), "selectedPath" );
            var selectedFolder = Variable( typeof( DirectoryInfo ), "selectedFolder" );
            var variables = new[] { dialog, dialogResult, owner, window, helper, selectedPath, selectedFolder };
            var body = Block(
                variables,
                new Expression[]
                {
                    Assign( dialog, newDialog ),
                    IfThen(
                            NotEqual( Property( selectFolder, nameof( SelectFolderInteraction.Folder ) ), Constant( default( IFolder ) ) ),
                            Assign( Property( dialog, "SelectedPath" ), Property( Property( selectFolder, nameof( SelectFolderInteraction.Folder ) ), nameof( IFolder.Name ) ) ) ),
                    Assign( window, Call( typeof( Window ).GetMethod( nameof( Window.GetWindow ) ), Property( action, typeof( SelectFolderAction ).GetProperty( nameof( AssociatedObject ),Instance | Public | NonPublic ) ) ) ),
                    IfThen(
                            NotEqual( window, Constant( default( Window ) ) ),
                            Block(
                                    Assign( owner, New( nativeWindowType ) ),
                                    Assign( helper, New( typeof( WindowInteropHelper ).GetConstructors().Single(), window ) ),
                                    Call( owner, "AssignHandle", new []{ typeof( IntPtr ) }, Property( helper, nameof( WindowInteropHelper.Handle ) ) ) ) ),
                    TryFinally(
                        Block(
                            Assign( dialogResult, Call( dialog, showDialog, owner ) ),
                            IfThen(
                                    Equal( dialogResult, Constant( Enum.Parse( Type.GetType( LegacyDialogResultTypeName ), "OK" ) ) ),
                                    Assign( selectedPath, Property( dialog, "SelectedPath" ) ) )
                            ),
                        Block(
                            Call( dialog, "Dispose", Type.EmptyTypes ),
                            IfThen(
                                    NotEqual( owner, Constant( null ) ),
                                    Call( owner, "ReleaseHandle", Type.EmptyTypes ) )
                            ) ),
                    IfThen(
                            Not( Call( typeof( string ).GetMethod( nameof( string.IsNullOrEmpty ) ), selectedPath ) ),
                            Assign( selectedFolder, New( typeof( DirectoryInfo ).GetConstructor( new []{ typeof( string ) } ), selectedPath ) ) ),
                    selectedFolder
                } );
            var lambda = Lambda<Func<SelectFolderAction, SelectFolderInteraction, DirectoryInfo>>( body, action, selectFolder );

            return lambda.Compile();
        }
    }
}