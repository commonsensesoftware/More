namespace More.Composition
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    [CLSCompliant( false )]
    public abstract partial class ShellViewBase : UserControl
    {
        /// <summary>
        /// Shows the view as the root visual.
        /// </summary>
        public virtual void Show()
        {
            var window = Window.Current;
            window.Content = this;
            window.Activate();
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should used typed FlowDirection property." )]
        string IShellView.FlowDirection
        {
            get => FlowDirection.ToString();
            set
            {
                if ( string.IsNullOrEmpty( value ) )
                {
                    FlowDirection = new FlowDirection();
                }
                else
                {
                    FlowDirection = (FlowDirection) Enum.Parse( typeof( FlowDirection ), value, false );
                }
            }
        }
    }
}