namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
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
    ///  xmlns:More="using:System.Windows.Interactivity">
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
        private sealed class SearchSuggestionRequestDeferralWrapper : IDisposable
        {
            private bool disposed;
            private SearchPaneSuggestionsRequestDeferral deferral;

            internal SearchSuggestionRequestDeferralWrapper( SearchPaneSuggestionsRequestDeferral deferral )
            {
                Contract.Requires( deferral != null );
                this.deferral = deferral;
            }

            public void Dispose()
            {
                if ( this.disposed )
                    return;

                this.disposed = true;

                if ( this.deferral != null )
                {
                    this.deferral.Complete();
                    this.deferral = null;
                }

                GC.SuppressFinalize( this );
            }
        }

        private sealed class SearchSuggestionRequestWrapper : More.Windows.Input.ISearchSuggestionsRequest
        {
            private readonly SearchPaneSuggestionsRequestedEventArgs args;

            internal SearchSuggestionRequestWrapper( SearchPaneSuggestionsRequestedEventArgs args )
            {
                Contract.Requires( args != null );
                this.args = args;
            }

            public string Language
            {
                get
                {
                    return this.args.Language ?? string.Empty;
                }
            }

            public string QueryText
            {
                get
                {
                    return this.args.QueryText ?? string.Empty;
                }
            }

            public bool IsCanceled
            {
                get
                {
                    return this.args.Request.IsCanceled;
                }
            }

            public uint QuerySuggestionCount
            {
                get
                {
                    return this.args.Request.SearchSuggestionCollection.Size;
                }
            }

            public IReadOnlyList<string> QueryTextAlternatives
            {
                get
                {
                    return this.args.LinguisticDetails.QueryTextAlternatives;
                }
            }

            public uint QueryTextCompositionLength
            {
                get
                {
                    return this.args.LinguisticDetails.QueryTextCompositionLength;
                }
            }

            public uint QueryTextCompositionStart
            {
                get
                {
                    return this.args.LinguisticDetails.QueryTextCompositionStart;
                }
            }

            public IDisposable GetDeferral()
            {
                return new SearchSuggestionRequestDeferralWrapper( this.args.Request.GetDeferral() );
            }

            public void AppendQuerySuggestion( string text )
            {
                this.args.Request.SearchSuggestionCollection.AppendQuerySuggestion( text );
            }

            public void AppendQuerySuggestions( IEnumerable<string> suggestions )
            {
                this.args.Request.SearchSuggestionCollection.AppendQuerySuggestions( suggestions );
            }

            public void AppendResultSuggestion( string text, string detailText, string tag, IRandomAccessStreamReference image, string imageAlternateText )
            {
                this.args.Request.SearchSuggestionCollection.AppendResultSuggestion( text, detailText, tag, image, imageAlternateText );
            }

            public void AppendSearchSeparator( string label )
            {
                this.args.Request.SearchSuggestionCollection.AppendSearchSeparator( label );
            }
        }

        private sealed class SearchRequestWrapper : ISearchRequest
        {
            private readonly string language;
            private readonly string query;

            internal SearchRequestWrapper( string language, string query )
            {
                this.language = language ?? string.Empty;
                this.query = query ?? string.Empty;
            }

            public string Language
            {
                get
                {
                    return this.language;
                }
            }

            public string QueryText
            {
                get
                {
                    return this.query;
                }
            }
        }

        /// <summary>
        /// Gets or sets the search request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SearchRequestProperty =
            DependencyProperty.Register( "SearchRequest", typeof( object ), typeof( SearchContractBehavior ), new PropertyMetadata( null, OnSearchRequestChanged ) );

        /// <summary>
        /// Gets or sets the search suggestions command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SearchSuggestionsCommandProperty =
            DependencyProperty.Register( "SearchSuggestionsCommand", typeof( ICommand ), typeof( SearchContractBehavior ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the result suggestion chosen command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SuggestionChosenCommandProperty =
            DependencyProperty.Register( "SuggestionChosenCommand", typeof( ICommand ), typeof( SearchContractBehavior ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the search command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register( "SearchCommand", typeof( ICommand ), typeof( SearchContractBehavior ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the placeholder text dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register( "PlaceholderText", typeof( string ), typeof( SearchContractBehavior ), new PropertyMetadata( string.Empty, OnPlaceholderTextPropertyChanged ) );

        /// <summary>
        /// Gets or sets the search history context dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SearchHistoryContextProperty =
            DependencyProperty.Register( "SearchHistoryContext", typeof( string ), typeof( SearchContractBehavior ), new PropertyMetadata( string.Empty, OnSearchHistoryContextPropertyChanged ) );

        /// <summary>
        /// Gets or sets the search history enabled dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SearchHistoryEnabledProperty =
            DependencyProperty.Register( "SearchHistoryEnabled", typeof( bool ), typeof( SearchContractBehavior ), new PropertyMetadata( true, OnSearchHistoryEnabledPropertyChanged ) );

        /// <summary>
        /// Gets or sets the show on keyboard input dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ShowOnKeyboardInputProperty =
            DependencyProperty.Register( "ShowOnKeyboardInput", typeof( bool ), typeof( SearchContractBehavior ), new PropertyMetadata( false, OnShowOnKeyboardInputPropertyChanged ) );

        private SearchPane searchPane;

        private SearchPane SearchPane
        {
            get
            {
                return this.searchPane;
            }
            set
            {
                if ( this.searchPane == value )
                    return;

                if ( this.searchPane != null )
                {
                    this.searchPane.QuerySubmitted -= this.OnQuerySubmitted;
                    this.searchPane.SuggestionsRequested -= this.OnSuggestionsRequested;
                    this.searchPane.ResultSuggestionChosen -= this.OnResultSuggestionChosen;
                }

                if ( ( this.searchPane = value ) == null )
                    return;

                this.searchPane.PlaceholderText = this.PlaceholderText;
                this.searchPane.SearchHistoryContext = this.SearchHistoryContext;
                this.searchPane.SearchHistoryEnabled = this.SearchHistoryEnabled;
                this.searchPane.ShowOnKeyboardInput = this.ShowOnKeyboardInput;
                this.searchPane.QuerySubmitted += this.OnQuerySubmitted;
                this.searchPane.SuggestionsRequested += this.OnSuggestionsRequested;
                this.searchPane.ResultSuggestionChosen += this.OnResultSuggestionChosen;
            }
        }

        /// <summary>
        /// Gets or sets the request that produces events that the behavior listens to in order to trigger when the Search flyout is shown.
        /// </summary>
        /// <value>The <see cref="IInteractionRequest">interaction request</see> that can trigger the Search flyout.</value>
        public IInteractionRequest SearchRequest
        {
            get
            {
                return (IInteractionRequest) this.GetValue( SearchRequestProperty );
            }
            set
            {
                this.SetValue( SearchRequestProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the command that is invoked when the Search suggestions are requested.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> invoked when the Search suggestions are requested.</value>
        /// <remarks>The parameter passed to the command will be an <see cref="ISearchSuggestionsRequest"/>.</remarks>
        public ICommand SearchSuggestionsCommand
        {
            get
            {
                return (ICommand) this.GetValue( SearchSuggestionsCommandProperty );
            }
            set
            {
                this.SetValue( SearchSuggestionsCommandProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the command that is invoked when a result suggestion is chosen.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> invoked when a result suggestion is chosen.</value>
        /// <remarks>The parameter passed to the command will be a <see cref="String">string</see> representing the suggestion chosen.</remarks>
        public ICommand SuggestionChosenCommand
        {
            get
            {
                return (ICommand) this.GetValue( SuggestionChosenCommandProperty );
            }
            set
            {
                this.SetValue( SuggestionChosenCommandProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the command that is invoked when a Search is executed.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> invoked when a Search is executed.</value>
        /// <remarks>The parameter passed to the command will be an <see cref="ISearchRequest"/>.</remarks>
        public ICommand SearchCommand
        {
            get
            {
                return (ICommand) this.GetValue( SearchCommandProperty );
            }
            set
            {
                this.SetValue( SearchCommandProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the placeholder text in the search box when the search box doesn't have the input focus and no characters have been entered.
        /// </summary>
        /// <value>The placeholder text to display in the search box.</value>
        public string PlaceholderText
        {
            get
            {
                return (string) this.GetValue( PlaceholderTextProperty );
            }
            set
            {
                this.SetValue( PlaceholderTextProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets a string that identifies the context of the search and is used to store the search history with the application.
        /// </summary>
        /// <value>The search history context string.</value>
        public string SearchHistoryContext
        {
            get
            {
                return (string) this.GetValue( SearchHistoryContextProperty );
            }
            set
            {
                this.SetValue( SearchHistoryContextProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether previous searches with the application are automatically tracked and used to provide suggestions.
        /// </summary>
        /// <value>True if the search search history is automatically tracked and used to provide suggestions; otherwise false. The default value is true.</value>
        public bool SearchHistoryEnabled
        {
            get
            {
                return (bool) this.GetValue( SearchHistoryEnabledProperty );
            }
            set
            {
                this.SetValue( SearchHistoryEnabledProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the search pane can open by typing.
        /// </summary>
        /// <value>True if the search pane can open by typing; otherwise false. The default value is false.</value>
        public bool ShowOnKeyboardInput
        {
            get
            {
                return (bool) this.GetValue( ShowOnKeyboardInputProperty );
            }
            set
            {
                this.SetValue( ShowOnKeyboardInputProperty, value );
            }
        }

        private static void OnSearchRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;
            var oldRequest = e.GetTypedOldValue<IInteractionRequest>();
            var newRequest = e.GetTypedNewValue<IInteractionRequest>();

            if ( oldRequest != null )
                oldRequest.Requested -= @this.OnSearchRequested;

            if ( newRequest != null )
                newRequest.Requested += @this.OnSearchRequested;
        }

        private void OnSearchRequested( object sender, InteractionRequestedEventArgs e )
        {
            var search = e.Interaction;
            var query = ( search.Content ?? string.Empty ).ToString();
            this.SearchPane.Show( query );
        }

        private void OnQuerySubmitted( SearchPane sender, SearchPaneQuerySubmittedEventArgs e )
        {
            var command = this.SearchCommand;

            if ( command == null )
                return;

            var request = new SearchRequestWrapper( e.Language, e.QueryText );

            if ( command.CanExecute( request ) )
                command.Execute( request );
        }

        private void OnSuggestionsRequested( SearchPane sender, SearchPaneSuggestionsRequestedEventArgs e )
        {
            var command = this.SearchSuggestionsCommand;

            if ( command == null )
                return;

            var request = new SearchSuggestionRequestWrapper( e );

            if ( command.CanExecute( request ) )
                command.Execute( request );
        }

        private void OnResultSuggestionChosen( SearchPane sender, SearchPaneResultSuggestionChosenEventArgs e )
        {
            var command = this.SuggestionChosenCommand;

            if ( command != null && command.CanExecute( e.Tag ) )
                command.Execute( e.Tag );
        }

        private static void OnPlaceholderTextPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;

            if ( @this.SearchPane != null )
                @this.SearchPane.PlaceholderText = (string) e.NewValue;
        }

        private static void OnSearchHistoryContextPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;

            if ( @this.SearchPane != null )
                @this.SearchPane.SearchHistoryContext = (string) e.NewValue;
        }

        private static void OnSearchHistoryEnabledPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;

            if ( @this.SearchPane != null )
                @this.SearchPane.SearchHistoryEnabled = (bool) e.NewValue;
        }

        private static void OnShowOnKeyboardInputPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SearchContractBehavior) sender;

            if ( @this.SearchPane != null )
                @this.SearchPane.ShowOnKeyboardInput = (bool) e.NewValue;
        }

        private void OnAssociatedObjectUnloaded( object sender, RoutedEventArgs e )
        {
            this.Detach();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Unloaded += this.OnAssociatedObjectUnloaded;
            this.SearchPane = SearchPane.GetForCurrentView();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.Unloaded -= this.OnAssociatedObjectUnloaded;
            this.SearchPane = null;
            base.OnDetaching();
        }
    }
}
