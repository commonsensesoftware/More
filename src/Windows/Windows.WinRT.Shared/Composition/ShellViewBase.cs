namespace More.Composition
{
    using System;
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

        string IShellView.FlowDirection
        {
            get
            {
                return this.FlowDirection.ToString();
            }
            set
            {
                if ( string.IsNullOrEmpty( value ) )
                    this.FlowDirection = new FlowDirection();
                else
                    this.FlowDirection = (FlowDirection) Enum.Parse( typeof( FlowDirection ), value, false );
            }
        }
    }
}
