namespace More.Windows.Interactivity
{
    using Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using global::Windows.ApplicationModel.Search;
    using global::Windows.Storage.Streams;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Represents a behavior which mediates the contract with the Search contract.
    /// </summary>
    /// <example>The following example demonstrates how to search data from a page.
    /// <code lang="Xaml"><![CDATA[
    /// <Page
    ///  x:Class="MyPage"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:i="using:System.Windows.Interactivity"
    ///  xmlns:More="using:More.Windows.Interactivity">
    ///  <i:Interaction.Behaviors>
    ///   <More:SearchContractBehavior SearchSuggestionsCommand="{Binding Commands[ProvideSuggestions]}" SearchCommand="{Binding Commands[Search]}" />
    ///  </i:Interaction.Behaviors>
    ///  <Grid>
    /// 
    ///  </Grid>
    /// </Page>
    /// ]]></code></example>
    [CLSCompliant( false )]
    public class SearchContractBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the search request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty SearchRequestProperty =
            DependencyProperty.Register( nameof( SearchRequest ), typeof( object ), typeof( SearchContractBehavior ), new PropertyMetadata( null, OnSearchRequestChanged ) );

        /// <summary>
        /// Gets or sets the search suggestions command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty SearchSuggestionsCommandProperty =
            DependencyProperty.Register( nameof( SearchSuggestionsCommand ), typeof( ICommand ), typeof( SearchContractBehavior ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the result suggestion chosen command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty SuggestionChosenCommandProperty =
            DependencyProperty.Register( nameof( SuggestionChosenCommand ), typeof( ICommand ), typeof( SearchContractBehavior ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the search command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register( nameof( SearchCommand ), typeof( ICommand ), typeof( SearchContractBehavior ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the placeholder text dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register( nameof( PlaceholderText ), typeof( string ), typeof( SearchContractBehavior ), new PropertyMetadata( string.Empty, OnPlaceholderTextPropertyChanged ) );

        /// <summary>
        /// Gets or sets the search history context dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty SearchHistoryContextProperty =
            DependencyProperty.Register( nameof( SearchHistoryContext ), typeof( string ), typeof( SearchContractBehavior ), new PropertyMetadata( string.Empty, OnSearchHistoryContextPropertyChanged ) );

        /// <summary>
        /// Gets or sets the search history enabled dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty SearchHistoryEnabledProperty =
            DependencyProperty.Register( nameof( SearchHistoryEnabled ), typeof( bool ), typeof( SearchContractBehavior ), new PropertyMetadata( true, OnSearchHistoryEnabledPropertyChanged ) );

        /// <summary>
        /// Gets or sets the show on keyboard input dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ShowOnKeyboardInputProperty =
            DependencyProperty.Register( nameof( ShowOnKeyboardInput ), typeof( bool ), typeof( SearchContractBehavior ), new PropertyMetadata( false, OnShowOnKeyboardInputPropertyChanged ) );

        SearchPane searchPane;

        SearchPane SearchPane
        {
            get => searchPane;
            set
            {
                if ( searchPane == value )
                {
                    return;
                }

                if ( searchPane != null )
                {
                    searchPane.QuerySubmitted -= OnQuerySubmitted;
                    searchPane.SuggestionsRequested -= OnSuggestionsRequested;
                    searchPane.ResultSuggestionChosen -= OnResultSuggestionChosen;
                }

                if ( ( searchPane = value ) == null )
                {
                    return;
                }

                searchPane.PlaceholderText = PlaceholderText;
                searchPane.SearchHistoryContext = SearchHistoryContext;
                searchPane.SearchHistoryEnabled = SearchHistoryEnabled;
                searchPane.ShowOnKeyboardInput = ShowOnKeyboardInput;
                searchPane.QuerySubmitted += OnQuerySubmitted;
                searchPane.SuggestionsRequested += OnSuggestionsRequested;
                searchPane.ResultSuggestionChosen += OnResultSuggestionChosen;
            }
        }

        /// <summary>
        /// Gets or sets the request that produces events that the behavior listens to in order to trigger when the Search flyout is shown.
        /// </summary>
        /// <value>The <see cref="IInteractionRequest">interaction request</see> that can trigger the Search flyout.</value>
        public IInteractionRequest SearchRequest
        {
            get => (IInteractionRequest) GetValue( SearchRequestProperty );
            set => SetValue( SearchRequestProperty, value );
        }

        /// <summary>
        /// Gets or sets the command that is invoked when the Search suggestions are requested.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> invoked when the Search suggestions are requested.</value>
        /// <remarks>The parameter passed to the command will be an <see cref="T:ISearchSuggestionsRequest"/>.</remarks>
        public ICommand SearchSuggestionsCommand
        {
            get => (ICommand) GetValue( SearchSuggestionsCommandProperty );
            set => SetValue( SearchSuggestionsCommandProperty, value );
        }

        /// <summary>
        /// Gets or sets the command that is invoked when a result suggestion is chosen.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> invoked when a result suggestion is chosen.</value>
        /// <remarks>The parameter passed to the command will be a <see cref="String">string</see> representing the suggestion chosen.</remarks>
        public ICommand SuggestionChosenCommand
        {
            get => (ICommand) GetValue( SuggestionChosenCommandProperty );
            set => SetValue( SuggestionChosenCommandProperty, value );
        }

        /// <summary>
        /// Gets or sets the command that is invoked when a Search is executed.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> invoked when a Search is executed.</value>
        /// <remarks>The parameter passed to the command will be an <see cref="ISearchRequest"/>.</remarks>
        public ICommand SearchCommand
        {
            get => (ICommand) GetValue( SearchCommandProperty );
            set => SetValue( SearchCommandProperty, value );
        }

        /// <summary>
        /// Gets or sets the placeholder text in the search box when the search box doesn't have the input focus and no characters have been entered.
        /// </summary>
        /// <value>The placeholder text to display in the search box.</value>
        public string PlaceholderText
        {
            get => (string) GetValue( PlaceholderTextProperty );
            set => SetValue( PlaceholderTextProperty, value );
        }

        /// <summary>
        /// Gets or sets a string that identifies the context of the search and is used to store the search history with the application.
        /// </summary>
        /// <value>The search history context string.</value>
        public string SearchHistoryContext
        {
            get => (string) GetValue( SearchHistoryContextProperty );
            set => SetValue( SearchHistoryContextProperty, value );
        }

        /// <summary>
        /// Gets or sets a value indicating whether previous searches with the application are automatically tracked and used to provide suggestions.
        /// </summary>
        /// <value>True if the search search history is automatically tracked and used to provide suggestions; otherwise false. The default value is true.</value>
        public bool SearchHistoryEnabled
        {
            get => (bool) GetValue( SearchHistoryEnabledProperty );
            set => SetValue( SearchHistoryEnabledProperty, value );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the search pane can open by typing.
        /// </summary>
        /// <value>True if the search pane can open by typing; otherwise false. The default value is false.</value>
        public bool ShowOnKeyboardInput
        {
            get => (bool) GetValue( ShowOnKeyboardInputProperty );
            set => SetValue( ShowOnKeyboardInputProperty, value );
        }

        static void OnSearchRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;
            var oldRequest = e.GetTypedOldValue<IInteractionRequest>();
            var newRequest = e.GetTypedNewValue<IInteractionRequest>();

            if ( oldRequest != null )
            {
                oldRequest.Requested -= @this.OnSearchRequested;
            }

            if ( newRequest != null )
            {
                newRequest.Requested += @this.OnSearchRequested;
            }
        }

        void OnSearchRequested( object sender, InteractionRequestedEventArgs e )
        {
            var search = e.Interaction;
            var query = ( search.Content ?? string.Empty ).ToString();
            SearchPane.Show( query );
        }

        void OnQuerySubmitted( SearchPane sender, SearchPaneQuerySubmittedEventArgs e )
        {
            var command = SearchCommand;

            if ( command == null )
            {
                return;
            }

            var request = new SearchRequestWrapper( e.Language, e.QueryText );

            if ( command.CanExecute( request ) )
            {
                command.Execute( request );
            }
        }

        void OnSuggestionsRequested( SearchPane sender, SearchPaneSuggestionsRequestedEventArgs e )
        {
            var command = SearchSuggestionsCommand;

            if ( command == null )
            {
                return;
            }

            var request = new SearchSuggestionRequestWrapper( e );

            if ( command.CanExecute( request ) )
            {
                command.Execute( request );
            }
        }

        void OnResultSuggestionChosen( SearchPane sender, SearchPaneResultSuggestionChosenEventArgs e )
        {
            var command = SuggestionChosenCommand;

            if ( command != null && command.CanExecute( e.Tag ) )
            {
                command.Execute( e.Tag );
            }
        }

        static void OnPlaceholderTextPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;

            if ( @this.SearchPane != null )
            {
                @this.SearchPane.PlaceholderText = (string) e.NewValue;
            }
        }

        static void OnSearchHistoryContextPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;

            if ( @this.SearchPane != null )
            {
                @this.SearchPane.SearchHistoryContext = (string) e.NewValue;
            }
        }

        static void OnSearchHistoryEnabledPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;

            if ( @this.SearchPane != null )
            {
                @this.SearchPane.SearchHistoryEnabled = (bool) e.NewValue;
            }
        }

        static void OnShowOnKeyboardInputPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;

            if ( @this.SearchPane != null )
            {
                @this.SearchPane.ShowOnKeyboardInput = (bool) e.NewValue;
            }
        }

        void OnAssociatedObjectUnloaded( object sender, RoutedEventArgs e ) => Detach();

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
            SearchPane = SearchPane.GetForCurrentView();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;
            SearchPane = null;
            base.OnDetaching();
        }

        sealed class SearchSuggestionRequestDeferralWrapper : IDisposable
        {
            bool disposed;
            SearchPaneSuggestionsRequestDeferral deferral;

            internal SearchSuggestionRequestDeferralWrapper( SearchPaneSuggestionsRequestDeferral deferral ) => this.deferral = deferral;

            public void Dispose()
            {
                if ( disposed )
                {
                    return;
                }

                disposed = true;

                if ( deferral != null )
                {
                    deferral.Complete();
                    deferral = null;
                }

                GC.SuppressFinalize( this );
            }
        }

        sealed class SearchSuggestionRequestWrapper : More.Windows.Input.ISearchSuggestionsRequest
        {
            readonly SearchPaneSuggestionsRequestedEventArgs args;

            internal SearchSuggestionRequestWrapper( SearchPaneSuggestionsRequestedEventArgs args ) => this.args = args;

            public string Language => args.Language ?? string.Empty;

            public string QueryText => args.QueryText ?? string.Empty;

            public bool IsCanceled => args.Request.IsCanceled;

            public uint QuerySuggestionCount => args.Request.SearchSuggestionCollection.Size;

            public IReadOnlyList<string> QueryTextAlternatives => args.LinguisticDetails.QueryTextAlternatives;

            public uint QueryTextCompositionLength => args.LinguisticDetails.QueryTextCompositionLength;

            public uint QueryTextCompositionStart => args.LinguisticDetails.QueryTextCompositionStart;

            [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Must be disposed by the caller." )]
            public IDisposable GetDeferral() => new SearchSuggestionRequestDeferralWrapper( args.Request.GetDeferral() );

            public void AppendQuerySuggestion( string text )
            {
                Arg.NotNullOrEmpty( text, nameof( text ) );
                args.Request.SearchSuggestionCollection.AppendQuerySuggestion( text );
            }

            public void AppendQuerySuggestions( IEnumerable<string> suggestions )
            {
                Arg.NotNull( suggestions, nameof( suggestions ) );
                args.Request.SearchSuggestionCollection.AppendQuerySuggestions( suggestions );
            }

            public void AppendResultSuggestion( string text, string detailText, string tag, IRandomAccessStreamReference image, string imageAlternateText )
            {
                Arg.NotNullOrEmpty( text, nameof( text ) );
                args.Request.SearchSuggestionCollection.AppendResultSuggestion( text, detailText, tag, image, imageAlternateText );
            }

            public void AppendSearchSeparator( string label )
            {
                Arg.NotNullOrEmpty( label, nameof( label ) );
                args.Request.SearchSuggestionCollection.AppendSearchSeparator( label );
            }
        }

        sealed class SearchRequestWrapper : ISearchRequest
        {
            internal SearchRequestWrapper( string language, string queryText )
            {
                Language = language ?? string.Empty;
                QueryText = queryText ?? string.Empty;
            }

            public string Language { get; }

            public string QueryText { get; }
        }
    }
}