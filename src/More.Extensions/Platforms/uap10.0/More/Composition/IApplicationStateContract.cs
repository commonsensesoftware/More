namespace More.Composition
{
    using global::Windows.ApplicationModel;
    using global::Windows.ApplicationModel.Activation;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IApplicationState ) )]
    internal abstract class IApplicationStateContract : IApplicationState
    {
        event EventHandler<ProgressChangedEventArgs> IApplicationState.ProgressChanged
        {
            add { }
            remove { }
        }

        IActivatedEventArgs IApplicationState.Activation
        {
            get
            {
                Contract.Ensures( Contract.Result<IActivatedEventArgs>() != null );
                return null;
            }
        }

        event EventHandler<object> IApplicationState.Resuming
        {
            add { }
            remove { }
        }

        event EventHandler<SuspendingEventArgs> IApplicationState.Suspending
        {
            add { }
            remove { }
        }

        event EventHandler ISupportInitializeNotification.Initialized
        {
            add { }
            remove { }
        }

        bool ISupportInitializeNotification.IsInitialized => default( bool );

        void ISupportInitialize.BeginInit() { }

        void ISupportInitialize.EndInit() { }
    }
}