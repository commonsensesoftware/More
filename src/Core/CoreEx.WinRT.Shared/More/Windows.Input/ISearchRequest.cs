namespace More.Windows.Input
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a search request.
    /// </summary>
    [ContractClass( typeof( ISearchRequestContract ) )]
    public interface ISearchRequest
    {
        /// <summary>
        /// Gets the Internet Engineering Task Force (IETF) language tag (BCP 47 standard) that identifies the
        /// language currently associated with the user's text input device.
        /// </summary>
        /// <value>The BCP 47 standard language tag.</value>
        string Language
        {
            get;
        }

        /// <summary>
        /// Gets the text that the application should provide suggestions for.
        /// </summary>
        /// <value>The query text that the application should provide suggestions for.</value>
        string QueryText
        {
            get;
        }
    }
}
