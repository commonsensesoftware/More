namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Threading.Tasks;
    using global::Windows.Security.Authentication.Web;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class WebAuthenticateAction
    {
        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        /// <remarks>The <see cref="P:Interaction.CancelCommand">cancel command</see> is invoked if the operation
        /// fails or is canceled.</remarks>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            var webAuthenticate = GetRequestedInteraction<WebAuthenticateInteraction>( parameter );

            if ( webAuthenticate == null )
                return;

            var requestUri = webAuthenticate.RequestUri;
            var callbackUri = webAuthenticate.CallbackUri ?? WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
            var options = webAuthenticate.Options;

            // note: SilentMode is not internally set in the options because it is deprecated in Windows Phone
            if ( webAuthenticate.UseSilentMode )
                options |= WebAuthenticationOptions.SilentMode;

            var result = await WebAuthenticationBroker.AuthenticateAsync( options, requestUri, callbackUri );

            InvokeCallbackCommand( webAuthenticate, result );
        }
    }
}
