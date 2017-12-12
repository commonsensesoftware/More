namespace More.Windows.Interactivity
{
    using Controls;
    using Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction{T}">interactivity action</see> that can be used to show the
    /// <see cref="Interaction">interaction</see> from an <see cref="IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class MessageDialogAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Prompts a user with an alert for the specified <see cref="Interaction">interaction</see> using a <see cref="MessageBox">message box</see>.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction">interaction</see> to display.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Alert( Interaction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );

            var dialog = new MessageDialog();

            dialog.Title = interaction.Title;
            dialog.Content = interaction.Content;
            dialog.DefaultCommandIndex = 0;
            dialog.Owner = Window.GetWindow( AssociatedObject );
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if ( interaction.Commands.Count == 0 )
            {
                dialog.Commands.Add( new NamedCommand<object>( SR.OKCaption, DefaultAction.None ) );
                dialog.ShowDialog();
            }
            else
            {
                var command = interaction.Commands[0].Delay();

                dialog.Commands.Add( command );
                dialog.ShowDialog();

                if ( command.HasExecuted )
                {
                    command.Invoke();
                }
            }
        }

        /// <summary>
        /// Prompts a user with the specified <see cref="Interaction">interaction</see> using a <see cref="MessageBox">message box</see>.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction">interaction</see> to display.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Prompt( Interaction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );

            var dialog = new MessageDialog();
            var commands = interaction.Commands.DelayAll().ToArray();
            var behavior = new WindowCloseBehavior() { CloseCommand = interaction.CancelCommand };

            System.Windows.Interactivity.Interaction.GetBehaviors( dialog ).Add( behavior );

            dialog.Owner = Window.GetWindow( AssociatedObject );
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Title = interaction.Title;
            dialog.Content = interaction.Content;
            dialog.DefaultCommandIndex = interaction.DefaultCommandIndex;
            dialog.CancelCommandIndex = interaction.CancelCommandIndex;
            dialog.Commands.AddRange( commands );
            dialog.ShowDialog();

            commands.InvokeExecuted();
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="args">The <see cref="InteractionRequestedEventArgs"/> event data provided by the corresponding trigger.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Invoke( InteractionRequestedEventArgs args )
        {
            Arg.NotNull( args, nameof( args ) );

            var interaction = args.Interaction;

            if ( interaction.Commands.Count > 1 )
            {
                Prompt( interaction );
            }
            else
            {
                Alert( interaction );
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