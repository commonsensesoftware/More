namespace More.Windows.Controls
{
    using More.Windows.Input;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Windows.Input;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Windows.Foundation;
    using global::Windows.System;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Controls.Primitives;
    using global::Windows.UI.Xaml.Input;
    using Key = global::Windows.System.VirtualKey;
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;

    /// <summary>
    /// Represents the base implementation for an input dialog.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of response.</typeparam>
    [CLSCompliant( false )]
    public abstract class InputDialog<T> : ContentControl where T : class
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
                var commandIndex = this.dialog.Commands.IndexOf( (INamedCommand) sender );
                this.dialog.OnCommandExecuted( commandIndex );
            }

            protected override void InsertItem( int index, INamedCommand item )
            {
                base.InsertItem( index, item );

                if ( item != null )
                    item.Executed += this.OnCommandExecuted;
            }

            protected override void RemoveItem( int index )
            {
                var item = this[index];
                base.RemoveItem( index );

                if ( item != null )
                    item.Executed -= this.OnCommandExecuted;
            }

            protected override void SetItem( int index, INamedCommand item )
            {
                var oldItem = this[index];
                base.SetItem( index, item );

                if ( oldItem != null )
                    oldItem.Executed -= this.OnCommandExecuted;

                if ( item != null )
                    item.Executed += this.OnCommandExecuted;
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
            this.commands = new CommandCollection( this );
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
                return this.commands;
            }
        }

        private Window Window
        {
            get
            {
#if NETFX_CORE
                return Window.Current;
#else
                return CoreWindow.GetForCurrentThread();
#endif
            }
        }

        private void OnWindowSizeChanged( object sender, WindowSizeChangedEventArgs e )
        {
            this.ArrangePopupContent( e.Size );
        }

        private void ArrangePopupContent( Size size )
        {
            this.Width = size.Width;
            this.popup.VerticalOffset = Math.Max( ( size.Height - this.ActualHeight ), 0d ) / 2d;
        }

        private Popup CreatePopup()
        {
            Contract.Ensures( Contract.Result<Popup>() != null );

            var newPopup = new Popup() {
                Child = this,
#if NETFX_CORE
                IsLightDismissEnabled = true
#endif
            };

            newPopup.Loaded += ( s, e ) => {
                this.OnOpened();

                var window = this.Window;

                if ( window == null )
                    return;

                this.ArrangePopupContent( new Size( window.Bounds.Width, window.Bounds.Height ) );
                window.SizeChanged += this.OnWindowSizeChanged;
            };

            return newPopup;
        }

        private void Close()
        {
            var window = this.Window;

            if ( window != null )
                window.SizeChanged -= this.OnWindowSizeChanged;

            if ( this.popup == null )
                return;

            this.popup.IsOpen = false;
            this.popup = null;
        }

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </summary>
        /// <returns>An object that represents the <see cref="IAsyncOperation{T}">asynchronous operation</see>.</returns>
        public IAsyncOperation<T> ShowAsync()
        {
            Contract.Ensures( Contract.Result<IAsyncOperation<T>>() != null );

            this.Response = null;
            this.popup = this.CreatePopup();
            this.popup.IsOpen = true;

            return AsyncInfo.Run( this.ShowDialog );
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

            this.completionSource = new TaskCompletionSource<T>();
            token.Register( () => {
                this.Close();
                this.completionSource.TrySetResult( null );
            } );
            return this.completionSource.Task;
        }

        /// <summary>
        /// Occurs when a command is executed.
        /// </summary>
        /// <param name="commandIndex">The zero-based index of the command that was executed.</param>
        protected virtual void OnCommandExecuted( int commandIndex )
        {
            Contract.Requires<ArgumentOutOfRangeException>( commandIndex >= -1, "commandIndex" );

            this.Close();

            var accepted = commandIndex == this.DefaultCommandIndex;
            var response = accepted ? this.Response : null;

            if ( this.completionSource != null )
                this.completionSource.TrySetResult( response );
        }

        internal void ExecuteCommand( int commandIndex )
        {
            Contract.Requires( commandIndex >= -1 );

            var command = this.Commands.ElementAtOrDefault( commandIndex );

            if ( command != null )
                command.Execute();

            this.OnCommandExecuted( commandIndex );
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
                        this.ExecuteCommand( this.DefaultCommandIndex );
                        break;
                    }
                case Key.Escape:
                    {
                        e.Handled = true;
                        this.ExecuteCommand( this.CancelCommandIndex );
                        break;
                    }
            }

            base.OnKeyUp( e );
        }
    }
}
