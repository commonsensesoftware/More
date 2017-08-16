namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;

    /// <content>
    /// Provides the command mediator in a separate source file.
    /// </content>
    public partial class WindowAction
    {
        sealed class CommandMediator : IDisposable
        {
            volatile bool closing;
            Window window;
            IEnumerable<INamedCommand> commands;

            internal CommandMediator( Window window, Interaction interaction, bool bindCancelToClose )
            {
                Contract.Requires( window != null );

                this.window = window;
                this.window.Closing += OnWindowClosing;

                if ( interaction == null )
                {
                    return;
                }

                commands = interaction.Commands;
                commands.ForEach( c => c.Executed += OnExecuted );

                if ( !bindCancelToClose )
                {
                    return;
                }

                var behaviors = System.Windows.Interactivity.Interaction.GetBehaviors( this.window );
                var behavior = behaviors.OfType<WindowCloseBehavior>().FirstOrDefault();

                if ( behavior == null )
                {
                    behavior = new WindowCloseBehavior();
                    behaviors.Add( behavior );
                }

                behavior.CloseCommand = interaction.CancelCommand;
            }

            void OnWindowClosing( object sender, CancelEventArgs e ) => closing = !e.Cancel;

            void OnExecuted( object sender, EventArgs e )
            {
                if ( closing )
                {
                    return;
                }

                closing = true;
                window.Close();
                closing = false;
            }

            public void Dispose()
            {
                if ( commands != null )
                {
                    commands.ForEach( c => c.Executed -= OnExecuted );
                }

                if ( window != null )
                {
                    window.Closing -= OnWindowClosing;
                }

                commands = null;
                window = null;
                GC.SuppressFinalize( this );
            }
        }
    }
}