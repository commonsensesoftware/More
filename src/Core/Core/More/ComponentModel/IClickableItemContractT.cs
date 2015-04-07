namespace More.ComponentModel
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;
    using global::System.Windows.Input;

    /// <summary>
    /// Provides the code contract definition for the <see cref="IClickableItem{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item.</typeparam>
    [ContractClassFor( typeof( IClickableItem<> ) )]
    internal abstract class IClickableItemContract<T> : IClickableItem<T>
    {
        T IClickableItem<T>.Value
        {
            get
            {
                return default( T );
            }
        }

        ICommand IClickableItem<T>.Click
        {
            get
            {
                Contract.Ensures( Contract.Result<ICommand>() != null );
                return default( ICommand );
            }
        }

        event EventHandler IClickableItem<T>.Clicked
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
