namespace More.Composition
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel;
    using global::Windows.ApplicationModel.Activation;

    /// <content>
    /// Provides additional application state capabilities specific to Windows Store applications.
    /// </content>
    [CLSCompliant( false )]
    [ContractClass( typeof( IApplicationStateContract ) )]
    public partial interface IApplicationState
    {
        /// <summary>
        /// Gets the application launch activation arguments.
        /// </summary>
        /// <value>The application <see cref="ILaunchActivatedEventArgs">launch activation arguments</see>.</value>
        ILaunchActivatedEventArgs Activation
        {
            get;
        }

        /// <summary>
        /// Occurs when the application transitions from the suspended state to running state.
        /// </summary>
        event EventHandler<object> Resuming;

        /// <summary>
        /// Occurs when the application transitions to the suspended state from some other state.
        /// </summary>
        event EventHandler<SuspendingEventArgs> Suspending;
    }
}
