namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the behavior of navigating event arguments.
    /// </summary>
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Represents the contact used for WinRT event arguments." )]
    public interface INavigationStartingEventArgs
    {
        /// <summary>
        /// Gets the source event arguments the interface abstracts.
        /// </summary>
        /// <value>The native source event arguments.</value>
        object SourceEventArgs { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation should be canceled.
        /// </summary>
        /// <value>True if the navigation should be canceled; otherwise, false.</value>
        bool Cancel { get; set; }

        /// <summary>
        /// Gets the current navigation Uniform Resource Indicator.
        /// </summary>
        /// <value>The current navigation <see cref="Uri">URI</see>.</value>
        Uri Uri { get; }
    }
}