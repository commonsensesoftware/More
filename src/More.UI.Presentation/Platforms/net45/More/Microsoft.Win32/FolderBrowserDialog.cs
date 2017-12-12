namespace Microsoft.Win32
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Security.Permissions;
    using static FileDialogOptions;
    using static NativeMethods;
    using static ShellItemDisplayName;
    using static System.Environment;
    using static System.Environment.SpecialFolder;
    using static System.IO.Path;
    using static System.Runtime.InteropServices.Marshal;

    /// <summary>
    /// Represents a folder browser dialog.
    /// </summary>
    public class FolderBrowserDialog : CommonDialog
    {
        string title = string.Empty;
        string selectedPath = string.Empty;
        SpecialFolder rootFolder = Desktop;

        /// <summary>
        /// Gets or sets the descriptive text displayed in the dialog box title.
        /// </summary>
        /// <value>The title to display. The default is an empty string ("").</value>
        [DefaultValue( "" )]
        [Browsable( true )]
        [Localizable( true )]
        [Category( "Folder Browsing" )]
        [Description( "The string that is displayed above the title of the dialog box." )]
        public string Title
        {
            get
            {
                Contract.Ensures( title != null );
                return title;
            }
            set => title = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the root folder where the browsing starts from.
        /// </summary>
        /// <value>One of the <see cref="System.Environment.SpecialFolder" /> values. The default is <see cref="System.Environment.SpecialFolder.Desktop" />.</value>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="System.Environment.SpecialFolder" /> values. </exception>
        [Browsable( true )]
        [Localizable( false )]
        [DefaultValue( 0 )]
        [Category( "Folder Browsing" )]
        [Description( "The location of the root folder from which to start browsing. Only the specified folder and any subfolders that are beneath it will appear in the dialog box." )]
        [TypeConverter( "System.Windows.Forms.SpecialFolderEnumConverter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" )]
        public SpecialFolder RootFolder
        {
            get => rootFolder;
            set
            {
                if ( !Enum.IsDefined( typeof( SpecialFolder ), value ) )
                {
                    throw new InvalidEnumArgumentException( nameof( value ), (int) value, typeof( SpecialFolder ) );
                }

                rootFolder = value;
            }
        }

        /// <summary>
        /// Gets or sets the path selected by the user.
        /// </summary>
        /// <value>The path of the folder first selected in the dialog box or the last folder selected by the user.
        /// The default is an empty string ("").</value>
        [DefaultValue( "" )]
        [Browsable( true )]
        [Localizable( true )]
        [Category( "CatFolderBrowsing" )]
        [Description( "FolderBrowserDialogSelectedPath" )]
        [Editor( "System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" )]
        public string SelectedPath
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return selectedPath;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                selectedPath = value;
            }
        }

        static IShellItem GetFolder( SpecialFolder rootFolder, string selectedPath )
        {
            Contract.Requires( selectedPath != null );
            Contract.Ensures( Contract.Result<IShellItem>() != null );

            if ( selectedPath.Length == 0 )
            {
                selectedPath = GetFolderPath( rootFolder );
            }
            else if ( !Directory.Exists( selectedPath ) )
            {
                if ( File.Exists( selectedPath ) )
                {
                    selectedPath = GetDirectoryName( selectedPath );
                }
                else
                {
                    selectedPath = GetDirectoryName( GetFolderPath( rootFolder ) );
                }
            }

            return CreateItemFromParsingName( selectedPath );
        }

        static string GetSelectedPath( IFileOpenDialog fileDialog )
        {
            fileDialog.GetResult( out var item );
            item.GetDisplayName( FileSystemPath, out var path );
            FinalReleaseComObject( item );

            return path;
        }

        static IFileOpenDialog CreateDialog()
        {
            Contract.Ensures( Contract.Result<IFileOpenDialog>() != null );

            var options = PickFolders | ForceFileSystem | PathMustExist;
            var dialog = new IFileOpenDialog();
            dialog.SetOptions( options );

            return dialog;
        }

        /// <summary>
        /// Resets the properties of a common dialog to their default values.
        /// </summary>
        public override void Reset()
        {
            Title = string.Empty;
            SelectedPath = string.Empty;
            RootFolder = Desktop;
        }

        /// <summary>
        /// Determines whether sufficient permissions for displaying a dialog exist.
        /// </summary>
        /// <remarks>The base implementation demands the <see cref="FileIOPermissionAccess.PathDiscovery">path discovery</see>
        /// <see cref="FileIOPermission">permission</see>.</remarks>
        protected override void CheckPermissionsToShowDialog()
        {
            var path = SelectedPath;

            if ( string.IsNullOrEmpty( path ) )
            {
                path = GetFolderPath( RootFolder );
            }

            new FileIOPermission( FileIOPermissionAccess.PathDiscovery, path ).Demand();
        }

        /// <summary>
        /// Displays a folder dialog of Win32 common dialog
        /// </summary>
        /// <param name="hwndOwner"><see cref="IntPtr">Handle</see> to the window that owns the dialog box.</param>
        /// <returns>If the user clicks the OK button of the dialog that is displayed, true is returned; otherwise, false.</returns>
        protected override bool RunDialog( IntPtr hwndOwner )
        {
            IShellItem folder = null;
            IFileOpenDialog dialog = null;
            var result = false;

            try
            {
                folder = GetFolder( RootFolder, SelectedPath );
                dialog = CreateDialog();
                dialog.SetTitle( Title );
                dialog.SetFolder( folder );
                dialog.SetFileName( string.Empty );

                var hresult = dialog.Show( hwndOwner );
                result = hresult == 0U;

                if ( result )
                {
                    SelectedPath = GetSelectedPath( dialog );
                }
            }
            finally
            {
                if ( folder != null )
                {
                    FinalReleaseComObject( folder );
                }

                if ( dialog != null )
                {
                    FinalReleaseComObject( dialog );
                }
            }

            return result;
        }
    }
}