namespace More.Windows.Controls
{
    using System;
    using global::Windows.Foundation;
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
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register( "Title", typeof( string ), typeof( TextInputDialog ), new PropertyMetadata( null ) );

        /// <summary>
        /// Gets or sets the default command index dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty DefaultCommandIndexProperty =
            DependencyProperty.Register( "DefaultCommandIndex", typeof( int ), typeof( TextInputDialog ), new PropertyMetadata( -1, OnDefaultCommandIndexChanged ) );

        /// <summary>
        /// Gets or sets the cancel command index dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty CancelCommandIndexProperty =
            DependencyProperty.Register( "CancelCommandIndex", typeof( int ), typeof( TextInputDialog ), new PropertyMetadata( -1, OnCancelCommandIndexChanged ) );

        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        /// <value>The dialog title.</value>
        public override string Title
        {
            get
            {
                return (string) this.GetValue( TitleProperty );
            }
            set
            {
                this.SetValue( TitleProperty, value );
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
                return (int) this.GetValue( DefaultCommandIndexProperty );
            }
            set
            {
                this.SetValue( DefaultCommandIndexProperty, value );
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
                return (int) this.GetValue( CancelCommandIndexProperty );
            }
            set
            {
                this.SetValue( CancelCommandIndexProperty, value );
            }
        }

        /// <summary>
        /// Occurs when the dialog is opened.
        /// </summary>
        protected override void OnOpened()
        {
            if ( this.input == null )
                return;

            this.input.Text = this.DefaultResponse;
            this.input.Focus( FocusState.Programmatic );
            this.input.SelectAll();
        }

        /// <summary>
        /// Overrides default behavior when the control template is applied to the control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.input = this.GetTemplateChild( "InputTextBox" ) as TextBox;

            if ( this.input != null )
                this.input.KeyUp += this.OnTextBoxKeyUp;
        }
    }
}
