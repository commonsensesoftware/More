namespace More.Windows
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the behavior of a continuation manager.
    /// </summary>
    /// <remarks>A continuation manager is used to facilitate dialog picker continuations.</remarks>
    [ContractClass( typeof( IContinuationManagerContract ) )]
    public interface IContinuationManager
    {
        /// <summary>
        /// Registers the specified continuation action and returns the registered continuation identifier.
        /// </summary>
        /// <typeparam name="TArg">The <see cref="Type">type</see> of parameter supplied to the continuation <see cref="Action{T}">action</see>.</typeparam>
        /// <param name="continuation">The continuation <see cref="Action{T}">action</see>.</param>
        /// <returns>The registered continuation identifier for the supplied <paramref name="continuation"/>.</returns>
        long Register<TArg>( Action<TArg> continuation );

        /// <summary>
        /// Continues a dialog picker operation.
        /// </summary>
        /// <typeparam name="TArg">The <see cref="Type">type</see> of parameter supplied to corresponding, registered continuation.</typeparam>
        /// <param name="arg">The <typeparamref name="TArg">argument</typeparamref> for the continued operation.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Continue", Justification = "Will not cause any known cross-language issues and is the most appropriate term." )]
        void Continue<TArg>( TArg arg );
    }
}
