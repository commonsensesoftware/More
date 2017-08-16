namespace More.Windows.Input
{
    using IO;
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents an interaction request to save a file with support for continuations.
    /// </summary>
    public class SaveFileInteractionRequest : ContinuableInteractionRequest<SaveFileInteraction, IFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteractionRequest"/> class.
        /// </summary>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public SaveFileInteractionRequest( Action<IFile> continuation ) : base( continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteractionRequest"/> class.
        /// </summary>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public SaveFileInteractionRequest( IContinuationManager continuationManager, Action<IFile> continuation )
            : base( continuationManager, continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteractionRequest"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public SaveFileInteractionRequest( string id, Action<IFile> continuation )
            : base( id, continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteractionRequest"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public SaveFileInteractionRequest( string id, IContinuationManager continuationManager, Action<IFile> continuation )
            : base( id, continuationManager, continuation ) { }

        /// <summary>
        /// Sets any applicable continuation data when an interaction is requested.
        /// </summary>
        /// <param name="interaction">The <see cref="SaveFileInteraction">interaction</see> to set the continuation data for.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected override void SetContinuationData( SaveFileInteraction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            base.SetContinuationData( interaction );
            interaction.ContinuationData["Continuation"] = "SaveFile";
        }
    }
}