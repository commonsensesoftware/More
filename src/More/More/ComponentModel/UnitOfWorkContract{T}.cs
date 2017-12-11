namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( UnitOfWork<> ) )]
    abstract class UnitOfWorkContract<T> : UnitOfWork<T> where T : class
    {
        protected override bool IsNew( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, nameof( item ) );
            return default( bool );
        }
    }
}