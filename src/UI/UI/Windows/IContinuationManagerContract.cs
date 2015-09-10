namespace More.Windows
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IContinuationManager ) )]
    internal abstract class IContinuationManagerContract : IContinuationManager
    {
        void IContinuationManager.Continue<TArg>( TArg arg )
        {
            Contract.Requires<ArgumentNullException>( arg != null, nameof( arg ) );
        }

        long IContinuationManager.Register<TArg>( Action<TArg> continuation )
        {
            Contract.Requires<ArgumentNullException>( continuation != null, nameof( continuation ) );
            return default( long );
        }
    }
}
