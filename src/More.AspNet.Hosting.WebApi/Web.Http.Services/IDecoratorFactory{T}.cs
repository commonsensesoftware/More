namespace More.Web.Http.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;

    /// <summary>
    /// Defines the behavior of a decorator factory.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of object to decorate.</typeparam>
    [ContractClass( typeof( IDecoratorFactoryContract<> ) )]
    public interface IDecoratorFactory<T> where T : class
    {
        /// <summary>
        /// Creates and returns a decorator for the specified request and instance.
        /// </summary>
        /// <param name="instance">The instance of type <typeparamref name="T"/> to be decorated.</param>
        /// <param name="request">The current <see cref="HttpRequestMessage">request</see> to create a decorator for.</param>
        /// <returns>An new instance of type <typeparamref name="T"/> that decorates the original <paramref name="instance"/>.</returns>
        T CreatePerRequestDecorator( T instance, HttpRequestMessage request );
    }
}