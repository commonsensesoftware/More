namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents an <see cref="T:System.Windows.Interactivity.TriggerAction{T}">interactivity action</see> that can be used to show the
    /// <see cref="Interaction">interaction</see> from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class MessageBoxAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        private static MessageBoxButton GetButtons( Interaction interaction )
        {
            Contract.Requires( interaction != null );

            var buttons = new List<MessageBoxResult>();

            // parse command names into message box result (buttons)
            foreach ( var command in interaction.Commands )
            {
                MessageBoxResult result;

                if ( Enum.TryParse<MessageBoxResult>( command.Name, true, out result ) )
                    buttons.Add( result );
            }

            // select enumeration based on button names
            if ( buttons.Count == 1 && buttons[0] == MessageBoxResult.OK )
            {
                return MessageBoxButton.OK;
            }
            else if ( buttons.Count == 2 )
            {
                if ( buttons.Contains( MessageBoxResult.OK ) && buttons.Contains( MessageBoxResult.Cancel ) )
                    return MessageBoxButton.OKCancel;

                if ( buttons.Contains( MessageBoxResult.Yes ) && buttons.Contains( MessageBoxResult.No ) )
                    return MessageBoxButton.YesNo;
            }
            else if ( buttons.Count == 3 && buttons.Contains( MessageBoxResult.Yes ) && buttons.Contains( MessageBoxResult.No ) && buttons.Contains( MessageBoxResult.Cancel ) )
            {
                return MessageBoxButton.YesNoCancel;
            }

            // fallback
            return MessageBoxButton.OK;
        }

        private static MessageBoxResult GetDefaultButton( Interaction interaction )
        {
            Contract.Requires( interaction != null );

            if ( interaction.DefaultCommandIndex < 0 || interaction.DefaultCommandIndex >= interaction.Commands.Count )
                return MessageBoxResult.None;

            var command = interaction.Commands[interaction.DefaultCommandIndex];
            MessageBoxResult result;

            if ( !Enum.TryParse<MessageBoxResult>( command.Name, true, out result ) )
                return MessageBoxResult.None;

            return result;
        }

        /// <summary>
        /// Prompts a user with an alert for the specified <see cref="Interaction">interaction</see> using a <see cref="MessageBox">message box</see>.
        /// </summary>
        /// <param name="interaction">The <see cref="T:Interaction">interaction</see> to display.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Alert( Interaction interaction )
        {
            Contract.Requires<ArgumentNullException>( interaction != null, "interaction" );

            var owner = Window.GetWindow( this.AssociatedObject );
            var title = interaction.Title;
            var text = interaction.Content as string ?? string.Empty;
            MessageBox.Show( owner, text, title, MessageBoxButton.OK );
        }

        /// <summary>
        /// Prompts a user with the specified <see cref="T:Interaction">interaction</see> using a <see cref="MessageBox">message box</see>.
        /// </summary>
        /// <param name="interaction">The <see cref="T:Interaction">interaction</see> to display.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Prompt( Interaction interaction )
        {
            Contract.Requires<ArgumentNullException>( interaction != null, "interaction" );

            var owner = Window.GetWindow( this.AssociatedObject );
            var title = interaction.Title;
            var text = interaction.Content as string ?? string.Empty;
            var buttons = GetButtons( interaction );
            var defaultButton = GetDefaultButton( interaction );
            var result = MessageBox.Show( owner, text, title, buttons, MessageBoxImage.None, defaultButton );
            var name = result.ToString();
            var comparer = StringComparer.OrdinalIgnoreCase;
            var button = interaction.Commands.FirstOrDefault( c => comparer.Equals( c.Name, name ) );

            if ( button != null && button.CanExecute() )
                button.Execute();
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="args">The <see cref="InteractionRequestedEventArgs"/> event data provided by the corresponding trigger.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Invoke( InteractionRequestedEventArgs args )
        {
            Contract.Requires<ArgumentNullException>( args != null, "args" );

            var interaction = args.Interaction;

            if ( interaction.Commands.Any() )
                this.Prompt( interaction );
            else
                this.Alert( interaction );
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
