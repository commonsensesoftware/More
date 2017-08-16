namespace More.Windows.Input
{
    using IO;
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents an interaction request to select a folder with support for continuations.
    /// </summary>
    public class SelectFolderInteractionRequest : ContinuableInteractionRequest<SelectFolderInteraction, IFolder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectFolderInteractionRequest"/> class.
        /// </summary>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public SelectFolderInteractionRequest( Action<IFolder> continuation ) : base( continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectFolderInteractionRequest"/> class.
        /// </summary>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public SelectFolderInteractionRequest( IContinuationManager continuationManager, Action<IFolder> continuation )
            : base( continuationManager, continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectFolderInteractionRequest"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public SelectFolderInteractionRequest( string id, Action<IFolder> continuation )
            : base( id, continuation ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectFolderInteractionRequest"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        public SelectFolderInteractionRequest( string id, IContinuationManager continuationManager, Action<IFolder> continuation )
            : base( id, continuationManager, continuation ) { }

        /// <summary>
        /// Sets any applicable continuation data when an interaction is requested.
        /// </summary>
        /// <param name="interaction">The <see cref="SelectFolderInteraction">interaction</see> to set the continuation data for.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected override void SetContinuationData( SelectFolderInteraction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            base.SetContinuationData( interaction );
            interaction.ContinuationData["Continuation"] = "SelectFolder";
        }
    }
}