namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IView<> ) )]
    abstract class IViewContract<T> : IView<T> where T : class
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