namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Defines the behavior of an expandable item.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of expandable item.</typeparam>
    [ContractClass( typeof( IExpandableItemContract<> ) )]
    public interface IExpandableItem<out T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded.
        /// </summary>
        /// <value>True if the item is expanded; otherwise, false.</value>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        /// <value>The value of the item.</value>
        T Value { get; }

        /// <summary>
        /// Gets a command that can be used to expand the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        ICommand Expand { get; }

        /// <summary>
        /// Gets a command that can be used to collapse the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        ICommand Collapse { get; }

        /// <summary>
        /// Occurs when the item has expanded.
        /// </summary>
        event EventHandler Expanded;

        /// <summary>
        /// Occurs when the item has collapsed.
        /// </summary>
        event EventHandler Collapsed;
    }
}