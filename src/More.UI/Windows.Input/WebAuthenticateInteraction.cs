namespace More.Windows.Input
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an interaction request to perform a web-based authentication operation.
    /// </summary>
    public class WebAuthenticateInteraction : Interaction
    {
        readonly Uri requestUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAuthenticateInteraction"/> class.
        /// </summary>
        /// <param name="requestUri">The starting URI of the web service. This URI must be a secure address of https://.</param>
        public WebAuthenticateInteraction( Uri requestUri )
        {
            Arg.NotNull( requestUri, nameof( requestUri ) );
            this.requestUri = requestUri;
        }

        /// <summary>
        /// Gets the starting Uniform Resource Identifier (URI) of the web service.
        /// </summary>
        /// <value>The starting <see cref="Uri">URI</see> of the web service. This
        /// <see cref="Uri">URI</see> must be a secure address of https://.</value>
        public Uri RequestUri
        {
            get
            {
                Contract.Ensures( requestUri != null );
                return requestUri;
            }
        }

        /// <summary>
        /// Gets or sets the callback Uniform Resource Indicator (URI) that indicates the completion of the web authentication. 
        /// </summary>
        /// <value>The callback <see cref="Uri">URI</see> that indicates the completion of the web authentication.
        /// The broker matches this <see cref="Uri">URI</see> against every <see cref="Uri">URI</see> that it is about to navigate to.</value>
        /// <remarks>The broker never navigates to this URI, instead the broker returns the control back to the application when the
        /// user clicks a link or a web server redirection is made.</remarks>
        public Uri CallbackUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the authentication operation uses corporate network authentication.
        /// </summary>
        /// <value>Tells the web authentication broker to render the webpage in an application container that
        /// supports privateNetworkClientServer, enterpriseAuthentication, and sharedUserCertificate capabilities.</value>
        /// <remarks>Note the application that uses this flag must have these capabilities as well.</remarks>
        public bool UseCorporateNetwork { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the authentication operation returns the HTTP POST body in the response.
        /// </summary>
        /// <value>Tells the web authentication broker to return the body of the HTTP POST.</value>
        /// <remarks>For use with single sign-on (SSO) only.</remarks>
        public bool UseHttpPost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the authentication operation returns the window title in the response.
        /// </summary>
        /// <value>Tells the web authentication broker to return the window title string of the webpage.</value>
        public bool UseTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation authenticates using silent mode.
        /// </summary>
        /// <value>Tells the web authentication broker to not render any UI.</value>
        /// <remarks>For use with Single Sign On (SSO). If the server tries to display a webpage,
        /// the authentication operation fails.</remarks>
        public bool UseSilentMode { get; set; }

        /// <summary>
        /// Gets or sets the response data when the operation completes successfully.
        /// </summary>
        /// <value>The success response data.</value>
        public string ResponseData { get; set; }

        /// <summary>
        /// Gets or sets the response status when the operation completes.
        /// </summary>
        /// <value>The response HTTP status code.</value>
        [CLSCompliant( false )]
        public uint ResponseStatus { get; set; }
    }
}