namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents a behavior which mediates window closure interactions.
    /// </summary>
    public class WindowCloseBehavior : System.Windows.Interactivity.Behavior<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the close command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register( nameof( CloseCommand ), typeof( ICommand ), typeof( WindowCloseBehavior ), new PropertyMetadata( OnCloseCommandChanged ) );

        /// <summary>
        /// Gets or sets the close request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty CloseRequestProperty =
            DependencyProperty.Register( nameof( CloseRequest ), typeof( IInteractionRequest ), typeof( WindowCloseBehavior ), new PropertyMetadata( OnCloseRequestChanged ) );

        private Window window;
        private volatile bool closing;

        /// <summary>
        /// Gets or sets the command the behavior invokes when the attached window is closed.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> the behavior invokes when the attached window is closed.</value>
        public ICommand CloseCommand
        {
            get => (ICommand) GetValue( CloseCommandProperty );
            set => SetValue( CloseCommandProperty, value );
        }

        /// <summary>
        /// Gets or sets the request that produces events that the behavior listens to in order to trigger when the attached window is closed.
        /// </summary>
        /// <value>The <see cref="IInteractionRequest">interaction request</see> that can trigger window closure.</value>
        public IInteractionRequest CloseRequest
        {
            get => (IInteractionRequest) GetValue( CloseRequestProperty );
            set => SetValue( CloseRequestProperty, value );
        }

        static void OnCloseCommandChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var behavior = (WindowCloseBehavior) sender;
            var oldCommand = (ICommand) e.OldValue;
            var newCommand = (ICommand) e.NewValue;

            if ( oldCommand != null )
            {
                oldCommand.CanExecuteChanged -= behavior.OnCanExecuteChanged;
            }

            if ( newCommand == null )
            {
                if ( oldCommand != null && behavior.window != null )
                {
                    behavior.window.SetCloseButtonEnabled( true );
                }
            }
            else
            {
                newCommand.CanExecuteChanged += behavior.OnCanExecuteChanged;
            }
        }

        static void OnCloseRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var behavior = (WindowCloseBehavior) sender;
            var oldRequest = (IInteractionRequest) e.OldValue;
            var newRequest = (IInteractionRequest) e.NewValue;

            if ( oldRequest != null )
            {
                oldRequest.Requested -= behavior.OnCloseRequested;
            }

            if ( newRequest != null )
            {
                newRequest.Requested += behavior.OnCloseRequested;
            }
        }

        void OnCanExecuteChanged( object sender, EventArgs e )
        {
            if ( window == null || !window.IsInitialized )
            {
                return;
            }

            var command = (ICommand) sender;

            if ( command != null )
            {
                window.SetCloseButtonEnabled( command.CanExecute() );
            }
        }

        void OnCloseRequested( object sender, InteractionRequestedEventArgs e )
        {
            if ( window == null )
            {
                return;
            }

            var notification = e.Interaction as WindowCloseInteraction;
            var dialogResult = new bool?( !notification.Canceled );
            var modal = System.Windows.Interop.ComponentDispatcher.IsThreadModal;

            if ( modal && !Nullable.Equals( window.DialogResult, dialogResult ) )
            {
                window.DialogResult = dialogResult;
            }

            if ( !closing )
            {
                closing = true;
                window.Close();
                closing = false;
            }
        }

        void OnAssociatedObjectLoaded( object sender, RoutedEventArgs e )
        {
            if ( window != null )
            {
                window.Closing -= OnWindowClosing;
                window.SourceInitialized -= OnWindowSourceInitialized;
            }

            if ( ( window = Window.GetWindow( AssociatedObject ) ) == null )
            {
                return;
            }

            window.Closing += OnWindowClosing;
            window.SourceInitialized += OnWindowSourceInitialized;
            OnCanExecuteChanged( CloseCommand, EventArgs.Empty );
        }

        void OnWindowSourceInitialized( object sender, EventArgs e ) => OnCanExecuteChanged( CloseCommand, EventArgs.Empty );

        void OnWindowClosing( object sender, CancelEventArgs e )
        {
            if ( closing || window.DialogResult != null )
            {
                return;
            }

            var command = CloseCommand;

            if ( command == null )
            {
                return;
            }

            closing = true;
            e.Cancel = !command.CanExecute();

            if ( !e.Cancel )
            {
                // we need a way to allow a view model to provide feedback. for example,
                // it may want to confirm whether the window really wants to close. since
                // this cannot be expressed with ICommand, it seems reasonable to forward
                // the event args and allow the view model to set the value of the Cancel
                // property.  CancelEventArgs is not directly part of any UI APIs.
                command.Execute( e );
            }

            closing = false;
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;

            if ( window != null )
            {
                window.Closing -= OnWindowClosing;
                window.SourceInitialized -= OnWindowSourceInitialized;
                window = null;
            }

            base.OnDetaching();
        }
    }
}