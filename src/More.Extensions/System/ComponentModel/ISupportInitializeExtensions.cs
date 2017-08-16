namespace System.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="ISupportInitialize"/> interface.
    /// </summary>
    public static class ISupportInitializeExtensions
    {
        sealed class InitializationScope : IDisposable
        {
            bool disposed;
            ISupportInitialize source;

            ~InitializationScope() => Dispose( false );

            internal InitializationScope( ISupportInitialize source )
            {
                Contract.Requires( source != null );
                this.source = source;
                this.source.BeginInit();
            }

            void Dispose( bool disposing )
            {
                if ( disposed )
                {
                    return;
                }

                disposed = true;

                if ( disposing && source != null )
                {
                    source.EndInit();
                }

                source = null;
            }

            public void Dispose()
            {
                Dispose( true );
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
        /// using System;
        /// using System.ComponentModel;
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
            Arg.NotNull( source, nameof( source ) );
            Contract.Ensures( Contract.Result<IDisposable>() != null );
            return new InitializationScope( source );
        }
    }
}