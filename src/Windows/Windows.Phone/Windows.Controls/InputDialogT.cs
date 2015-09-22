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
    }
}
