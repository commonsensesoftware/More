namespace More.ComponentModel
{
    using System;

    /// <summary>
    /// Defines the behavior of components that support deferment<seealso cref="DeferManager"/>.
    /// </summary>
    public interface IDeferrable
    {
        /// <summary>
        /// Begins a level of deferment.
        /// </summary>
        void BeginDefer();

        /// <summary>
        /// Ends a level of deferment.
        /// </summary>
        void EndDefer();
    }
}
