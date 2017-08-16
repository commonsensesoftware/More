namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    [ContractClassFor( typeof( IClickableItem<> ) )]
    abstract class IClickableItemContract<T> : IClickableItem<T>
    {
        T IClickableItem<T>.Value => default( T );

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
            add { }
            remove { }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }
    }
}