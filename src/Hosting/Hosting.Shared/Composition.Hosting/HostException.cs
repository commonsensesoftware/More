namespace More.Composition.Hosting
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the exception that is thrown when a hosting error occurs.
    /// </summary>
    public partial class HostException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostException" /> class.
        /// </summary>
        public HostException()
            : this( ExceptionMessage.HostException, null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public HostException( string message )
            : this( message, null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the
        /// <i>innerException</i> property is not a null reference, the current exception is raised in a
        /// catch block that handles the inner exception.</param>
        public HostException( string message, Exception innerException )
            : base( message, innerException )
        {
            Arg.NotNull( message, nameof( message ) );
        }
    }
}
