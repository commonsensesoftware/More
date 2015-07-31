namespace More.Windows
{
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static InteractionRequest<TInteraction> CreateInteractionRequest<TInteraction, TEventArgs>( this IContinuationManager continuationManager, Action<TEventArgs> continuation )
            where TInteraction : Interaction
            where TEventArgs : IContinuationActivatedEventArgs
        {
            Arg.NotNull( continuationManager, nameof( continuationManager ) );
            Arg.NotNull( continuation, nameof( continuation ) );
            Contract.Ensures( Contract.Result<InteractionRequest<TInteraction>>() != null );
            return continuationManager.CreateInteractionRequest<TInteraction, TEventArgs>( null, continuation );
        }
    }
}
