namespace More.Windows
{
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    [ContractClassFor( typeof( IContinuationManager ) )]
    internal abstract class IContinuationManagerContract : IContinuationManager
    {
        void IContinuationManager.Continue<TEventArgs>( TEventArgs eventArgs )
        {
            Contract.Requires<ArgumentNullException>( eventArgs != null, "eventArgs" );
        }

        InteractionRequest<TInteraction> IContinuationManager.CreateInteractionRequest<TInteraction, TEventArgs>( string id, Action<TEventArgs> continuation )
        {
            Contract.Requires<ArgumentNullException>( continuation != null, "continuation" );
            Contract.Ensures( Contract.Result<InteractionRequest<TInteraction>>() != null );
            return null;
        }
    }
}
