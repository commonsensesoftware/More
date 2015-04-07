namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( UnitOfWork<> ) )]
    internal abstract class UnitOfWorkContract<T> : UnitOfWork<T> where T : class
    {
        protected override bool IsNew( T item )
        {
            Contract.Requires<ArgumentNullException>( item != null, "item" );
            return default( bool );
        }
    }
}
