namespace More.Windows
{
    using System;
    using Input;
    using global::Windows.Security.Authentication.Web;

    sealed class WebAuthenticationResultAdapter : IWebAuthenticationResult
    {
        readonly WebAuthenticationResult result;

        internal WebAuthenticationResultAdapter( WebAuthenticationResult result ) => this.result = result;

        public bool Canceled => result.ResponseStatus == WebAuthenticationStatus.UserCancel;

        public string ResponseData => result.ResponseData;

        public int ResponseErrorDetail => (int) result.ResponseErrorDetail;

        public bool Succeeded => result.ResponseStatus == WebAuthenticationStatus.Success;
    }
}