namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Provides the code contract definition for the <see cref="IExpandableItem{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item.</typeparam>
    [ContractClassFor( typeof( IExpandableItem<> ) )]
    internal abstract class IExpandableItemContract<T> : IExpandableItem<T>
    {
        bool IExpandableItem<T>.IsExpanded
        {
            get
            {
                return default( bool );
            }
            set
            {
            }
        }

        T IExpandableItem<T>.Value
        {
            get
            {
                return default( T );
            }
        }

        ICommand IExpandableItem<T>.Expand
        {
            get
            {
                Contract.Ensures( Contract.Result<ICommand>() != null );
                return null;
            }
        }

        ICommand IExpandableItem<T>.Collapse
        {
            get
            {
                Contract.Ensures( Contract.Result<ICommand>() != null );
                return null;
            }
        }

        event EventHandler IExpandableItem<T>.Expanded
        {
            add
            {
            }
            remove
            {
            }
        }

        event EventHandler IExpandableItem<T>.Collapsed
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
