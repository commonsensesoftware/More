namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents an interaction request to select a folder with support for continuations.
    /// </summary>
    public class WebAuthenticateInteractionRequest : ContinuableInteractionRequest<WebAuthenticateInteraction, IWebAuthenticationResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebAuthenticateInteractionRequest"/> class.
        /// </summary>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public WebAuthenticateInteractionRequest( Action<IWebAuthenticationResult> continuation ) : base( continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAuthenticateInteractionRequest"/> class.
        /// </summary>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public WebAuthenticateInteractionRequest( IContinuationManager continuationManager, Action<IWebAuthenticationResult> continuation )
            : base( continuationManager, continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAuthenticateInteractionRequest"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public WebAuthenticateInteractionRequest( string id, Action<IWebAuthenticationResult> continuation )
            : base( id, continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAuthenticateInteractionRequest"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public WebAuthenticateInteractionRequest( string id, IContinuationManager continuationManager, Action<IWebAuthenticationResult> continuation )
            : base( id, continuationManager, continuation ) { }

        /// <summary>
        /// Sets any applicable continuation data when an interaction is requested.
        /// </summary>
        /// <param name="interaction">The <see cref="WebAuthenticateInteraction">interaction</see> to set the continuation data for.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected override void SetContinuationData( WebAuthenticateInteraction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            base.SetContinuationData( interaction );
            interaction.ContinuationData["Continuation"] = "WebAuthenticate";
        }
    }
}