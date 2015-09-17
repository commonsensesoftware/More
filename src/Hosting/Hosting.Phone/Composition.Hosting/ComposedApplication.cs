namespace More.Composition.Hosting
{
    using System;
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
        protected override void OnActivated( IActivatedEventArgs args )
        {
            Activation = args;
            Init();

            if ( args.PreviousExecutionState != ApplicationExecutionState.Running )
                return;

            IContinuationManager continuationManager;

            // since we were already in a running state, we can attempt a continuation now;
            // otherwise, we have to let the shell view trigger the continuation after
            // everything has been reinitialized
            if ( Host.TryGetService( out continuationManager ) )
                continuationManager.Continue( args );
        }
    }
}
