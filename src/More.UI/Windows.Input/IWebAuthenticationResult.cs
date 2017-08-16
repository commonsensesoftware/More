namespace More.Windows.Input
{
    using System;

    /// <summary>
    /// Defines the behavior of a web authentication result.
    /// </summary>
    public interface IWebAuthenticationResult
    {
        /// <summary>
        /// Gets the protocol data when the operation successfully completes.
        /// </summary>
        /// <value>The returned data.</value>
        string ResponseData { get; }

        /// <summary>
        /// Gets the HTTP error code when an error occurs.
        /// </summary>
        /// <value>The HTTP status code.</value>
        int ResponseErrorDetail { get; }

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        /// <value>True if the operation succeeded; othewise, false.</value>
        bool Succeeded { get; }

        /// <summary>
        /// Gets a value indicating whether the operation was canceled.
        /// </summary>
        /// <value>True if the operation was canceled; othewise, false.</value>
        /// <remarks>If the operation was not canceled and the operation did not succeed, then an error occurred.</remarks>
        bool Canceled { get; }
    }
}