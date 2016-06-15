namespace More.Windows.Interactivity
{
    using Microsoft.Win32;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Interop;
    using static System.Environment;
    using static System.Windows.Window;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to select a folder for the
    /// <see cref="SelectFolderInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class SelectFolderAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        private const int WinXP = 6;
        private const string LegacyFolderBrowserDialogTypeName = "System.Windows.Forms.FolderBrowserDialog, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private const string LegacyNativeWindowTypeName = "System.Windows.Forms.NativeWindow, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private const string LegacyWin32WindowTypeName = "System.Windows.Forms.IWin32Window, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        /// <summary>
        /// Gets or sets the root folder where the browsing starts from.
        /// </summary>
        /// <value>One of the <see cref="T:System.Environment+SpecialFolder"/> values. The default value is
        /// <see cref="F:System.Environment+SpecialFolder.Desktop"/>.</value>
        public SpecialFolder RootFolder
        {
            get;
            set;
        }

        private static object CreateLegacyWindowOwner( Window window )
        {
            if ( window == null )
                return null;

            var nativeWindowType = Type.GetType( LegacyNativeWindowTypeName, true, false );
            var nativeWindow = Activator.CreateInstance( nativeWindowType );
            var helper = new WindowInteropHelper( window );

            nativeWindowType.GetMethod( "AssignHandle" ).Invoke( nativeWindow, new object[] { helper.Handle } );
            return nativeWindow;
        }

        private DirectoryInfo LegacySelectFolder( SelectFolderInteraction selectFolder )
        {
            Contract.Requires( selectFolder != null );

            // note: this code path is highly unlikely to be used anymore.  use reflection to take an indirect dependency on
            // on the System.Windows.Form BCL library.
            var dialogType = Type.GetType( LegacyFolderBrowserDialogTypeName, true, false );
            var dialog = Activator.CreateInstance( dialogType );

            dialogType.GetProperty( "Description" ).SetValue( dialog, selectFolder.Title, null );
            dialogType.GetProperty( "RootFolder " ).SetValue( dialog, RootFolder, null );

            if ( selectFolder.Folder != null )
                dialogType.GetProperty( "SelectedPath" ).SetValue( dialog, selectFolder.Folder.Name, null );

            const int OK = 1;
            var owner = CreateLegacyWindowOwner( Window.GetWindow( AssociatedObject ) );
            string selectedPath = null;

            try
            {
                var win32WindowType = Type.GetType( LegacyWin32WindowTypeName, true, false );
                var argTypes = new[] { win32WindowType };
                var method = dialogType.GetMethod( "ShowDialog", argTypes );
                var args = new object[] { owner };
                var result = method.Invoke( dialog, args );
                var dialogResult = Convert.ToInt32( result, CultureInfo.CurrentCulture );

                if ( dialogResult == OK )
                    selectedPath = (string) dialogType.GetProperty( "SelectedPath" ).GetValue( dialog, null );
            }
            finally
            {
                if ( owner != null )
                    owner.GetType().GetMethod( "ReleaseHandle" ).Invoke( owner, null );
            }

            if ( string.IsNullOrEmpty( selectedPath ) )
                return null;

            return new DirectoryInfo( selectedPath );
        }

        private DirectoryInfo SelectFolder( SelectFolderInteraction selectFolder )
        {
            Contract.Requires( selectFolder != null );

            var dialog = new FolderBrowserDialog();

            dialog.Title = selectFolder.Title;
            dialog.RootFolder = RootFolder;

            if ( selectFolder.Folder != null )
                dialog.SelectedPath = selectFolder.Folder.Path;

            var owner = GetWindow( AssociatedObject );
            var result = dialog.ShowDialog( owner ) ?? false;

            if ( result )
                return new DirectoryInfo( dialog.SelectedPath );

            return null;
        }

        private static void InvokeCallbackCommand( SelectFolderInteraction selectedFolder, DirectoryInfo folder )
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
                return;

            DirectoryInfo folder = null;

            if ( OSVersion.Version.Major < WinXP )
                folder = LegacySelectFolder( selectFolder );
            else
                folder = SelectFolder( selectFolder );

            InvokeCallbackCommand( selectFolder, folder );
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="parameter">The parameter supplied from the corresponding trigger.</param>
        /// <remarks>This method is not meant to be called directly by your code.</remarks>
        protected sealed override void Invoke( object parameter )
        {
            var args = parameter as InteractionRequestedEventArgs;

            if ( args != null )
                Invoke( args );
        }
    }
}
