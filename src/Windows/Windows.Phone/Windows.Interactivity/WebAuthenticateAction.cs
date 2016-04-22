namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using global::Windows.Foundation.Collections;
    using global::Windows.Security.Authentication.Web;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class WebAuthenticateAction
    {
        private static void Authenticate( WebAuthenticateInteraction webAuthenticate )
        {
            Contract.Requires( webAuthenticate != null );
            
            var requestUri = webAuthenticate.RequestUri;
            var callbackUri = webAuthenticate.CallbackUri ?? WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
            var options = CreateOptions( webAuthenticate );
            var continuationData = new ValueSet();

            continuationData.AddRange( webAuthenticate.ContinuationData );

            WebAuthenticationBroker.AuthenticateAndContinue( requestUri, callbackUri, continuationData, options );
        }

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

            if ( webAuthenticate == null || !webAuthenticate.UseSilentMode )
                return;

            var requestUri = webAuthenticate.RequestUri;
            var options = CreateOptions( webAuthenticate );
            var result = await WebAuthenticationBroker.AuthenticateSilentlyAsync( requestUri, options );

            InvokeCallbackCommand( webAuthenticate, result );
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>The result of the action. The default implementation always executes asynchronously and returns null.</returns>
        /// <remarks>The <see cref="P:Interaction.CancelCommand">cancel command</see> is invoked if the operation
        /// fails or is canceled.</remarks>
        public override object Execute( object sender, object parameter )
        {
            var webAuthenticate = GetRequestedInteraction<WebAuthenticateInteraction>( parameter );

            if ( webAuthenticate == null || webAuthenticate.UseSilentMode )
                return base.Execute( sender, parameter );

            Authenticate( webAuthenticate );
            return null;
        }
    }
}
