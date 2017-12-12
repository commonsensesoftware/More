namespace More.Composition
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Markup;
    using static System.String;

    /// <content>
    /// Provides the Windows Presentation Foundation implementation of a shell view.
    /// </content>
    public abstract partial class ShellViewBase : Window
    {
        /// <summary>
        /// Shows the view as the main window.
        /// </summary>
        public virtual new void Show() => Application.Current.MainWindow = this;

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed Language property." )]
        string IShellView.Language
        {
            get => Language?.IetfLanguageTag;
            set => Language = IsNullOrEmpty( value ) ? null : XmlLanguage.GetLanguage( value );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed FlowDirection property." )]
        string IShellView.FlowDirection
        {
            get => FlowDirection.ToString();
            set => FlowDirection = IsNullOrEmpty( value ) ? default( FlowDirection ) : (FlowDirection) Enum.Parse( typeof( FlowDirection ), value, false );
        }
    }
}