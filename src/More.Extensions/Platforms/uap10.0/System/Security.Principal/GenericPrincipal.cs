namespace System.Security.Principal
{
    using System.Collections.Generic;
    using static System.String;

    /// <summary>
    /// Represents a generic principal.
    /// </summary>
    public class GenericPrincipal : IPrincipal
    {
        readonly HashSet<string> roles = new HashSet<string>( StringComparer.OrdinalIgnoreCase );

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericPrincipal"/> class.
        /// </summary>
        /// <param name="identity">The <see cref="IIdentity"/> representing the user.</param>
        public GenericPrincipal( IIdentity identity )
            : this( identity, new string[0] ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericPrincipal"/> class.
        /// </summary>
        /// <param name="identity">The <see cref="IIdentity"/> representing the user.</param>
        /// <param name="roles">An <see cref="IEnumerable{T}"/> sequence of role names to which
        /// the user represented by <paramref name="identity"/> belongs to.</param>
        public GenericPrincipal( IIdentity identity, IEnumerable<string> roles )
        {
            Arg.NotNull( identity, nameof( identity ) );
            Arg.NotNull( roles, nameof( roles ) );

            Identity = identity;
            this.roles.AddRange( roles );
        }

        /// <summary>
        /// Gets or sets the identity of the user represented by the current principal.
        /// </summary>
        /// <value>An <see cref="IIdentity"/> object.</value>
        public IIdentity Identity { get; protected set; }

        /// <summary>
        /// Determines whether the current System.Security.Principal.GenericPrincipal belongs to the specified role.
        /// </summary>
        /// <param name="role">The name of the role for which to check membership.</param>
        /// <returns>True if the current principal is a member of the specified role; otherwise, false.</returns>
        public virtual bool IsInRole( string role ) => !IsNullOrEmpty( role ) && roles.Contains( role );
    }
}