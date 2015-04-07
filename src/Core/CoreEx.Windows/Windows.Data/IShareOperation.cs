namespace More.Windows.Data
{
    using System;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial interface IShareOperation
    {
        /// <summary>
        /// Closes the share pane.
        /// </summary>
        void DismissUI();
    }
}
