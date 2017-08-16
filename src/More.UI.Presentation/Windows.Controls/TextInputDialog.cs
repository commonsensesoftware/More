namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
#if UAP10_0
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Input;
    using Key = global::Windows.System.VirtualKey;
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
#endif

    /// <summary>
    /// Represents a text input dialog.
    /// </summary>
#if UAP10_0
    [CLSCompliant( false )]
#endif
    [TemplatePart( Name = "InputTextBox", Type = typeof( TextBox ) )]
    public partial class TextInputDialog : InputDialog<string>
    {
        /// <summary>
        /// Gets or sets the default response dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty DefaultResponseProperty =
            DependencyProperty.Register( nameof( DefaultResponse ), typeof( string ), typeof( TextInputDialog ), new PropertyMetadata( null ) );

        TextBox input;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInputDialog"/> class.
        /// </summary>
        public TextInputDialog() => DefaultStyleKey = typeof( TextInputDialog );

        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sender", Justification = "Required event handler signature." )]
        static void OnDefaultCommandIndexChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e ) => OnCommandIndexChanged( nameof( DefaultCommandIndex ), e );

        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sender", Justification = "Required event handler signature." )]
        static void OnCancelCommandIndexChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e ) => OnCommandIndexChanged( nameof( CancelCommandIndex ), e );

        static void OnCommandIndexChanged( string propertyName, DependencyPropertyChangedEventArgs e )
        {
            var value = (int) e.NewValue;

            if ( value < -1 )
            {
                throw new ArgumentOutOfRangeException( propertyName );
            }
        }

        void OnTextBoxKeyUp( object sender, KeyEventArgs e )
        {
            if ( e.Key != Key.Enter )
            {
                return;
            }

            e.Handled = true;
            ExecuteCommand( DefaultCommandIndex );
        }

        /// <summary>
        /// Gets or sets the input dialog default response.
        /// </summary>
        /// <value>The input dialog default response.</value>
        public override string DefaultResponse
        {
            get => (string) GetValue( DefaultResponseProperty );
            set => SetValue( DefaultResponseProperty, value );
        }

        /// <summary>
        /// Occurs when a command is executed.
        /// </summary>
        /// <param name="commandIndex">The zero-based index of the command that was executed.</param>
        protected override void OnCommandExecuted( int commandIndex )
        {
            if ( commandIndex == DefaultCommandIndex && input != null )
            {
                Response = input.Text;
            }

            base.OnCommandExecuted( commandIndex );
        }
    }
}