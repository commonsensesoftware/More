namespace More.Windows.Controls
{
    using System;
    using global::Windows.Foundation;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public abstract partial class InputDialog<T>
    {
        private static Window Window
        {
            get
            {
                return Window.Current;
            }
        }

        private void ArrangePopupContent( Size size )
        {
            // align center, full width
            Width = size.Width;

            // force the popup to consume the entire screen by resizing
            // the grid to the height of the window
            ( (Grid) popup.Child ).Height = size.Height;
        }
    }
}
