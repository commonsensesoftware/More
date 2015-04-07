namespace More.Windows.Controls.Automation.Peers
{
    using System;
    using System.Windows.Automation;

    /// <summary>
    /// Represents an automation peer for the <see cref="TextInputDialog"/> class.
    /// </summary>
    public class TextInputDialogAutomationPeer : DialogAutomationPeer<TextInputDialog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputDialogAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="TextInputDialog"/> the automation peer is for.</param>
        public TextInputDialogAutomationPeer( TextInputDialog owner )
            : base( owner )
        {
            this.IsReadOnly = false;
            this.IsModal = true;
            this.IsTopmost = true;
            this.Maximizable = false;
            this.Minimizable = false;
            this.InteractionState = this.Window.IsOpen ? WindowInteractionState.ReadyForUserInteraction : WindowInteractionState.Running;
            this.Window.Opened += ( s, e ) => this.InteractionState = WindowInteractionState.ReadyForUserInteraction;
            this.Window.Closing += ( s, e ) => this.InteractionState = WindowInteractionState.Closing;
        }

        /// <summary>
        /// Gets or sets the response of the input box.
        /// </summary>
        /// <value>The input box response.</value>
        public string Response
        {
            get
            {
                return this.Value;
            }
            set
            {
                this.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the default response of the input box.
        /// </summary>
        /// <value>The input box default response.</value>
        public string DefaultResponse
        {
            get
            {
                return this.Window.DefaultResponse;
            }
            set
            {
                this.Window.DefaultResponse = value;
            }
        }

        /// <summary>
        /// Performs the invoke pattern for the automation peer.
        /// </summary>
        public override void Invoke()
        {
            this.Window.ExecuteCommand( this.Window.DefaultCommandIndex );
        }

        /// <summary>
        /// Gets or sets the state provided by the automation peer.
        /// </summary>
        /// <value>The automation peer state.</value>
        public override string Value
        {
            get
            {
                return this.Window.Response;
            }
            protected set
            {
                this.Window.Response = value ?? string.Empty;
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
