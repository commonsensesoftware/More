namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts; 

    /// <summary>
    /// Defines the behavior for a view.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of view model associated with the view.</typeparam>
    [ContractClass( typeof( IViewContract<> ) )] 
    public interface IView<out T> where T : class
    {
        /// <summary>
        /// Gets the view model associated with the view.
        /// </summary>
        /// <value>A view model of type <typeparamref name="T"/>.</value>
        T Model
        {
            get;
        }
    }
}
