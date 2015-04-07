namespace More.Composition
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IView<,> ) )]
    internal abstract class IViewContract<TOutput, TInput> : IView<TOutput, TInput>
        where TOutput : class
        where TInput : TOutput
    {
        void IView<TOutput, TInput>.AttachModel( TInput model )
        {
            Contract.Requires<ArgumentNullException>( model != null, "model" );
        }

        TOutput IView<TOutput>.Model
        {
            get
            {
                return default( TOutput );
            }
        }
    }
}
