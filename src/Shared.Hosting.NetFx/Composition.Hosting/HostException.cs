namespace More.Composition.Hosting
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// Represents the exception that is thrown when a hosting error occurs.
    /// </summary>
    [Serializable]
    public partial class HostException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <permission cref="SecurityPermission">Inheritors require the <see cref="SecurityPermissionFlag">SerializationFormatter</see> permission.</permission>
        [SecurityPermission( SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.SerializationFormatter )]
        protected HostException( SerializationInfo info, StreamingContext context ) : base( info, context ) { }
    }
}