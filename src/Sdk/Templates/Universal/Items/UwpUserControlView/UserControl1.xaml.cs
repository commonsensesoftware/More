namespace $rootnamespace$
{$if$($hasviewmodel$ == true)
    using $viewmodelnamespace$;$endif$
    using More;$if$ ($interfaceoption$ >= 1)
    using More.Composition;$endif$
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <summary>
    /// Represents a <see cref="UserControl">user control</see>-based view for a view model.
    /// </summary>
    public partial class $safeitemrootname$ : UserControl$if$ ($hasviewmodel$ == true), INotifyPropertyChanged$endif$$if$ ($interfaceoption$ == 1), IView<$viewmodel$>$endif$$if$ ($interfaceoption$ == 2), IView<$viewmodel$,$viewmodel$>$endif$
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>
        public $safeitemrootname$() => InitializeComponent();$if$ ($hasviewmodel$ == true)

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>
        /// <param name="model">The <see cref="$viewmodel$">view model</see> associated with the view.</param>
        public $safeitemrootname$( $viewmodel$ model ) : this() => DataContext = model;

        /// <summary>
        /// Gets or sets the view model associated with the view.
        /// </summary>
        /// <value>The <see cref="$viewmodel$">view model</see> associated with the view.</value>
        public $viewmodel$ Model
        {
            get => DataContext as $viewmodel$;
            set => AttachModel( value );
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
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e ) => PropertyChanged?.Invoke( this, e );

        /// <summary>
        /// Attaches the specified view model to the view.
        /// </summary>
        /// <param name="model">The view model to attach.</param>
        protected void AttachModel( $viewmodel$ model )
        {
            if ( object.Equals( DataContext, model ) )
            {
                return;
            }

            DataContext = model;
            OnPropertyChanged( nameof( Model ) );
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have changed by using either
        /// <c>null</c>or <see cref="F:String.Empty"/> as the property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;$endif$$if$ ($interfaceoption$ >= 2)

        void IView<$viewmodel$, $viewmodel$>.AttachModel( $viewmodel$ model ) => AttachModel( model );$endif$
    }
}