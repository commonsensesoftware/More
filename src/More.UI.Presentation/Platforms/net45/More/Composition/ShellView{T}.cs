namespace More.Composition
{
    using System;
    using System.Windows;
    using System.Windows.Markup;
    using static System.String;

    /// <summary>
    /// Represents a shell view with an associated view model in a composite application.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of view model associated with the view.</typeparam>
    public class ShellView<T> : DialogView<T>, IShellView where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView{T}"/> class.
        /// </summary>
        /// <remarks>This constructor automatically calls <see cref="M:System.Windows.Application.LoadComponent"/> for the new view.</remarks>
        public ShellView() : base( loadComponent: true ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView{T}"/> class.
        /// </summary>
        /// <param name="loadComponent">Indicates whether <see cref="M:System.Windows.Application.LoadComponent"/> is invoked for view.</param>
        public ShellView( bool loadComponent ) : base( loadComponent ) { }

        /// <summary>
        /// Shows the view as the main window.
        /// </summary>
        public virtual new void Show() => Application.Current.MainWindow = this;

        string IShellView.Language
        {
            get => Language?.IetfLanguageTag;
            set => Language = IsNullOrEmpty( value ) ? null : XmlLanguage.GetLanguage( value );
        }

        string IShellView.FlowDirection
        {
            get => FlowDirection.ToString();
            set => FlowDirection = IsNullOrEmpty( value ) ? new FlowDirection() : (FlowDirection) Enum.Parse( typeof( FlowDirection ), value, false );
        }
    }
}
