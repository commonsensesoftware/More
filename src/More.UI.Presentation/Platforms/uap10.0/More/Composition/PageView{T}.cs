﻿namespace More.Composition
{
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    /// <summary>
    /// Represents a page view.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of view model for the view.</typeparam>
    [CLSCompliant( false )]
    public partial class PageView<T> : Page, INotifyPropertyChanged, IView<T, T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageView{T}"/> class.
        /// </summary>
        /// <remarks>This constructor automatically calls <see cref="Application.LoadComponent(object, Uri)"/> for
        /// the new <see cref="UserControl"/>.</remarks>
        public PageView() : this( true ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageView{T}"/> class.
        /// </summary>
        /// <param name="loadComponent">Indicates whether <see cref="Application.LoadComponent(object, Uri)"/> is invoked for
        /// the new <see cref="UserControl"/>.</param>
        public PageView( bool loadComponent )
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
        /// Attaches the specified view model to the view.
        /// </summary>
        /// <param name="model">The model to attach.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected void AttachModel( T model )
        {
            Arg.NotNull( model, nameof( model ) );

            if ( object.Equals( DataContext, model ) )
            {
                return;
            }

            DataContext = model;
            OnPropertyChanged( nameof( Model ) );
        }

        void IView<T, T>.AttachModel( T model )
        {
            Arg.NotNull( model, nameof( model ) );
            AttachModel( model );
        }

        /// <summary>
        /// Gets or sets the view model for the view.
        /// </summary>
        /// <value>An object of type <typeparamref name="T"/>.</value>
        public virtual T Model
        {
            get => (T) DataContext;
            set => AttachModel( (T) value );
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have changed by using either
        /// <c>null</c>or <see cref="string.Empty"/> as the property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}