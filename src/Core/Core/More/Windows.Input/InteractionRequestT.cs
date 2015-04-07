namespace More.Windows.Input
{
    using More.ComponentModel;
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a user interface interaction request.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="Interaction">interaction</see> requested.</typeparam>
    public class InteractionRequest<T> : ObservableObject, IInteractionRequest where T : Interaction
    {
        private string id;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionRequest{T}"/> class.
        /// </summary>
        public InteractionRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionRequest{T}"/> class.
        /// </summary>
        /// <param name="id">The identifier associated with the request.</param>
        public InteractionRequest( string id )
        {
            this.id = id;
        }

        /// <summary>
        /// Raises the <see cref="E:Requested"/> event.
        /// </summary>
        /// <param name="e">The <see cref="InteractionRequestedEventArgs"/> event data.</param>
        protected virtual void OnRequested( InteractionRequestedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.Requested;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Requests user interaction.
        /// </summary>
        /// <param name="interaction">The interaction <see cref="Interaction">interaction</see> request.</param>
        public void Request( T interaction )
        {
            Contract.Requires<ArgumentException>( interaction != null, "interaction" );
            this.OnRequested( new InteractionRequestedEventArgs( interaction ) );
        }

        /// <summary>
        /// Gets or sets the identifier associated with the interaction request.
        /// </summary>
        /// <value>The request identifier. The default value is null.</value>
        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.SetProperty( ref this.id, value );
            }
        }

        /// <summary>
        /// Occurs when the user interaction is requested.
        /// </summary>
        public event EventHandler<InteractionRequestedEventArgs> Requested;
    }
}
