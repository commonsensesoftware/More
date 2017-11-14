namespace System.ComponentModel
{
    using System;

    /// <summary>
    /// Allows coordination of initialization for a component and its dependent properties.
    /// </summary>
    public interface ISupportInitializeNotification : ISupportInitialize
    {
        /// <summary>
        /// Occurs when the object has been initialized.
        /// </summary>
        event EventHandler Initialized;

        /// <summary>
        /// Gets or sets a value indicating whether the object is initialized.
        /// </summary>
        /// <value>True if the object is initialized; otherwise, false.</value>
        bool IsInitialized { get; }
    }
}