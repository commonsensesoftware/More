namespace More.Composition.Hosting
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using Windows;
    using global::Windows.ApplicationModel.Activation;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public abstract partial class ComposedApplication
    {
        /// <summary>
        /// Overrides the default behavior when the application is activated.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The platform will never pass null." )]
        protected override void OnActivated( IActivatedEventArgs args )
        {
            Contract.Assume( args != null );

            Activation = args;
            Init();

            if ( args.PreviousExecutionState != ApplicationExecutionState.Running )
                return;

            IContinuationManager continuationManager;

            if ( Host.TryGetService( out continuationManager ) )
                continuationManager.Continue( args );
        }
    }
}
