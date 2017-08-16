namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

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
        T Value { get; }

        /// <summary>
        /// Gets a command that can be used to click the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        ICommand Click { get; }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        event EventHandler Clicked;
    }
}