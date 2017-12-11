namespace More.Web.Mvc
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web;

    [ContractClassFor( typeof( IDecoratorFactory<> ) )]
    abstract class IDecoratorFactoryTContract<T> : IDecoratorFactory<T> where T : class
    {
        T IDecoratorFactory<T>.CreatePerRequestDecorator( T instance, HttpRequestBase request )
        {
            Contract.Requires<ArgumentNullException>( instance != null, nameof( instance ) );
            Contract.Ensures( Contract.Result<T>() != null );
            return null;
        }
    }
}