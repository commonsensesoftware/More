namespace More.Windows.Interactivity
{
    using global::Windows.Foundation;
    using global::Windows.Security.Credentials.UI;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction">interactivity action</see> that can be used to capture a user credential
    /// <see cref="CredentialInteraction">interaction</see> received from an <see cref="IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public class CredentialAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Gets or sets a value indicating whether the option of whether the dialog box is displayed.
        /// </summary>
        /// <value>The option of always displaying the dialog box. The default value is false.</value>
        public bool AlwaysDisplayDialog { get; set; }

        /// <summary>
        /// Gets or sets the authentication protocol.
        /// </summary>
        /// <value>One of the <see cref="AuthenticationProtocol"/> values.  The default value is <see cref="AuthenticationProtocol.Negotiate"/>.</value>
        public AuthenticationProtocol AuthenticationProtocol { get; set; }

        /// <summary>
        /// Gets or sets whether the authentication protocol is custom rather than a standard authentication protocol.
        /// </summary>
        /// <value>The authentication protocol is custom rather than a standard authentication protocol. The default value is none.</value>
        public string CustomAuthenticationProtocol { get; set; }

        /// <summary>
        /// Gets or sets the option on saving credentials.
        /// </summary>
        /// <value>One of the <see cref="CredentialSaveOption"/> values.</value>
        public CredentialSaveOption CredentialSaveOption { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the caller wants to save the credentials.
        /// </summary>
        /// <value>Gets or sets whether the caller wants to save the credentials. The default value is false.</value>
        public bool CallerSavesCredential { get; set; }

        /// <summary>
        /// Gets or sets the name of the target computer.
        /// </summary>
        /// <value>The name of the target computer. The default value is the caller's computer.</value>
        public string TargetName { get; set; }

        IAsyncOperation<CredentialPickerResults> SelectCredentialAsync( CredentialInteraction interaction )
        {
            Contract.Requires( interaction != null );
            Contract.Ensures( Contract.Result<IAsyncOperation<CredentialPickerResults>>() != null );

            var content = interaction == null ? string.Empty : interaction.Content.ToString();
            var options = new CredentialPickerOptions()
            {
                AlwaysDisplayDialog = AlwaysDisplayDialog,
                AuthenticationProtocol = AuthenticationProtocol,
                CallerSavesCredential = CallerSavesCredential,
                CredentialSaveOption = CredentialSaveOption,
                Caption = interaction.Title,
                Message = content,
                PreviousCredential = interaction.Credential.AsBuffer(),
            };

            if ( !string.IsNullOrEmpty( CustomAuthenticationProtocol ) )
            {
                options.CustomAuthenticationProtocol = CustomAuthenticationProtocol;
            }

            if ( !string.IsNullOrEmpty( TargetName ) )
            {
                options.TargetName = TargetName;
            }

            return CredentialPicker.PickAsync( options );
        }

        static void InvokeCallbackCommand( CredentialInteraction selectedCredential, CredentialPickerResults result )
        {
            Contract.Requires( selectedCredential != null );
            Contract.Requires( result != null );

            INamedCommand button = null;

            if ( result.ErrorCode == 0x800704C7U )
            {
                button = selectedCredential.CancelCommand;
            }
            else
            {
                selectedCredential.SavedByCredentialManager = result.CredentialSaved;
                selectedCredential.UserElectedToSaveCredential = result.CredentialSaveOption == CredentialSaveOption.Selected;
                selectedCredential.Credential = result.Credential.ToArray();
                selectedCredential.DomainName = result.CredentialDomainName;
                selectedCredential.UserName = result.CredentialUserName;
                selectedCredential.Password = result.CredentialPassword;
                button = selectedCredential.DefaultCommand;
            }

            if ( button != null && button.CanExecute() )
            {
                button.Execute();
            }
        }

        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        protected override async Task ExecuteAsync( object sender, object parameter )
        {
            var interaction = GetRequestedInteraction<CredentialInteraction>( parameter );

            if ( interaction == null )
            {
                return;
            }

            var result = await SelectCredentialAsync( interaction );
            InvokeCallbackCommand( interaction, result );
        }
    }
}