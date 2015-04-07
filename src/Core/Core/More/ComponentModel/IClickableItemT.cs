namespace More.ComponentModel
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Windows.Input;

    /// <summary>
    /// Defines the behavior of a clickable item.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of value associated with the item.</typeparam>
    [ContractClass( typeof( IClickableItemContract<> ) )] 
    public interface IClickableItem<out T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        /// <value>The value of the item.</value>
        T Value
        {
            get;
        }

        /// <summary>
        /// Gets a command that can be used to click the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        ICommand Click
        {
            get;
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        event EventHandler Clicked;
    }
}
