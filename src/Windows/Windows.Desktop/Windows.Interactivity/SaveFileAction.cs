namespace More.Windows.Interactivity
{
    using Microsoft.Win32;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction{T}">interactivity action</see> that can be used to save a file for the
    /// <see cref="SaveFileInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class SaveFileAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        private static IEnumerable<string> FixUpAsterisks( IEnumerable<string> extensions )
        {
            Contract.Requires( extensions != null );
            Contract.Ensures( Contract.Result<IEnumerable<string>>() != null );

            foreach ( var extension in extensions )
            {
                if ( !extension.StartsWith( "*", StringComparison.OrdinalIgnoreCase ) )
                    yield return "*" + extension;

                yield return extension;
            }
        }

        private static string CreateFilter( SaveFileInteraction notification )
        {
            Contract.Requires( notification != null );

            var culture = System.Globalization.CultureInfo.InvariantCulture;
            var fileTypes = from choice in notification.FileTypeChoices
                            where choice.Value != null && choice.Value.Any()
                            let fileExts = string.Join( ";", FixUpAsterisks( choice.Value ) )
                            let fileType = string.Format( culture, "{0} ({1})|{1}", choice.Key, fileExts )
                            select fileType;

            return string.Join( "|", fileTypes );
        }

        private static SaveFileDialog CreateDialog( SaveFileInteraction saveFile )
        {
            Contract.Requires( saveFile != null );
            Contract.Ensures( Contract.Result<SaveFileDialog>() != null );

            // legacy dialogs use a pipe-separated filter
            var filter = CreateFilter( saveFile );
            var dialog = new SaveFileDialog();

            dialog.DefaultExt = saveFile.DefaultFileExtension;
            dialog.Filter = filter;

            // always default to first index, if there is one
            if ( !string.IsNullOrEmpty( filter ) )
                dialog.FilterIndex = 1;

            if ( !string.IsNullOrEmpty( saveFile.FileName ) )
                dialog.FileName = saveFile.FileName;

            dialog.Title = saveFile.Title;

            return dialog;
        }

        private void InvokeCallbackCommand( SaveFileInteraction saveFile )
        {
            Contract.Requires( saveFile != null );

            var owner = Window.GetWindow( this.AssociatedObject );
            var dialog = CreateDialog( saveFile );
            var result = dialog.ShowDialog( owner ) ?? false;

            if ( result )
            {
                saveFile.FileName = dialog.FileName ?? string.Empty;
                saveFile.SavedFile = new SavedFileAdapter( dialog.FileName, dialog.OpenFile );

                // execute accept
                saveFile.ExecuteDefaultCommand();
            }
            else
            {
                // execute cancel
                saveFile.ExecuteCancelCommand();
            }
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="args">The <see cref="InteractionRequestedEventArgs"/> event data provided by the corresponding trigger.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Invoke( InteractionRequestedEventArgs args )
        {
            Contract.Requires<ArgumentNullException>( args != null, "args" );

            var saveFile = args.Interaction as SaveFileInteraction;

            if ( saveFile != null )
                this.InvokeCallbackCommand( saveFile );
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
                this.Invoke( args );
        }
    }
}
