namespace More.Windows.Interactivity
{
    using global::Windows.Security.Authentication.Web;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using static global::Windows.Security.Authentication.Web.WebAuthenticationOptions;
    using static global::Windows.Security.Authentication.Web.WebAuthenticationStatus;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction">interactivity action</see> that can be used to perform web-based authentication
    /// <see cref="WebAuthenticateInteraction">interactions</see> received from an <see cref="IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class WebAuthenticateAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        /// <remarks>The <see cref="Interaction.CancelCommand">cancel command</see> is invoked if the operation
        /// fails or is canceled.</remarks>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            var webAuthenticate = GetRequestedInteraction<WebAuthenticateInteraction>( parameter );

            if ( webAuthenticate == null )
            {
                return;
            }

            var requestUri = webAuthenticate.RequestUri;
            var callbackUri = webAuthenticate.CallbackUri ?? WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
            var options = CreateOptions( webAuthenticate );
            var result = await WebAuthenticationBroker.AuthenticateAsync( options, requestUri, callbackUri );

            InvokeCallbackCommand( webAuthenticate, result );
        }

        static WebAuthenticationOptions CreateOptions( WebAuthenticateInteraction interaction )
        {
            Contract.Requires( interaction != null );

            var options = None;

            if ( interaction.UseCorporateNetwork )
            {
                options |= UseCorporateNetwork;
            }

            if ( interaction.UseHttpPost )
            {
                options |= UseHttpPost;
            }

            if ( interaction.UseTitle )
            {
                options |= UseTitle;
            }

            return options;
        }

        static void InvokeCallbackCommand( WebAuthenticateInteraction webAuthenticate, WebAuthenticationResult authenticationResult )
        {
            Contract.Requires( webAuthenticate != null );
            Contract.Requires( authenticationResult != null );

            var result = default( IWebAuthenticationResult );

            if ( authenticationResult.ResponseStatus == Success )
            {
                result = new WebAuthenticationResultAdapter( authenticationResult );
                webAuthenticate.ResponseStatus = 200U;
                webAuthenticate.ResponseData = authenticationResult.ResponseData;
                webAuthenticate.DefaultCommand?.Execute( result );
            }
            else
            {
                webAuthenticate.ResponseStatus = authenticationResult.ResponseErrorDetail;
                webAuthenticate.CancelCommand?.Execute( result );
            }
        }
    }
}