namespace System.Security.Principal
{
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a generic user.
    /// </summary>
    /// <remarks>
    /// An identity object represents the user on whose behalf the code is running.
    /// </remarks>
    public class GenericIdentity : IIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIdentity"/> class.
        /// </summary>
        /// <param name="name">The identity name.</param>
        public GenericIdentity( string name )
            : this( name, string.Empty, !string.IsNullOrEmpty( name ) )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIdentity"/> class.
        /// </summary>
        /// <param name="name">The identity name.</param>
        /// <param name="authenticationType">The identity authentication type.</param>
        public GenericIdentity( string name, string authenticationType )
            : this( name, authenticationType, !string.IsNullOrEmpty( name ) )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( authenticationType != null, "authenticationType" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIdentity"/> class.
        /// </summary>
        /// <param name="name">The identity name.</param>
        /// <param name="authenticationType">The identity authentication type.</param>
        /// <param name="authenticated">Indicates whether the identity is authenticated.</param>
        public GenericIdentity( string name, string authenticationType, bool authenticated )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( authenticationType != null, "authenticationType" );
            this.Name = name;
            this.AuthenticationType = authenticationType;
            this.IsAuthenticated = authenticated;
        }

        /// <summary>
        /// Gets or sets the type of authentication used to identify the user.
        /// </summary>
        /// <value>The type of authentication used to identify the user.</value>
        public string AuthenticationType
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user has been authenticated.
        /// </summary>
        /// <value>True if the user was has been authenticated; otherwise, false.</value>
        public bool IsAuthenticated
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        /// <value>The name of the user on whose behalf the code is being run.</value>
        public string Name
        {
            get;
            protected set;
        }
    }
}
