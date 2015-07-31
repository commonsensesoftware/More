namespace More.Windows.Controls.Automation.Peers
{
    using System;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a common dialog automation peer.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of control the automation peer is for.</typeparam>
    public abstract class DialogAutomationPeer<T> :
        FrameworkElementAutomationPeer,
        IInvokeProvider,
        IValueProvider,
        IWindowProvider
        where T : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogAutomationPeer{T}"/> class.
        /// </summary>
        /// <param name="owner">The owner control of type <typeparamref name="T"/>.</param>
        protected DialogAutomationPeer( T owner )
            : base( owner )
        {
        }

        /// <summary>
        /// Gets the window for the automation peer.
        /// </summary>
        /// <value>A <typeparamref name="T"/> object.</value>
        protected T Window
        {
            get
            {
                return (T) Owner;
            }
        }

        /// <summary>
        /// Performs the invoke pattern for the automation peer.
        /// </summary>
        public abstract void Invoke();

        /// <summary>
        /// Gets or sets a value indicating whether the state provided by the automation peer is read-only.
        /// </summary>
        /// <value>True if the state is read-only; otherwise, false.</value>
        public bool IsReadOnly
        {
            get;
            protected set;
        }

        /// <summary>
        /// Sets the state provided by the automation peer.
        /// </summary>
        /// <param name="value">The state to set.</param>
        public virtual void SetValue( string value )
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the state provided by the automation peer.
        /// </summary>
        /// <value>The automation peer state.</value>
        public abstract string Value
        {
            get;
            protected set;
        }

        /// <summary>
        /// Invokes the close method of the window provider automation peer.
        /// </summary>
        public virtual void Close()
        {
            Window.Close();
        }

        /// <summary>
        /// Gets or sets the interaction state of the window provider automation peer.
        /// </summary>
        /// <value>One of the <see cref="WindowInteractionState"/> values.</value>
        public WindowInteractionState InteractionState
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window provider automation peer is modal.
        /// </summary>
        /// <value>True if the window provider automation peer is modal; otherwise, false.</value>
        public bool IsModal
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window provider automation peer is a topmost window.
        /// </summary>
        /// <value>True if the window provider automation peer is a topmost window; otherwise, false.</value>
        public bool IsTopmost
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window provider automation peer can be maximized.
        /// </summary>
        /// <value>True if the window provider automation peer can be maximized; otherwise, false.</value>
        public bool Maximizable
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window provider automation peer can be minimized.
        /// </summary>
        /// <value>True if the window provider automation peer can be minimized; otherwise, false.</value>
        public bool Minimizable
        {
            get;
            protected set;
        }

        /// <summary>
        /// Sets the visual state of window provider automation peer.
        /// </summary>
        /// <param name="state">One of the <see cref="WindowVisualState"/> values.</param>
        public virtual void SetVisualState( WindowVisualState state )
        {
            VisualState = state;
        }

        /// <summary>
        /// Gets or sets the visual state of window provider automation peer.
        /// </summary>
        /// <value>One of the <see cref="WindowVisualState"/> values.</value>
        public abstract WindowVisualState VisualState
        {
            get;
            protected set;
        }

        /// <summary>
        /// Causes the calling code to block for the specified time or until the associated process enters
        /// an idle state, whichever completes first.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated
        /// process to become idle.</param>
        /// <returns>True if the window has entered the idle state; false if the timeout occurred.</returns>
        /// <remarks>The default implementation always returns true.</remarks>
        public virtual bool WaitForInputIdle( int milliseconds )
        {
            return true;
        }
    }
}
