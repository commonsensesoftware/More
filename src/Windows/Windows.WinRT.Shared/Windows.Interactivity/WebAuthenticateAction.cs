namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using global::Windows.Security.Authentication.Web;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to perform web-based authentication
    /// <see cref="WebAuthenticateInteraction">interactions</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public partial class WebAuthenticateAction : System.Windows.Interactivity.TriggerAction
    {
        private static void InvokeCallbackCommand( WebAuthenticateInteraction webAuthenticate, WebAuthenticationResult result )
        {
            Contract.Requires( webAuthenticate != null );
            Contract.Requires( result != null );

            if ( result.ResponseStatus == WebAuthenticationStatus.Success )
            {
                webAuthenticate.ResponseStatus = 200U;
                webAuthenticate.ResponseData = result.ResponseData;
                webAuthenticate.ExecuteDefaultCommand();
            }
            else
            {
                webAuthenticate.ResponseStatus = result.ResponseErrorDetail;
                webAuthenticate.ExecuteCancelCommand();
            }
        }
    }
}
