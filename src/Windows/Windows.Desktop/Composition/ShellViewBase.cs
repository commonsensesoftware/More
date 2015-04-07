namespace More.Composition
{
    using System;
    using System.Windows;
    using System.Windows.Markup;

    /// <content>
    /// Provides the Windows Presentation Foundation implementation of a shell view.
    /// </content>
    public abstract partial class ShellViewBase : Window
    {
        /// <summary>
        /// Shows the view as the main window.
        /// </summary>
        public virtual new void Show()
        {
            Application.Current.MainWindow = this;
        }

        string IShellView.Language
        {
            get
            {
                if ( this.Language == null )
                    return null;

                return this.Language.IetfLanguageTag;
            }
            set
            {
                if ( string.IsNullOrEmpty( value ) )
                    this.Language = null;
                else
                    this.Language = XmlLanguage.GetLanguage( value );
            }
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
