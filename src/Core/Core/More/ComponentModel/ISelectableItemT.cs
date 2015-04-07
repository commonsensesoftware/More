namespace More.ComponentModel
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Windows.Input;

    /// <summary>
    /// Defines the behavior of a selectable item.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of value associated with the item.</typeparam>
    [ContractClass( typeof( ISelectableItemContract<> ) )] 
    public interface ISelectableItem<out T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        /// <value>A <see cref="Nullable{T}"/> object.</value>
        bool? IsSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        /// <value>The value of the item.</value>
        T Value
        {
            get;
        }

        /// <summary>
        /// Gets a command that can be used to select the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Select", Justification = "This will not cause a cross language issues." )]
        ICommand Select
        {
            get;
        }

        /// <summary>
        /// Gets a command that can be used to unselect the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        ICommand Unselect
        {
            get;
        }

        /// <summary>
        /// Occurs when the item is selected.
        /// </summary>
        event EventHandler Selected;

        /// <summary>
        /// Occurs when the item is unselected.
        /// </summary>
        event EventHandler Unselected;

        /// <summary>
        /// Occurs when the selected state of the item is indeterminate.
        /// </summary>
        event EventHandler Indeterminate;
    }
}
