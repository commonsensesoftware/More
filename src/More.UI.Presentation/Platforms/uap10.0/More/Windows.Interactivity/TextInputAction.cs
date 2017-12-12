namespace More.Windows.Interactivity
{
    using More.Windows.Controls;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction">interactivity action</see> that can be used to show the <see cref="TextInputInteraction">interaction</see>
    /// from an <see cref="IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class TextInputAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            var textInput = GetRequestedInteraction<TextInputInteraction>( parameter );

            if ( textInput == null )
            {
                return;
            }

            var dialog = new TextInputDialog();
            var commands = textInput.Commands.DelayAll().ToArray();

            dialog.Title = textInput.Title;
            dialog.Content = textInput.Content;
            dialog.DefaultResponse = textInput.DefaultResponse;
            dialog.DefaultCommandIndex = textInput.DefaultCommandIndex;
            dialog.CancelCommandIndex = textInput.CancelCommandIndex;
            dialog.Commands.AddRange( commands );

            var response = await dialog.ShowAsync();

            textInput.Response = response;
            commands.InvokeExecuted();
        }
    }
}