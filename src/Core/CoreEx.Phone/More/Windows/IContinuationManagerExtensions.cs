namespace More.Windows
{
    using More.Windows.Input;
    using global::System;
    using global::System.Diagnostics.Contracts;
    using global::System.Windows.Input;
    using global::Windows.ApplicationModel.Activation;

    /// <summary>
    /// Provides extension methods for the <see cref="IContinuationManager"/> class.
    /// </summary>
    [CLSCompliant( false )]
    public static class IContinuationManagerExtensions
    {
        /// <summary>
        /// Creates and returns an interation request that supports continuation to the specified callback.
        /// </summary>
        /// <typeparam name="TInteraction">The <see cref="Type">type</see> of <see cref="Interaction">interaction</see> to create a request for.</typeparam>
        /// <typeparam name="TEventArgs">The <see cref="IContinuationActivatedEventArgs">continuation event arguments</see> provided to the
        /// callback <see cref="Action{T}">action</see>.</typeparam>
        /// <param name="continuationManager">The extended <see cref="IContinuationManager">continuation manager</see>.</param>
        /// <param name="continuation">The continuation <see cref="Action{T}">action</see>.</param>
        /// <returns>A new <see cref="InteractionRequest{T}">interaction request</see> with support for continuations.</returns>
        public static InteractionRequest<TInteraction> CreateInterationRequest<TInteraction, TEventArgs>( this IContinuationManager continuationManager, Action<TEventArgs> continuation )
            where TInteraction : Interaction
            where TEventArgs : IContinuationActivatedEventArgs
        {
            Contract.Requires<ArgumentNullException>( continuationManager != null, "continuationManager" );
            Contract.Requires<ArgumentNullException>( continuation != null, "continuation" );
            Contract.Ensures( Contract.Result<InteractionRequest<TInteraction>>() != null );
            return continuationManager.CreateInterationRequest<TInteraction, TEventArgs>( null, continuation );
        }
    }
}
