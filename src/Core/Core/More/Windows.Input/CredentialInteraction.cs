namespace More.Windows.Input
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an interaction request to capture credentials from a user.
    /// </summary>
    public class CredentialInteraction : Interaction
    {
        private bool saved;
        private bool shouldSave;
        private byte[] credential;
        private string domain;
        private string userName;
        private string password;

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialInteraction"/> class.
        /// </summary>
        public CredentialInteraction()
            : this( string.Empty )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        public CredentialInteraction( string title )
            : base( title )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the credential was saved by the credential manager.
        /// </summary>
        /// <value>True if the credential was saved by the credential manager; otherwise, false.</value>
        public bool SavedByCredentialManager
        {
            get
            {
                return this.saved;
            }
            set
            {
                this.SetProperty( ref this.saved, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a user elected to save their credential.
        /// </summary>
        /// <value>True if the user wants their credential saved; otherwise, false.</value>
        public bool UserElectedToSaveCredential
        {
            get
            {
                return this.shouldSave;
            }
            set
            {
                this.SetProperty( ref this.shouldSave, value );
            }
        }

        /// <summary>
        /// Gets or sets a user's credential.
        /// </summary>
        /// <value>The user's credential in binary form.</value>
        public byte[] Credential
        {
            get
            {
                return this.credential;
            }
            set
            {
                this.SetProperty( ref this.credential, value );
            }
        }

        /// <summary>
        /// Gets or sets the domain name portion of the credential.
        /// </summary>
        /// <value>The domain name of the credential. If the domain name is not available, an empty string is returned.</value>
        public string DomainName
        {
            get
            {
                return this.domain;
            }
            set
            {
                this.SetProperty( ref this.domain, value );
            }
        }

        /// <summary>
        /// Gets or sets the user name of the credential.
        /// </summary>
        /// <value>The user name of the credential used. If the user name is not available, an empty string is returned.</value>
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.SetProperty( ref this.userName, value );
            }
        }

        /// <summary>
        /// Gets or sets the password portion of the credential.
        /// </summary>
        /// <value>The password of the credential. If the password is not available, an empty string is returned.</value>
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.SetProperty( ref this.password, value );
            }
        }
    }
}
