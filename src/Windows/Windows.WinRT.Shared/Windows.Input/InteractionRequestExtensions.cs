namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="InteractionRequest{T}"/> class.
    /// </summary>
    public static class InteractionRequestExtensions
    {
        /// <summary>
        /// Requests a web authentication interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="requestUri">The starting Uniform Resource Identifier (URI) of the web service.</param>
        /// <param name="callbackUri">The callback <see cref="Uri">URI</see> that indicates the completion of the web authentication.
        /// The broker matches this <see cref="Uri">URI</see> against every <see cref="Uri">URI</see> that it is about to navigate to.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IWebAuthenticationResult">authentication result</see>.</returns>
        /// <remarks>The requested <see cref="WebAuthenticateInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "OK" and "Cancel".</remarks>
        public static Task<IWebAuthenticationResult> RequestAsync( this InteractionRequest<WebAuthenticateInteraction> interactionRequest, Uri requestUri, Uri callbackUri )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( requestUri, nameof( requestUri ) );
            return interactionRequest.RequestAsync( new WebAuthenticateInteraction( requestUri ) { CallbackUri = callbackUri } );
        }

        /// <summary>
        /// Requests a web authentication interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="interaction">The requested <see cref="WebAuthenticateInteraction">interaction</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IWebAuthenticationResult">authentication result</see>.</returns>
        /// <remarks>The requested <see cref="WebAuthenticateInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "OK" and "Cancel". If the specified <paramref name="interaction"/> has any commands predefined, they will be cleared.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated a code contract." )]
        public static Task<IWebAuthenticationResult> RequestAsync( this InteractionRequest<WebAuthenticateInteraction> interactionRequest, WebAuthenticateInteraction interaction )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( interaction, nameof( interaction ) );

            var source = new TaskCompletionSource<IWebAuthenticationResult>();

            // commands cannot be defined by caller in order to hook up completion
            interaction.Commands.Clear();
            interaction.Commands.Add( new NamedCommand<IWebAuthenticationResult>( "OK", source.SetResult ) );
            interaction.Commands.Add( new NamedCommand<IWebAuthenticationResult>( "Cancel", source.SetResult ) );
            interactionRequest.Request( interaction );

            return source.Task;
        }
    }
}
