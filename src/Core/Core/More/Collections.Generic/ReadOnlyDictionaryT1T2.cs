namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Collections.Specialized;
    using global::System.ComponentModel;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Threading;

    /// <summary>
    /// Represents a read-only dictionary.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type">type</see> of item key.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of item value.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( DictionaryDebugView<,> ) )]
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IDictionary<TKey, TValue> dict;
        private object syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}">dictionary</see> to make read-only.</param>
        public ReadOnlyDictionary( IDictionary<TKey, TValue> dictionary )
        {
            Contract.Requires<ArgumentNullException>( dictionary != null, "dictionary" );
            this.dict = dictionary;

            var propertyChanged = dictionary as INotifyPropertyChanged;

            if ( propertyChanged != null )
                propertyChanged.PropertyChanged += this.SourcePropertyChanged;

            var collectionChanged = dictionary as INotifyCollectionChanged;

            if ( collectionChanged != null )
                collectionChanged.CollectionChanged += ( s, e ) => this.OnCollectionChanged( e );
        }

        /// <summary>
        /// Gets the dictionary that this <see cref="ReadOnlyDictionary{TKey,TValue}"/> wraps.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> object.</value>
        protected IDictionary<TKey, TValue> Dictionary
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
                return this.dict;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a
        /// <see cref="KeyNotFoundException"/>.</returns>
        public TValue this[TKey key]
        {
            get
            {
                return this.Dictionary[key];
            }
        }

        private void SourcePropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            switch ( e.PropertyName )
            {
                case "Count":
                case "Item[]":
                    {
                        this.OnPropertyChanged( e );
                        break;
                    }
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.PropertyChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.CollectionChanged;

            if ( handler != null )
                handler( this, e );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IDictionary<TKey, TValue>.Add( TKey key, TValue value )
        {
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKey( TKey key )
        {
            Contract.Assume( false || this.Count > 0 );
            return this.Dictionary.ContainsKey( key );
        }

        /// <summary>
        /// Gets a collection containing the keys of the dictionary.
        /// </summary>
        /// <value>An <see cref="ICollection{TKey}"/> object.</value>
        public ICollection<TKey> Keys
        {
            get
            {
                if ( this.Dictionary.Keys.IsReadOnly )
                    return this.Dictionary.Keys;

                return this.Dictionary.Keys.ToArray().AsReadOnly();
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return this.Keys;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool IDictionary<TKey, TValue>.Remove( TKey key )
        {
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key
        /// is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>True if the dictioanry contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue( TKey key, out TValue value )
        {
            Contract.Assume( this.ContainsKey( key ) );
            return this.Dictionary.TryGetValue( key, out value );
        }

        /// <summary>
        /// Gets a collection containing the values of the dictionary.
        /// </summary>
        /// <value>An <see cref="ICollection{TValue}"/> object.</value>
        public ICollection<TValue> Values
        {
            get
            {
                if ( this.Dictionary.Values.IsReadOnly )
                    return this.Dictionary.Values;

                return this.Dictionary.Values.ToArray().AsReadOnly();
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return this.Values;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this.Dictionary[key];
            }
            set
            {
                throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void ICollection<KeyValuePair<TKey, TValue>>.Add( KeyValuePair<TKey, TValue> item )
        {
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        /// <summary>
        /// Determines whether the dictionry contains the specified value.
        /// </summary>
        /// <param name="item">The object to locate in the dictionary.</param>
        /// <returns>True if the item is found; otherwise, false.</returns>
        public bool Contains( KeyValuePair<TKey, TValue> item )
        {
            Contract.Assume( false || this.Count > 0 );
            return this.Dictionary.Contains( item );
        }

        /// <summary>
        /// Copies the elements of the dictionary to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public void CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
        {
            Contract.Assume( arrayIndex + this.Dictionary.Count <= array.Length );
            this.Dictionary.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the dictionary.
        /// </summary>
        /// <value>The number of key/value pairs in the dictionary.</value>
        public int Count
        {
            get
            {
                return this.Dictionary.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary{TKey,TValue}"/> object is read-only.
        /// </summary>
        /// <value><see langkeyword="true">True</see> if the <see cref="IDictionary{TKey,TValue}"/> object is read-only; otherwise, false.</value>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove( KeyValuePair<TKey, TValue> item )
        {
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{KeyValuePair}"/> object.</returns>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IDictionary.Add( object key, object value )
        {
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IDictionary.Clear()
        {
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool IDictionary.Contains( object key )
        {
            if ( key is TKey )
                return this.ContainsKey( (TKey) key );

            return false;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Dictionary<TKey, TValue>( this.Dictionary ).GetEnumerator();
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool IDictionary.IsFixedSize
        {
            get
            {
                var idict = this.Dictionary as IDictionary;

                if ( idict != null )
                    return idict.IsFixedSize;

                // NOTE: there's not way to know for sure whether the underlying dictionary is read-only.  we'll assume it's not and that we'll receive
                // appropriate events via INotifyCollectionChnaged interface on the underlying dictionary.
                return false;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        ICollection IDictionary.Keys
        {
            get
            {
                return this.Dictionary.Keys.ToList();
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IDictionary.Remove( object key )
        {
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        ICollection IDictionary.Values
        {
            get
            {
                return this.Dictionary.Values.ToList();
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        object IDictionary.this[object key]
        {
            get
            {
                if ( key is TKey )
                    return this[(TKey) key];

                return null;
            }
            set
            {
                throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        void ICollection.CopyTo( Array array, int index )
        {
            var col = this.Dictionary as ICollection;

            if ( col != null )
            {
                Contract.Assume( index <= array.Length + col.Count );
                col.CopyTo( array, index );
            }
            else
            {
                var items = array as KeyValuePair<TKey, TValue>[];

                if ( items == null )
                    throw new ArrayTypeMismatchException( ExceptionMessage.ArrayMismatch );

                Contract.Assume( index + this.Count <= items.Length );
                this.CopyTo( items, index );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        object ICollection.SyncRoot
        {
            get
            {
                if ( this.syncRoot == null )
                {
                    var col = this.Dictionary as ICollection;

                    if ( col != null )
                        this.syncRoot = col.SyncRoot;

                    Interlocked.CompareExchange( ref this.syncRoot, new object(), null );
                }

                Contract.Assume( this.syncRoot != null );
                return this.syncRoot;
            }
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
