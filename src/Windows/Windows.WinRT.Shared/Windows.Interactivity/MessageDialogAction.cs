namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Input;
    using System.Threading.Tasks;    
    using global::Windows.Foundation;
    using global::Windows.UI.Popups;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to show the <see cref="Interaction">interaction</see>
    /// from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class MessageDialogAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Prompts a user with an alert for the specified <see cref="Interaction">interaction</see> using a <see cref="MessageDialog">message box</see>.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction">interaction</see> to display.</param>
        /// <returns>An object representing the <see cref="IAsyncOperation{T}">asynchronous operation</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual IAsyncOperation<IUICommand> AlertAsync( Interaction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            Contract.Ensures( Contract.Result<IAsyncOperation<IUICommand>>() != null );

            var content = interaction.Content == null ? string.Empty : interaction.Content.ToString();
            var dialog = new MessageDialog( content, interaction.Title );

            dialog.DefaultCommandIndex = 0;

            if ( interaction.Commands.Count == 0 )
                dialog.Commands.Add( new UICommand( SR.OKCaption, DefaultAction.None ) );
            else
                dialog.Commands.Add( interaction.Commands[0].AsUICommand() );

            return dialog.ShowAsync();
        }

        /// <summary>
        /// Prompts a user with the specified <see cref="Interaction">interaction</see> using a <see cref="MessageDialog">message box</see>.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction">interaction</see> to display.</param>
        /// <returns>An object representing the <see cref="IAsyncOperation{T}">asynchronous operation</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual IAsyncOperation<IUICommand> PromptAsync( Interaction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            Contract.Ensures( Contract.Result<IAsyncOperation<IUICommand>>() != null );

            var content = interaction.Content == null ? string.Empty : interaction.Content.ToString();
            var dialog = new MessageDialog( content, interaction.Title );

            if ( interaction.DefaultCommandIndex >= 0 )
                dialog.DefaultCommandIndex = (uint) interaction.DefaultCommandIndex;

            if ( interaction.CancelCommandIndex >= 0 )
                dialog.CancelCommandIndex = (uint) interaction.CancelCommandIndex;

            dialog.Commands.AddRange( interaction.Commands.Select( c => c.AsUICommand() ) );

            return dialog.ShowAsync();
        }

        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The <see cref="FrameworkElement"/> that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            if ( parameter == null )
                return;

            var interaction = GetRequestedInteraction<Interaction>( parameter );

            if ( interaction.Commands.Count > 1 )
                await PromptAsync( interaction );
            else
                await AlertAsync( interaction );
        }
    }
}
