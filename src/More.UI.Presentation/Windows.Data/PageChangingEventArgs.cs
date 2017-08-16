namespace More.Windows.Data
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Provides event data when a data page changes.
    /// </summary>
    public class PageChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageChangingEventArgs" /> class.
        /// </summary>
        /// <param name="newPageIndex">The index of the requested page.</param>
        public PageChangingEventArgs( int newPageIndex ) => NewPageIndex = newPageIndex;

        /// <summary>
        /// Gets the index of the requested page.
        /// </summary>
        /// <value>The index of the requested page.</value>
        public int NewPageIndex { get; }
    }
}