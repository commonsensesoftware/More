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
        static Window Window => Window.Current;
    }
}