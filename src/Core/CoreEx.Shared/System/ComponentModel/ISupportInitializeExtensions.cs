namespace System.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts; 

    /// <summary>
    /// Provides extension methods for the <see cref="ISupportInitialize"/> interface.
    /// </summary>
    public static class ISupportInitializeExtensions
    {
        private sealed class InitializationScope : IDisposable
        {
            private bool disposed;
            private ISupportInitialize source;

            ~InitializationScope()
            {
                this.Dispose( false );
            }

            internal InitializationScope( ISupportInitialize source )
            {
                Contract.Requires( source != null ); 
                this.source = source;
                this.source.BeginInit();
            }

            private void Dispose( bool disposing )
            {
                if ( this.disposed )
                    return;

                this.disposed = true;

                if ( disposing && this.source != null )
                    this.source.EndInit();

                this.source = null;
            }

            public void Dispose()
            {
                this.Dispose( true );
                GC.SuppressFinalize( this );
            }
        }

        /// <summary>
        /// Creates, begins, and returns the initialization scope for an object.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of <see cref="ISupportInitialize">object</see> to create an initialization scope for.</typeparam>
        /// <param name="source">The object of <typeparamref name="TObject"/> to create an initialization scope for.</param>
        /// <returns>An <see cref="IDisposable"/> object representing the initialization scope.</returns>
        /// <remarks>When the return initialization scope is <see cref="M:IDisposable.Dispose">disposed</see>, the initialization of the object is
        /// <see cref="M:ISupportInitialize.EndInit">committed</see>.</remarks>
        /// <example>This example demonstrates initializing an object.
        /// <code lang="C#"><![CDATA[
        /// using global::System;
        /// using global::System.ComponentModel;
        /// 
        /// public static void Main()
        /// {
        ///     var obj = new MyObject();
        ///     
        ///     using ( obj.Initialize() )
        ///     {
        ///         // TODO: initialization work
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The returned object is disposable. When the object is disposed, the scope ends." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IDisposable Initialize<TObject>( this TObject source ) where TObject : ISupportInitialize
        {
            Arg.NotNull( source, "source" );
            Contract.Ensures( Contract.Result<IDisposable>() != null );
            return new InitializationScope( source );
        }
    }
}
