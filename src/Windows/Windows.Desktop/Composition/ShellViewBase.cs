namespace More.Composition
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed Language property." )]
        string IShellView.Language
        {
            get
            {
                if ( Language == null )
                    return null;

                return Language.IetfLanguageTag;
            }
            set
            {
                if ( string.IsNullOrEmpty( value ) )
                    Language = null;
                else
                    Language = XmlLanguage.GetLanguage( value );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed FlowDirection property." )]
        string IShellView.FlowDirection
        {
            get
            {
                return FlowDirection.ToString();
            }
            set
            {
                if ( string.IsNullOrEmpty( value ) )
                    FlowDirection = new FlowDirection();
                else
                    FlowDirection = (FlowDirection) Enum.Parse( typeof( FlowDirection ), value, false );
            }
        }
    }
}
