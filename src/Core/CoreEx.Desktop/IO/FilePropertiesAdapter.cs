namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    internal sealed class FilePropertiesAdapter : BasicPropertiesAdapter<FileInfo>
    {
        internal FilePropertiesAdapter( FileInfo file )
            : base( file )
        {
            Contract.Requires( file != null );
        }

        public override long Size
        {
            get
            {
                return this.StorageItem.Length;
            }
        }

        protected override void OnAttributeReadersCreated( IDictionary<string, Func<FileInfo, object>> attributeReaders )
        {
            attributeReaders.Add( "IsReadOnly", f => f.IsReadOnly );
        }

        protected override void OnAttributeWritersCreated( IDictionary<string, Action<FileInfo, object>> attributeWriters )
        {
            attributeWriters.Add( "IsReadOnly", ( f, v ) => f.IsReadOnly = (bool) v );
        }
    }
}
