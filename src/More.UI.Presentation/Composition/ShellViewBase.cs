
namespace More.Composition
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows;
#if UAP10_0
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// Represents a shell view in a composite application.
    /// </summary>
    public abstract partial class ShellViewBase : IShellView, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewBase"/> class.
        /// </summary>
        /// <remarks>This constructor automatically calls <see cref="M:System.Windows.Application.LoadComponent"/> for the new view.</remarks>
        protected ShellViewBase() : this( loadComponent: true ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewBase"/> class.
        /// </summary>
        /// <param name="loadComponent">Indicates whether <see cref="M:System.Windows.Application.LoadComponent"/> is invoked for the new view.</param>
        protected ShellViewBase( bool loadComponent )
        {
            if ( loadComponent )
            {
                Application.LoadComponent( this, GetType().CreateXamlResourceUri() );
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the supplied property name.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName ) => OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event with the supplied arguments.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            PropertyChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have changed by using either
        /// <c>null</c>or <see cref="F:String.Empty"/> as the property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}