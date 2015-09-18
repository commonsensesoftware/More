namespace More.Windows.Controls
{
    using System;   
    using global::Windows.Foundation;
    using global::Windows.UI.Core;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public abstract partial class InputDialog<T>
    {
        private static CoreWindow Window
        {
            get
            {
                return CoreWindow.GetForCurrentThread();
            }
        }

        private void ArrangePopupContent( Size size )
        {
            // align top, full width
            Width = size.Width;
            popup.VerticalOffset = 0;
        }
    }
}
