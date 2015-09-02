namespace More.Windows
{
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;
    using global::Windows.ApplicationModel.Activation;

    /// <summary>
    /// Defines the behavior of a continuation manager.
    /// </summary>
    /// <remarks>A continuation manager is used to facilitate dialog picker continuations.</remarks>
    [CLSCompliant( false )]
    [ContractClass( typeof( IContinuationManagerContract ) )]
    public interface IContinuationManager
    {
        /// <summary>
        /// Creates and returns an interation request that supports continuation to the specified callback.
        /// </summary>
        /// <typeparam name="TInteraction">The <see cref="Type">type</see> of <see cref="Interaction">interaction</see> to create a request for.</typeparam>
        /// <typeparam name="TEventArgs">The <see cref="IContinuationActivatedEventArgs">continuation event arguments</see> provided to the
        /// callback <see cref="Action{T}">action</see>.</typeparam>
        /// <param name="id">The identifier of the created interaction request.</param>
        /// <param name="continuation">The continuation <see cref="Action{T}">action</see>.</param>
        /// <returns>A new <see cref="InteractionRequest{T}">interaction request</see> with support for continuations.</returns>
        InteractionRequest<TInteraction> CreateInteractionRequest<TInteraction, TEventArgs>( string id, Action<TEventArgs> continuation )
            where TInteraction : Interaction
            where TEventArgs : IContinuationActivatedEventArgs;

        /// <summary>
        /// Continues a dialog picker operation.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> <see cref="IContinuationActivatedEventArgs">continuation event arguments</see>.</typeparam>
        /// <param name="eventArgs">The <typeparamref name="TEventArgs">event arguments</typeparamref> for the continued operation.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Continue", Justification = "Will not cause any known cross-language issues and is the most appropriate term." )]
        void Continue<TEventArgs>( TEventArgs eventArgs ) where TEventArgs : IContinuationActivatedEventArgs;
    }
}
