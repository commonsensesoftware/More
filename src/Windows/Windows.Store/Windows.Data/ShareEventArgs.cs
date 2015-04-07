namespace More.Windows.Data
{
    using System;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public partial class ShareEventArgs
    {
        /// <summary>
        /// Closes the share pane.
        /// </summary>
        public void DismissUI()
        {
            this.adapted.DismissUI();
        }
    }
}
