namespace More.Windows.Controls
{
    using Input;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Collections.ObjectModel;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <content>
    /// Provides additional implementation specified to Windows Runtime applications.
    /// </content>
    public partial class TextInputDialog
    {
        /// <summary>
        /// Gets or sets the title dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register( nameof( Title ), typeof( string ), typeof( TextInputDialog ), new PropertyMetadata( null ) );

        /// <summary>
        /// Gets or sets the default command index dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty DefaultCommandIndexProperty =
            DependencyProperty.Register( nameof( DefaultCommandIndex ), typeof( int ), typeof( TextInputDialog ), new PropertyMetadata( -1, OnDefaultCommandIndexChanged ) );

        /// <summary>
        /// Gets or sets the cancel command index dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty CancelCommandIndexProperty =
            DependencyProperty.Register( nameof( CancelCommandIndex ), typeof( int ), typeof( TextInputDialog ), new PropertyMetadata( -1, OnCancelCommandIndexChanged ) );

        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        /// <value>The dialog title.</value>
        public override string Title
        {
            get
            {
                return (string) GetValue( TitleProperty );
            }
            set
            {
                SetValue( TitleProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the default command index.
        /// </summary>
        /// <value>The default command index, which is the zero-based index of the
        /// <see cref="P:Commands">command</see> that is invoked when the ENTER key
        /// is pressed. The default value is -1, which indicates there is no default
        /// command.</value>
        public override int DefaultCommandIndex
        {
            get
            {
                return (int) GetValue( DefaultCommandIndexProperty );
            }
            set
            {
                SetValue( DefaultCommandIndexProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the cancel command index.
        /// </summary>
        /// <value>The cancel command index, which is the zero-based index of the
        /// <see cref="P:Commands">command</see> that is invoked when the ESC key
        /// is pressed. The default value is -1, which indicates there is no cancel
        /// command.</value>
        public override int CancelCommandIndex
        {
            get
            {
                return (int) GetValue( CancelCommandIndexProperty );
            }
            set
            {
                SetValue( CancelCommandIndexProperty, value );
            }
        }

        /// <summary>
        /// Occurs when the dialog is opened.
        /// </summary>
        protected override void OnOpened()
        {
            if ( input == null )
                return;

            input.Text = DefaultResponse;
            input.Focus( FocusState.Programmatic );
            input.SelectAll();
        }

        /// <summary>
        /// Overrides default behavior when the control template is applied to the control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            input = GetTemplateChild( "InputTextBox" ) as TextBox;

            if ( input != null )
                input.KeyUp += OnTextBoxKeyUp;
        }
    }
}
