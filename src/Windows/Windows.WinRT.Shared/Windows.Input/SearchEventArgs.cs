namespace More.Windows.Input
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents the arguments for a search event.
    /// </summary>
    public class SearchEventArgs : EventArgs, ISearchRequest
    {
        private readonly string language;
        private readonly string query;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchEventArgs"/> class.
        /// </summary>
        /// <param name="query">The query text.</param>
        /// <remarks>The language is based on the <see cref="P:CultureInfo.CurrentCulture">current culture</see>.</remarks>
        public SearchEventArgs( string query )
            : this( CultureInfo.CurrentCulture.Name, query )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchEventArgs"/> class.
        /// </summary>
        /// <param name="language">The IETF language tag.</param>
        /// <param name="query">The query text.</param>
        public SearchEventArgs( string language, string query )
        {
            this.language = language ?? string.Empty;
            this.query = query ?? string.Empty;
        }

        /// <summary>
        /// Gets the Internet Engineering Task Force (IETF) language tag (BCP 47 standard) that identifies the
        /// language currently associated with the user's text input device.
        /// </summary>
        /// <value>The BCP 47 standard language tag.</value>
        public string Language
        {
            get
            {
                return this.language;
            }
        }

        /// <summary>
        /// Gets the text that the application should provide suggestions for.
        /// </summary>
        /// <value>The query text that the application should provide suggestions for.</value>
        public string QueryText
        {
            get
            {
                return this.query;
            }
        }
    }
}
