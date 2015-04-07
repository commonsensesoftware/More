namespace More.Composition
{
    using global::System;
    using global::System.ComponentModel;

    /// <summary>
    /// Defines the behavior for the state of an application.
    /// </summary>
    public partial interface IApplicationState : ISupportInitializeNotification
    {
        /// <summary>
        /// Occurs when the progress of the application initialization has changed.
        /// </summary>
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;
    }
}
