namespace More.Windows.Controls
{
    using More.Windows.Controls.Automation.Peers;
    using More.Windows.Input;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Represents a message box window.
    /// </summary>
    /// <example>The following example demonstrates how to show a simple message box.
    /// <code lang="C#">
    /// <![CDATA[
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Input;
    ///
    /// public class Program
    /// {
    ///     public static void Main( param string[] args )
    ///     {
    ///         var messageBox = new MessageDialog()
    ///         {
    ///             Title = "Example"
    ///             Content = "This is an example message dialog.",
    ///             Commands =
    ///             {
    ///                 new NamedCommand<object>( "OK", p => { } )
    ///             }
    ///         };
    ///
    ///         messageBox.ShowDialog();
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <example>The following example demonstrates how to show a confirmation message box.
    /// <code lang="C#">
    /// <![CDATA[
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Input;
    ///
    /// public class Program
    /// {
    ///     public static void Main( param string[] args )
    ///     {
    ///         var messageBox = new MessageDialog()
    ///         {
    ///             Title = "Confirm"
    ///             Content = "Are you sure you want to continue?",
    ///             DefaultCommandIndex = 0,
    ///             CancelCommandIndex = 1,
    ///             Commands =
    ///             {
    ///                 new NamedCommand<object>( "Yes", p => { } )
    ///                 new NamedCommand<object>( "No", p => { } )
    ///             }
    ///         };
    ///
    ///         messageBox.ShowDialog();
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class MessageDialog : Window
    {
        sealed class CommandCollection : ObservableCollection<INamedCommand>
        {
            readonly MessageDialog dialog;

            internal CommandCollection( MessageDialog dialog ) => this.dialog = dialog;

            void OnCommandExecuted( object sender, EventArgs e )
            {
                var commandIndex = dialog.Commands.IndexOf( (INamedCommand) sender );
                dialog.OnCommandExecuted( commandIndex );
            }

            protected override void InsertItem( int index, INamedCommand item )
            {
                base.InsertItem( index, item );

                if ( item != null )
                {
                    item.Executed += OnCommandExecuted;
                }
            }

            protected override void RemoveItem( int index )
            {
                var item = this[index];
                base.RemoveItem( index );

                if ( item != null )
                {
                    item.Executed -= OnCommandExecuted;
                }
            }

            protected override void SetItem( int index, INamedCommand item )
            {
                var oldItem = this[index];
                base.SetItem( index, item );

                if ( oldItem != null )
                {
                    oldItem.Executed -= OnCommandExecuted;
                }

                if ( item != null )
                {
                    item.Executed += OnCommandExecuted;
                }
            }
        }

        static readonly DependencyPropertyKey ClientSizeProperty =
            DependencyProperty.RegisterReadOnly( "ClientSize", typeof( Size ), typeof( MessageDialog ), new PropertyMetadata( Size.Empty ) );

        /// <summary>
        /// Gets or sets the default command index dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty DefaultCommandIndexProperty =
            DependencyProperty.Register( nameof( DefaultCommandIndex ), typeof( int ), typeof( MessageDialog ), new PropertyMetadata( -1, OnDefaultCommandIndexChanged ) );

        /// <summary>
        /// Gets or sets the cancel command index dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty CancelCommandIndexProperty =
            DependencyProperty.Register( nameof( CancelCommandIndex ), typeof( int ), typeof( MessageDialog ), new PropertyMetadata( -1, OnCancelCommandIndexChanged ) );

        readonly CommandCollection commands;
        bool opened;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDialog"/> class.
        /// </summary>
        public MessageDialog()
        {
            commands = new CommandCollection( this );
            DefaultStyleKey = typeof( MessageDialog );
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SizeChanged += ( s, e ) => SetValue( ClientSizeProperty, this.GetClientSize() );
        }

        /// <summary>
        /// Gets a value indicating whether the input box is open.
        /// </summary>
        /// <value>True if the input box is open; otherwise, false.</value>
        protected internal bool IsOpen
        {
            get => opened;
            private set
            {
                if ( opened == value )
                {
                    return;
                }

                opened = value;

                if ( opened )
                {
                    OnOpened( EventArgs.Empty );
                }
            }
        }

        /// <summary>
        /// Gets or sets the default command index.
        /// </summary>
        /// <value>The default command index, which is the zero-based index of the
        /// <see cref="Commands">command</see> that is invoked when the ENTER key
        /// is pressed. The default value is -1, which indicates there is no default
        /// command.</value>
        public int DefaultCommandIndex
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= -1 );
                return (int) GetValue( DefaultCommandIndexProperty );
            }
            set
            {
                Arg.GreaterThanOrEqualTo( value, -1, nameof( value ) );
                SetValue( DefaultCommandIndexProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the cancel command index.
        /// </summary>
        /// <value>The cancel command index, which is the zero-based index of the
        /// <see cref="Commands">command</see> that is invoked when the ESC key
        /// is pressed. The default value is -1, which indicates there is no cancel
        /// command.</value>
        public int CancelCommandIndex
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= -1 );
                return (int) GetValue( CancelCommandIndexProperty );
            }
            set
            {
                Arg.GreaterThanOrEqualTo( value, -1, nameof( value ) );
                SetValue( CancelCommandIndexProperty, value );
            }
        }

        /// <summary>
        /// Gets a collection of commands associcated with the message dialog.
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

        /// <summary>
        /// Gets the size of the windows client area.
        /// </summary>
        /// <value>A <see cref="Size"/> structure.</value>
        public Size ClientSize => (Size) GetValue( ClientSizeProperty.DependencyProperty );

        /// <summary>
        /// Raises the <see cref="Opened"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnOpened( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Opened?.Invoke( this, e );
        }

        /// <summary>
        /// Overrides the default behavior when the underlying window source is initialized.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            this.RemoveIcon();

            // WPF does not have an "Opened" event or method to override.  Simulate "Opened" after the window is initialized.
            IsOpen = true;
        }

        /// <summary>
        /// Occurs when a command is executed.
        /// </summary>
        /// <param name="commandIndex">The zero-based index of the command that was executed.</param>
        protected virtual void OnCommandExecuted( int commandIndex )
        {
            Arg.GreaterThanOrEqualTo( commandIndex, -1, nameof( commandIndex ) );

            // setting the dialog result before Show() or ShowDialog() is called
            // will throw an exception. guard against that scenario.
            if ( !IsOpen )
            {
                return;
            }

            if ( commandIndex == DefaultCommandIndex )
            {
                DialogResult = true;
            }
            else if ( commandIndex == CancelCommandIndex )
            {
                DialogResult = false;
            }
            else
            {
                Close();
            }
        }

        internal void ExecuteCommand( int commandIndex )
        {
            Contract.Requires( commandIndex >= -1 );

            Commands.ElementAtOrDefault( commandIndex )?.Execute();
            OnCommandExecuted( commandIndex );
        }

        static void OnDefaultCommandIndexChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e ) =>
            OnCommandIndexChanged( nameof( DefaultCommandIndex ), e );

        static void OnCancelCommandIndexChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e ) =>
            OnCommandIndexChanged( nameof( CancelCommandIndex ), e );

        static void OnCommandIndexChanged( string propertyName, DependencyPropertyChangedEventArgs e )
        {
            var value = (int) e.NewValue;

            if ( value < -1 )
            {
                throw new ArgumentOutOfRangeException( propertyName );
            }
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
                    e.Handled = true;
                    ExecuteCommand( DefaultCommandIndex );
                    break;
                case Key.Escape:
                    e.Handled = true;
                    ExecuteCommand( CancelCommandIndex );
                    break;
            }

            base.OnKeyUp( e );
        }

        /// <summary>
        /// Occurs when the input box has been closed.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected override void OnClosed( EventArgs e )
        {
            base.OnClosed( e );
            IsOpen = false;
        }

        /// <summary>
        /// Creates an automation peer for the this control.
        /// </summary>
        /// <returns>A <see cref="System.Windows.Automation.Peers.AutomationPeer"/> object.</returns>
        protected override AutomationPeer OnCreateAutomationPeer() => new MessageDialogAutomationPeer( this );

        /// <summary>
        /// Occurs when the input box is opened.
        /// </summary>
        public event EventHandler Opened;
    }
}