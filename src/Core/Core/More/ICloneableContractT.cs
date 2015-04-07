namespace More
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( ICloneable<> ) )]
    internal abstract class ICloneableContract<T> : ICloneable<T>
    {
        TClone ICloneable<T>.Clone<TClone>()
        {
            Contract.Ensures( Contract.Result<TClone>() != null );
            return default( TClone );
        }
    }
}
