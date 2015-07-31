namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to show the
    /// <see cref="T:Interaction">interaction</see> from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    public class PrintAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Gets the dependency property indicating whether the dialog allows user print options.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ShowPrintOptionsProperty =
            DependencyProperty.Register( nameof( ShowPrintOptions ), typeof( bool ), typeof( PrintAction ), new PropertyMetadata( true ) );

        /// <summary>
        /// Gets the dependency property for the window <see cref="Style">style</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register( nameof( Style ), typeof( Style ), typeof( PrintAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property indicating whether search is supported.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SupportsSearchProperty =
            DependencyProperty.Register( nameof( SupportsSearch ), typeof( bool ), typeof( PrintAction ), new PropertyMetadata( false ) );

        /// <summary>
        /// Gets the dependency property for the document.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register( nameof( Document ), typeof( FixedDocument ), typeof( PrintAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property for the document source resource URI.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty DocumentSourceProperty =
            DependencyProperty.Register( nameof( DocumentSource ), typeof( Uri ), typeof( PrintAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets a value indicating whether the action allow user print preferences.
        /// </summary>
        /// <value>True if the action presents print preferences; otherwise, false.</value>
        public bool ShowPrintOptions
        {
            get
            {
                return (bool) GetValue( ShowPrintOptionsProperty );
            }
            set
            {
                SetValue( ShowPrintOptionsProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the window shown by the action.
        /// </summary>
        /// <value>The <see cref="Style">style</see> applied to the <see cref="Window">window</see>
        /// shown by the action.</value>
        public Style Style
        {
            get
            {
                return (Style) GetValue( StyleProperty );
            }
            set
            {
                SetValue( StyleProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the print dialog supports searching.
        /// </summary>
        /// <value>True if the print dialog supports searching; otherwise, false. The default value is false.</value>
        public bool SupportsSearch
        {
            get
            {
                return (bool) GetValue( SupportsSearchProperty );
            }
            set
            {
                SetValue( SupportsSearchProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the document to print.
        /// </summary>
        /// <value>The <see cref="FixedDocument">document</see> to print.</value>
        public FixedDocument Document
        {
            get
            {
                return (FixedDocument) GetValue( DocumentProperty );
            }
            set
            {
                SetValue( DocumentProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the resource Uniform Resource Indicator (URI) of the document source.
        /// </summary>
        /// <value>The document source <see cref="Uri">URI</see>.</value>
        /// <remarks>If a print <see cref="P:Document">document</see> has been set, then this property is ignored.</remarks>
        public Uri DocumentSource
        {
            get
            {
                return (Uri) GetValue( DocumentSourceProperty );
            }
            set
            {
                SetValue( DocumentSourceProperty, value );
            }
        }

        private static FixedDocument GetDocument( Uri resourceUri )
        {
            Contract.Requires( resourceUri != null );
            Contract.Ensures( Contract.Result<FixedDocument>() != null );

            var streamInfo = Application.GetResourceStream( resourceUri );
            FixedDocument document = null;

            using ( var stream = streamInfo.Stream )
                document = (FixedDocument) XamlReader.Load( stream );

            return document;
        }

        /// <summary>
        /// Prints the specified document with the provided data context.
        /// </summary>
        /// <param name="interaction">The requested print <see cref="PrintInteraction">interaction</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Print( PrintInteraction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );

            var doc = Document ?? GetDocument( DocumentSource );
            doc.DataContext = interaction.Content;

            var dialog = new PrintDialog();

            if ( ShowPrintOptions )
            {
                var result = dialog.ShowDialog() ?? false;

                if ( result )
                    interaction.ExecuteDefaultCommand();
                else
                    interaction.ExecuteCancelCommand();

                return;
            }

            dialog.PrintDocument( doc.DocumentPaginator, interaction.Title );
            interaction.ExecuteDefaultCommand();
        }

        /// <summary>
        /// Previews a print job for the specified document with the provided data context.
        /// </summary>
        /// <param name="interaction">The requested print preview <see cref="PrintInteraction">interaction</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void PrintPreview( PrintInteraction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );

            var doc = Document ?? GetDocument( DocumentSource );
            doc.DataContext = interaction.Content;

            var viewer = new DocumentViewer()
            {
                Document = doc
            };
            var window = new Window()
            {
                Title = SR.PrintPreviewTitle.FormatDefault( interaction.Title ),
                Content = viewer,
                Owner = Window.GetWindow( AssociatedObject ),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if ( Style != null )
                window.Style = Style;

            viewer.ApplyTemplate();

            // set default height if it's not set by a style
            if ( double.IsNaN( window.Height ) )
                window.Height = 500d;

            // set default width if it's not set by a style
            if ( double.IsNaN( window.Width ) )
                window.Width = 500d;

            if ( !SupportsSearch )
            {
                // hide the search box by collapsing the templated content control
                var searchBox = (ContentControl) viewer.Template.FindName( "PART_FindToolBarHost", viewer );
                searchBox.Visibility = Visibility.Collapsed;
            }

            var result = window.ShowDialog() ?? false;

            if ( result )
                interaction.ExecuteDefaultCommand();
            else
                interaction.ExecuteCancelCommand();
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="args">The <see cref="InteractionRequestedEventArgs"/> event data provided by the corresponding trigger.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Invoke( InteractionRequestedEventArgs args )
        {
            Arg.NotNull( args, nameof( args ) );

            if ( DocumentSource == null )
                return;

            var interaction = args.Interaction as PrintInteraction;

            if ( interaction == null )
                return;

            if ( interaction.PrintPreview )
                PrintPreview( interaction );
            else
                Print( interaction );
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="parameter">The parameter supplied from the corresponding trigger.</param>
        /// <remarks>This method is not meant to be called directly by your code.</remarks>
        protected sealed override void Invoke( object parameter )
        {
            var args = parameter as InteractionRequestedEventArgs;

            if ( args != null )
                Invoke( args );
        }
    }
}
