namespace More.Composition.Hosting
{
    using More.Windows;
    using System;
    using System.Windows;
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
            base.OnActivated( args );

            var e = args as IContinuationActivatedEventArgs;
            IContinuationManager continuationManager;

            // continue the operation as appropriate
            if ( e != null && this.Host.TryGetService( out continuationManager ) )
                continuationManager.Continue( e );
        }
    }
}
