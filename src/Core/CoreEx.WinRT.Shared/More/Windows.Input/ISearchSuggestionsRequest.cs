namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.Storage.Streams;

    /// <summary>
    /// Defines the behavior of a search suggestion request.
    /// </summary>
    [CLSCompliant( false )]
    [ContractClass( typeof( ISearchSuggestionsRequestContract ) )]
    public interface ISearchSuggestionsRequest
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

        /// <summary>
        /// Gets a value indicating whether the request for suggestions to display is canceled.
        /// </summary>
        /// <value>True if the request was canceled, otherwise false. The default value is false.</value>
        bool IsCanceled
        {
            get;
        }

        /// <summary>
        /// Gets the number of provided suggestions.
        /// </summary>
        /// <value>The number of provided suggestions.</value>
        uint QuerySuggestionCount
        {
            get;
        }

        /// <summary>
        /// Gets a read-only list of the text alternatives for the current query text.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">list</see> of the text alternatives for the query text.</value>
        IReadOnlyList<string> QueryTextAlternatives
        {
            get;
        }

        /// <summary>
        /// Gets the length of the portion of the query text that the user is composing with an Input Method Editor (IME).
        /// </summary>
        /// <value>The length of the portion of the query text that the user is composing with an IME.</value>
        uint QueryTextCompositionLength
        {
            get;
        }

        /// <summary>
        /// Gets the starting location of the text that the user is composing with an Input Method Editor (IME).
        /// </summary>
        /// <value>The starting location of the query text that the user is composing with an IME.</value>
        uint QueryTextCompositionStart
        {
            get;
        }

        /// <summary>
        /// Retrieves an object that lets an application respond to a request for suggestions asynchronously.
        /// </summary>
        /// <returns>An object that allows an application to provide search suggestions asynchronously. When the
        /// object is disposed, the deferral to the original operation ends.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method has side effects and should not be a property." )]
        IDisposable GetDeferral();

        /// <summary>
        /// Appends a query suggestion to the list of search suggestions for the search pane.
        /// </summary>
        /// <param name="text">The text of the query suggestion.</param>
        void AppendQuerySuggestion( string text );

        /// <summary>
        /// Appends a sequence of query suggestions to the list of search suggestions for the search pane.
        /// </summary>
        /// <param name="suggestions">The <see cref="IEnumerable{T}">sequence</see> of query suggestions to append.</param>
        void AppendQuerySuggestions( IEnumerable<string> suggestions );

        /// <summary>
        /// Appends a suggested search result to the list of suggestions to display in the search pane.
        /// </summary>
        /// <param name="text">The text of the suggested result.</param>
        /// <param name="detailText">The detail text for the suggested result.</param>
        /// <param name="tag">The unique tag that identifies this suggested result.</param>
        /// <param name="image">The image to accompany the results suggestion.</param>
        /// <param name="imageAlternateText">The alternate text for the image.</param>
        void AppendResultSuggestion( string text, string detailText, string tag, IRandomAccessStreamReference image, string imageAlternateText );
        
        /// <summary>
        /// Appends a text label that is used to separate groups of suggestions in the search pane.
        /// </summary>
        /// <param name="label">The text to use as a separator. This text should be descriptive of any suggestions that are appended after it.</param>
        void AppendSearchSeparator( string label );
    }
}
