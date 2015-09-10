namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.Security.Authentication.Web;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to perform web-based authentication
    /// <see cref="WebAuthenticateInteraction">interactions</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public partial class WebAuthenticateAction : System.Windows.Interactivity.TriggerAction
    {
        private static void InvokeCallbackCommand( WebAuthenticateInteraction webAuthenticate, WebAuthenticationResult authenticationResult )
        {
            Contract.Requires( webAuthenticate != null );
            Contract.Requires( authenticationResult != null );

            IWebAuthenticationResult result = null;

            if ( authenticationResult.ResponseStatus == WebAuthenticationStatus.Success )
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
