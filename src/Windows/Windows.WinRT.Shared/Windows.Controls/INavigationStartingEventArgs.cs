namespace More.Windows.Controls
{
    using System;

    /// <summary>
    /// Defines the behavior of navigating event arguments.
    /// </summary>
    public interface INavigationStartingEventArgs
    {
        /// <summary>
        /// Gets the source event arguments the interface abstracts.
        /// </summary>
        /// <value>The native source event arguments.</value>
        object SourceEventArgs
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation should be canceled.
        /// </summary>
        /// <value>True if the navigation should be canceled; otherwise, false.</value>
        bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current navigation Uniform Resource Indicator.
        /// </summary>
        /// <value>The current navigation <see cref="Uri">URI</see>.</value>
        Uri Uri
        {
            get;
        }
    }
}
