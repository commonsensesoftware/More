namespace More.Windows.Interactivity
{
    using More.Windows.Controls;
    using More.Windows.Input;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction">interactivity action</see> that can be used to show the <see cref="TextInputInteraction">interaction</see>
    /// from an <see cref="IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public partial class TextInputAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="args">The <see cref="InteractionRequestedEventArgs"/> event data provided by the corresponding trigger.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Invoke( InteractionRequestedEventArgs args )
        {
            Arg.NotNull( args, nameof( args ) );

            var textInput = args.Interaction as TextInputInteraction;

            if ( textInput == null )
            {
                return;
            }

            var dialog = new TextInputDialog();
            var commands = textInput.Commands.DelayAll().ToArray();
            var behavior = new WindowCloseBehavior() { CloseCommand = textInput.CancelCommand };

            System.Windows.Interactivity.Interaction.GetBehaviors( dialog ).Add( behavior );

            dialog.Owner = Window.GetWindow( AssociatedObject );
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Title = textInput.Title;
            dialog.Content = textInput.Content;
            dialog.DefaultResponse = textInput.DefaultResponse;
            dialog.DefaultCommandIndex = textInput.DefaultCommandIndex;
            dialog.CancelCommandIndex = textInput.CancelCommandIndex;
            dialog.Commands.AddRange( commands );
            dialog.ShowDialog();

            textInput.Response = dialog.Response;
            commands.InvokeExecuted();
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