namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IView<,> ) )]
    abstract class IViewContract<TOutput, TInput> : IView<TOutput, TInput>
        where TOutput : class
        where TInput : TOutput
    {
        void IView<TOutput, TInput>.AttachModel( TInput model ) =>
            Contract.Requires<ArgumentNullException>( model != null, nameof( model ) );

        TOutput IView<TOutput>.Model => default( TOutput );
    }
}