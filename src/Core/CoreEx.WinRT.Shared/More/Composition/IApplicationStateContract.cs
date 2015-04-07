namespace More.Composition
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel;
    using global::Windows.ApplicationModel.Activation;

    [ContractClassFor( typeof( IApplicationState ) )]
    internal abstract class IApplicationStateContract : IApplicationState
    {
        event EventHandler<ProgressChangedEventArgs> IApplicationState.ProgressChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        ILaunchActivatedEventArgs IApplicationState.Activation
        {
            get
            {
                Contract.Ensures( Contract.Result<ILaunchActivatedEventArgs>() != null );
                return null;
            }
        }

        event EventHandler<object> IApplicationState.Resuming
        {
            add
            {
            }
            remove
            {
            }
        }

        event EventHandler<SuspendingEventArgs> IApplicationState.Suspending
        {
            add
            {
            }
            remove
            {
            }
        }

        event EventHandler ISupportInitializeNotification.Initialized
        {
            add
            {
            }
            remove
            {
            }
        }

        bool ISupportInitializeNotification.IsInitialized
        {
            get
            {
                return default( bool );
            }
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
        }
    }
}
