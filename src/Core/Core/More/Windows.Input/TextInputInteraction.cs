namespace More.Windows.Input
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an interaction request for text input from a user.
    /// </summary>
    /// <example>The following example demonstrates how to show an input box from a view model
    /// without any directly knowledge or coupling with a view.
    /// <code lang="C#">
    /// <![CDATA[
    /// using global::System;
    /// using global::System.Diagnostics;
    /// using global::System.Windows;
    /// using global::System.Windows.Input;
    /// 
    /// public class MyViewModel
    /// {
    ///     private readonly InteractionRequest<TextInputNotification> getInput = new InteractionRequest<TextInputNotification>();
    ///     
    ///     public IInteractionRequest InputInteraction
    ///     {
    ///         get
    ///         {
    ///             return this.getInput;
    ///         }
    ///     }
    ///     
    ///     public void PromptUserForInput()
    ///     {
    ///         TextInputInteraction interaction = null;
    ///         
    ///         interaction = new TextInputInteraction()
    ///         {
    ///             Title = "Feedback",
    ///             Content = "Please provide some input:",
    ///             DefaultResponse = "[Comments]",
    ///             DefaultCommandIndex = 0,
    ///             CancelCommandIndex = 1,
    ///             Commands =
    ///             {
    ///                 new NamedCommand<object>( "Submit", p => Debug.WriteLine( interaction.Response ) ),
    ///                 new NamedCommand<object>( "Cancel", p => {} )
    ///             }
    ///         };
    ///         
    ///         this.getInput.Request( interaction );
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class TextInputInteraction : Interaction
    {
        private string defaultResponse;
        private string response;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputInteraction"/> class.
        /// </summary>
        public TextInputInteraction()
            : this( string.Empty, string.Empty, string.Empty )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="prompt">The prompt associated with the interaction.</param>
        public TextInputInteraction( string title, string prompt )
            : this( title, prompt, string.Empty )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
            Contract.Requires<ArgumentNullException>( prompt != null, "prompt" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="prompt">The prompt associated with the interaction.</param>
        /// <param name="defaultResponse">The default text input response.</param>
        public TextInputInteraction( string title, string prompt, string defaultResponse )
            : base( title, prompt )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
            Contract.Requires<ArgumentNullException>( prompt != null, "prompt" );
            Contract.Requires<ArgumentNullException>( defaultResponse != null, "defaultResponse" );

            this.Content = prompt;
            this.defaultResponse = defaultResponse;
        }

        /// <summary>
        /// Gets or sets the default text input response.
        /// </summary>
        /// <value>The text input default response.</value>
        public string DefaultResponse
        {
            get
            {
                Contract.Ensures( this.defaultResponse != null );
                return this.defaultResponse;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.SetProperty( ref this.defaultResponse, value );
            }
        }

        /// <summary>
        /// Gets or sets the text input response.
        /// </summary>
        /// <value>The text input response.</value>
        public string Response
        {
            get
            {
                return this.response;
            }
            set
            {
                this.SetProperty( ref this.response, value );
            }
        }
    }
}
