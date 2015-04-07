namespace More.Windows.Input
{
    using More.ComponentModel;
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;
    using global::System.Windows.Input;

    [ContractClassFor( typeof( INamedCommand ) )]
    internal abstract class INamedCommandContract : INamedCommand
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
                Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( value ), "value" );
            }
        }

        void INotifyCommandChanged.RaiseCanExecuteChanged()
        {
        }

        event EventHandler<EventArgs> INotifyCommandChanged.Executed
        {
            add
            {
            }
            remove
            {
            }
        }

        bool ICommand.CanExecute( object parameter )
        {
            return default( bool );
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        void ICommand.Execute( object parameter )
        {
        }

        string INamedComponent.Name
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        string INamedComponent.Description
        {
            get
            {
                return null;
            }
            set
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
