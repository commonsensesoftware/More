namespace More.Windows.Input
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    [ContractClassFor( typeof( INamedCommand ) )]
    abstract class INamedCommandContract : INamedCommand
    {
        string INamedCommand.Id
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return null;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( value ), nameof( value ) );
            }
        }

        void INotifyCommandChanged.RaiseCanExecuteChanged() { }

        event EventHandler<EventArgs> INotifyCommandChanged.Executed
        {
            add { }
            remove { }
        }

        bool ICommand.CanExecute( object parameter ) => default( bool );

        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        void ICommand.Execute( object parameter ) { }

        string INamedComponent.Name
        {
            get => null;
            set { }
        }

        string INamedComponent.Description
        {
            get => null;
            set { }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }
    }
}