namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IEditTransaction ) )]
    internal abstract class IEditTransactionContract : IEditTransaction
    {
        void IEditTransaction.Begin()
        {
        }

        void IEditTransaction.Commit()
        {
        }

        IEditSavepoint IEditTransaction.CreateSavepoint()
        {
            Contract.Ensures( Contract.Result<IEditSavepoint>() != null );
            return null;
        }

        void IEditTransaction.Rollback()
        {
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        void IEditTransaction.Rollback( IEditSavepoint savepoint )
        {
            // savepoint must have been created by the current transaction
            Contract.Requires<ArgumentNullException>( savepoint != null, "savepoint" );
            Contract.Requires<ArgumentException>( Equals( savepoint.Transaction ), "savepoint" );
        }
    }
}
