namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;

    internal sealed class FilePropertiesAdapter : BasicPropertiesAdapter<FileInfo>
    {
        internal FilePropertiesAdapter( FileInfo file )
            : base( file )
        {
            Contract.Requires( file != null );
        }

        public override long Size => StorageItem.Length;

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Internal. Verified by code contract." )]
        protected override void OnAttributeReadersCreated( IDictionary<string, Func<FileInfo, object>> attributeReaders ) =>
            attributeReaders.Add( nameof( FileInfo.IsReadOnly ), f => f.IsReadOnly );

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Internal. Verified by code contract." )]
        protected override void OnAttributeWritersCreated( IDictionary<string, Action<FileInfo, object>> attributeWriters ) =>
            attributeWriters.Add( nameof( FileInfo.IsReadOnly ), ( f, v ) => f.IsReadOnly = (bool) v );
    }
}
