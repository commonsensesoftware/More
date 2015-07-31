namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a user interface interaction request that supports continuation.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="Interaction">interaction</see> requested.</typeparam>
    public class ContinuableInteractionRequest<T> : InteractionRequest<T> where T : Interaction
    {
        private readonly long continuationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuableInteractionRequest{T}"/> class.
        /// </summary>
        /// <param name="continuationId">The continuation identifier associated with requested interactions.</param>
        public ContinuableInteractionRequest( long continuationId )
        {
            this.continuationId = continuationId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuableInteractionRequest{T}"/> class.
        /// </summary>
        /// <param name="id">The identifier associated with the request.</param>
        /// <param name="continuationId">The continuation identifier associated with requested interactions.</param>
        public ContinuableInteractionRequest( string id, long continuationId )
            : base( id )
        {
            this.continuationId = continuationId;
        }

        /// <summary>
        /// Sets any applicable continuation data when an interaction is requested.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction">interaction</see> to set the continuation data for.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void SetContinuationData( Interaction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            interaction.ContinuationData["ContinuationId"] = continuationId;
        }

        /// <summary>
        /// Raises the <see cref="E:Requested"/> event.
        /// </summary>
        /// <param name="e">The <see cref="InteractionRequestedEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected override void OnRequested( InteractionRequestedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            SetContinuationData( e.Interaction );
            base.OnRequested( e );
        }
    }
}
