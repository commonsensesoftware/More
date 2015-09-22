namespace More.Windows.Controls
{
    using Input;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Windows.Input;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Windows.Foundation;
    using global::Windows.UI;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Controls.Primitives;
    using global::Windows.UI.Xaml.Media;
    using global::Windows.UI.Xaml.Shapes;
    using Key = global::Windows.System.VirtualKey;
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;

    /// <summary>
    /// Represents the base implementation for an input dialog.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of response.</typeparam>
    [CLSCompliant( false )]
    public abstract partial class InputDialog<T> : ContentControl where T : class
    {
        private sealed class CommandCollection : ObservableCollection<INamedCommand>
        {
            private readonly InputDialog<T> dialog;

            internal CommandCollection( InputDialog<T> dialog )
            {
                Contract.Requires( dialog != null );
                this.dialog = dialog;
            }

            private void OnCommandExecuted( object sender, EventArgs e )
            {
                var commandIndex = dialog.Commands.IndexOf( (INamedCommand) sender );
                dialog.OnCommandExecuted( commandIndex );
            }

            protected override void InsertItem( int index, INamedCommand item )
            {
                base.InsertItem( index, item );

                if ( item != null )
                    item.Executed += OnCommandExecuted;
            }

            protected override void RemoveItem( int index )
            {
                var item = this[index];
                base.RemoveItem( index );

                if ( item != null )
                    item.Executed -= OnCommandExecuted;
            }

            protected override void SetItem( int index, INamedCommand item )
            {
                var oldItem = this[index];
                base.SetItem( index, item );

                if ( oldItem != null )
                    oldItem.Executed -= OnCommandExecuted;

                if ( item != null )
                    item.Executed += OnCommandExecuted;
            }
        }

        private readonly CommandCollection commands;
        private Popup popup;
        private TaskCompletionSource<T> completionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDialog{T}"/> class.
        /// </summary>
        protected InputDialog()
        {
            commands = new CommandCollection( this );
        }

        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        /// <value>The dialog title.</value>
        public virtual string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the input dialog response.
        /// </summary>
        /// <value>The input dialog response.</value>
        public virtual T Response
        {
            get;
            protected internal set;
        }

        /// <summary>
        /// Gets or sets the input dialog default response.
        /// </summary>
        /// <value>The input dialog default response.</value>
        public virtual T DefaultResponse
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default command index.
        /// </summary>
        /// <value>The default command index, which is the zero-based index of the
        /// <see cref="P:Commands">command</see> that is invoked when the ENTER key
        /// is pressed. The default value is -1, which indicates there is no default
        /// command.</value>
        public virtual int DefaultCommandIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cancel command index.
        /// </summary>
        /// <value>The cancel command index, which is the zero-based index of the
        /// <see cref="P:Commands">command</see> that is invoked when the ESC key
        /// is pressed. The default value is -1, which indicates there is no cancel
        /// command.</value>
        public virtual int CancelCommandIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of commands associcated with the dialog.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}">observable collection</see> of <see cref="INamedCommand">commands</see>.</value>
        public virtual ObservableCollection<INamedCommand> Commands
        {
            get
            {
                Contract.Ensures( Contract.Result<ObservableCollection<INamedCommand>>() != null );
                return commands;
            }
        }

        private void OnWindowSizeChanged( object sender, WindowSizeChangedEventArgs e ) => ArrangePopupContent( e.Size );

        private void ArrangePopupContent( Size size )
        {
            // align center, full width
            Width = size.Width;

            // force the popup to consume the entire screen by resizing
            // the grid to the height of the window
            ( (Grid) popup.Child ).Height = size.Height;
        }

        private Popup CreatePopup()
        {
            Contract.Ensures( Contract.Result<Popup>() != null );

            var newPopup = new Popup();
            var grid = new Grid();
            var modalVeneer = new Rectangle() { Fill = new SolidColorBrush( Colors.Transparent ) };

            // this will prevent any content underneath the popup from receiving input,
            // which could cancel the dialog or have other unintended behavior
            grid.Children.Add( modalVeneer );
            grid.Children.Add( this );
            newPopup.Child = grid;

            newPopup.Loaded += ( s, e ) =>
            {
                OnOpened();

                var window = Window;

                if ( window == null )
                    return;

                ArrangePopupContent( new Size( window.Bounds.Width, window.Bounds.Height ) );
                window.SizeChanged += OnWindowSizeChanged;
            };

            return newPopup;
        }

        private void Close()
        {
            var window = Window;

            if ( window != null )
                window.SizeChanged -= OnWindowSizeChanged;

            if ( popup == null )
                return;

            popup.IsOpen = false;
            popup = null;
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <returns>An object that represents the <see cref="IAsyncOperation{T}">asynchronous operation</see>.</returns>
        public IAsyncOperation<T> ShowAsync()
        {
            Contract.Ensures( Contract.Result<IAsyncOperation<T>>() != null );

            Response = null;
            popup = CreatePopup();
            popup.IsOpen = true;

            return AsyncInfo.Run( ShowDialog );
        }

        /// <summary>
        /// Occurs when the dialog is opened.
        /// </summary>
        /// <remarks>Note to inheritors: The base class does not have an implementation.</remarks>
        protected virtual void OnOpened()
        {
        }

        private Task<T> ShowDialog( CancellationToken token )
        {
            Contract.Ensures( Contract.Result<Task<T>>() != null );

            completionSource?.TrySetResult( null );
            completionSource = new TaskCompletionSource<T>();
            token.Register(
                () =>
                {
                    Close();
                    completionSource.TrySetResult( null );
                } );
            return completionSource.Task;
        }

        /// <summary>
        /// Occurs when a command is executed.
        /// </summary>
        /// <param name="commandIndex">The zero-based index of the command that was executed.</param>
        protected virtual void OnCommandExecuted( int commandIndex )
        {
            Arg.GreaterThanOrEqualTo( commandIndex, -1, nameof( commandIndex ) );

            var accepted = commandIndex == DefaultCommandIndex;
            var response = accepted ? Response : null;

            Close();
            completionSource?.TrySetResult( response );
        }

        internal void ExecuteCommand( int commandIndex )
        {
            Contract.Requires( commandIndex >= -1 );

            Commands.ElementAtOrDefault( commandIndex )?.Execute();
            OnCommandExecuted( commandIndex );
        }

        /// <summary>
        /// Overrides the default behavior when a key is released.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This should never be null" )]
        protected override void OnKeyUp( KeyEventArgs e )
        {
            switch ( e.Key )
            {
                case Key.Enter:
                    {
                        e.Handled = true;
                        ExecuteCommand( DefaultCommandIndex );
                        break;
                    }
                case Key.Escape:
                    {
                        e.Handled = true;
                        ExecuteCommand( CancelCommandIndex );
                        break;
                    }
            }

            base.OnKeyUp( e );
        }
    }
}
