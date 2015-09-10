namespace More.Windows.Input
{
    using IO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents an interaction request to open a file with support for continuations.
    /// </summary>
    public class OpenFileInteractionRequest : ContinuableInteractionRequest<OpenFileInteraction, IList<IFile>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileInteractionRequest"/> class.
        /// </summary>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support Action<T> with a generic argument." )]
        public OpenFileInteractionRequest( Action<IList<IFile>> continuation )
            : base( continuation )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileInteractionRequest"/> class.
        /// </summary>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support Action<T> with a generic argument." )]
        public OpenFileInteractionRequest( IContinuationManager continuationManager, Action<IList<IFile>> continuation )
            : base( continuationManager, continuation )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileInteractionRequest"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support Action<T> with a generic argument." )]
        public OpenFileInteractionRequest( string id, Action<IList<IFile>> continuation )
            : base( id, continuation )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileInteractionRequest"/> class.
        /// </summary>
        /// <param name="id">The interaction request identifier.</param>
        /// <param name="continuationManager">The <see cref="IContinuationManager">continuation manager</see> used to register continuations.</param>
        /// <param name="continuation">The interaction continuation <see cref="Action{T}">action</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support Action<T> with a generic argument." )]
        public OpenFileInteractionRequest( string id, IContinuationManager continuationManager, Action<IList<IFile>> continuation )
            : base( id, continuationManager, continuation )
        {
        }

        /// <summary>
        /// Sets any applicable continuation data when an interaction is requested.
        /// </summary>
        /// <param name="interaction">The <see cref="OpenFileInteraction">interaction</see> to set the continuation data for.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected override void SetContinuationData( OpenFileInteraction interaction )
        {
            Arg.NotNull( interaction, nameof( interaction ) );
            base.SetContinuationData( interaction );
            interaction.ContinuationData["Continuation"] = "OpenFile";
        }
    }
}
