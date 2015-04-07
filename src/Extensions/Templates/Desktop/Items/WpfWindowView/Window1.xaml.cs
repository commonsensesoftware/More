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
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents a <see cref="Window">window</see>-based view for a view model.
    /// </summary>
    public partial class $safeitemrootname$ : Window$if$ ($hasviewmodel$ == true), INotifyPropertyChanged$endif$$if$ ($interfaceoption$ == 1), IView<$viewmodel$>$endif$$if$ ($interfaceoption$ == 2), IView<$viewmodel$,$viewmodel$>$endif$$if$ ($interfaceoption$ == 3), IDialogView<$viewmodel$>$endif$$if$ ($isshell$ == true), IShellView$endif$
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>
        public $safeitemrootname$()
        {
            this.InitializeComponent();
        }$if$ ($hasviewmodel$ == true)

        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>
        /// <param name="model">The <see cref="$viewmodel$">view model</see> associated with the view.</param>
        public $safeitemrootname$( $viewmodel$ model )
            : this()
        {
            this.DataContext = model;
        }

        /// <summary>
        /// Gets or sets the view model associated with the view.
        /// </summary>
        /// <value>The <see cref="$viewmodel$">view model</see> associated with the view.</value
        public $viewmodel$ Model
        {
            get
            {
                return this.DataContext as $viewmodel$;
            }
            set
            {
                this.AttachModel( value );
            }
        }

         /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the supplied property name.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event with the supplied arguments.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            var handler = this.PropertyChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Attaches the specified view model to the view.
        /// </summary>
        /// <param name="model">The view model to attach.</param>
        protected void AttachModel( $viewmodel$ model )
        {
            if ( object.Equals( this.DataContext, model ) )
                return;

            this.DataContext = model;
            this.OnPropertyChanged( "Model" );
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have changed by using either
        /// <c>null</c>or <see cref="F:String.Empty"/> as the property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;$endif$$if$ ($interfaceoption$ >= 2)

        void IView<$viewmodel$, $viewmodel$>.AttachModel( $viewmodel$ model )
        {
            this.AttachModel( model );
        }$endif$$if$ ($interfaceoption$ == 3)

        /// <summary>
        /// Closes the dialog view.
        /// </summary>
        /// <param name="dialogResult">The <see cref="Nullable{T}">dialog result</see> associated with the dialog.</param>
        public void Close( bool? dialogResult )
        {
            // close is automatically triggered if the dialog result changes
            if ( Nullable.Equals( this.DialogResult, dialogResult ) )
                this.Close();
            else
                this.DialogResult = dialogResult;
        }

        /// <summary>
        /// Shows the view as a modal dialog.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the <see cref="Nullable{T}">dialog result</see> that signifies
        /// how a view was closed by the user.</returns>
        public Task<bool?> ShowDialogAsync()
        {
            return Task.FromResult( this.ShowDialog() );
        }$endif$
    }
}