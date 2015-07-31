namespace More.Windows.Controls.Automation.Peers
{
    using System;
    using System.Windows.Automation;

    /// <summary>
    /// Represents an automation peer for the <see cref="MessageDialog"/> class.
    /// </summary>
    public class MessageDialogAutomationPeer : DialogAutomationPeer<MessageDialog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDialogAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="MessageDialog"/> the automation peer is for.</param>
        public MessageDialogAutomationPeer( MessageDialog owner )
            : base( owner )
        {
            IsReadOnly = true;
            IsModal = true;
            IsTopmost = true;
            Maximizable = false;
            Minimizable = false;
            InteractionState = Window.IsOpen ? WindowInteractionState.ReadyForUserInteraction : WindowInteractionState.Running;
            Window.Opened += ( s, e ) => InteractionState = WindowInteractionState.ReadyForUserInteraction;
            Window.Closing += ( s, e ) => InteractionState = WindowInteractionState.Closing;
        }

        /// <summary>
        /// Performs the invoke pattern for the automation peer.
        /// </summary>
        public override void Invoke()
        {
            Close();
        }

        /// <summary>
        /// Gets or sets the state provided by the automation peer.
        /// </summary>
        /// <value>The automation peer state.</value>
        public override string Value
        {
            get
            {
                if ( Window.Content == null )
                    return null;

                return Window.Content.ToString();
            }
            protected set
            {
                Window.Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the visual state of window provider automation peer.
        /// </summary>
        /// <value>One of the <see cref="WindowVisualState"/> values.</value>
        public override WindowVisualState VisualState
        {
            get
            {
                return WindowVisualState.Normal;
            }
            protected set
            {
                // cannot be changed
            }
        }
    }
}
