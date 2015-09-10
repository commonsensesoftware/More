namespace More.Windows
{
    using Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Net;
    using global::Windows.Security.Authentication.Web;

    internal sealed class WebAuthenticationResultAdapter : IWebAuthenticationResult
    {
        private readonly WebAuthenticationResult result;

        internal WebAuthenticationResultAdapter( WebAuthenticationResult result )
        {
            Contract.Requires( result != null );
            this.result = result;
        }

        public bool Canceled
        {
            get
            {
                return result.ResponseStatus == WebAuthenticationStatus.UserCancel;
            }
        }

        public string ResponseData
        {
            get
            {
                return result.ResponseData;
            }
        }

        public HttpStatusCode ResponseErrorDetail
        {
            get
            {
                return (HttpStatusCode) result.ResponseErrorDetail;
            }
        }

        public bool Succeeded
        {
            get
            {
                return result.ResponseStatus == WebAuthenticationStatus.Success;
            }
        }
    }
}
