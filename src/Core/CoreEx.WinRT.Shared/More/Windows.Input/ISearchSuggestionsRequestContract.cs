namespace More.Windows.Input
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.Contracts;
    using global::Windows.Storage.Streams;

    [ContractClassFor( typeof( ISearchSuggestionsRequest ) )]
    internal abstract class ISearchSuggestionsRequestContract : ISearchSuggestionsRequest
    {
        string ISearchSuggestionsRequest.Language
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return null;
            }
        }

        string ISearchSuggestionsRequest.QueryText
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return null;
            }
        }

        bool ISearchSuggestionsRequest.IsCanceled
        {
            get
            {
                return default( bool );
            }
        }

        uint ISearchSuggestionsRequest.QuerySuggestionCount
        {
            get
            {
                return 0U;
            }
        }

        IReadOnlyList<string> ISearchSuggestionsRequest.QueryTextAlternatives
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<string>>() != null );
                return null;
            }
        }

        uint ISearchSuggestionsRequest.QueryTextCompositionLength
        {
            get
            {
                return 0U;
            }
        }

        uint ISearchSuggestionsRequest.QueryTextCompositionStart
        {
            get
            {
                return 0U;
            }
        }

        IDisposable ISearchSuggestionsRequest.GetDeferral()
        {
            Contract.Ensures( Contract.Result<IDisposable>() != null );
            return null;
        }

        void ISearchSuggestionsRequest.AppendQuerySuggestion( string text )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( text ), "text" );
        }

        void ISearchSuggestionsRequest.AppendQuerySuggestions( IEnumerable<string> suggestions )
        {
            Contract.Requires<ArgumentNullException>( suggestions != null, "suggestions" );
            Contract.Requires( Contract.ForAll( suggestions, s => !string.IsNullOrEmpty( s ) ) );
        }

        void ISearchSuggestionsRequest.AppendResultSuggestion( string text, string detailText, string tag, IRandomAccessStreamReference image, string imageAlternateText )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( text ), "text" );
        }

        void ISearchSuggestionsRequest.AppendSearchSeparator( string label )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( label ), "label" );
        }
    }
}
