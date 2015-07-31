namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts; 

    /// <summary>
    /// Represents a deferment manager for deferrable components<seealso cref="IDeferrable"/>.
    /// </summary>
    public class DeferManager : IDisposable
    {
        private readonly IDeferrable source;

        /// <summary>
        /// Finalizes an instance of the <see cref="DeferManager"/> class.
        /// </summary>
        ~DeferManager()
        {
            Dispose( false );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferManager"/> class.
        /// </summary>
        /// <param name="source">The <see cref="IDeferrable"/> souce.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public DeferManager( IDeferrable source )
        {
            Arg.NotNull( source, nameof( source ) );

            this.source = source;
            this.source.BeginDefer();
        }

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// </summary>
        /// <value>True if the object has been disposed; otherwise, false.</value>
        protected bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="DeferManager"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing", Justification = "This is the standard implementation of the Dispose pattern." )]
        protected virtual void Dispose( bool disposing )
        {
            if ( IsDisposed )
                return;

            IsDisposed = true;
            source.EndDefer();
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="DeferManager"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
