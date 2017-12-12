namespace More.Composition
{
    using global::Windows.ApplicationModel;
    using global::Windows.ApplicationModel.Activation;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <content>
    /// Provides additional application state capabilities specific to Windows Store applications.
    /// </content>
    [CLSCompliant( false )]
    [ContractClass( typeof( IApplicationStateContract ) )]
    public partial interface IApplicationState
    {
        /// <summary>
        /// Gets the application activation arguments.
        /// </summary>
        /// <value>The application <see cref="IActivatedEventArgs">activation arguments</see>.</value>
        IActivatedEventArgs Activation { get; }

        /// <summary>
        /// Occurs when the application transitions from the suspended state to running state.
        /// </summary>
        [SuppressMessage( "Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "WinRT events do not inherit from EventArgs." )]
        event EventHandler<object> Resuming;

        /// <summary>
        /// Occurs when the application transitions to the suspended state from some other state.
        /// </summary>
        [SuppressMessage( "Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "WinRT events do not inherit from EventArgs." )]
        event EventHandler<SuspendingEventArgs> Suspending;
    }
}