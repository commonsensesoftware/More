namespace More.Windows.Controls
{
    using System;

    /// <summary>
    /// Defines the behavior of navigation event arguments.
    /// </summary>
    public interface INavigationEventArgs
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
        /// Gets a value indicating whether the navigation succeeded.
        /// </summary>
        /// <value>True if the navigation succeeded; otherwise, false.</value>
        bool IsSuccess
        {
            get;
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
