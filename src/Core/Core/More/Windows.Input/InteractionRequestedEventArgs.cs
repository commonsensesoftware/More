namespace More.Windows.Input
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the event data provided when a user interaction request event is raised.
    /// </summary>
    public class InteractionRequestedEventArgs : EventArgs
    {
        private readonly Interaction interaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction">interaction</see> being requested.</param>
        public InteractionRequestedEventArgs( Interaction interaction )
        {
            Contract.Requires<ArgumentNullException>( interaction != null, "interaction" );
            this.interaction = interaction;
        }

        /// <summary>
        /// Gets the requested interaction.
        /// </summary>
        /// <value>The requested <see cref="Interaction">interaction</see>.</value>
        public Interaction Interaction
        {
            get
            {
                Contract.Ensures( this.interaction != null );
                return this.interaction;
            }
        }
    }
}
