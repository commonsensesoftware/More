namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a user interaction request to close a window.
    /// </summary>
    public class WindowCloseInteraction : Interaction
    {
        private bool canceled;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCloseInteraction"/> class.
        /// </summary>
        public WindowCloseInteraction()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCloseInteraction"/> class.
        /// </summary>
        /// <param name="canceled">Indicates whether the interaction was canceled.</param>
        public WindowCloseInteraction( bool canceled )
            : base()
        {
            this.canceled = canceled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCloseInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the notification.</param>
        /// <param name="content">The content associated with the notification.</param>
        /// <param name="canceled">Indicates whether the interaction was canceled.</param>
        public WindowCloseInteraction( string title, object content, bool canceled )
            : base( title, content )
        {
            Arg.NotNull( title, nameof( title ) );
            this.canceled = canceled;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the interaction was canceled.
        /// </summary>
        /// <value>True if the interaction was canceled; otherwise, false. The default value is false.</value>
        public bool Canceled
        {
            get
            {
                return canceled;
            }
            set
            {
                SetProperty( ref canceled, value );
            }
        }
    }
}
