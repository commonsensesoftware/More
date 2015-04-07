namespace More.Windows.Input
{
    using global::System;

    /// <summary>
    /// Defines the behavior of a user interface interaction request.
    /// </summary>
    public interface IInteractionRequest
    {
        /// <summary>
        /// Gets or sets the identifier associated with the interaction request.
        /// </summary>
        /// <value>The request identifier.</value>
        string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when the user interaction is requested.
        /// </summary>
        event EventHandler<InteractionRequestedEventArgs> Requested;
    }
}
