namespace More.Web.Http.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;

    [ContractClassFor( typeof( IDecoratorFactory<> ) )]
    abstract class IDecoratorFactoryTContract<T> : IDecoratorFactory<T> where T : class
    {
        T IDecoratorFactory<T>.CreatePerRequestDecorator( T instance, HttpRequestMessage request )
        {
            Contract.Requires<ArgumentNullException>( instance != null, nameof( instance ) );
            Contract.Ensures( Contract.Result<T>() != null );
            return null;
        }
    }
}