namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.Contracts;

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
            Arg.NotNull( interaction, nameof( interaction ) );
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
                Contract.Ensures( interaction != null );
                return interaction;
            }
        }
    }
}
