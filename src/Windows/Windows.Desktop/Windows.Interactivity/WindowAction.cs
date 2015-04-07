namespace More.Windows.Interactivity
{
    using More.Composition;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to show the <see cref="Interaction">interaction</see>
    /// from an <see cref="E:IInteractionRequest.Requested">interaction request</see> in a <see cref="Window">view</see>.
    /// </summary>
    public partial class WindowAction : System.Windows.Interactivity.TriggerAction<FrameworkElement>
    {
        private readonly Dictionary<Type, Delegate> attachMethods = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Gets the dependency property of the view name.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ViewNameProperty =
            DependencyProperty.Register( "ViewName", typeof( string ), typeof( WindowAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property of the view <see cref="Type">type</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ViewTypeProperty =
            DependencyProperty.Register( "ViewType", typeof( Type ), typeof( WindowAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property indicating whether the view is modal.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register( "IsModal", typeof( bool ), typeof( WindowAction ), new PropertyMetadata( true ) );

        /// <summary>
        /// Gets the dependency property for the view <see cref="Style">style</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register( "Style", typeof( Style ), typeof( WindowAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the dependency property for the view <see cref="DataTemplate">content template</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register( "ContentTemplate", typeof( DataTemplate ), typeof( WindowAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets a value indicating whether the provided cancel command is bound to the window close button.
        /// </summary>
        /// <value>True if the provided <see cref="P:Interaction.CancelCommand">cancel command</see> is
        /// bound to the window close button. The default value is <c>false</c>.</value>
        /// <remarks>The associated <see cref="T:Window">window</see> is usually closed after any command is executed. If an action is
        /// conditional or should be cancelled, the command should not raise the <see cref="E:INotifyCommandChanged.Executed"/> event.
        /// In the scenario where a user clicks the standard window close button, no command is executed by default. To handle that
        /// scenario, set this property to <c>true</c> in order to automatically bind the
        /// <see cref="P:Interaction.CancelCommand">cancel command</see> to the
        /// <see cref="P:WindowCloseBehavior.CloseCommand"/> which will be applied to the underlying <see cref="T:Window">window</see>
        /// <see cref="T:System.Windows.Interactivity.BehaviorCollection">behaviors</see>.</remarks>
        public bool BindCancelToClose
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the view the action shows.
        /// </summary>
        /// <value>The view name associated with the <see cref="Window">window</see> that the action shows.</value>
        public string ViewName
        {
            get
            {
                return (string) this.GetValue( ViewNameProperty );
            }
            set
            {
                this.SetValue( ViewNameProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the type of window the action shows.
        /// </summary>
        /// <value>The view <see cref="Type">type</see> of the <see cref="Window">window</see> that the action shows.</value>
        public Type ViewType
        {
            get
            {
                return (Type) this.GetValue( ViewTypeProperty );
            }
            set
            {
                this.SetValue( ViewTypeProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the action shows a modal window.
        /// </summary>
        /// <value>True if the action shows a modal <see cref="Window">window</see>; otherwise, false to show
        /// a modeless <see cref="Window">window</see>. The default value is true.</value>
        public bool IsModal
        {
            get
            {
                return (bool) this.GetValue( IsModalProperty );
            }
            set
            {
                this.SetValue( IsModalProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the window shown by the action.
        /// </summary>
        /// <value>The <see cref="Style">style</see> applied to the <see cref="Window">window</see>
        /// shown by the action.</value>
        public Style Style
        {
            get
            {
                return (Style) this.GetValue( StyleProperty );
            }
            set
            {
                this.SetValue( StyleProperty, value );
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
                return (DataTemplate) this.GetValue( ContentTemplateProperty );
            }
            set
            {
                this.SetValue( ContentTemplateProperty, value );
            }
        }

        /// <summary>
        /// Creates the window used by the action.
        /// </summary>
        /// <returns>A new <see cref="Window"/> object or null if <see cref="P:ViewType"/> is null.</returns>
        /// <remarks>If the <see cref="Window">window</see> specified by the <see cref="P:ViewType">window type</see>
        /// is a composed <see cref="Window">window</see>, the default implementation will first use the
        /// <see cref="P:ServiceProvider.Currrent">service provider</see> to retrieve it.  If the view is not found,
        /// then the fallback method will be <see cref="M:Activator.CreateInstance(Type)"/>.</remarks>
        protected virtual Window CreateWindow()
        {
            Contract.Ensures( Contract.Result<Window>() != null );

            Window window = null;

            if ( this.ViewType == null )
            {
                window = new Window();
            }
            else
            {
                var services = ServiceProvider.Current;
                object view = null;

                // if the view is resolved from the service provider, it is assumed to be composed
                if ( !services.TryGetService( this.ViewType, out view, this.ViewName ) )
                {
                    // use fallback method
                    view = Activator.CreateInstance( this.ViewType );

                    ICompositionService composer;

                    // try to compose the view
                    if ( services.TryGetService( out composer ) )
                        composer.Compose( view );
                }

                if ( ( window = view as Window ) == null )
                {
                    window = new Window();
                    window.Content = view;
                }
            }

            window.Owner = Window.GetWindow( this.AssociatedObject );
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            return window;
        }

        private static Delegate CreateAttachDelegate( Window window, Interaction interaction )
        {
            Contract.Requires( window != null );
            Contract.Requires( interaction != null );

            // find matching interface
            var viewType = window.GetType();
            var viewModelType = interaction.GetType();
            var interfaces = viewType.GetInterfaces();
            var view = typeof( IView<,> );
            var query = from i in interfaces
                        where i.IsGenericType && i.GetGenericTypeDefinition().Equals( view )
                        let vm = i.GetGenericArguments()[1]
                        where vm.IsAssignableFrom( viewModelType )
                        select i;
            var @interface = query.FirstOrDefault();

            // IView<TOutput,TInput> is not implemented
            if ( @interface == null )
                return null;

            // get the interface mapping and create a delegate for the Attach method
            var mapping = viewType.GetInterfaceMap( @interface );
            var delegateType = typeof( Action<,> ).MakeGenericType( viewType, viewModelType );
            var attach = mapping.TargetMethods[0].CreateDelegate( delegateType );

            return attach;
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="args">The <see cref="InteractionRequestedEventArgs"/> event data provided by the corresponding trigger.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual void Invoke( InteractionRequestedEventArgs args )
        {
            Contract.Requires<ArgumentNullException>( args != null, "args" );

            var window = this.CreateWindow();

            if ( window == null )
                return;

            // copy content template from action
            if ( this.ContentTemplate != null )
                window.ContentTemplate = this.ContentTemplate;

            // copy style from action
            if ( this.Style != null )
                window.Style = this.Style;

            // attempt to get or create a matching delegate for the IView<TOutput,TIntput>.Attach method
            var attach = this.attachMethods.GetOrAdd( window.GetType(), () => CreateAttachDelegate( window, args.Interaction ) );

            // if a delegate cannot be created, use the default behavior
            if ( attach == null )
            {
                // use interaction if there is no content
                if ( window.Content == null )
                    window.Content = args.Interaction;

                // window might already have its own view model, if it doesn't
                // use the interaction as the model
                if ( window.DataContext == null )
                    window.DataContext = args.Interaction;
            }
            else
            {
                // attach the view model
                attach.DynamicInvoke( window, args.Interaction );
            }

            using ( new CommandMediator( window, args.Interaction, this.BindCancelToClose ) )
            {
                if ( this.IsModal )
                    window.ShowDialog();
                else
                    window.Show();
            }
        }

        /// <summary>
        /// Invokes the triggered action.
        /// </summary>
        /// <param name="parameter">The parameter supplied from the corresponding trigger.</param>
        /// <remarks>This method is not meant to be called directly by your code.</remarks>
        protected sealed override void Invoke( object parameter )
        {
            var args = parameter as InteractionRequestedEventArgs;

            if ( args != null )
                this.Invoke( args );
        }

        /// <summary>
        /// Occurs when the action is about to be detached.
        /// </summary>
        protected override void OnDetaching()
        {
            this.attachMethods.Clear();
            base.OnDetaching();
        }
    }
}
