namespace More.Windows.Media
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( MediaContent<> ) )]
    abstract class MediaContentContract<T> : MediaContent<T>
    {
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected override Task<T> OnReadStreamAsync( Stream stream )
        {
            Contract.Requires<ArgumentNullException>( stream != null );
            Contract.Requires<InvalidOperationException>( stream.CanRead, "stream.CanRead" );
            Contract.Ensures( Contract.Result<Task<T>>() != null );
            return null;
        }
    }
}