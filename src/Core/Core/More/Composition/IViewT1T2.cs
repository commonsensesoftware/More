namespace More.Composition
{
    using global::System;
    using global::System.Diagnostics.Contracts; 

    /// <summary>
    /// Defines the behavior of a view that can be attached to a view model.
    /// </summary>
    /// <typeparam name="TOutput">The <see cref="Type">type</see> of view model to attach to the view.</typeparam>
    /// <typeparam name="TInput">The <see cref="Type">type</see> of view model that can be used to attach the view.</typeparam>
    [ContractClass( typeof( IViewContract<,> ) )] 
    public interface IView<out TOutput, in TInput> : IView<TOutput>
        where TOutput : class
        where TInput : TOutput
    {
        /// <summary>
        /// Attaches the specified view model to the view.
        /// </summary>
        /// <param name="model">The view model of type <typeparamref name="TInput"/> to attach.</param>
        void AttachModel( TInput model );
    }
}
