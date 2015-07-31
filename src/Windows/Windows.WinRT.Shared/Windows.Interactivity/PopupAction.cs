namespace More.Windows.Interactivity
{
    using Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using global::Windows.Foundation;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Controls.Primitives;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to show the <see cref="Interaction">interaction</see>
    /// from an <see cref="E:IInteractionRequest.Requested">interaction request</see> in a <see cref="Window">view</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class PopupAction : System.Windows.Interactivity.TriggerAction
    {
        private sealed class ChildWindow
        {
            private bool executed;
            private Window window;
            private Popup popup;
            private Interaction interaction;

            internal ChildWindow( Window window, Popup popup, Interaction interaction )
            {
                Contract.Requires( popup != null );

                // wire up popup
                this.popup = popup;
                this.popup.Closed += OnPopupClosed;

                // wire up commands
                if ( ( this.interaction = interaction ) != null )
                    this.interaction.Commands.ForEach( c => c.Executed += OnExecuted );

                // force layout and update layout when window resizes
                if ( ( this.window = window ) != null )
                {
                    ArrangePopupContent( new Size( window.Bounds.Width, window.Bounds.Height ) );
                    window.SizeChanged += OnWindowSizeChanged;
                }
            }

            internal bool IsOpen
            {
                get
                {
                    return popup != null && popup.IsOpen;
                }
            }

            private void OnWindowSizeChanged( object sender, WindowSizeChangedEventArgs e ) => ArrangePopupContent( e.Size );

            private void ArrangePopupContent( Size size )
            {
                var content = (FrameworkElement) popup.Child;
                content.Width = size.Width;
                popup.VerticalOffset = Math.Max( ( size.Height - content.ActualHeight ), 0d ) / 2d;
            }

            private void OnPopupClosed( object sender, object e )
            {
                if ( executed || interaction == null )
                {
                    Close();
                    return;
                }

                var cancel = interaction.CancelCommand;

                Close();

                if ( cancel != null )
                    cancel.Execute();
            }

            private void OnExecuted( object sender, EventArgs e )
            {
                if ( executed )
                    return;

                executed = true;
                Close();
            }

            internal void Show()
            {
                if ( popup != null )
                    popup.IsOpen = true;
            }

            internal void Close()
            {
                if ( interaction != null )
                {
                    interaction.Commands.ForEach( c => c.Executed -= OnExecuted );
                    interaction = null;
                }

                if ( popup != null )
                {
                    popup.Closed -= OnPopupClosed;
                    popup.IsOpen = false;
                    popup = null;
                }

                if ( window != null )
                {
                    window.SizeChanged -= OnWindowSizeChanged;
                    window = null;
                }
            }
        }

        /// <summary>
        /// Gets the dependency property of the view name.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ViewNameProperty =
            DependencyProperty.Register( nameof( ViewName ), typeof( string ), typeof( PopupAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property of the view <see cref="Type">type</see> name.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ViewTypeNameProperty =
            DependencyProperty.Register( nameof( ViewTypeName ), typeof( string ), typeof( PopupAction ), new PropertyMetadata( null, OnViewTypeNamePropertyChanged ) );

        /// <summary>
        /// Gets the dependency property for the popup view <see cref="T:Style">style</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty PopupStyleProperty =
            DependencyProperty.Register( nameof( PopupStyle ), typeof( Style ), typeof( PopupAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property for the view <see cref="DataTemplate">content template</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register( nameof( ContentTemplate ), typeof( DataTemplate ), typeof( PopupAction ), new PropertyMetadata( (object) null ) );

        private Lazy<Type> viewType = new Lazy<Type>( () => null );
        private ChildWindow childWindow;

        /// <summary>
        /// Gets or sets the name of the view the action shows.
        /// </summary>
        /// <value>The view name associated with the content that the action shows.</value>
        public string ViewName
        {
            get
            {
                return (string) GetValue( ViewNameProperty );
            }
            set
            {
                SetValue( ViewNameProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the type name of view the action shows.
        /// </summary>
        /// <value>The view <see cref="Type">type</see> name of the content that the action shows.</value>
        public string ViewTypeName
        {
            get
            {
                return (string) GetValue( ViewTypeNameProperty );
            }
            set
            {
                SetValue( ViewTypeNameProperty, value );
            }
        }

        /// <summary>
        /// Gets the type of content the action shows.
        /// </summary>
        /// <value>The view <see cref="Type">type</see> of content that the action shows.</value>
        public Type ViewType
        {
            get
            {
                return viewType.Value;
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the window shown by the action.
        /// </summary>
        /// <value>The <see cref="T:Style">style</see> applied to the <see cref="Window">window</see>
        /// shown by the action.</value>
        public Style PopupStyle
        {
            get
            {
                return (Style) GetValue( PopupStyleProperty );
            }
            set
            {
                SetValue( PopupStyleProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the content template applied to the window shown by the action.
        /// </summary>
        /// <value>The <see cref="DataTemplate">content template</see> applied to the
        /// <see cref="Window">window</see> shown by the action.</value>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate) GetValue( ContentTemplateProperty );
            }
            set
            {
                SetValue( ContentTemplateProperty, value );
            }
        }

        private static Window Window
        {
            get
            {
                return Window.Current;
            }
        }

        private static Type ResolveType( string newViewTypeName )
        {
            if ( string.IsNullOrEmpty( newViewTypeName ) )
                return null;

            ITypeResolutionService service;

            // use the registered type resolution service, if one is found; otherwise, failover to default type resolution
            if ( ServiceProvider.Current.TryGetService( out service ) )
                return service.GetType( newViewTypeName, true );

            return Type.GetType( newViewTypeName, true );
        }

        private static void OnViewTypeNamePropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (PopupAction) sender;
            var typeName = (string) e.NewValue;

            @this.viewType = new Lazy<Type>( () => ResolveType( typeName ) );
        }

        /// <summary>
        /// Creates the view used by the action.
        /// </summary>
        /// <returns>A new object or null if <see cref="P:ViewType"/> is null.</returns>
        /// <remarks>If the view specified by the <see cref="P:ViewType">window type</see> is a composed view,
        /// the default implementation will be to use the <see cref="P:ServiceLocator.Current">current service provider</see>
        /// to compose it.  If the view is not composed, then the fallback method will be <see cref="M:Activator.CreateInstance(T:Type)"/>.</remarks>
        protected virtual object CreateView()
        {
            object view = null;

            if ( ViewType == null )
            {
                if ( string.IsNullOrEmpty( ViewName ) )
                    return view;

                ServiceProvider.Current.TryGetService( typeof( object ), out view, ViewName );
                return view;
            }

            if ( !ServiceProvider.Current.TryGetService( ViewType, out view, ViewName ) )
                view = Activator.CreateInstance( ViewType );

            return view;
        }

        private Popup CreatePopup( Interaction interaction, object view )
        {
            Contract.Requires( interaction != null );
            Contract.Ensures( Contract.Result<Popup>() != null );

            // create content presenter
            var presenter = new ContentPresenter();

            if ( ContentTemplate != null )
                presenter.ContentTemplate = ContentTemplate;

            // use view or confirmation as content
            presenter.Content = view ?? interaction;
            presenter.DataContext = interaction;

            // create popup
            var popup = new Popup()
            {
                Child = presenter,
                IsLightDismissEnabled = true
            };

            if ( PopupStyle != null )
                popup.Style = PopupStyle;

            return popup;
        }

        private void ShowPopup( Interaction interaction, object view )
        {
            Contract.Requires( interaction != null );

            var window = Window;
            var popup = CreatePopup( interaction, view );

            childWindow = new ChildWindow( window, popup, interaction );
            childWindow.Show();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>True if the action performed a navigation operation; otherwise, false.</returns>
        public override object Execute( object sender, object parameter )
        {
            var interaction = GetRequestedInteraction<Interaction>( parameter );

            if ( interaction == null )
                return null;

            // cannot execute action if the child window is already being shown
            if ( childWindow != null && childWindow.IsOpen )
                return null;

            var view = CreateView();
            ShowPopup( interaction, view );

            return null;
        }
    }
}
