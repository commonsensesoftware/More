namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    [ContractClassFor( typeof( IActivity ) )]
    abstract class IActivityContract : IActivity
    {
        Guid IActivity.Id => default( Guid );

        Guid? IActivity.InstanceId { get; set; }

        bool IActivity.IsCompleted => default( bool );

        DateTime? IActivity.Expiration
        {
            get => default( DateTime? );
            set { }
        }

        ICollection<IActivity> IActivity.Dependencies
        {
            get
            {
                Contract.Ensures( Contract.Result<ICollection<IActivity>>() != null );
                return default( ICollection<IActivity> );
            }
        }

        void IActivity.LoadState( IDictionary<string, string> stateBag ) =>
            Contract.Requires<ArgumentNullException>( stateBag != null, nameof( stateBag ) );

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        void IActivity.SaveState( IDictionary<string, string> stateBag )
        {
            Contract.Requires<ArgumentNullException>( stateBag != null, nameof( stateBag ) );
            Contract.Requires<ArgumentException>( !stateBag.IsReadOnly, nameof( stateBag ) );
        }

        bool IActivity.CanExecute( IServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null, nameof( serviceProvider ) );
            return default( bool );
        }

        void IActivity.Execute( IServiceProvider serviceProvider ) =>
            Contract.Requires<ArgumentNullException>( serviceProvider != null, nameof( serviceProvider ) );

        event EventHandler<ActivityCompletedEventArgs> IActivity.Completed
        {
            add { }
            remove { }
        }

        bool IEquatable<IActivity>.Equals( IActivity other ) => default( bool );

        bool ICommand.CanExecute( object parameter ) => default( bool );

        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        void ICommand.Execute( object parameter ) { }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }

        string INamedComponent.Name { get; set; }

        string INamedComponent.Description { get; set; }
    }
}