namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    /// <content>
    /// Provides the command mediator in a separate source file.
    /// </content>
    public partial class WindowAction
    {
        private sealed class CommandMediator : IDisposable
        {
            private volatile bool closing;
            private Window window;
            private IEnumerable<INamedCommand> commands;

            internal CommandMediator( Window window, Interaction interaction, bool bindCancelToClose )
            {
                Contract.Requires( window != null );

                this.window = window;
                this.window.Closing += this.OnWindowClosing;

                if ( interaction == null )
                    return;

                this.commands = interaction.Commands;
                this.commands.ForEach( c => c.Executed += this.OnExecuted );

                if ( !bindCancelToClose )
                    return;

                // determine if behavior is already applied
                var behaviors = System.Windows.Interactivity.Interaction.GetBehaviors( this.window );
                var behavior = behaviors.OfType<WindowCloseBehavior>().FirstOrDefault();

                // add behavior which can support binding the cancel command to the window close button (X)
                if ( behavior == null )
                {
                    behavior = new WindowCloseBehavior();
                    behaviors.Add( behavior );
                }

                behavior.CloseCommand = interaction.CancelCommand;
            }

            private void OnWindowClosing( object sender, CancelEventArgs e )
            {
                this.closing = !e.Cancel;
            }

            private void OnExecuted( object sender, EventArgs e )
            {
                if ( this.closing )
                    return;

                this.closing = true;
                this.window.Close();
                this.closing = false;
            }

            public void Dispose()
            {
                if ( this.commands != null )
                    this.commands.ForEach( c => c.Executed -= this.OnExecuted );

                if ( this.window != null )
                    this.window.Closing -= this.OnWindowClosing;

                this.commands = null;
                this.window = null;
                GC.SuppressFinalize( this );
            }
        }
    }
}
