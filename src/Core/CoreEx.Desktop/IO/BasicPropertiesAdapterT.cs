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
                { "Attributes", fsi => fsi.Attributes },
                { "CreationTime", fsi => fsi.CreationTime },
                { "CreationTimeUtc", fsi => fsi.CreationTimeUtc },
                { "LastAccessTime", fsi => fsi.LastAccessTime },
                { "LastAccessTimeUtc", fsi => fsi.LastAccessTimeUtc },
                { "LastWriteTime", fsi => fsi.LastWriteTime },
                { "LastWriteTimeUtc", fsi => fsi.LastWriteTimeUtc },
            };
        }

        private static IDictionary<string, Action<T, object>> CreateAttributeWriters()
        {
            Contract.Ensures( Contract.Result<IDictionary<string, Action<T, object>>>() != null );

            return new Dictionary<string, Action<T, object>>()
            {
                { "Attributes", ( fsi, v ) => fsi.Attributes = (FileAttributes) v },
                { "CreationTime", ( fsi, v ) => fsi.CreationTime = (DateTime) v },
                { "CreationTimeUtc", ( fsi, v ) => fsi.CreationTimeUtc = (DateTime) v },
                { "LastAccessTime", ( fsi, v ) => fsi.LastAccessTime = (DateTime) v },
                { "LastAccessTimeUtc", ( fsi, v ) => fsi.LastAccessTimeUtc = (DateTime) v },
                { "LastWriteTime", ( fsi, v ) => fsi.LastWriteTime = (DateTime) v },
                { "LastWriteTimeUtc", ( fsi, v ) => fsi.LastWriteTimeUtc = (DateTime) v },
            };
        }

        private IDictionary<string, Func<T, object>> AttributeReaders
        {
            get
            {
                if ( !lazyAttributeReaders.IsValueCreated )
                    this.OnAttributeReadersCreated( lazyAttributeReaders.Value );

                return lazyAttributeReaders.Value;
            }
        }

        private IDictionary<string, Action<T, object>> AttributeWriters
        {
            get
            {
                if ( !lazyAttributeWriters.IsValueCreated )
                    this.OnAttributeReadersCreated( lazyAttributeReaders.Value );

                return lazyAttributeWriters.Value;
            }
        }

        protected virtual void OnAttributeReadersCreated( IDictionary<string, Func<T, object>> attributeReaders )
        {
            Contract.Requires( attributeReaders != null );
        }

        protected virtual void OnAttributeWritersCreated( IDictionary<string, Action<T, object>> attributeWriters )
        {
            Contract.Requires( attributeWriters != null );
        }

        protected T StorageItem
        {
            get
            {
                Contract.Ensures( this.storageItem != null );
                return this.storageItem;
            }
        }

        public DateTimeOffset DateModified
        {
            get
            {
                return this.storageItem.LastWriteTime;
            }
        }

        public DateTimeOffset ItemDate
        {
            get
            {
                return this.storageItem.LastAccessTime;
            }
        }

        public virtual long Size
        {
            get
            {
                return 0L;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task<IDictionary<string, object>> RetrievePropertiesAsync( IEnumerable<string> propertiesToRetrieve )
        {
            Arg.NotNull( propertiesToRetrieve, "propertiesToRetrieve" );

            IDictionary<string, object> properties = new Dictionary<string, object>();

            foreach ( var property in propertiesToRetrieve )
            {
                Func<T, object> reader;

                if ( this.AttributeReaders.TryGetValue( property, out reader ) )
                    properties[property] = reader( this.StorageItem );
            }

            return Task.FromResult( properties );
        }

        public Task SavePropertiesAsync()
        {
            // changes to properties take place immediately
            return Task.FromResult( 0 );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public Task SavePropertiesAsync( IEnumerable<KeyValuePair<string, object>> propertiesToSave )
        {
            Arg.NotNull( propertiesToSave, "propertiesToSave" );

            foreach ( var property in propertiesToSave )
            {
                Action<T, object> writer;

                if ( this.AttributeWriters.TryGetValue( property.Key, out writer ) )
                    writer( this.StorageItem, property.Value );
            }

            return Task.FromResult( 0 );
        }
    }
}
