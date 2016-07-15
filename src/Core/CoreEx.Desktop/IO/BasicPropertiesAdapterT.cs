namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    internal class BasicPropertiesAdapter<T> : IBasicProperties where T : FileSystemInfo
    {
        private static readonly Lazy<IDictionary<string, Func<T, object>>> lazyAttributeReaders = new Lazy<IDictionary<string, Func<T, object>>>( CreateAttributeReaders );
        private static readonly Lazy<IDictionary<string, Action<T, object>>> lazyAttributeWriters = new Lazy<IDictionary<string, Action<T, object>>>( CreateAttributeWriters );
        private readonly T storageItem;

        internal BasicPropertiesAdapter( T storageItem )
        {
            Contract.Requires( storageItem != null );
            this.storageItem = storageItem;
        }

        private static IDictionary<string, Func<T, object>> CreateAttributeReaders()
        {
            Contract.Ensures( Contract.Result<IDictionary<string, Func<T, object>>>() != null );

            return new Dictionary<string, Func<T, object>>()
            {
                { nameof( FileSystemInfo.Attributes ), fsi => fsi.Attributes },
                { nameof( FileSystemInfo.CreationTime ), fsi => fsi.CreationTime },
                { nameof( FileSystemInfo.CreationTimeUtc ), fsi => fsi.CreationTimeUtc },
                { nameof( FileSystemInfo.LastAccessTime ), fsi => fsi.LastAccessTime },
                { nameof( FileSystemInfo.LastAccessTimeUtc ), fsi => fsi.LastAccessTimeUtc },
                { nameof( FileSystemInfo.LastWriteTime ), fsi => fsi.LastWriteTime },
                { nameof( FileSystemInfo.LastWriteTimeUtc ), fsi => fsi.LastWriteTimeUtc },
            };
        }

        private static IDictionary<string, Action<T, object>> CreateAttributeWriters()
        {
            Contract.Ensures( Contract.Result<IDictionary<string, Action<T, object>>>() != null );

            return new Dictionary<string, Action<T, object>>()
            {
                { nameof( FileSystemInfo.Attributes ), ( fsi, v ) => fsi.Attributes = (FileAttributes) v },
                { nameof( FileSystemInfo.CreationTime ), ( fsi, v ) => fsi.CreationTime = (DateTime) v },
                { nameof( FileSystemInfo.CreationTimeUtc ), ( fsi, v ) => fsi.CreationTimeUtc = (DateTime) v },
                { nameof( FileSystemInfo.LastAccessTime ), ( fsi, v ) => fsi.LastAccessTime = (DateTime) v },
                { nameof( FileSystemInfo.LastAccessTimeUtc ), ( fsi, v ) => fsi.LastAccessTimeUtc = (DateTime) v },
                { nameof( FileSystemInfo.LastWriteTime ), ( fsi, v ) => fsi.LastWriteTime = (DateTime) v },
                { nameof( FileSystemInfo.LastWriteTimeUtc ), ( fsi, v ) => fsi.LastWriteTimeUtc = (DateTime) v },
            };
        }

        private IDictionary<string, Func<T, object>> AttributeReaders
        {
            get
            {
                if ( !lazyAttributeReaders.IsValueCreated )
                    OnAttributeReadersCreated( lazyAttributeReaders.Value );

                return lazyAttributeReaders.Value;
            }
        }

        private IDictionary<string, Action<T, object>> AttributeWriters
        {
            get
            {
                if ( !lazyAttributeWriters.IsValueCreated )
                    OnAttributeWritersCreated( lazyAttributeWriters.Value );

                return lazyAttributeWriters.Value;
            }
        }

        protected virtual void OnAttributeReadersCreated( IDictionary<string, Func<T, object>> attributeReaders )
        {
        }

        protected virtual void OnAttributeWritersCreated( IDictionary<string, Action<T, object>> attributeWriters )
        {
        }

        protected T StorageItem => storageItem;

        public DateTimeOffset DateModified => storageItem.LastWriteTime;

        public DateTimeOffset ItemDate => storageItem.LastAccessTime;

        public virtual long Size => 0L;

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task<IDictionary<string, object>> RetrievePropertiesAsync( IEnumerable<string> propertiesToRetrieve )
        {
            Arg.NotNull( propertiesToRetrieve, nameof( propertiesToRetrieve ) );

            IDictionary<string, object> properties = new Dictionary<string, object>();

            foreach ( var property in propertiesToRetrieve )
            {
                Func<T, object> reader;

                if ( AttributeReaders.TryGetValue( property, out reader ) )
                    properties[property] = reader( StorageItem );
            }

            return Task.FromResult( properties );
        }

        public Task SavePropertiesAsync() => Task.FromResult( 0 );

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task SavePropertiesAsync( IEnumerable<KeyValuePair<string, object>> propertiesToSave )
        {
            Arg.NotNull( propertiesToSave, nameof( propertiesToSave ) );

            foreach ( var property in propertiesToSave )
            {
                Action<T, object> writer;

                if ( AttributeWriters.TryGetValue( property.Key, out writer ) )
                    writer( StorageItem, property.Value );
            }

            return Task.FromResult( 0 );
        }
    }
}
