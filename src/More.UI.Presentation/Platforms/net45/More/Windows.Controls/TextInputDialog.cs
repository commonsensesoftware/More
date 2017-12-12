namespace More.Windows.Controls
{
    using More.Windows.Controls.Automation.Peers;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;

    /// <content>
    /// Provides implementation specific to the full CLR version of the .NET Framework.
    /// </content>
    public partial class TextInputDialog
    {
        /// <summary>
        /// Creates an automation peer for the this control.
        /// </summary>
        /// <returns>A <see cref="AutomationPeer"/> object.</returns>
        protected override AutomationPeer OnCreateAutomationPeer() => new TextInputDialogAutomationPeer( this );

        /// <summary>
        /// Overrides the default behavior when the underlying window source is initialized.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );

            if ( input != null )
            {
                input.Text = DefaultResponse;
                input.Focus();
                input.SelectAll();
            }

            Response = null;
        }

        /// <summary>
        /// Overrides default behavior when the control template is applied to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if ( input != null )
            {
                input.KeyUp -= OnTextBoxKeyUp;
            }

            input = GetTemplateChild( "InputTextBox" ) as TextBox;

            if ( input != null )
            {
                input.KeyUp += OnTextBoxKeyUp;
            }
        }
    }
}