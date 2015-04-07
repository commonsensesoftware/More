namespace More.Composition
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IView<> ) )]
    internal abstract class IViewContract<T> : IView<T> where T : class
    {
        T IView<T>.Model
        {
            get
            {
                Contract.Ensures( Contract.Result<T>() != null );
                return default( T );
            }
        }
    }
}
