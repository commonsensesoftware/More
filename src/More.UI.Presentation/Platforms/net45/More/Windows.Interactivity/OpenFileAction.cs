namespace More.Windows.Interactivity
{
    using Microsoft.Win32;
    using More.IO;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction{T}">interactivity action</see> that can be used to open a file for the
    /// <see cref="OpenFileInteraction">interaction</see> received from an <see cref="IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class OpenFileAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        IEnumerable<IFile> OpenFiles( OpenFileInteraction openFile )
        {
            Contract.Requires( openFile != null );
            Contract.Ensures( Contract.Result<IEnumerable<IFile>>() != null );

            var dialog = new OpenFileDialog()
            {
                Filter = openFile.FileTypeFilter.ToFileFilter(),
                Multiselect = openFile.Multiselect,
                Title = openFile.Title,
            };

            if ( !string.IsNullOrEmpty( dialog.Filter ) )
            {
                dialog.FilterIndex = 1;
            }

            var owner = Window.GetWindow( AssociatedObject );
            var result = dialog.ShowDialog( owner ) ?? false;

            if ( result )
            {
                return dialog.FileNames.Select( f => new FileInfo( f ).AsFile() );
            }

            return new IFile[0];
        }

        void InvokeCallbackCommand( OpenFileInteraction openFile )
        {
            Contract.Requires( openFile != null );

            var files = OpenFiles( openFile );

            if ( files.Any() )
            {
                openFile.Files.ReplaceAll( files );
                openFile.ExecuteDefaultCommand();
            }
            else
            {
                openFile.ExecuteCancelCommand();
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

            if ( args.Interaction is OpenFileInteraction openFile )
            {
                InvokeCallbackCommand( openFile );
            }
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
    }
}