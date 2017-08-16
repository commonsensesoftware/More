namespace More.Windows.Interactivity
{
    using Microsoft.Win32;
    using Input;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction{T}">interactivity action</see> that can be used to save a file for the
    /// <see cref="SaveFileInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class SaveFileAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        static SaveFileDialog CreateDialog( SaveFileInteraction saveFile )
        {
            Contract.Requires( saveFile != null );
            Contract.Ensures( Contract.Result<SaveFileDialog>() != null );

            var dialog = new SaveFileDialog()
            {
                DefaultExt = saveFile.DefaultFileExtension,
                Filter = saveFile.FileTypeChoices.ToFileFilter(),
                Title = saveFile.Title
            };

            if ( !string.IsNullOrEmpty( dialog.Filter ) )
            {
                dialog.FilterIndex = 1;
            }

            if ( !string.IsNullOrEmpty( saveFile.FileName ) )
            {
                dialog.FileName = saveFile.FileName;
            }

            return dialog;
        }

        void InvokeCallbackCommand( SaveFileInteraction saveFile )
        {
            Contract.Requires( saveFile != null );

            var owner = Window.GetWindow( AssociatedObject );
            var dialog = CreateDialog( saveFile );
            var result = dialog.ShowDialog( owner ) ?? false;

            if ( result )
            {
                saveFile.FileName = dialog.FileName ?? string.Empty;
                saveFile.SavedFile = new SavedFileAdapter( dialog.FileName, dialog.OpenFile );
                saveFile.ExecuteDefaultCommand();
            }
            else
            {
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
            Arg.NotNull( args, nameof( args ) );

            if ( args.Interaction is SaveFileInteraction saveFile )
            {
                InvokeCallbackCommand( saveFile );
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