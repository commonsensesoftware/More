namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Security;
    using System.Security.Principal;
    using System.Windows.Input;

    [ContractClassFor( typeof( IActivity ) )]
    internal abstract class IActivityContract : IActivity
    {
        Guid IActivity.Id
        {
            get
            {
                return default( Guid );
            }
        }

        Guid? IActivity.InstanceId
        {
            get;
            set;
        }

        bool IActivity.IsCompleted
        {
            get
            {
                return default( bool );
            }
        }

        DateTime? IActivity.Expiration
        {
            get
            {
                return default( DateTime? );
            }
            set
            {
            }
        }

        ICollection<IActivity> IActivity.Dependencies
        {
            get
            {
                Contract.Ensures( Contract.Result<ICollection<IActivity>>() != null );
                return default( ICollection<IActivity> );
            }
        }

        void IActivity.LoadState( IDictionary<string, string> stateBag )
        {
            Contract.Requires<ArgumentNullException>( stateBag != null, "stateBag" );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        void IActivity.SaveState( IDictionary<string, string> stateBag )
        {
            Contract.Requires<ArgumentNullException>( stateBag != null, "stateBag" );
            Contract.Requires<ArgumentException>( !stateBag.IsReadOnly, "stateBag" );
        }

        bool IActivity.CanExecute( IServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null, "serviceProvider" );
            return default( bool );
        }

        void IActivity.Execute( IServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null, "serviceProvider" );
        }

        event EventHandler<ActivityCompletedEventArgs> IActivity.Completed
        {
            add
            {
            }
            remove
            {
            }
        }

        bool IEquatable<IActivity>.Equals( IActivity other )
        {
            return default( bool );
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

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        string INamedComponent.Name
        {
            get;
            set;
        }

        string INamedComponent.Description
        {
            get;
            set;
        }
    }
}
