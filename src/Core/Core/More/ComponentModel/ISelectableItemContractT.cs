namespace More.ComponentModel
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;
    using global::System.Windows.Input;

    /// <summary>
    /// Provides the code contract definition for the <see cref="ISelectableItem{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item.</typeparam>
    [ContractClassFor( typeof( ISelectableItem<> ) )]
    internal abstract class ISelectableItemContract<T> : ISelectableItem<T>
    {
        bool? ISelectableItem<T>.IsSelected
        {
            get;
            set;
        }

        T ISelectableItem<T>.Value
        {
            get
            {
                return default( T );
            }
        }

        ICommand ISelectableItem<T>.Select
        {
            get
            {
                Contract.Ensures( Contract.Result<ICommand>() != null );
                return default( ICommand );
            }
        }

        ICommand ISelectableItem<T>.Unselect
        {
            get
            {
                Contract.Ensures( Contract.Result<ICommand>() != null );
                return default( ICommand );
            }
        }

        event EventHandler ISelectableItem<T>.Selected
        {
            add
            {
            }
            remove
            {
            }
        }

        event EventHandler ISelectableItem<T>.Unselected
        {
            add
            {
            }
            remove
            {
            }
        }

        event EventHandler ISelectableItem<T>.Indeterminate
        {
            add
            {
            }
            remove
            {
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
            }
            remove
            {
            }
        }
    }
}
