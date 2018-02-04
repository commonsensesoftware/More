namespace System.Web.Mvc
{
    using More.Web.Mvc;
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides extension methods for the <see cref="Controller"/> class.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Decorates the specified instance for a controller.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of object to decorate.</typeparam>
        /// <param name="controller">The extended <see cref="Controller">controller</see>.</param>
        /// <param name="instance">The instance to be decorated.</param>
        /// <returns>A new, decorated object of <typeparamref name="T"/> if a corresponding <see cref="IDecoratorFactory{T}">decorator factory</see>
        /// can be resolved.  If the <paramref name="instance"/> is <c>null</c> or a corresponding <see cref="IDecoratorFactory{T}">decorator factory</see>
        /// is not registered, the original <paramref name="instance"/> is returned.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static T Decorate<T>( this Controller controller, T instance ) where T : class
        {
            Arg.NotNull( controller, nameof( controller ) );

            if ( instance == null )
            {
                return instance;
            }

            var factory = DependencyResolver.Current.GetService<IDecoratorFactory<T>>();

            return factory == null ? instance : factory.CreatePerRequestDecorator( instance, controller.Request );
        }
    }
}