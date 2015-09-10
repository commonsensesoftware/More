using System.Diagnostics.Contracts;
namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a user interface interaction request that supports continuation.
    /// </summary>
    /// <typeparam name="TInteraction">The <see cref="Type">type</see> of <see cref="Interaction">interaction</see> requested.</typeparam>
    /// <typeparam name="TArg">The <see cref="Type">type</see> of argument passed to the continuation <see cref="Action{T}">action</see>.</typeparam>
    public class ContinuableInteractionRequest<TInteraction, TArg> : InteractionRequest<TInteraction> where TInteraction : Interaction
    {
        private readonly long continuationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuableInteractionRequest{TInteraction, TArg}"/> class.
        /// </summary>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public ContinuableInteractionRequest( Action<TArg> continuation )
            : this( ResolveContinuationManager(), continuation )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuableInteractionRequest{TInteraction, TArg}"/> class.
        /// </summary>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0" )]
        public ContinuableInteractionRequest( IContinuationManager continuationManager, Action<TArg> continuation )
        {
            Arg.NotNull( continuationManager, nameof( continuationManager ) );
            Arg.NotNull( continuation, nameof( continuation ) );
            continuationId = continuationManager.Register( continuation );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuableInteractionRequest{TInteraction, TArg}"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public ContinuableInteractionRequest( string id, Action<TArg> continuation )
            : this( id, ResolveContinuationManager(), continuation )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuableInteractionRequest{TInteraction, TArg}"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public ContinuableInteractionRequest( string id, IContinuationManager continuationManager, Action<TArg> continuation )
            : base( id )
        {
            Arg.NotNull( continuationManager, nameof( continuationManager ) );
            Arg.NotNull( continuation, nameof( continuation ) );
            continuationId = continuationManager.Register( continuation );
        }

        private static IContinuationManager ResolveContinuationManager()
        {
            Contract.Ensures( Contract.Result<IContinuationManager>() != null );

            IContinuationManager continuationManager;

            if ( ServiceProvider.Current.TryGetService( out continuationManager ) )
                return continuationManager;

            return DefaultContinuationManager.Instance;
        }

        /// <summary>
        /// Sets any applicable continuation data when an interaction is requested.
        /// </summary>
        /// <param name="interaction">The <typeparamref name="TInteraction">interaction</typeparamref> to set the continuation data for.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void SetContinuationData( TInteraction interaction )
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
            SetContinuationData( (TInteraction) e.Interaction );
            base.OnRequested( e );
        }
    }
}
