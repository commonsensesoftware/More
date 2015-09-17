namespace More.Composition.Hosting
{
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
        }
    }
}
